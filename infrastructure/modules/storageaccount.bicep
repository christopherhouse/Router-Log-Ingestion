param storageAccountName string
param location string

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_ZRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: false
    allowSharedKeyAccess: true
    networkAcls: {
      defaultAction: 'Deny'
      bypass: 'AzureServices'
    }
    supportsHttpsTrafficOnly: true
  }
}

output name string = storageAccount.name
output id string = storageAccount.id
output apiVersion string = storageAccount.apiVersion
