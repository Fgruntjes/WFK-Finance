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

set -x

CONTAINER_REGISTRY_HOSTNAME="$(echo "${APP_PROJECT_SLUG}" | sed "s/-//g").azurecr.io"
CONTAINER_REGISTRY="${CONTAINER_REGISTRY_HOSTNAME}/${APP_ENVIRONMENT}"

# check if we need to login, if so do it
# az acr login --name ${APP_PROJECT_CONTAINER_REGISTRY}

function docker_build {
    TARGET=$1
    IMAGE=$(echo "${TARGET}" | tr '[:upper:]' '[:lower:]')
    REGISTRY="${CONTAINER_REGISTRY}"
    docker build . \
        --file "${TARGET}/Dockerfile" \
        --tag "${REGISTRY}/${IMAGE}:${APP_VERSION}" \
        --tag "${IMAGE}:${APP_VERSION}" \
        --tag "${IMAGE}" \
        --provenance=false
    docker push "${REGISTRY}/${IMAGE}:${APP_VERSION}"
}

if [ "$#" -ne 0 ]; then
    for arg in "$@"; do
        docker_build $arg
    done
else
    docker_build App.Backend
    docker_build App.Data.Migrations
fi