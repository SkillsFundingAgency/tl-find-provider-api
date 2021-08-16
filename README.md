[![Build Status](https://dev.azure.com/dfe-ssp/S126-Tlevelservice/_apis/build/status/tl-find-provider-api?repoName=SkillsFundingAgency%2Ftl-find-provider-api&branchName=main)](https://dev.azure.com/dfe-ssp/S126-Tlevelservice/_build/latest?definitionId=1184&repoName=SkillsFundingAgency%2Ftl-find-provider-api&branchName=main)

# tl-find-provider-api

Tlevels Find a Provider API repository


## Database notes

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

Deletion of data is by setting the IsDeleted flag - this is done in the stored procedure.

Updating a deleted row will set the row to undeleted; this acts to logically re-add a row that was previously soft-deleted.


