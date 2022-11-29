param accountName string
param location string = resourceGroup().location
param tags object = {}

param containers array = [
  {
    name: 'Ratings'
    id: 'Ratings'
    partitionKey: 'userId'
  }
]
param databaseName string = ''
param keyVaultName string

// Because databaseName is optional in main.bicep, we make sure the database name is set here.
var defaultDatabaseName = 'Ratings'
var actualDatabaseName = !empty(databaseName) ? databaseName : defaultDatabaseName

module cosmos 'database/cosmos/sql/cosmos-sql-db.bicep' = {
  name: 'cosmos-sql'
  params: {
    accountName: accountName
    databaseName: actualDatabaseName
    location: location
    containers: containers
    keyVaultName: keyVaultName
    tags: tags
  }
}

output connectionStringKey string = cosmos.outputs.connectionStringKey
output databaseName string = cosmos.outputs.databaseName
output endpoint string = cosmos.outputs.endpoint
