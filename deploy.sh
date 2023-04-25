#!/usr/bin/env bash

set -e

cd "$(dirname "$(realpath "$0")")";

# Load env variables
set -a
source .env
source .deploy.env
test -f .local.env && source .local.env
set +a

pulumi stack select "${APP_ENVIRONMENT}" \
    --cwd "App.Deploy" \
    --create \
    --non-interactive

pulumi up \
    --cwd "App.Deploy" \
    --non-interactive \
    --config "backend.url=gs://${GOOGLE_PROJECT_SLUG}-pulumi" \
    --config "gcp:project=${GOOGLE_PROJECT_SLUG}" \
    --config "gcp:region=${GOOGLE_REGION}" \
    --config "app:auth_domain=${MONGODB_PROJECT_ID}" \
    --config "app:mongodb_project_id=${MONGODB_PROJECT_ID}" \
    --config "app:google_region=${GOOGLE_REGION}" \
    --config "app:environment=${APP_ENVIRONMENT}" \
    --config "app:auth0_domain=${AUTH0_DOMAIN}" \
    --config "app:nordigen_secret_id=${NORDIGEN_SECRET_ID}" \
    --config "app:nordigen_secret_key=${NORDIGEN_SECRET_KEY}" \
    --yes \
    --show-full-output \
    --show-config \
    --diff \
    --stack "${APP_ENVIRONMENT}" \
    "${@:2}"