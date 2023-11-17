#!/usr/bin/env bash
# shellcheck disable=SC2034

set -e
set +x

cd "$(dirname "$(realpath "$0")")"

# TODO ensure required cli tools are installed / configured: az (Microsoft azure), gh (Github cli)

# Load env variables
set -a
source .env
test -f .local.env && source .local.env
set +a

export MSYS_NO_PATHCONV=1

# Create service principal
ARM_PRINCIPAL_NAME="github-actions-${APP_PROJECT_SLUG}"
ARM_SUBSCRIPTION_ID=$(az account show | jq -r '.id')

ARM_SERVICE_PRINCIPAL_ID=$(az ad sp list --display-name "${ARM_PRINCIPAL_NAME}" | jq -r '.[0].id')

if [ "${ARM_SERVICE_PRINCIPAL_ID}" == "null" ]; then
    echo "Azure service principal '${ARM_PRINCIPAL_NAME}' does not exist, creating"
    ARM_SERVICE_PROVIDER_INFO=$(
        az ad sp create-for-rbac \
            --name "${ARM_PRINCIPAL_NAME}" \
            --role "Owner" \
            --scopes "/subscriptions/${ARM_SUBSCRIPTION_ID}/resourceGroups/${APP_PROJECT_SLUG}"
    )
else
    echo "Azure service principal '${ARM_PRINCIPAL_NAME}' already exists, resetting credentials"
    ARM_SERVICE_PROVIDER_INFO=$(az ad sp credential reset --id "${ARM_SERVICE_PRINCIPAL_ID}")
fi

ARM_TENANT_ID=$(echo "${ARM_SERVICE_PROVIDER_INFO}" | jq -r '.tenant')
ARM_CLIENT_ID=$(echo "${ARM_SERVICE_PROVIDER_INFO}" | jq -r '.appId')
ARM_CLIENT_SECRET=$(echo "${ARM_SERVICE_PROVIDER_INFO}" | jq -r '.password')
APP_STORAGE_SLUG=${APP_PROJECT_SLUG//-/}

# Register required resource providers
az provider register --subscription "${ARM_SUBSCRIPTION_ID}" --namespace Microsoft.Storage

# Create resource group
if [[ $(az group exists --name "${APP_PROJECT_SLUG}") == false ]]; then
    az group create --name "${APP_PROJECT_SLUG}" --location "${ARM_LOCATION}"
    echo "Created azure resource group ${APP_PROJECT_SLUG}"
fi

# Create storage account for terraform state
ARM_STORAGE_ACCOUNT_NAME="${APP_STORAGE_SLUG}terraform"
az storage account create \
    --name "${ARM_STORAGE_ACCOUNT_NAME}" \
    --resource-group "${APP_PROJECT_SLUG}" \
    --location "${ARM_LOCATION}" \
    --allow-blob-public-access false \
    --sku "Standard_LRS"

az storage container create \
    --name terraform \
    --account-name "${ARM_STORAGE_ACCOUNT_NAME}"

# Create registry for docker images
ARM_ACR_REGISTRY_INFO=$(
    az acr create \
        --resource-group "${APP_PROJECT_SLUG}" \
        --name "${APP_STORAGE_SLUG}" \
        --location "${ARM_LOCATION}" \
        --sku Basic
)
ARM_ACR_REGISTRY_ID=$(echo "${ARM_ACR_REGISTRY_INFO}" | jq -r '.id')

az role assignment create \
    --assignee "${ARM_CLIENT_ID}" \
    --scope "${ARM_ACR_REGISTRY_ID}" \
    --role AcrPush

# Ensure pages is set to workflow
gh api -X PUT "/repos/${GITHUB_REPOSITORY}/pages" -f build_type=workflow --silent

# Generate randon GH_ENCRYPT_KEY, used to pass sensitive variables between github workflows
GH_ENCRYPT_KEY=$(openssl rand -hex 32)

# Ensure Github secrets are set
echo "Creating github secrets"
function storeSecret {
    SECRET_NAME="${1}"
    SECRET_VALUE="${!SECRET_NAME}"
    IS_SECRET="${2}"
    if [ -z "${SECRET_VALUE}" ]; then
        echo "Missing environment variable '${SECRET_NAME}', please configure one in your '.env.local' file."
        echo "${SECRET_VALUE}"
        exit 1
    fi

    if [ "${IS_SECRET}" = true ]; then
        echo "${SECRET_VALUE}" | gh secret set "${SECRET_NAME}" --app actions
        echo "${SECRET_VALUE}" | gh secret set "${SECRET_NAME}" --app dependabot
    else
        echo "${SECRET_VALUE}" | gh variable set "${SECRET_NAME}"
    fi

    echo "${SECRET_NAME}='${SECRET_VALUE}'" >>.deploy.env
}
cat /dev/null >.deploy.env
storeSecret APP_PROJECT_SLUG
storeSecret GH_ENCRYPT_KEY true
storeSecret AUTH0_DOMAIN
storeSecret AUTH0_CLIENT_ID
storeSecret AUTH0_CLIENT_SECRET true
storeSecret NORDIGEN_SECRET_ID
storeSecret NORDIGEN_SECRET_KEY true
storeSecret ARM_LOCATION
storeSecret ARM_SUBSCRIPTION_ID
storeSecret ARM_TENANT_ID
storeSecret ARM_CLIENT_ID
storeSecret ARM_CLIENT_SECRET true
