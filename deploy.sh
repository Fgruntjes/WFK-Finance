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

pulumi login gs://${GOOGLE_PROJECT_SLUG}-pulumi \
    --non-interactive \
    -v=5

pulumi stack select "${APP_ENVIRONMENT}" \
    --cwd "App.Deploy" \
    --create \
    --non-interactive
PULUMI_ACTION=${1:-"up"}

if [[ "${PULUMI_ACTION}" == "up" ]]; then
    pulumi ${PULUMI_ACTION} \
        --cwd "App.Deploy" \
        --non-interactive \
        --yes \
        --stack "${APP_ENVIRONMENT}" \
        --config "backend.url=gs://${GOOGLE_PROJECT_SLUG}-pulumi" \
        --config "gcp:project=${GOOGLE_PROJECT_SLUG}" \
        --config "gcp:region=${GOOGLE_REGION}" \
        --config "app:auth_domain=${MONGODB_PROJECT_ID}" \
        --config "app:mongodb_project_id=${MONGODB_PROJECT_ID}" \
        --config "app:google_region=${GOOGLE_REGION}" \
        --config "app:google_project_slug=${GOOGLE_PROJECT_SLUG}" \
        --config "app:environment=${APP_ENVIRONMENT}" \
        --config "app:version=${APP_VERSION}" \
        --config "app:auth0_domain=${AUTH0_DOMAIN}" \
        --config "app:nordigen_secret_id=${NORDIGEN_SECRET_ID}" \
        --config "app:nordigen_secret_key=${NORDIGEN_SECRET_KEY}" \
        --config "app:cloudflare_account_id=${CLOUDFLARE_ACCOUNT_ID}" \
        --config "app:cloudflare_api_token=${CLOUDFLARE_API_TOKEN}" \
        --show-full-output \
        --show-config \
        --diff \
        "${@:2}"
else
    pulumi ${PULUMI_ACTION} \
        --cwd "App.Deploy" \
        --non-interactive \
        --yes \
        --stack "${APP_ENVIRONMENT}" \
        "${@:2}"
fi