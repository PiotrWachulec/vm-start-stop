[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [string]
    $TemplateFilePath,
    [Parameter(Mandatory = $true)]
    [string]
    $TemplateParameterFilePath,
    [Parameter(Mandatory = $true)]
    [string]
    $ResourceGroupName,
    [Parameter()]
    [string]
    $Validate
)

$timestamp = Get-Date -Format 'yyyyMMddHHmmss'

$isValidation = $Validate -eq 'true'

$params = @{
    ResourceGroupName     = $ResourceGroupName
    TemplateFile          = $TemplateFilePath
    TemplateParameterFile = $TemplateParameterFilePath
    Name                  = "vms_$timestamp"
    Mode                  = 'Complete'
    Force                 = !$isValidation
    WhatIf                = $isValidation
}

New-AzResourceGroupDeployment @params