param storageAccountName string
param containersToCreate array
param homeIpAddress string
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
      ipRules: [
         {
          value: homeIpAddress
         }
      ]
    }
    supportsHttpsTrafficOnly: true
  }
}

resource blobServices 'Microsoft.Storage/storageAccounts/blobServices@2022-05-01' = {
  name: 'default'
  parent: storageAccount
}

resource container 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-05-01' = [for containerName in containersToCreate: {
  name: containerName
  parent: blobServices
}]

resource shares 'Microsoft.Storage/storageAccounts/fileServices@2022-05-01' = {
  name: 'default'
  parent: storageAccount
}

resource share 'Microsoft.Storage/storageAccounts/fileServices/shares@2022-05-01' = {
  name: 'content'
  parent: shares
}

output name string = storageAccount.name
output id string = storageAccount.id
output apiVersion string = storageAccount.apiVersion
