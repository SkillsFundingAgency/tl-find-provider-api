$ErrorActionPreference = "Stop"

if ((Get-AzContext).Subscription.Name -ne 's126-tlevelservice-development') {
    throw 'Azure Context references incorrect subscription'
}

$scriptRoot = $PSScriptRoot
if (($PSScriptRoot).Length -eq 0) { $scriptRoot = $PWD.Path}

$location = "westeurope"
$applicationPrefix = "fprapi"
$envPrefix = "s126d99"
$environmentNameAbbreviation = "xxx"
$certsToUpload = @{   
    "dev-api-findatlevelprovider" = '7E73F2CADAAF4A270A07318453A105C8E97230E0'
    "dev-connect-tlevels-gov-uk"  = 'DD76F0AFCDD2366AC750910C25D8AACE17D07DBB'
}

$sharedResourceGroupName = $envPrefix + "-$($applicationPrefix)-shared"
$envResourceGroupName = $envPrefix + "-$($applicationPrefix)-$($environmentNameAbbreviation)"

# purge the keyvault if it's in InRemoveState due to resource group deletion
# this is up front as it's such a common reason for the script to fail
if (Get-AzKeyVault -Vaultname "$($envPrefix)fprapisharedkv" -Location $location -InRemovedState) { 
    Write-Host 'Purging vault'
    Remove-AzKeyVault -VaultName s126d99fprapisharedkv -InRemovedState -Location $location -Force
}

Get-AzResourceGroup -Name $sharedResourceGroupName -ErrorVariable notPresent -ErrorAction SilentlyContinue
if ($notPresent) {
    $tags = @{
        "Environment" = "Dev"
        "Parent Business" = "Education and Skills Funding Agency"
        "Portfolio" = "Education and Skills Funding Agency"
        "Product" = "T-Levels"
        "Service" = "ESFA T Level Service"
        "Service Line" = "Professional and Technical Education"
        "Service Offering" = "ESFA T Level Service"
    }
    New-AzResourceGroup -Name $sharedResourceGroupName -Location $location -Tag $tags
}

$sharedDeploymentParameters = @{
    Name                    = "test-{0:yyyyMMdd-HHmmss}" -f (Get-Date)
    ResourceGroupName       = $sharedResourceGroupName
    Mode                    = "Complete"
    Force                   = $true
    TemplateFile            = "$($scriptRoot)/findproviderapi-shared.json"
    TemplateParameterObject = @{
        environmentNameAbbreviation             = "$($envPrefix)-fprapi"
        sqlServerAdminUsername                  = "xxxServerAdminxxx"
        sqlServerAdminPassword                  = ([System.Web.Security.Membership]::GeneratePassword(16, 2))
        sqlServerActiveDirectoryAdminLogin      = "s126-tlevelservice-Managers USR"
        sqlServerActiveDirectoryAdminObjectId   = "56f27acd-6ea8-4526-a25c-29436d62826c"
        threatDetectionEmailAddress             = "noreply@example.com"
        appServicePlanTier                      = "Standard"
        appServicePlanSize                      = "1"
        appServicePlanInstances                 = 1
        azureWebsitesRPObjectId                 = "0b11c7a6-2868-4728-b83c-d14be9147a97"
        keyVaultReadWriteObjectIds              = @("0316d3ae-e503-4dae-9665-c999fca7cf10", "a6621090-e704-45ec-b65f-50257f9d4dcd")
        keyVaultFullAccessObjectIds             = @("b3b225a1-7c11-4698-9f15-32c345cf5bc2")  
        redisCacheSKU                           = "Basic"
        redisCacheFamily                        = "C"
        redisCacheCapacity                      = 0        
    }
}

$sharedDeployment = New-AzResourceGroupDeployment @sharedDeploymentParameters

foreach ($key in $certsToUpload.Keys) {
    # first get a random 32 character alphanumeric key into a SecureString
    $certPassword = ConvertTo-SecureString `
                        -AsPlainText `
                        -Force `
                        -String ([System.Web.Security.Membership]::GeneratePassword(64, 0) -replace "[^a-zA-Z0-9]","").Substring(0,32)

    # save the certificate including private key into file protected with the above password
    Export-PfxCertificate `
        -Password $certPassword `
        -FilePath "$($key).pfx" `
        -Cert "cert://CurrentUser/my/$($certsToUpload[$key])"

    # import the certificate to KeyVault 
    Import-AzKeyVaultCertificate `
            -VaultName "$($envPrefix)fprapisharedkv" `
            -Name $key `
            -FilePath "$($key).pfx" `
            -Password $certPassword   
    
    # and then delete the file and forget the password
    Remove-Item -Path "$($key).pfx"
    Clear-Variable -Name certPassword
}

Get-AzResourceGroup -Name $envResourceGroupName -ErrorVariable notPresent -ErrorAction SilentlyContinue
if ($notPresent) {
    $tags = @{
        "Environment" = "Dev"
        "Parent Business" = "Education and Skills Funding Agency"
        "Portfolio" = "Education and Skills Funding Agency"
        "Product" = "T-Levels"
        "Service" = "ESFA T Level Service"
        "Service Line" = "Professional and Technical Education"
        "Service Offering" = "ESFA T Level Service"
    }
    New-AzResourceGroup -Name $envResourceGroupName -Location $location -Tag $tags
}

$deploymentParameters = @{
    Name                    = "test-{0:yyyyMMdd-HHmmss}" -f (Get-Date)
    ResourceGroupName       = $envResourceGroupName
    Mode                    = "Incremental"
    TemplateFile            = "$($scriptRoot)/findproviderapi-environment.json"
    TemplateParameterObject = @{
        environmentNameAbbreviation             = $environmentNameAbbreviation
        resourceNamePrefix                      = ("$($envPrefix)-fprapi-" + $environmentNameAbbreviation)
        sharedASPName                           = "$($envPrefix)-fprapi-shared-asp"
        sharedEnvResourceGroup                  = $sharedResourceGroupName
        sharedKeyVaultName                      = "$($envPrefix)fprapisharedkv"
        sharedSQLServerName                     = "$($envPrefix)-fprapi-shared-sql"
        sqlDatabaseSkuName                      = "S0"
        sqlDatabaseTier                         = "Standard"   
        apiCustomHostName                       = "dev.api.findatlevelprovider.education.gov.uk"  
        apiCertificateName                      = "dev-api-findatlevelprovider"
        uiCustomHostname                        = "dev.connect.tlevels.gov.uk"                     
        uiCertificateName                       = "dev-connect-tlevels-gov-uk"
        configurationStorageConnectionString    = ($sharedDeployment.Outputs.configStorageConnectionString.Value)
    }
}

$envDeployment = New-AzResourceGroupDeployment @deploymentParameters
if ($envDeployment.ProvisioningState -eq "Succeeded") {
    Write-Output "Yippee!!"
}


<# 

# you have to remove the diagnostic settings separately as they hang around if you don't and mess things up badly
$subscriptionId = (Get-AzContext).Subscription.Id
$diagnosticResourceIds = @(
    "/subscriptions/$($subscriptionId)/resourceGroups/$($sharedResourceGroupName)/providers/Microsoft.KeyVault/vaults/$($envPrefix)$($applicationPrefix)sharedkv",
    "/subscriptions/$($subscriptionId)/resourceGroups/$($envResourceGroupName)/providers/Microsoft.Web/sites/$($envPrefix)-$($applicationPrefix)-$($environmentNameAbbreviation)-web",
    "/subscriptions/$($subscriptionId)/resourceGroups/$($envResourceGroupName)/providers/Microsoft.Web/sites/$($envPrefix)-$($applicationPrefix)-$($environmentNameAbbreviation)-ui"
)
foreach ($diagnosticResourceId in $diagnosticResourceIds) {
    Write-Host "Finding settings in $($diagnosticResourceId) to remove"
    foreach ($setting in (Get-AzDiagnosticSetting -ResourceId $diagnosticResourceId -ErrorAction SilentlyContinue)) {
        Write-Host "Removing $(($setting).Name)"
        Remove-AzDiagnosticSetting -ResourceId $diagnosticResourceId -Name $setting.Name
    }   
}


Remove-AzOperationalInsightsWorkspace -ResourceGroupName $sharedResourceGroupName -Name "$($sharedResourceGroupName)-log" -ForceDelete -Force -ErrorAction SilentlyContinue
Remove-AzResourceGroup -ResourceGroupName $envResourceGroupName -Force -ErrorAction SilentlyContinue
Remove-AzResourceGroup -ResourceGroupName $sharedResourceGroupName -Force -ErrorAction SilentlyContinue
#>