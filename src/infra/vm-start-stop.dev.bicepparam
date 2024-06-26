using 'vm-start-stop.bicep'

param storageAccountName = 'eitvmsplcdevst1'
param appServicePlanName = 'eit-vms-plc-dev-plan-1'
param functionAppName = 'eit-vms-plc-dev-func-1'
param keyVaultName = 'eit-vms-plc-dev-kv-1'
param serviceBusName = 'eit-vms-plc-dev-sb-1'
param applicationInsightsName = 'eit-vms-plc-dev-appi-1'
param logAnalyticsWorkspaceName = 'eit-ssm-plc-shd-log-1'
param logAnalyticsWorkspaceResourceGroupName = 'eit-ssm-plc-shd-rg-1'
param managedIdentityName = 'eit-vms-plc-dev-id-1'
param notificationWebhook = readEnvironmentVariable('NOTIFICATION_WEBHOOK')
