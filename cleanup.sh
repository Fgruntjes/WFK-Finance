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

# Deployed environments
DEPLOYED_ENVIRONMENTS=$(az resource list --tag "environment" | jq -r '.[].tags.environment' | sort | uniq)

# Loop over deployed environments
for ENVIRONMENT in $DEPLOYED_ENVIRONMENTS; do
    if [[ $ENVIRONMENT == *"-merge" ]]; then
        PR_NUMBER="${ENVIRONMENT%-merge}"
        # Check if there is an open pull request with the PR number
        if gh pr view "$PR_NUMBER" --repo "${GITHUB_REPOSITORY}" --json state --jq '.state!"MERGED"' >/dev/null 2>&1; then
            echo "Open pull request found for PR number $PR_NUMBER"
            # Add your logic here for handling open pull requests
        else
            echo "No open pull request found for PR number $PR_NUMBER"
            # Add your logic here for handling no open pull requests
        fi
    else
        # Check if branch exists with the same name as the environment
        if git show-ref --verify --quiet "refs/heads/$ENVIRONMENT"; then
            echo "Branch $ENVIRONMENT exists"
            # Add your logic here for handling existing branches
        else
            echo "Branch $ENVIRONMENT does not exist"
            # Add your logic here for handling non-existing branches
        fi
    fi
done

# Fetch all images in use
IMAGES_IN_USE=$(
    az containerapp list |
        jq -r '.[].properties.template.containers[].image' |
        awk -F ':' '{print $2}' |
        tr '\n' '|'
)
IMAGES_IN_USE="${IMAGES_IN_USE%|}"

# Purge images registry
echo "## Delete images no longer in use"
echo "  images in use: ${IMAGES_IN_USE}"
az acr run \
    --registry "${APP_PROJECT_SLUG//-/}" \
    --cmd "acr purge --keep 2 --ago 8h --filter \".*:(${IMAGES_IN_USE})\"" \
    /dev/null

# Deploy frontends
echo "::warning file=cleanup.sh,::Delete frontends not implmented yet"
