param name string
param location string
param applicationName string

var resourceToken = toLower(uniqueString(subscription().id, name, location))
var tags = { 'azd-env-name': name }
var abbrs = loadJsonContent('../abbreviations.json')

resource ai 'Microsoft.Insights/components@2020-02-02' existing = {
  name: '${abbrs.insightsComponents}${resourceToken}'
}

resource storage 'Microsoft.Storage/storageAccounts@2021-09-01' existing = {
  name: '${abbrs.storageAccount}${resourceToken}'
}

resource hostingPlan 'Microsoft.Web/serverfarms@2020-10-01' existing = {
  name:  '${abbrs.hostingPlan}${resourceToken}'
}

resource functionApp 'Microsoft.Web/sites@2020-06-01' = {
  name: applicationName
  location: location
  tags: tags
  kind: 'functionapp,linux'
  properties: {
    httpsOnly: true
    serverFarmId: hostingPlan.id
    clientAffinityEnabled: false
    siteConfig: {
      linuxFxVersion: 'DOTNET-ISOLATED|7.0'
      
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storage.name};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${storage.listKeys().keys[0].value}'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storage.name};EndpointSuffix=${az.environment().suffixes.storage};AccountKey=${storage.listKeys().keys[0].value}'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: ai.properties.InstrumentationKey
        }
      ]
    }
  }
}

output application_url string = functionApp.properties.hostNames[0]
