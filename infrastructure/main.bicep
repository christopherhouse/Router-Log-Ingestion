param storageAccountName string
param homeIpAddress string
param logAnalyticsWorkspaceName string
param appInsightsName string
param keyVaultName string
param keyVaultAdminObjectId string
param cosmosAccountName string
param databaseName string
param containerName string
param partitionKeyPath string
param eventHubNamespaceName string
param eventHubName string
param functionAppName string

param location string = resourceGroup().location
param deploymentSuffix string = utcNow('MMddyyyy_HHmmss')

var storageAccountDeploymentName = '${storageAccountName}-${deploymentSuffix}'
var logAnalyticsDeploymentName = '${logAnalyticsWorkspaceName}-${deploymentSuffix}'
var appinsightsDeploymentName = '${appInsightsName}-${deploymentSuffix}'
var keyVaultDeploymentName = '${keyVaultName}-${deploymentSuffix}'
var cosmosDeploymentName = '${cosmosAccountName}-${deploymentSuffix}'
var eventHubDeploymentName = '${eventHubNamespaceName}-${deploymentSuffix}'
var functionAppDeploymentName = '${functionAppName}-${deploymentSuffix}'

var tenantId = subscription().tenantId
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
    homeIpAddress: homeIpAddress
  }
}

module cosmosDb 'modules/cosmosaccount.bicep' = {
  name: cosmosDeploymentName
  params: {
    location: location
    containerName: containerName
    partitionKeyPath: partitionKeyPath
    cosmosAccountName: cosmosAccountName
    databaseName: databaseName
    homeIpAddress: homeIpAddress
  }
}

module eventHub 'modules/eventhub.bicep' = {
  name: eventHubDeploymentName
  params: {
    location: location
    eventHubName: eventHubName
    eventHubNamespaceName: eventHubNamespaceName
  }
}

resource storageRef 'Microsoft.Storage/storageAccounts@2022-05-01' existing = {
  name: storageAccountName
}

module functionApp 'modules/functionapp.bicep' = {
  name: functionAppDeploymentName
  params: {
    location: location
    appInsightsConnectionString: appInsights.outputs.connectionString
    appInsightsInstrumentationKey: appInsights.outputs.instrumentationKey
    functionAppName: functionAppName
    storageConnectionString: 'DefaultEndpointsProtocol=https;AccountName=${storageRef.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(resourceId(subscription().subscriptionId, resourceGroup().name, storageRef.type, storageRef.name), storageRef.apiVersion).keys[0].value};'
  }
  dependsOn: [
    storageAccount
  ]
}
