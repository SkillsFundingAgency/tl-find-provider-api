[![Build Status](https://dev.azure.com/dfe-ssp/S126-Tlevelservice/_apis/build/status/S126-TL/Find%20Provider%20API/tl-find-provider-api?repoName=SkillsFundingAgency%2Ftl-find-provider-api&branchName=main)](https://dev.azure.com/dfe-ssp/S126-Tlevelservice/_build/latest?definitionId=1184&repoName=SkillsFundingAgency%2Ftl-find-provider-api&branchName=main)

# tl-find-provider-api

Tlevels Find a Provider API repository.


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
        "PostcodeApiSettings": {
            "BaseUri": "https://api.postcodes.io/"
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
> **Qualifications**
> - https://localhost:55961/findproviders/api/qualifications
> 
> **Provider search**
> - https://localhost:55961/findproviders/api/providers?postcode=CV1%202WT
> - https://localhost:55961/findproviders/api/providers?postcode=CV1%202WT&qualificationId=37
> - https://localhost:55961/findproviders/api/providers?postcode=CV1%202WT&qualificationId=37&page=3
> - https://localhost:55961/findproviders/api/providers?postcode=CV1%202WT&qualificationId=37&page=0&pageSize=10

For provider search, the postcode at the end of the url is required. 
The `qualificationId` filter is optional and defaults to null or 0; 
`page` and `pageSize` are also optional and default to 0 and 5 respectively.

If the postcode is not found in provider search, the API will return a 404 result with a message indicating the postcode was not found.


## Database

##### Route mapping

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


## External APIs

Postcode details are retrieved using the postcodes.io API. Where possible,
results are cached to avoid duplicate calls.

