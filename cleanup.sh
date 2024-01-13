#!/usr/bin/env bash
set -e

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

function delete_environment {
    export APP_ENVIRONMENT=$1
    export APP_VERSION="cleanup"
    echo "## Delete environment $APP_ENVIRONMENT"
    ./deploy.sh destroy -auto-approve
}

declare -A ENVIRONMENT_CACHE
function environment_in_use {
    ENVIRONMENT=$1

    if [[ -n "${ENVIRONMENT_CACHE[$ENVIRONMENT]}" ]]; then
        echo "Cached result for $ENVIRONMENT: ${ENVIRONMENT_CACHE[$ENVIRONMENT]}"
        # Return cached exit status
        return "${ENVIRONMENT_CACHE[$ENVIRONMENT]}"
    fi

    if [[ $ENVIRONMENT == *"-merge" ]]; then
        PR_NUMBER="${ENVIRONMENT%-merge}"
        PR_STATE=$(gh pr view "$PR_NUMBER" --repo "${GITHUB_REPOSITORY}" --json state --jq '.state')
        # Check if there is an open pull request with the PR number
        if [[ "${PR_STATE}" == "OPEN" ]]; then
            echo "Open pull request found for PR number $PR_NUMBER, skipping"
            ENVIRONMENT_CACHE[$ENVIRONMENT]=1
            return 1
        else
            echo "No open pull request found for PR number $PR_NUMBER"
            ENVIRONMENT_CACHE[$ENVIRONMENT]=0
            return 0
        fi
    else
        # Check if branch exists with the same name as the environment
        if git show-ref --verify --quiet "refs/remotes/origin/$ENVIRONMENT"; then
            echo "Branch $ENVIRONMENT exists, skipping"
            ENVIRONMENT_CACHE[$ENVIRONMENT]=1
            return 1
        elif [[ "$ENVIRONMENT" == "dev" ]]; then
            echo "Dev environment, skipping deletion"
            ENVIRONMENT_CACHE[$ENVIRONMENT]=1
            return 1
        else
            echo "Branch $ENVIRONMENT does not exist"
            ENVIRONMENT_CACHE[$ENVIRONMENT]=0
            return 0
        fi
    fi
}

function clean_backend_deployments {
    echo "## Cleaning backend deployments"

    # Deployed environments
    DEPLOYED_ENVIRONMENTS=$(az resource list --tag "environment" | jq -r '.[].tags.environment' | sort | uniq)

    # Fetch all branches
    git fetch --depth=1 origin +refs/heads/*:refs/remotes/origin/*

    # Loop over deployed environments
    for ENVIRONMENT in $DEPLOYED_ENVIRONMENTS; do
        if ! environment_in_use "$ENVIRONMENT"; then
            echo "Environment $ENVIRONMENT is in use, skipping"
        else
            delete_environment "$ENVIRONMENT"
        fi
    done
}

function clean_container_registry {
    echo "## Cleaning container registry"

    # Purge images registry
    echo "## Delete images no longer in use"
    # Fetch all images in use
    IMAGES_IN_USE=$(
        az containerapp list |
            jq -r '.[].properties.template.containers[].image' |
            awk -F ':' '{print $2}' |
            sort |
            uniq |
            tr '\n' '|'
    )
    IMAGES_IN_USE=${IMAGES_IN_USE%|}

    echo "  images in use: ${IMAGES_IN_USE}"
    IMAGE_REPOSITORIES=$(az acr repository list --name "${APP_PROJECT_SLUG//-/}" --output tsv)
    for IMAGE_REPOSITORY in $IMAGE_REPOSITORIES; do
        IMAGE_TAGS=$(az acr repository show-tags --name "${APP_PROJECT_SLUG//-/}" --repository "${IMAGE_REPOSITORY}" --output tsv)
        for IMAGE_TAG in $IMAGE_TAGS; do
            IMAGE="${IMAGE_REPOSITORY}:${IMAGE_TAG}"
            
            # Check if image is in IMAGES_IN_USE
            if [[ "${IMAGES_IN_USE}" == *"${IMAGE_TAG}"* ]]; then
                echo " SKIPPED ${IMAGE}"
                continue
            else
                echo " DELETE ${IMAGE}"
                az acr repository delete --name "${APP_PROJECT_SLUG//-/}" --image "${IMAGE}" --yes
            fi
        done
    done
}

function clean_frontend_deployments {
    echo "## Cleaning frontend deployments"

    CLOUDFLARE_DEPLOYMENTS_URL="https://api.cloudflare.com/client/v4/accounts/${CLOUDFLARE_ACCOUNT_ID}/pages/projects/${APP_PROJECT_SLUG}/deployments"

    # Delete old frontend deployments, can not delete deployments of environments still in use, since we do not have a way to identify them
    CLOUDFLARE_DEPLOYMENTS_PAGE=1
    while true; do
        CLOUDFLARE_DEPLOYMENTS=$(curl -s -X GET "${CLOUDFLARE_DEPLOYMENTS_URL}?page=${CLOUDFLARE_DEPLOYMENTS_PAGE}&per_page=25" \
            -H "Authorization: Bearer ${CLOUDFLARE_API_TOKEN}" \
            -H "Content-Type: application/json")

        echo "${CLOUDFLARE_DEPLOYMENTS}" | jq -c '.result[]' | while read -r CLOUDFLARE_DEPLOYMENT; do
            DEPLOYMENT_ENVIRONMENT=$(echo "$CLOUDFLARE_DEPLOYMENT" | jq -r '.deployment_trigger.metadata.branch')
            DEPLOYMENT_ID=$(echo "$CLOUDFLARE_DEPLOYMENT" | jq -r '.id')

            echo

            CLOUDFLARE_DEPLOYMENT_URL="${CLOUDFLARE_DEPLOYMENTS_URL}/${DEPLOYMENT_ID}"
            if environment_in_use "$DEPLOYMENT_ENVIRONMENT"; then
                echo " DELETE ${DEPLOYMENT_ENVIRONMENT} - ${DEPLOYMENT_ID}"
                curl -s -X DELETE "${CLOUDFLARE_DEPLOYMENT_URL}?force=true" \
                    -H "Authorization: Bearer ${CLOUDFLARE_API_TOKEN}" \
                    -H "Content-Type: application/json"
            fi
        done

        ((CLOUDFLARE_DEPLOYMENTS_PAGE++))
        if [[ $(echo "${CLOUDFLARE_DEPLOYMENTS}" | jq -r '.result | length') -eq 0 ]]; then
            break
        fi
    done
}

ACTION=${1:-"all"}
if [[ "${ACTION}" == "backend" ]]; then
    clean_backend_deployments
elif [[ "${ACTION}" == "container-registry" ]]; then
    clean_container_registry
elif [[ "${ACTION}" == "frontend" ]]; then
    clean_frontend_deployments
elif [[ "${ACTION}" == "all" ]]; then
    clean_backend_deployments
    echo ""
    echo ""
    clean_container_registry
    echo ""
    echo ""
    clean_frontend_deployments
    echo ""
    echo ""
else
    echo "Supported actions: cleanup.sh {backend, container-registry, frontend, all}"
fi
