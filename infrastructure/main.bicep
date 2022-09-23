param location string = resourceGroup().location
param deploymentSuffix string = utcNow('MMddyyyy_HHmmss')
param storageAccountName string

var storageAccountDeploymentName = '${deployment().name}-${deploymentSuffix}'
var functionContainers = [
  'azure-webjobs-secrets', 'azure-webjobs-hosts'
]

module storageAccount 'modules/storageaccount.bicep' = {
  name: storageAccountDeploymentName
  params: {
    storageAccountName: storageAccountName
    location: location
    containersToCreate: functionContainers
  }
}
