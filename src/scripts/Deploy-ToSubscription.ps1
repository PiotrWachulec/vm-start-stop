[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [string]
    $TemplateFilePath,
    [Parameter(Mandatory = $true)]
    [string]
    $TemplateParameterFilePath
)

$params = @{
    TemplateFile          = $TemplateFilePath
    TemplateParameterFile = $TemplateParameterFilePath
    Location              = 'polandcentral'
}

New-AzDeployment @params