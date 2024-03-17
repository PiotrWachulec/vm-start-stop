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
    $ResourceGroupName
)

$timestamp = Get-Date -Format 'yyyyMMddHHmmss'

$params = @{
    ResourceGroupName     = $ResourceGroupName
    TemplateFile          = $TemplateFilePath
    TemplateParameterFile = $TemplateParameterFilePath
    Name                  = "vms_$timestamp"
}

New-AzResourceGroupDeployment @params