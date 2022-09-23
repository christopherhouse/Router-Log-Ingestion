param keyVaultName string
param adminObjectId string
param tenantId string
param location string

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: keyVaultName
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    accessPolicies: [
      {
        objectId: adminObjectId
        permissions: {
          certificates: [ 'all' ]
          secrets: [ 'all' ]
          keys: [ 'all' ]
        }
        tenantId: tenantId
      }
    ]
    tenantId: tenantId
  }
}

output name string = keyVault.name
output id string = keyVault.id
output apiVersion string = keyVault.apiVersion
