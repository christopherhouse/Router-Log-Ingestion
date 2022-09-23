param logAnalyticsWorkspaceName string
param location string

resource workspace 'Microsoft.OperationalInsights/workspaces@2021-12-01-preview' = {
  name: logAnalyticsWorkspaceName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    } 
    retentionInDays: 45
  }
}

output name string = workspace.name
output id string = workspace.id
output apiVersion string = workspace.apiVersion
