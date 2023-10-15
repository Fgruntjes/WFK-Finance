#!/usr/bin/env bash
# Terraform wrapper that loads env variables and passes them to terraform

set -e

cd "$(dirname "$(realpath "$0")")";

# Load env variables
set -a
CURRENT_ENV=$(declare -p -x)
source .env
test -f .deploy.env && source .deploy.env
test -f .local.env && source .local.env
eval "${CURRENT_ENV}"
set +a

# Enter terraform dir
cd "terraform";

set -x

# Init and plan
ARM_STORAGE_ACCOUNT_NAME=$(echo "${APP_PROJECT_SLUG}-terraform" | sed "s/-//g")
terraform init \
    -backend-config="resource_group_name=${APP_PROJECT_SLUG}" \
    -backend-config="storage_account_name=${ARM_STORAGE_ACCOUNT_NAME}"

ACTION=${1:-"apply"}

cat > variables.tfvars <<EOF
app_project_slug = "${APP_PROJECT_SLUG}"
app_environment = "${APP_ENVIRONMENT}"
app_version = "${APP_VERSION}"
arm_location = "${ARM_LOCATION}"
arm_tenant_id = "${ARM_TENANT_ID}"
arm_subscription_id = "${ARM_SUBSCRIPTION_ID}"
arm_client_id = "${ARM_CLIENT_ID}"
arm_client_secret = "${ARM_CLIENT_SECRET}"
auth0_domain = "${AUTH0_DOMAIN}"
nordigen_secret_id = "${NORDIGEN_SECRET_ID}"
nordigen_secret_key = "${NORDIGEN_SECRET_KEY}"
EOF

if [[ "${ACTION}" == "plan" ]] || [[ "${ACTION}" == "apply" ]]; then
    terraform "plan" \
        -out="tfplan" \
        -var-file="variables.tfvars" \
        "${@:2}"
    if [[ "${ACTION}" == "apply" ]]; then
        terraform apply "tfplan"
    fi
elif [[ "${ACTION}" == "import" ]]; then
    terraform import \
        -var-file="variables.tfvars" \
        "${@:2}"
else
    terraform "${@:1}"
fi