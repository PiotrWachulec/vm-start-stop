targetScope = 'subscription'

@description('Resource group name')
param resourceGroupName string

@description('Location of the resource group')
@allowed([
  'polandcentral'
])
param location string

resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceGroupName
  location: location
}
