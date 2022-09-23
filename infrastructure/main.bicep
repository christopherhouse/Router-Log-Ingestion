param storageAccountName string
param homeIpAddress string
param logAnalyticsWorkspaceName string
param appInsightsName string
param keyVaultName string
param keyVaultAdminObjectId string

param location string = resourceGroup().location
param deploymentSuffix string = utcNow('MMddyyyy_HHmmss')

var tenantId = subscription().tenantId
var storageAccountDeploymentName = '${storageAccountName}-${deploymentSuffix}'
var logAnalyticsDeploymentName = '${logAnalyticsWorkspaceName}-${deploymentSuffix}'
var appinsightsDeploymentName = '${appInsightsName}-${deploymentSuffix}'
var keyVaultDeploymentName = '${keyVaultName}-${deploymentSuffix}'

var functionContainers = [
  'azure-webjobs-secrets', 'azure-webjobs-hosts'
]

module storageAccount 'modules/storageaccount.bicep' = {
  name: storageAccountDeploymentName
  params: {
    storageAccountName: storageAccountName
    location: location
    containersToCreate: functionContainers
    homeIpAddress: homeIpAddress
  }
}

module logAnalyticsWorkspace 'modules/loganalyticsworkspace.bicep' = {
  name: logAnalyticsDeploymentName
  params: {
    location: location
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
  }
}

module appInsights 'modules/applicationinsights.bicep' = {
  name: appinsightsDeploymentName
  params: {
    location: location
    appInsightsName: appInsightsName
    logAnalyticsWorkspaceId: logAnalyticsWorkspace.outputs.id
  }
}

module keyVault 'modules/keyvault.bicep' = {
  name: keyVaultDeploymentName
  params: {
    keyVaultName: keyVaultName
    location: location
    tenantId: tenantId
    adminObjectId: keyVaultAdminObjectId
  }
}
