// ============================================================
// AuditorPRO TI — Azure Infrastructure (Bicep)
// ILG Logistics | Deployment: Production
// ============================================================

@description('Environment name')
@allowed(['dev', 'staging', 'prod'])
param environment string = 'prod'

@description('Azure region for all resources')
param location string = resourceGroup().location

@description('Unique suffix for globally unique resource names')
param suffix string = uniqueString(resourceGroup().id)

@description('Entra ID Tenant ID')
param tenantId string

@description('App Registration Client ID for AuditorPRO TI')
param clientId string

@description('Azure OpenAI deployment name')
param openAiDeploymentName string = 'gpt-4o'

// ============================================================
// Variables
// ============================================================
var prefix = 'auditorpro-${environment}'
var tags = {
  application: 'AuditorPRO TI'
  environment: environment
  owner: 'ILG Logistics TI'
  costCenter: 'TI-Auditoria'
}

// ============================================================
// Log Analytics Workspace
// ============================================================
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: '${prefix}-logs-${suffix}'
  location: location
  tags: tags
  properties: {
    sku: { name: 'PerGB2018' }
    retentionInDays: 90
  }
}

// ============================================================
// Application Insights
// ============================================================
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${prefix}-insights-${suffix}'
  location: location
  tags: tags
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
    IngestionMode: 'LogAnalytics'
    RetentionInDays: 90
  }
}

// ============================================================
// Key Vault
// ============================================================
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: '${prefix}-kv-${suffix}'
  location: location
  tags: tags
  properties: {
    sku: { family: 'A', name: 'standard' }
    tenantId: tenantId
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 30
    networkAcls: {
      defaultAction: 'Deny'
      bypass: 'AzureServices'
    }
  }
}

// ============================================================
// Azure SQL Server + Database
// ============================================================
resource sqlServer 'Microsoft.Sql/servers@2023-05-01-preview' = {
  name: '${prefix}-sql-${suffix}'
  location: location
  tags: tags
  properties: {
    administratorLogin: 'auditorpro_admin'
    administratorLoginPassword: '' // Use Managed Identity in production
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Disabled'
  }
  identity: { type: 'SystemAssigned' }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-05-01-preview' = {
  parent: sqlServer
  name: 'AuditorPRO'
  location: location
  tags: tags
  sku: {
    name: 'GP_Gen5_4'
    tier: 'GeneralPurpose'
    family: 'Gen5'
    capacity: 4
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 107374182400 // 100 GB
    zoneRedundant: false
    readScale: 'Disabled'
    requestedBackupStorageRedundancy: 'Geo'
  }
}

// ============================================================
// Storage Account (Blob for evidencias)
// ============================================================
resource storage 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: '${replace(prefix, '-', '')}st${suffix}'
  location: location
  tags: tags
  sku: { name: 'Standard_GRS' }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
    publicNetworkAccess: 'Disabled'
  }
}

resource evidenciasContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  name: '${storage.name}/default/evidencias'
  properties: { publicAccess: 'None' }
}

resource reportesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  name: '${storage.name}/default/reportes'
  properties: { publicAccess: 'None' }
}

// ============================================================
// App Service Plan + API App
// ============================================================
resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: '${prefix}-plan-${suffix}'
  location: location
  tags: tags
  sku: { name: 'P1v3', tier: 'PremiumV3', capacity: 1 }
  properties: { reserved: false }
}

resource apiApp 'Microsoft.Web/sites@2023-01-01' = {
  name: '${prefix}-api-${suffix}'
  location: location
  tags: tags
  identity: { type: 'SystemAssigned' }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      netFrameworkVersion: 'v9.0'
      minTlsVersion: '1.2'
      appSettings: [
        { name: 'APPLICATIONINSIGHTS_CONNECTION_STRING', value: appInsights.properties.ConnectionString }
        { name: 'AzureAd__TenantId', value: tenantId }
        { name: 'AzureAd__ClientId', value: clientId }
        { name: 'AzureAd__Audience', value: 'api://auditorpro-ti' }
        { name: 'KeyVaultName', value: keyVault.name }
        { name: 'WEBSITE_RUN_FROM_PACKAGE', value: '1' }
      ]
      connectionStrings: [
        {
          name: 'DefaultConnection'
          connectionString: 'Server=${sqlServer.properties.fullyQualifiedDomainName};Database=AuditorPRO;Authentication=Active Directory Managed Identity;Encrypt=True;'
          type: 'SQLAzure'
        }
      ]
    }
  }
}

resource frontendApp 'Microsoft.Web/sites@2023-01-01' = {
  name: '${prefix}-frontend-${suffix}'
  location: location
  tags: tags
  identity: { type: 'SystemAssigned' }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      minTlsVersion: '1.2'
      appSettings: [
        { name: 'VITE_API_BASE_URL', value: 'https://${apiApp.properties.defaultHostName}/api' }
        { name: 'VITE_AZURE_CLIENT_ID', value: clientId }
        { name: 'VITE_AZURE_TENANT_ID', value: tenantId }
      ]
    }
  }
}

// ============================================================
// Outputs
// ============================================================
output apiAppUrl string = 'https://${apiApp.properties.defaultHostName}'
output frontendAppUrl string = 'https://${frontendApp.properties.defaultHostName}'
output keyVaultName string = keyVault.name
output appInsightsConnectionString string = appInsights.properties.ConnectionString
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName
output storageAccountName string = storage.name
