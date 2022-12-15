$ErrorActionPreference = "Stop"
# make sure we're on the right subscription to start with
Set-AzContext -Tenant "9c7d9dd3-840c-4b3f-818e-552865082e16" -Subscription "s126-tlevelservice-development" 

$location = "westeurope"
$envPrefix = "s126d99"
$environmentNameAbbreviation = "xxx"
$sharedResourceGroupName = $envPrefix + "-fprapi-shared"

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
    TemplateFile            = "findproviderapi-shared.json"
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

# now upload the certificates to the keyvault for api and connect, these are sourced from 
# the CurrentUser/my/ store your machine so you will need to have access to these up upload

# 7E73F2CADAAF4A270A07318453A105C8E97230E0 = dev-api-findatlevelprovider
# DD76F0AFCDD2366AC750910C25D8AACE17D07DBB = dev-connect-tlevels-gov-uk     

$certsToUpload = @{   
    "dev-api-findatlevelprovider" = '7E73F2CADAAF4A270A07318453A105C8E97230E0'
    "dev-connect-tlevels-gov-uk"  = 'DD76F0AFCDD2366AC750910C25D8AACE17D07DBB'
}

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

$envResourceGroupName = $envPrefix + "-fprapi-xxx"

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
    TemplateFile            = "findproviderapi-environment.json"
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