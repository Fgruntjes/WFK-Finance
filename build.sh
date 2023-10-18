#!/usr/bin/env bash

set -e

cd "$(dirname "$(realpath "$0")")"

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

function build {
    TARGET=$1
    if [[ "${TARGET}" == "frontend" ]]; then
        export NODE_ENV=production
        cd frontend
        npm run build
    else
        docker_build $TARGET
    fi
}

function docker_build {
    TARGET=$1
    IMAGE=$(echo "${TARGET}" | tr '[:upper:]' '[:lower:]')
    REGISTRY="${CONTAINER_REGISTRY}"
    docker build . \
        --file "${TARGET}/Dockerfile" \
        --tag "${REGISTRY}/${IMAGE}:${APP_VERSION}" \
        --tag "${IMAGE}:${APP_VERSION}" \
        --tag "${IMAGE}" \
        --label "org.opencontainers.image.created=$(date --rfc-3339=seconds --utc)" \
        --label "org.opencontainers.image.revision=${APP_VERSION}" \
        --label "environment=${APP_ENVIRONMENT}" \
        --provenance=false
    docker push "${REGISTRY}/${IMAGE}:${APP_VERSION}"
}

if [ "$#" -ne 0 ]; then
    for arg in "$@"; do
        build $arg
    done
else
    build App.Backend
    build App.Data.Migrations
    build frontend
fi
