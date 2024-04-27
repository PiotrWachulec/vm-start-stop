@description('Location of the resources')
param location string = resourceGroup().location

@description('Name of the storage account')
param storageAccountName string

@description('Name of the app service plan')
param appServicePlanName string

@description('Name of the function app')
param functionAppName string

@description('Name of the key vault')
param keyVaultName string

@description('Name of the service bus')
param serviceBusName string

@description('Name of the application insights')
param applicationInsightsName string

@description('Name of the log analytics workspace')
param logAnalyticsWorkspaceName string

@description('Resource group name where LAW is placed')
param logAnalyticsWorkspaceResourceGroupName string

@description('Name of the managed identity')
param managedIdentityName string

var timeTriggerServiceBusQueueName = 'time-trigger-service-bus-queue'
var turnOnVMServiceBusQueueName = 'turn-on-vm-service-bus-queue'
var turnOffVMServiceBusQueueName = 'turn-off-vm-service-bus-queue'

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
    defaultToOAuthAuthentication: true
    minimumTlsVersion: 'TLS1_2'
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
}

resource functionApp 'Microsoft.Web/sites@2023-01-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  properties: {
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower(functionAppName)
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsights.properties.InstrumentationKey
        }
        {
          name: 'VM_START_STOP_TAG_NAME'
          value: 'VM-START-STOP-SCHEDULE'
        }
        {
          name: 'RunOnStartup'
          value: 'false'
        }
        {
          name: 'ReadServiceBusConnection'
          value: serviceBusReadPolicy.listKeys().primaryConnectionString
        }
        {
          name: 'WriteServiceBusConnection'
          value: serviceBusWritePolicy.listKeys().primaryConnectionString
        }
        {
          name: 'AZURE_CLIENT_ID'
          value: managedIdentity.properties.clientId
        }
      ]
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
      use32BitWorkerProcess: false
      netFrameworkVersion: 'v8.0'
    }
    httpsOnly: true
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentity.id}': {}
    }
  }
}

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-07-31-preview' = {
  name: managedIdentityName
  location: location
}

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: tenant().tenantId
    enableRbacAuthorization: true
  }
}

resource serviceBus 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' = {
  name: serviceBusName
  location: location
  sku: {
    name: 'Basic'
  }
}

resource timeTriggerQueue 'Microsoft.ServiceBus/namespaces/queues@2022-10-01-preview' = {
  parent: serviceBus
  name: timeTriggerServiceBusQueueName
}

resource turnOnVmQueue 'Microsoft.ServiceBus/namespaces/queues@2022-10-01-preview' = {
  parent: serviceBus
  name: turnOnVMServiceBusQueueName
}

resource turnOffVmQueue 'Microsoft.ServiceBus/namespaces/queues@2022-10-01-preview' = {
  parent: serviceBus
  name: turnOffVMServiceBusQueueName
}

resource serviceBusReadPolicy 'Microsoft.ServiceBus/namespaces/authorizationRules@2022-10-01-preview' = {
  parent: serviceBus
  name: 'ReadQueuePolicy'
  properties: {
    rights: [
      'Listen'
    ]
  }
}

resource serviceBusWritePolicy 'Microsoft.ServiceBus/namespaces/authorizationRules@2022-10-01-preview' = {
  parent: serviceBus
  name: 'WriteQueuePolicy'
  properties: {
    rights: [
      'Send'
    ]
  }
}

resource law 'Microsoft.OperationalInsights/workspaces@2023-09-01' existing = {
  name: logAnalyticsWorkspaceName
  scope: resourceGroup(logAnalyticsWorkspaceResourceGroupName)
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: location
  kind: 'web'
  properties: {
    WorkspaceResourceId: law.id
    Application_Type: 'web'
    Request_Source: 'rest'
  }
}
