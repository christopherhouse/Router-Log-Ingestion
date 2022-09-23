param keyVaultName string
param location string

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: keyVaultName
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
  }
}

output name string = keyVault.name
output id string = keyVault.id
output apiVersion string = keyVault.apiVersion
