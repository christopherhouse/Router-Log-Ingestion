param cosmosAccountName string
param databasesToCreate array
param containersToCreate array
param homeIpAddress string
param location string

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2022-05-15' = {
  name: cosmosAccountName
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        locationName: location
        isZoneRedundant: false
        failoverPriority: 0
      }
    ]
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
    networkAclBypass: 'AzureServices'
    publicNetworkAccess: 'Enabled'
    ipRules: [
      {
        ipAddressOrRange: homeIpAddress
      }
    ]
  }
}

resource databases 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2022-05-15' = [for database in databasesToCreate: {
  name: database
  parent: cosmosAccount
  properties: {
    resource: {
      id: database
    }
  }
}]

output name string = cosmosAccount.name
output id string = cosmosAccount.id
output apiVersion string = cosmosAccount.apiVersion
