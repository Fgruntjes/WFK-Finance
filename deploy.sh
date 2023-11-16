#!/usr/bin/env bash
# Terraform wrapper that loads env variables and passes them to terraform

cd "$(dirname "$(realpath "$0")")"

# Load env variables
set -a
CURRENT_ENV=$(declare -p -x)
source .env
test -f .deploy.env && source .deploy.env
test -f .local.env && source .local.env
eval "${CURRENT_ENV}"
set +a

# Enter terraform dir
cd "terraform"

set -x
set -e

ARM_STORAGE_ACCOUNT_NAME=$(echo "${APP_PROJECT_SLUG}-terraform" | sed "s/-//g")
ACTION=${1:-"apply"}

# Init and plan
if [[ "${ACTION}" == "init" ]]; then
    terraform init \
        -backend-config="resource_group_name=${APP_PROJECT_SLUG}" \
        -backend-config="storage_account_name=${ARM_STORAGE_ACCOUNT_NAME}" \
        -backend-config="key=${APP_ENVIRONMENT}.tfstate" \
        "${@:2}"
    exit
else
    terraform init \
        -backend-config="resource_group_name=${APP_PROJECT_SLUG}" \
        -backend-config="storage_account_name=${ARM_STORAGE_ACCOUNT_NAME}" \
        -backend-config="key=${APP_ENVIRONMENT}.tfstate" \
        -input=false
fi

GITHUB_REPOSITORY_OWNER=$(echo "${GITHUB_REPOSITORY}" | cut -d'/' -f1 | tr '[:upper:]' '[:lower:]')
GITHUB_REPOSITORY_NAME=$(echo "${GITHUB_REPOSITORY}" | cut -d'/' -f2)

cat >variables.tfvars <<EOF
app_project_slug = "${APP_PROJECT_SLUG}"
app_environment = "${APP_ENVIRONMENT}"
app_version = "${APP_VERSION}"
app_frontend_url = "https://${GITHUB_REPOSITORY_OWNER}.github.io/${GITHUB_REPOSITORY_NAME}"
arm_location = "${ARM_LOCATION}"
arm_tenant_id = "${ARM_TENANT_ID}"
arm_subscription_id = "${ARM_SUBSCRIPTION_ID}"
arm_client_id = "${ARM_CLIENT_ID}"
arm_client_secret = "${ARM_CLIENT_SECRET}"
auth0_domain = "${AUTH0_DOMAIN}"
auth0_client_id = "${AUTH0_CLIENT_ID}"
auth0_client_secret = "${AUTH0_CLIENT_SECRET}"
nordigen_secret_id = "${NORDIGEN_SECRET_ID}"
nordigen_secret_key = "${NORDIGEN_SECRET_KEY}"
EOF
cat variables.tfvars

if [[ "${ACTION}" == "plan" ]] || [[ "${ACTION}" == "apply" ]]; then
    # In case the database or server is deleted outside of terraform we produce this error
    # to prevent we are recreating the database and server before we are doing a new plan / apply
    # Error: unable to read user [dev-backend].[dev-backend-database-read-write]: db connection failed after 30s timeout
    terraform "plan" \
        -out="database.tfplan" \
        -var-file="variables.tfvars" \
        -input=false \
        -target azurerm_mssql_server.backend_database \
        -target azurerm_mssql_database.backend_database \
        -target time_sleep.database_create

    if [[ "${ACTION}" == "apply" ]]; then
        terraform apply \
            -auto-approve \
            -target azurerm_mssql_server.backend_database \
            -target azurerm_mssql_database.backend_database \
            -target time_sleep.database_create \
            "database.tfplan"
    fi

    terraform "plan" \
        -out="tfplan" \
        -var-file="variables.tfvars" \
        -input=false \
        "${@:2}"

    if [[ "${ACTION}" == "apply" ]]; then
        terraform apply \
            -auto-approve \
            "tfplan"

        # Output variables to github ci
        if [[ -z "${GITHUB_OUTPUT}" ]]; then
            GITHUB_OUTPUT=".github_output"
        fi

        terraform output -json | jq -r 'to_entries[] | "\(.key)=\(.value.value)"' | while read -r OUTPUT_LINE; do
            VARIABLE_KEY=$(echo "$OUTPUT_LINE" | cut -d'=' -f1)
            VARIABLE_VALUE=$(echo "$OUTPUT_LINE" | cut -d'=' -f2)
            echo "$VARIABLE_KEY=$VARIABLE_VALUE" >>"$GITHUB_OUTPUT"
        done
    fi
elif [[ "${ACTION}" == "destroy" ]]; then
    if [[ "${APP_ENVIRONMENT}" == "main" ]]; then
        echo "Main environment can not be destroyed"
        exit 1
    fi
    terraform destroy \
        -var-file="variables.tfvars" \
        "${@:2}"
elif [[ "${ACTION}" == "import" ]] || [[ "${ACTION}" == "destroy" ]]; then
    terraform "${ACTION}" \
        -var-file="variables.tfvars" \
        "${@:2}"
else
    terraform "${@:1}"
fi
