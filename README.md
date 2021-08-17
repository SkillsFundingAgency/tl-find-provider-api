[![Build Status](https://dev.azure.com/dfe-ssp/S126-Tlevelservice/_apis/build/status/S126-TL/Find%20Provider%20API/tl-find-provider-api?repoName=SkillsFundingAgency%2Ftl-find-provider-api&branchName=main)](https://dev.azure.com/dfe-ssp/S126-Tlevelservice/_build/latest?definitionId=1184&repoName=SkillsFundingAgency%2Ftl-find-provider-api&branchName=main)

# tl-find-provider-api

Tlevels Find a Provider API repository


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

Course directory API values can be obtained from the NCS API portal. Ask the NCS Course Directory Team for details.
        
CRON schedule needs to use a valid CRON string, such as `0 0 9 ? * MON-FRI`


## Database

Data insert/update/delete is handled in stored procedures, which are passed table-valued parameters 
to be merged into the tables. 

The merge statements in the stored procedures need to use this syntax for comparing nullable columns:
```
    OR ISNULL(NULLIF(t.[AddressLine1], s.[AddressLine1]), NULLIF(s.[AddressLine1], t.[AddressLine1])) IS NOT NULL
```

These procedures pass back a summary of changes, with counts of inserted, updated and (soft) deleted rows.

Dapper is used as the ORM layer. For more information on using table valued parameters, see [Bulk Upsert with Dapper and Sql Server](https://blog.schroederspace.com/tumbleweed-technology/bulk-upsert-with-dapper-and-sql-server)

The qualification Id column is the T Level Framework id from the Course Directory tleveldefinitions. 
It is used as the primary key there is no identity key in Qualifications).

Deletion of data is by setting the IsDeleted flag (soft delete) as part of the merge statements in the stored procedures. 
Soft deletion avoids complitions with cascading deletes; all selects must check the IsDeleted flag to avoid reading deleted data.
Deletion of rows in LocationQualification is a hard delete, since this is a mapping table.

Updating a deleted row will set the row to undeleted; this acts to logically re-add a row that was previously soft-deleted.

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

