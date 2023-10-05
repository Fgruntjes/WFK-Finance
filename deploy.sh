#!/usr/bin/env bash

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

# Init and plan
ARM_STORAGE_ACCOUNT_NAME=$(echo "${APP_PROJECT_SLUG}-terraform" | sed "s/-//g")
terraform init \
    -backend-config="resource_group_name=${APP_PROJECT_SLUG}" \
    -backend-config="storage_account_name=${ARM_STORAGE_ACCOUNT_NAME}"

ACTION=${1:-"apply"}

if [[ "${ACTION}" == "apply" ]]; then
    terraform plan \
        -out=tfplan \
        -var "app_project_slug=${APP_PROJECT_SLUG}" \
        -var "app_environment=${APP_ENVIRONMENT}" \
        -var "app_version=${APP_VERSION}" \
        -var "arm_location=${ARM_LOCATION}" \
        -var "arm_tenant_id=${ARM_TENANT_ID}" \
        -var "arm_subscription_id=${ARM_SUBSCRIPTION_ID}" \
        -var "arm_client_id=${ARM_CLIENT_ID}" \
        -var "arm_client_secret=${ARM_CLIENT_SECRET}" \
        -var "auth0_domain=${AUTH0_DOMAIN}" \
        -var "nordigen_secret_id=${NORDIGEN_SECRET_ID}" \
        -var "nordigen_secret_key=${NORDIGEN_SECRET_KEY}"
        
    terraform apply "tfplan"
else
    terraform destroy
fi