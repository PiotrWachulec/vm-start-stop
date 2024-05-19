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
    $Validate,
    [PSCustomObject]
    $CustomParams
)

$timestamp = Get-Date -Format 'yyyyMMddHHmmss'

$isValidation = $Validate -eq 'true'

$deploymentParams = @{
    ResourceGroupName     = $ResourceGroupName
    TemplateFile          = $TemplateFilePath
    TemplateParameterFile = $TemplateParameterFilePath
    Name                  = "vms_$timestamp"
    Mode                  = 'Complete'
    Force                 = !$isValidation
    WhatIf                = $isValidation
}

if ($null -eq $CustomParams) {
    $params = $deploymentParams
}
else {
    $params = $deploymentParams + $CustomParams
}

New-AzResourceGroupDeployment @params