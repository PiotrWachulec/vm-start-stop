@description('Principal ID of the resource deployment principal')
param resourceDeploymentPrincipalId string

var contributorRoleId = 'b24988ac-6180-42a0-ab88-20f7382dd24c'

resource devRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().name, contributorRoleId, resourceDeploymentPrincipalId)
  properties: {
    principalId: resourceDeploymentPrincipalId
    roleDefinitionId: contributorRoleId
    principalType: 'ServicePrincipal'
  }
}
