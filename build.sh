#!/usr/bin/env bash
# shellcheck source=.env
# shellcheck source=.deploy.env
# shellcheck source=.local.env

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

if [[ -z "${APP_VERSION}" ]]; then
  APP_VERSION="dev-$(git describe --tags --always)-$(date +%Y%m%d%H%M%S)"
  echo "APP_VERSION=${APP_VERSION}" >> .deploy.env
fi

echo "## Running build on ${APP_ENVIRONMENT} ${APP_VERSION} ##"

CONTAINER_REGISTRY_HOSTNAME=${APP_PROJECT_SLUG//-/}
CONTAINER_REGISTRY="${CONTAINER_REGISTRY_HOSTNAME}.azurecr.io"

function build {
    TARGET=$1
    if [[ "${TARGET}" == "frontend" ]]; then
        export NODE_ENV=production
        cd frontend
        pnpm install \
            --frozen-lockfile \
            --dev
        pnpm run build
    else
        docker_build $TARGET
    fi
}

function docker_build {
    TARGET=$1
    IMAGE=$(echo "${TARGET}" | tr '[:upper:]' '[:lower:]')
    REGISTRY="${CONTAINER_REGISTRY}"

    # check if we need to login, if so do it
    az acr login --name "${CONTAINER_REGISTRY_HOSTNAME}"

    docker buildx build . \
        --file "${TARGET}/Dockerfile" \
        --tag "${REGISTRY}/${IMAGE}:${APP_VERSION}" \
        --tag "${IMAGE}:${APP_VERSION}" \
        --tag "${IMAGE}" \
        --label "org.opencontainers.image.created=$(date --rfc-3339=seconds --utc)" \
        --label "org.opencontainers.image.revision=${APP_VERSION}" \
        --label "environment=${APP_ENVIRONMENT}" \
        --cache-from "type=gha,scope=${GITHUB_REF_NAME}-${TARGET}" \
        --cache-to "type=gha,mode=max,scope=${GITHUB_REF_NAME}-${TARGET}" \
        --provenance=false \
        --load
    docker push "${REGISTRY}/${IMAGE}:${APP_VERSION}"
}

if [ "$#" -ne 0 ]; then
    for arg in "$@"; do
        build $arg
    done
else
    build App.Backend
    build App.DataMigrations
    build frontend
fi
