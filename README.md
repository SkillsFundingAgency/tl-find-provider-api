# tl-find-provider-api
Tlevels find-provider-api repository

## Database notes

Data insert/update/delete is handled in stored procedures, which are passed table-valued parameters 
to be merged into the tables. 
These procedures pass back a summary of changes, with counts of inserted, updated and (soft) deleted rows.

Dapper is used as the ORM layer. For more information on using table valued parameters, see [Bulk Upsert with Dapper and Sql Server](https://blog.schroederspace.com/tumbleweed-technology/bulk-upsert-with-dapper-and-sql-server)

The qualification Id column is the T Level Framework id from the Course Directory tleveldefinitions. 
It is used as the primary key there is no identity key in Qualifications).

Deletion of data is by setting the IsDeleted flag - this is done in the stored procedure.

Updating a deleted row will set the row to undeleted; this acts to logically re-add a row that was previously soft-deleted.


