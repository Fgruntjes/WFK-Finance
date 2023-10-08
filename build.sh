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

APP_PROJECT_CONTAINER_REGISTRY=$(echo "${APP_PROJECT_SLUG}" | sed "s/-//g")
APP_VERSION_IMAGE="${APP_PROJECT_CONTAINER_REGISTRY}.azurecr.io/${APP_ENVIRONMENT}/backend:${APP_VERSION}"

docker build App.Backend \
    --tag "${APP_VERSION_IMAGE}" \
    --provenance=false \
    --push