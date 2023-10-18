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

# Registry name
ACR_REPOSITORY_NAME="${APP_PROJECT_SLUG//-/}"

az acr run \
    --registry "${ACR_REPOSITORY_NAME}" \
    --cmd 'acr purge --keep 2 --ago 8h --filter ".*:.*"' \
    /dev/null
