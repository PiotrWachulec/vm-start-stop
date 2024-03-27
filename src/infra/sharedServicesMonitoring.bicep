@description('Location of the resources')
param location string = 'polandcentral'

@description('Name of the Log Analytics workspace')
param workspaceName string

resource law 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: workspaceName
  location: location
}
