#!/bin/bash
# ============================================================
# AuditorPRO TI — Azure Deployment Script
# Usage: ./infra/scripts/deploy.sh <environment> <tenant-id> <client-id>
# ============================================================

set -euo pipefail

ENVIRONMENT=${1:-"dev"}
TENANT_ID=${2:?"Tenant ID required"}
CLIENT_ID=${3:?"Client ID required"}
RESOURCE_GROUP="rg-auditorpro-${ENVIRONMENT}"
LOCATION="eastus"

echo "============================================"
echo " AuditorPRO TI — Deploying to Azure"
echo " Environment: ${ENVIRONMENT}"
echo " Resource Group: ${RESOURCE_GROUP}"
echo "============================================"

# Create resource group if not exists
az group create --name "${RESOURCE_GROUP}" --location "${LOCATION}" \
  --tags application="AuditorPRO TI" environment="${ENVIRONMENT}"

echo "[1/4] Resource group ready."

# Deploy Bicep template
DEPLOYMENT_OUTPUT=$(az deployment group create \
  --resource-group "${RESOURCE_GROUP}" \
  --template-file infra/bicep/main.bicep \
  --parameters \
    environment="${ENVIRONMENT}" \
    tenantId="${TENANT_ID}" \
    clientId="${CLIENT_ID}" \
  --query properties.outputs \
  --output json)

echo "[2/4] Infrastructure deployed."

# Extract outputs
API_URL=$(echo "${DEPLOYMENT_OUTPUT}" | jq -r '.apiAppUrl.value')
FRONTEND_URL=$(echo "${DEPLOYMENT_OUTPUT}" | jq -r '.frontendAppUrl.value')
KV_NAME=$(echo "${DEPLOYMENT_OUTPUT}" | jq -r '.keyVaultName.value')
SQL_FQDN=$(echo "${DEPLOYMENT_OUTPUT}" | jq -r '.sqlServerFqdn.value')

echo "[3/4] Outputs extracted."
echo "  API URL:     ${API_URL}"
echo "  Frontend URL: ${FRONTEND_URL}"
echo "  Key Vault:   ${KV_NAME}"
echo "  SQL Server:  ${SQL_FQDN}"

# Apply database schema
echo "[4/4] Schema deployment note:"
echo "  Run the SQL script against ${SQL_FQDN}/AuditorPRO:"
echo "  src/sql/01_schema_completo.sql"
echo ""
echo "============================================"
echo " Deployment complete!"
echo " Open: ${FRONTEND_URL}"
echo "============================================"
