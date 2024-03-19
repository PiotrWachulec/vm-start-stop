targetScope = 'subscription'

@description('Resource group name')
param resourceGroupName string

@description('Location of the resource group')
@allowed([
  'polandcentral'
])
param location string

@description('Principal ID of the resource deployment principal')
param resourceDeploymentPrincipalId string

resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceGroupName
  location: location
}

module ra 'roleAssignment.bicep' = {
  name: '${deployment().name}-ra'
  scope: resourceGroup(rg.name)
  params: {
    resourceDeploymentPrincipalId: resourceDeploymentPrincipalId
  }
}
