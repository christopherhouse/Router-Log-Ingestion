param storageAccountName string
param homeIpAddress string
param logAnalyticsWorkspaceName string
param appInsightsName string

param location string = resourceGroup().location
param deploymentSuffix string = utcNow('MMddyyyy_HHmmss')

var storageAccountDeploymentName = 'storageaccount-${deploymentSuffix}'
var logAnalyticsDeploymentName = 'loganalytics-${deploymentSuffix}'
var appinsightsDeploymentName = 'appinsights-${deploymentSuffix}'

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
