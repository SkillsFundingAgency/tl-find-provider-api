# tl-find-provider-api

This is the Tlevels Find a Provider API.


## Configuration

The configuration values will be read from an Azure table. To make this work on a development machine, add a row to the Configuration table in the Azure Storage Explorer with partition key `LOCAL` and RowKey 'Sfa.Tl.Matching_1.0', and with Data populated with the required json.

### Local Configuration

[Azure Storage Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator) 
and [Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/) 
need to be installed on local development machines. Alternatively, Azurite could be used with Docker.

To set up local configuration, make sure Azure Storage Emulator is running, 
open Storage Explorer and navigate to the local storage account emulator.

Add a table `Configuration` if it doesn't already exist.

Add a new row to the table with:
> **PartitionKey** : `LOCAL`  
> **RowKey** : `Sfa.Tl.Find.Provider.Api_1.0_`  
> Add a property **Data** and set the value as below.  
> 
```
    {
        "SqlConnectionString": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TLevelProviders;Integrated Security=True;",
        "AllowedOrigins": "*",
        "CourseDirectoryApiSettings": {
        "BaseUri": "<Course Directory API>",
        "ApiKey": "<API Key>"
        },
        "PostcodeApiSettings": {
        "BaseUri": "https://api.postcodes.io/"
        }
    }
```

Course directory API values can be obtained from the NCS API portal. Ask the NCS Course Directory Team for details.




