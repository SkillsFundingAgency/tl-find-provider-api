-- Switch Incorrect Framework Codes
--
-- This script should switch the location qualification mapping 
-- for framework codes 49/50 and 50/51.
-- It should be run after the Qualification and RouteQualification
-- seed scripts have been fixed.
-- Change rollback to commit to run after this has been validated.
-- Could be included in the post-deployment with a check on the values of 
-- the qualification name for these codes.

begin transaction 

declare @idTable table (
	LocationQualificationId int, 
	QualificationId int,
	NewQualificationId int)

select * from LocationQualification
where QualificationId in (48, 49, 50, 51)

insert into @idTable
select	Id, 
		QualificationId int,
		case
			when QualificationId = 48 then 49
			when QualificationId = 49 then 48
			when QualificationId = 50 then 51
			when QualificationId = 51 then 50
		end
from LocationQualification
where QualificationId in (48, 49, 50, 51)

select * from @idTable

select * from locationqualification where id in (21, 2194)

update LocationQualification
set QualificationId = x.NewQualificationId
from LocationQualification lq
inner join @idTable x
on x.LocationQualificationId = lq.id
and x.QualificationId = lq.QualificationId

select * from LocationQualification where id in (21, 2194)
--2194	48	49
--21	49	48
--need to insert into temp table, then switch back

select * from LocationQualification
where QualificationId in (48, 49, 50, 51)

rollback
--commit
