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

# Deployed environments
DEPLOYED_ENVIRONMENTS=$(az resource list --tag "environment" | jq -r '.[].tags.environment' | sort | uniq)

# Fetch all branches
git fetch --depth=1 origin +refs/heads/*:refs/remotes/origin/*

# Loop over deployed environments
for ENVIRONMENT in $DEPLOYED_ENVIRONMENTS; do
    if [[ $ENVIRONMENT == *"-merge" ]]; then
        PR_NUMBER="${ENVIRONMENT%-merge}"
        PR_STATE=$(gh pr view "$PR_NUMBER" --repo "${GITHUB_REPOSITORY}" --json state --jq '.state')
        # Check if there is an open pull request with the PR number
        if [[ "${PR_STATE}" == "OPEN" ]]; then
            echo "Open pull request found for PR number $PR_NUMBER, skipping"
        else
            echo "No open pull request found for PR number $PR_NUMBER"
            delete_environment "$ENVIRONMENT"
        fi
    else
        # Check if branch exists with the same name as the environment
        if git show-ref --verify --quiet "refs/remotes/origin/$ENVIRONMENT"; then
            echo "Branch $ENVIRONMENT exists, skipping"
        elif [[ "$ENVIRONMENT" == "dev" ]]; then
            echo "Dev environment, skipping deletion"
        else
            echo "Branch $ENVIRONMENT does not exist"
            delete_environment "$ENVIRONMENT"
        fi
    fi
done


# Purge images registry
echo "## Delete images no longer in use"
# Fetch all images in use
IMAGES_IN_USE=$(
    az containerapp list |
        jq -r '.[].properties.template.containers[].image' |
        awk -F ':' '{print $2}' |
        tr '\n' '|'
)

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


# Deploy frontends
echo "::warning file=cleanup.sh,::Delete frontends not implmented yet"
