param location string
param resourceToken string
param tags object
param name string
var abbrs = loadJsonContent('abbreviations.json')
var cosmosDatabaseName = 'RatingsDB'

resource logs 'Microsoft.OperationalInsights/workspaces@2021-12-01-preview' = {
  name: '${abbrs.operationalInsightsWorkspaces}${resourceToken}'
  location: location
  tags: tags
  properties: any({
    retentionInDays: 30
    features: {
      searchVersion: 1
    }
    sku: {
      name: 'PerGB2018'
    }
  })
}

resource ai 'Microsoft.Insights/components@2020-02-02' = {
  name: '${abbrs.insightsComponents}${resourceToken}'
  location: location
  tags: tags
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logs.id
  }
}

resource storageNotify 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: '${abbrs.storageAccount}${resourceToken}'
  location: location
  tags: tags
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource storageBatch 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: '${abbrs.storageAccountBatch}${resourceToken}'
  location: location
  tags: tags
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource hostingPlan 'Microsoft.Web/serverfarms@2020-10-01' = {
  name:  '${abbrs.hostingPlan}${resourceToken}'
  location: location
  tags: tags
  kind: 'elastic'
  properties: {
    reserved: true
  }
  sku: {
    name: 'EP1' 
    capacity: 1
  }
}

module keyVault 'modules/security/keyvault.bicep' = {
  name: 'keyvault'
  params: {
    name: '${abbrs.keyVaultVaults}${resourceToken}'
    location: location
    tags: tags
  }
}

module cosmos 'modules/db.bicep' = {
  name: 'cosmos'
  params: {
    accountName: '${abbrs.documentDBDatabaseAccounts}${resourceToken}'
    databaseName: cosmosDatabaseName
    location: location
    tags: tags
    keyVaultName: keyVault.outputs.name
  }
}

module function 'modules/function.bicep' = {
  name: name
  params: {
    name: name
    location: location
    applicationName: '${name}httpfunctions'
  }
}

module apiKeyVaultAccess 'modules/security/keyvault-access.bicep' = {
  name: 'api-keyvault-access'
  params: {
    keyVaultName: keyVault.outputs.name
    principalId: function.outputs.principalId
  }
}

