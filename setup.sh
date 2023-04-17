#!/usr/bin/env bash

set -e

cd "$(dirname "$(realpath "$0")")";

# TODO ensure required cli tools are installed / configured: gcloud (google), gh (github)

# Load env variables
set -a
source .env
test -f .local.env && source .local.env
set +a

# Env defaults
GOOGLE_SERVICE_ACCOUNT="github-cicd"
GOOGLE_IDENTITY_POOL="github-pool"
GOOGLE_IDENTITY_PROVIDER="github-provider"
GOOGLE_IDENTIY_ROLES=(
    "roles/artifactregistry.repoAdmin"
    "roles/storage.objectAdmin"
    "roles/iam.serviceAccountUser"
    "roles/secretmanager.admin"
)

# We only want to generate password once
if [ -z "${PULUMI_CONFIG_PASSPHRASE}" ]; then
    PULUMI_CONFIG_PASSPHRASE=$(echo $RANDOM | md5sum | head -c 20; echo;)
    echo "PULUMI_CONFIG_PASSPHRASE='${PULUMI_CONFIG_PASSPHRASE}'" >> .local.env
fi

# Create google project
if ! gcloud projects describe "${GOOGLE_PROJECT_SLUG}" > /dev/null; then
    echo "Creating project: ${GOOGLE_PROJECT_SLUG}"
    gcloud projects create "${GOOGLE_PROJECT_SLUG}" --name "${GOOGLE_PROJECT_SLUG}"
else
    echo "Project already exists: ${GOOGLE_PROJECT_SLUG}"
fi
echo ""


# Link billing account
GOOGLE_BILLING_ACCOUNT_FIRST=$(gcloud beta billing accounts list --filter="open=true" --format="value(name)" --limit=1)
GOOGLE_BILLING_ACCOUNT_ID=${GOOGLE_BILLING_ACCOUNT_ID:-${GOOGLE_BILLING_ACCOUNT_FIRST}}
echo "Ensure project ${GOOGLE_PROJECT_SLUG} is linked to billing account ${GOOGLE_BILLING_ACCOUNT_ID}"
gcloud beta billing projects link "${GOOGLE_PROJECT_SLUG}" "--billing-account=${GOOGLE_BILLING_ACCOUNT_ID}"
echo ""


# Enable required services
echo "Enabling required services"
gcloud services enable iamcredentials.googleapis.com --project "${GOOGLE_PROJECT_SLUG}"
gcloud services enable artifactregistry.googleapis.com --project "${GOOGLE_PROJECT_SLUG}"
gcloud services enable run.googleapis.com --project "${GOOGLE_PROJECT_SLUG}"
gcloud services enable secretmanager.googleapis.com --project "${GOOGLE_PROJECT_SLUG}"
echo ""


# Create identity pool to for ci/cd auth
# @see (https://github.com/google-github-actions/auth#setup)
if ! gcloud iam workload-identity-pools describe --location="global" --project="${GOOGLE_PROJECT_SLUG}" "${GOOGLE_IDENTITY_POOL}" > /dev/null; then
    echo "Creating identity pool: ${GOOGLE_IDENTITY_POOL}"
    gcloud iam workload-identity-pools create "${GOOGLE_IDENTITY_POOL}" \
        --project="${GOOGLE_PROJECT_SLUG}" \
        --location="global" \
        --display-name="${GOOGLE_IDENTITY_POOL}"
else
    echo "Identity pool already exists: ${GOOGLE_IDENTITY_POOL}"
fi
GOOGLE_IDENTITY_POOL_ID=$(
    gcloud iam workload-identity-pools describe "${GOOGLE_IDENTITY_POOL}" \
        --project="${GOOGLE_PROJECT_SLUG}" \
        --location="global" \
        --format="value(name)"
)
echo ""

if ! gcloud iam workload-identity-pools providers describe \
    --workload-identity-pool="${GOOGLE_IDENTITY_POOL}" \
    --location="global" \
    --project="${GOOGLE_PROJECT_SLUG}" "${GOOGLE_IDENTITY_PROVIDER}" > /dev/null
then
    echo "Creating identity pool provider: ${GOOGLE_IDENTITY_PROVIDER}"
    gcloud iam workload-identity-pools providers create-oidc "${GOOGLE_IDENTITY_PROVIDER}" \
        --project="${GOOGLE_PROJECT_SLUG}" \
        --location="global" \
        --workload-identity-pool="${GOOGLE_IDENTITY_POOL}" \
        --display-name="${GOOGLE_IDENTITY_PROVIDER}" \
        --attribute-mapping="google.subject=assertion.sub,attribute.actor=assertion.actor,attribute.repository=assertion.repository" \
        --issuer-uri="https://token.actions.githubusercontent.com"
else
    echo "Identity pool provider already exists: ${GOOGLE_IDENTITY_PROVIDER}"
fi
echo ""


# Add service account and permissions
GOOGLE_SERVICE_ACCOUNT_EMAIL="${GOOGLE_SERVICE_ACCOUNT}@${GOOGLE_PROJECT_SLUG}.iam.gserviceaccount.com"
if ! gcloud iam service-accounts describe --project="${GOOGLE_PROJECT_SLUG}" "${GOOGLE_SERVICE_ACCOUNT_EMAIL}" > /dev/null 2>&1; then
    echo "Creating service account: ${GOOGLE_SERVICE_ACCOUNT}"
    gcloud iam service-accounts create --project="${GOOGLE_PROJECT_SLUG}" ${GOOGLE_SERVICE_ACCOUNT}
else
    echo "Service account already exists: ${GOOGLE_SERVICE_ACCOUNT}"
fi

echo "Setting gcloud roles"
gcloud iam service-accounts add-iam-policy-binding "${GOOGLE_SERVICE_ACCOUNT_EMAIL}" \
    --project="${GOOGLE_PROJECT_SLUG}" \
    --role="roles/iam.workloadIdentityUser" \
    --member="principalSet://iam.googleapis.com/${GOOGLE_IDENTITY_POOL_ID}/attribute.repository/${GITHUB_REPOSITORY}"

for GOOGLE_IDENTIY_ROLE in "${GOOGLE_IDENTIY_ROLES[@]}"; do
	  gcloud projects add-iam-policy-binding "${GOOGLE_PROJECT_SLUG}" \
        --role="${GOOGLE_IDENTIY_ROLE}" \
        --member="serviceAccount:${GOOGLE_SERVICE_ACCOUNT_EMAIL}"
done
GOOGLE_WORKLOAD_IDENTITY_PROVIDER=$(
    gcloud iam workload-identity-pools providers describe "${GOOGLE_IDENTITY_PROVIDER}" \
        --project="${GOOGLE_PROJECT_SLUG}" \
        --location="global" \
        --workload-identity-pool="${GOOGLE_IDENTITY_POOL}" \
        --format="value(name)"
)
echo ""


# Create pulumi state bucket
if ! gcloud storage buckets describe --project="${GOOGLE_PROJECT_SLUG}" "gs://${GOOGLE_PROJECT_SLUG}-pulumi" > /dev/null; then
    echo "Creating pulumi state bucket"
    gcloud storage buckets create "gs://${GOOGLE_PROJECT_SLUG}-pulumi" \
        --no-public-access-prevention \
        --location="${GOOGLE_REGION}" \
        --project="${GOOGLE_PROJECT_SLUG}"
        
    gcloud storage buckets update "gs://${GOOGLE_PROJECT_SLUG}-pulumi" \
      --versioning \
      --lifecycle-file=App.Deploy/pulumi-state-bucket-lifecycle.json
else
    echo "Pulumi state bucket already created"
fi
echo ""


# Ensure Github secrets are set
echo "Creating github secrets"
function storeSecret {
    SECRET_NAME="${1}"
    SECRET_VALUE="${!SECRET_NAME}"
    IS_SECRET="${2}"
    if [ -z "${SECRET_VALUE}" ]; then
        echo "Missing environment variable '${SECRET_NAME}', please configure one in your '.env.local' file."
        echo "${SECRET_VALUE}"
        exit 1            
    fi
    
    if [ $IS_SECRET ]; then
        echo "${SECRET_VALUE}" | gh secret set "${SECRET_NAME}" --app actions
        echo "${SECRET_VALUE}" | gh secret set "${SECRET_NAME}" --app dependabot
    else
        echo "${SECRET_VALUE}" | gh variable set "${SECRET_NAME}"
    fi
    
    echo "${SECRET_NAME}='${SECRET_VALUE}'" >> .deploy.env
}
cat /dev/null > .deploy.env
storeSecret GOOGLE_WORKLOAD_IDENTITY_PROVIDER
storeSecret GOOGLE_SERVICE_ACCOUNT_EMAIL
storeSecret GOOGLE_REGION
storeSecret GOOGLE_PROJECT_SLUG
storeSecret PULUMI_CONFIG_PASSPHRASE true
storeSecret AUTH0_DOMAIN
storeSecret AUTH0_CLIENT_ID
storeSecret AUTH0_CLIENT_SECRET true
storeSecret MONGODB_PROJECT_ID
storeSecret MONGODB_ATLAS_PUBLIC_KEY
storeSecret MONGODB_ATLAS_PRIVATE_KEY true
storeSecret NORDIGEN_SECRET_ID
storeSecret NORDIGEN_SECRET_KEY true
