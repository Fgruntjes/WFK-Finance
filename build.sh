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


set +x

docker build App.Backend \
    --tag "${GOOGLE_REGION}-docker.pkg.dev/${GOOGLE_PROJECT_SLUG}/docker/${APP_VERSION}/backend:${APP_VERSION}" \
    --provenance=false \
    --push
