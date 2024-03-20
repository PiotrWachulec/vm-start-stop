@description('Principal ID of the resource deployment principal')
param resourceDeploymentPrincipalId string

var contributorRoleId = 'b24988ac-6180-42a0-ab88-20f7382dd24c'

resource contributorRole 'Microsoft.Authorization/roleDefinitions@2022-05-01-preview' existing = {
  scope: subscription()
  name: contributorRoleId
}

resource devRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().name, contributorRoleId, resourceDeploymentPrincipalId)
  properties: {
    principalId: resourceDeploymentPrincipalId
    roleDefinitionId: contributorRole.id
    principalType: 'ServicePrincipal'
  }
}
