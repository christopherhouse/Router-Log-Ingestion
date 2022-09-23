param eventHubNamespaceName string
param eventHubName string
param location string

resource eventHubNamespace 'Microsoft.EventHub/namespaces@2022-01-01-preview' = {
  name: eventHubNamespaceName
  location: location
  sku:{
    name: 'Standard'
    capacity: 1
  }
  properties: {
    minimumTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
}

resource eventHub 'Microsoft.EventHub/namespaces/eventhubs@2022-01-01-preview' = {
  name: eventHubName
  parent: eventHubNamespace
  properties: {
    messageRetentionInDays: 3
    partitionCount: 8
  }
}

output eventHubName string = eventHub.name
output eventHubId string = eventHub.id
output eventHubApiVersion string = eventHub.apiVersion
output eventHubNamespaceName string = eventHubNamespace.name
output eventHubNamespaceId string = eventHubNamespace.id
output eventHubNamespaceApiVersion string = eventHubNamespace.apiVersion
