param appInsightsName string
param logAnalyticsWorkspaceId string
param location string

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspaceId
  }
}

output name string = appInsights.name
output id string = appInsights.id
output apiVersion string = appInsights.apiVersion
output instrumentationKey string = appInsights.properties.InstrumentationKey
output connectionString string = appInsights.properties.ConnectionString
