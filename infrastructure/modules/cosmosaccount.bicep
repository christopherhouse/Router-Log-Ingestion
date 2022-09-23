param cosmosAccountName string
param databaseName string
param containerName string
param partitionKeyPath string

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

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2022-05-15' = {
  name: databaseName
  parent: cosmosAccount
  properties: {
    resource: {
      id: databaseName
    }
  }
}

resource container 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2022-05-15' = {
  name: containerName
  parent: database
  properties: {
    resource: {
      id: containerName
      partitionKey: {
       paths: [ partitionKeyPath ]
       kind: 'Hash' 
      }
      indexingPolicy: {
        includedPaths: [
          {
            path: '/*'
          }
        ]
        excludedPaths: [ 
          {
            path: '/_etag/?'
          }
        ]
      }
    }
  }
} 

output name string = cosmosAccount.name
output id string = cosmosAccount.id
output apiVersion string = cosmosAccount.apiVersion
