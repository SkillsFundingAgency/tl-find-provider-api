[![Build Status](https://dev.azure.com/dfe-ssp/S126-Tlevelservice/_apis/build/status/S126-TL/Find%20Provider%20API/tl-find-provider-api?repoName=SkillsFundingAgency%2Ftl-find-provider-api&branchName=main)](https://dev.azure.com/dfe-ssp/S126-Tlevelservice/_build/latest?definitionId=1184&repoName=SkillsFundingAgency%2Ftl-find-provider-api&branchName=main)

# tl-find-provider-api

Tlevels Find a Provider API repository.


## Configuration

The configuration values will be read from an Azure table. To make this work on a development machine, add a row to the Configuration table in the Azure Storage Explorer with partition key `LOCAL` and RowKey 'Sfa.Tl.Matching_1.0', and with Data populated with the required json.

### Local Configuration

[Azure Storage Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator) 
and [Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/) 
need to be installed on local development machines. 

> Azure Storage Emulator is now deprecated. Use [Azurite](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite) instead.
> To install Azurite, run
> ```
> npm install -g azurite
> ```
> To run Azurite, open a terminal and run
> ```
> azurite
> ```
> #### Notes
> - Unless you add a different path as an option when starting Azurite it will save the tables in a local file `__azurite_db_table__.json` so make sure you run it in the same directory each time.
> - If you have a configuration table saved in Azure Storage Emulator, you will need to recreate it in azurite.

To set up local configuration, make sure Azure Storage Emulator or Azurite is running, 
open Storage Explorer and navigate to the local storage account emulator.

Add a table `Configuration` if it doesn't already exist.

Add a new row to the table with:
> **PartitionKey** : `LOCAL`  
> **RowKey** : `Sfa.Tl.Find.Provider.Api_1.0`  
> Add a property **Data** and set the value as below.  
> 
```
    {
        "SqlConnectionString": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TLevelProviders;Integrated Security=True;",
        "AllowedCorsOrigins": "*",
        "ApiSettings": {
            "AppId": "<Application ID>",
            "ApiKey": "<API Key>"
        },
        "CourseDirectoryApiSettings": {
            "BaseUri": "<Course Directory API>",
            "ApiKey": "<API Key>"
        },
        "CourseDirectoryImportSchedule": "<CRON string>",
        "EmailSettings": {
            "GovNotifyApiKey": "<key from GOV.UK Notify>",
            "SupportEmailAddress": "<email>",
            "DeliveryStatusToken" :  "<token>" 
        },
        "TownDataImportSchedule": "<CRON string>",
        "PostcodeApiSettings": {
            "BaseUri": "https://api.postcodes.io/"
        },
        "SearchSettings": {
            "MergeAdditionalProviderData": false
        }
    }
```

The API Settings are maintained by the T Levels DevOps team, and contain the Application ID and API Key used for HMAC. 

Course directory API values can be obtained from the NCS API portal. Ask the NCS Course Directory Team for details.

CRON schedule needs to use a valid CRON string, such as `0 0 9 ? * MON-FRI`

### Troubleshooting

If you encounter an error starting the project:
```
System.InvalidOperationException: 'Configuration could not be loaded. Please check your configuration files or see the inner exception for details'
```
then you probably haven't started the Azure Storage Emulator, or haven't completed the configuration above.


## Calling the API

When running locally, assuming localhost with port 55961, you can either use the Swagger UI or call directly as below:

> **Swagger UI**
> - https://localhost:55961/swagger/index.html
> 
> **Generate API definition file**
> - https://localhost:55961/swagger/v3/swagger.json
>   - This can be saved into `\api-definitions\findaproviderapi.json`
> 
> **Routes** *(Skill Areas)*
> - https://localhost:55961/api/v3/routes
>  
> **Qualifications**
> - https://localhost:55961/api/v3/qualifications
>  
> **Locations**
> - https://localhost:55961/api/v3/locations?searchTerm=Oxford
>
> **Provider search**
> 
> *searchTerm used for postcode or town*
> - https://localhost:55961/api/v3/providers?lat=51.742141&lon=-1.295653
> - https://localhost:55961/api/v3/providers?searchTerm=CV1%202WT
> - https://localhost:55961/api/v3/providers?searchTerm=CV1%202WT&routeId=6
> - https://localhost:55961/api/v3/providers?searchTerm=CV1%202WT&qualificationId=37
> - https://localhost:55961/api/v3/providers?searchTerm=CV1%202WT&qualificationId=37&page=3
> - https://localhost:55961/api/v3/providers?searchTerm=CV1%202WT&qualificationId=37&page=0&pageSize=10


For provider search, the postcode at the end of the url is required. 
The `qualificationId` and `routeId` filters are optional and defaults to null or 0; 
`page` and `pageSize` are also optional and default to 0 and 5 respectively.

If the postcode is not found in provider search, the API will return a 404 result with a message indicating the postcode was not found.

API calls need to include an `Authorization` header with an HMAC signature.
(Locations currently does not require the authorization header.)



## Database

##### Initialization and post-deployment

Some data is seeded into the database during deployment:

- **Qualifications** - an initial list of qualifications, which will be overwriten by the nightly import
- **Routes** - a hard-coded list of routes
- **RouteQualification** - a hard-coded list of mappings ([below](#route_mapping))

Note that qualifications will be imported from the Course Directory; however the route qualification mapping needs the qualification ids so they have been added in the post-deployment script.


##### <a name="route_mapping"></a> Route mapping

Routes are hard-coded and initialised from the database post-deployment scripts. If any routes are addied they will need to be added to the script `Sfa.Tl.Find.Provider.Api.Database\PostDeployment\Seed Routes.sql`.

Route mapping is also done in the script. This will need to be modified after new qualifications have been imported, and the project redeployed.
This is done in the script `Sfa.Tl.Find.Provider.Api.Database\PostDeployment\Seed RouteQualification.sql`.

##### Data import

Data insert/update/delete is handled in stored procedures, which are passed table-valued parameters 
to be merged into the tables. 

The merge statements in the stored procedures need to use this syntax for comparing nullable columns:
```
	ISNULL(NULLIF(t.[AddressLine1] COLLATE Latin1_General_CS_AS, s.[AddressLine1] COLLATE Latin1_General_CS_AS), 
	  	   NULLIF(s.[AddressLine1] COLLATE Latin1_General_CS_AS, t.[AddressLine1] COLLATE Latin1_General_CS_AS))
		IS NOT NULL
```
- The NULLIF() function returns NULL if two expressions are equal, otherwise it returns the first expression
- The effect is to return null if the values are the same, so the final IS NOT NULL means the values differ
- case-sensitive comparison requres use of `COLLATE Latin1_General_CS_AS`

These procedures pass back a summary of changes, with counts of inserted, updated and (soft) deleted rows.

Dapper is used as the ORM layer. For more information on using table valued parameters, see [Bulk Upsert with Dapper and Sql Server](https://blog.schroederspace.com/tumbleweed-technology/bulk-upsert-with-dapper-and-sql-server)

The qualification Id column is the T Level Framework id from the Course Directory tleveldefinitions. 
It is used as the primary key there is no identity key in Qualifications).

Deletion of data is by setting the IsDeleted flag (soft delete) as part of the merge statements in the stored procedures. 
Soft deletion avoids complitions with cascading deletes; all selects must check the IsDeleted flag to avoid reading deleted data.
Deletion of rows in LocationQualification is a hard delete, since this is a mapping table.

Updating a deleted row will set the row to undeleted; this acts to logically re-add a row that was previously soft-deleted.

##### Search
The search procedure takes the top n locations (where n = `@pageSize`) and returns them, together with 
delivery years and qualifications, as a flat list that might have more than more than n rows.

The search procedure can be called directly to search from a latitude/longitude:
```
EXEC [dbo].[SearchProviders]
	@fromLatitude = 52.400997,
	@fromLongitude = -1.508122,
	@qualificationId = NULL,
	@page = 0,
	@pageSize = 5
```
`@page` is a 0-based index used for paging.
`@pageSize` must be > 0, otherwise an error will be thrown.
`@qualificationId` can be used to filter the results. NULL will return all qualifications.


## Rate Limiting

The API uses the `AspNetCoreRateLimit` to limit the number of incoming requests, with configuration in `appSettings.json`.
To make sure the IP address of the original requster is passed to the rate limiting code, where it is used to limit requests from indivudual IP addresses, `UseForwardedHeaders` from the `Microsoft.AspNetCore.HttpOverrides` package has been used. 


## External APIs

Postcode details are retrieved using the postcodes.io API. Where possible,
results are cached to avoid duplicate calls.

Provider and course details are read from the NCS Course directory API. This is called from a scheduled Quartz job.


## Notes

While we wait for the final framework codes, temporary codes have been created in the database post deployment scripts.

Until then, calls to `_courseDirectoryService.ImportQualifications()` 
have been removed from `InitializationJob` and `CourseDataImportJob`
When these lines are restored, two tests will need to be fixed because they now expect the mothod not to be called.
