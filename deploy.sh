#!/usr/bin/env bash
# Terraform wrapper that loads env variables and passes them to terraform

cd "$(dirname "$(realpath "$0")")"

# Load env variables
set -a
CURRENT_ENV=$(declare -p -x)
# shellcheck source=/dev/null
source .env
# shellcheck source=/dev/null
test -f .deploy.env && source .deploy.env
# shellcheck source=/dev/null
test -f .local.env && source .local.env
eval "${CURRENT_ENV}"
set +a

# Enter terraform dir
cd "terraform"

set -e

ARM_STORAGE_ACCOUNT_NAME=$(echo "${APP_PROJECT_SLUG}-terraform" | sed "s/-//g")
ACTION=${1:-"apply"}

echo "## Running deploy on ${APP_ENVIRONMENT} ${APP_VERSION} - action ${ACTION} ##"

# Init and plan
echo "## Terraform init ##"
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
        -input=false \
        -reconfigure
fi

GITHUB_REPOSITORY_OWNER=$(echo "${GITHUB_REPOSITORY}" | cut -d'/' -f1 | tr '[:upper:]' '[:lower:]')
GITHUB_REPOSITORY_NAME=$(echo "${GITHUB_REPOSITORY}" | cut -d'/' -f2)

cat >variables.tfvars <<EOF
app_project_slug = "${APP_PROJECT_SLUG}"
app_environment = "${APP_ENVIRONMENT}"
app_version = "${APP_VERSION}"
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
sentry_organisation = "${SENTRY_ORGANISATION}"
sentry_token = "${SENTRY_TOKEN}"

EOF

function mssql_user_reimport {
    DATABASE="${APP_ENVIRONMENT}-backend"
    RESOURCE="mssql_user.${1}"
    USER="${2}"

    SERVER_HOST="${APP_PROJECT_SLUG}-${APP_ENVIRONMENT}.database.windows.net"
    RESOURCE_ID="mssql://${SERVER_HOST}/${DATABASE}/${USER}"

    echo "### Remove state ${RESOURCE}"
    terraform state rm "${RESOURCE}" || true

    if nslookup "${SERVER_HOST}" >/dev/null; then
        echo "### Import state ${RESOURCE} @ ${RESOURCE_ID}"
        MSSQL_TENANT_ID="${ARM_TENANT_ID}" \
            MSSQL_CLIENT_ID="${ARM_CLIENT_ID}" \
            MSSQL_CLIENT_SECRET="${ARM_CLIENT_SECRET}" \
            terraform import \
            -var-file="variables.tfvars" \
            -input=false \
            "${RESOURCE}" \
            "${RESOURCE_ID}" || true
    else
        echo "### Import state ${RESOURCE} @ ${RESOURCE_ID} SKIPPED - DNS $SERVER_HOST does not exist"
    fi
}

function refresh_mssql_users {
    echo "## Delete / reimport mssql_user states ##"
    # The mssql_user state is not updated when a server is deleted.
    # This causes Error: unable to read user [...].[...]: db connection failed after 30s timeout
    mssql_user_reimport backend_database_migrations "${APP_ENVIRONMENT}-db-owner"
    mssql_user_reimport read_write "${APP_ENVIRONMENT}-db-read-write"
    if [[ "${APP_ENVIRONMENT}" != "main" ]]; then
        mssql_user_reimport integration_test_admin[0] test-admin
    fi
}

if [[ "${ACTION}" == "plan" ]] || [[ "${ACTION}" == "apply" ]]; then
    refresh_mssql_users

    echo "## Planning ##"
    terraform "plan" \
        -out="tfplan" \
        -var-file="variables.tfvars" \
        -input=false \
        "${@:2}"

    if [[ "${ACTION}" == "apply" ]]; then
        echo "## Applying ##"
        terraform apply \
            -auto-approve \
            "tfplan"

        # Output variables to github ci
        if [[ -z "${GITHUB_OUTPUT}" ]]; then
            GITHUB_OUTPUT="$(pwd)/.github_output"
            echo "## Create github output file ${GITHUB_OUTPUT} ##"
            cat /dev/null >"${GITHUB_OUTPUT}"
        fi

        echo "## Setting github outputs"
        terraform output -json | jq -c -r 'to_entries[]' | while read -r OUTPUT_LINE; do
            VARIABLE_KEY=$(echo "${OUTPUT_LINE}" | jq -r '.key')
            VARIABLE_VALUE=$(echo "${OUTPUT_LINE}" | jq -r '.value.value')
            VARIABLE_SENSITIVE=$(echo "${OUTPUT_LINE}" | jq -r '.value.sensitive')

            if [ "${VARIABLE_SENSITIVE}" = true ]; then
                echo "::add-mask::${VARIABLE_VALUE}"

                VARIABLE_VALUE=$(echo -n "$VARIABLE_VALUE" | openssl enc -pbkdf2 -a -salt -pass "pass:$GH_ENCRYPT_KEY" | base64 -w 0)

                echo " - ${VARIABLE_KEY}=****"
            else
                echo " - ${VARIABLE_KEY}=${VARIABLE_VALUE}"
            fi

            echo "${VARIABLE_KEY}=${VARIABLE_VALUE}" >>"${GITHUB_OUTPUT}"
        done
    fi
elif [[ "${ACTION}" == "destroy" ]]; then
    if [[ "${APP_ENVIRONMENT}" == "main" && "${APP_DESTROY_MAIN}" != "force" ]]; then
        echo "Main environment can only be destroyed when APP_DESTROY_MAIN is set to 'force'"
        exit 1
    fi

    refresh_mssql_users

    echo "## Destroying environment ##"
    terraform destroy -var-file="variables.tfvars" "${@:2}"
elif [[ "${ACTION}" == "import" ]] || [[ "${ACTION}" == "destroy" ]]; then
    echo "## Running ${ACTION} with var-file ##"
    terraform "${ACTION}" \
        -var-file="variables.tfvars" \
        "${@:2}"
else
    echo "## Running ${ACTION} ##"
    terraform "${@:1}"
fi
