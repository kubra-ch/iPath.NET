/*********************************************************************************
*
*
*  draft for converting data from iPath Verison 2
*
*
**********************************************************************************/




-- clean data
delete from Nodes;
delete from GroupMembers;
delete from Groups;
delete from Communities;
delete from Users;


/*
* person => Users
*
************************************************************/

SET IDENTITY_INSERT Users ON;

INSERT INTO Users (id, Username, UsernameInvariant, Email, EmailInvariant, PasswordHash, iPath2PasswordHash, CreatedOn, ModifiedOn, IsActive)
SELECT id, TRIM(username), TRIM(LOWER(username)), TRIM(email), TRIM(LOWER(email)), '', password, entered, modified, 0
  FROM ipath_basys.dbo.person 

SET IDENTITY_INSERT Users OFF;


-- import data and info field in separate taböe
CREATE TABLE UserXml (UserId int, data nvarchar(max), info nvarchar(max));

INSERT INTO UserXml (UserId, data, info)
SELECT  id, data, info FROM ipath_basys.dbo.person 

-- name
UPDATE Users 
   SET Familyname = LEFT(TRY_CONVERT(XML, x.data).value('(/data/name/node())[1]', 'nvarchar(max)'), 100)
	FROM Users u 
	JOIN UserXml x ON x.UserId = U.Id
	
-- Firstname
UPDATE Users 
   SET Firstname = LEFT(TRY_CONVERT(XML, x.data).value('(/data/firstname/node())[1]', 'nvarchar(max)'), 100)
	FROM Users u 
	JOIN UserXml x ON x.UserId = U.Id

-- country
UPDATE Users 
   SET Country = LEFT(TRY_CONVERT(XML, x.data).value('(/data/country/node())[1]', 'nvarchar(max)'), 50)
	FROM Users u 
	JOIN UserXml x ON x.UserId = U.Id
	
-- specialisation
UPDATE Users 
   SET Specialisation = LEFT(TRY_CONVERT(XML, x.data).value('(/data/specialisation/node())[1]', 'nvarchar(max)'), 200)
	FROM Users u 
	JOIN UserXml x ON x.UserId = U.Id







/*
 * community => Community
 *
 ************************************************************/
SET IDENTITY_INSERT Communities ON;

INSERT INTO Communities (Id, Name, Descritption, OwnerId, CreatedOn, Visibility)
select id, name, description, created_by, created_on, 0
  from [ipath_basys].[ipath_basys].[community]

SET IDENTITY_INSERT Communities OFF;




/*
 * group => Groups
 *
 ************************************************************/
SET IDENTITY_INSERT Groups ON;

ALTER TABLE Groups ADD _status INT; -- temp field

INSERT INTO Groups (Id, Name, CreatedOn, Visibility, GroupType, _status)
select id, name, entered, 1, ISNULL(type, 1), status
  from ipath_basys.dbo.groups

SET IDENTITY_INSERT Groups OFF;


-- import data and info field in separate taböe
CREATE TABLE GroupXml (GroupId int, info nvarchar(max));

INSERT INTO GroupXml (GroupId, info)
SELECT  id, info FROM ipath_basys.dbo.groups



/*
 * members => GroupMembers
 *
 ************************************************************/
 SET IDENTITY_INSERT GroupMembers ON;

 -- first insert all as users (role = 1)
 INSERT INTO GroupMembers (Id, GroupId, UserId, Role)
 select id, group_id, user_id, 1 from ipath_basys.dbo.group_member
 WHERE group_id IN (select ID FROM Groups)
 
 SET IDENTITY_INSERT GroupMembers OFF;


 -- Moderators (Flag 4)
 UPDATE GroupMembers 
    SET Role = Role & 4
  FROM GroupMembers GM 
	JOIN ipath_basys.dbo.group_member m ON m.id = Gm.Id
 WHERE (m.status & 4) > 0
 
 -- Inactive (Flag 2)
 UPDATE GroupMembers 
    SET Role = Role & 2
  FROM GroupMembers GM 
	JOIN ipath_basys.dbo.group_member m ON m.id = Gm.Id
 WHERE (m.status & 2) > 0
 
 -- Guest (Flag 8)
 UPDATE GroupMembers 
    SET Role = Role & 8
  FROM GroupMembers GM 
	JOIN ipath_basys.dbo.group_member m ON m.id = Gm.Id
 WHERE (m.status & 8) > 0





/*
 * objects => Nodes
 *
 ************************************************************/

