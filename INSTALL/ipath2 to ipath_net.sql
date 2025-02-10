/*********************************************************************************
*
*
*  draft for converting data from iPath Verison 2
*
*
**********************************************************************************/




-- clean data
delete from NodeAnnotations;
delete from NodeXml;
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
	JOIN UserXml x ON x.UserId = U.Id;
	
-- Firstname
UPDATE Users 
   SET Firstname = LEFT(TRY_CONVERT(XML, x.data).value('(/data/firstname/node())[1]', 'nvarchar(max)'), 100)
	FROM Users u 
	JOIN UserXml x ON x.UserId = U.Id;

-- country
UPDATE Users 
   SET Country = LEFT(TRY_CONVERT(XML, x.data).value('(/data/country/node())[1]', 'nvarchar(max)'), 50)
	FROM Users u 
	JOIN UserXml x ON x.UserId = U.Id;
	
-- specialisation
UPDATE Users 
   SET Specialisation = LEFT(TRY_CONVERT(XML, x.data).value('(/data/specialisation/node())[1]', 'nvarchar(max)'), 200)
	FROM Users u 
	JOIN UserXml x ON x.UserId = U.Id;


-- Upödate admin User
UPDATE Users SET IsActive = 1, IsSysAdmin = 1 WHERE Username='admin'






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
ALTER TABLE Groups ADD _status INT; -- temp field

SET IDENTITY_INSERT Groups ON;

INSERT INTO Groups (Id, Name, CreatedOn, Visibility, GroupType, _status)
SELECT id, name, entered, 1, ISNULL(type, 1), status
  FROM ipath_basys.dbo.groups

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
	 SELECT id, group_id, user_id, 1 from ipath_basys.dbo.group_member
	  WHERE group_id IN (select ID FROM Groups)
 
 SET IDENTITY_INSERT GroupMembers OFF;


 -- Moderators (Flag 4)
 UPDATE GroupMembers 
    SET Role = Role & 4
  FROM GroupMembers GM 
	JOIN ipath_basys.dbo.group_member m ON m.id = Gm.Id
 WHERE (m.status & 4) > 0;
 
 -- Inactive (Flag 2)
 UPDATE GroupMembers 
    SET Role = Role & 2
  FROM GroupMembers GM 
	JOIN ipath_basys.dbo.group_member m ON m.id = Gm.Id
 WHERE (m.status & 2) > 0;
 
 -- Guest (Flag 8)
 UPDATE GroupMembers 
    SET Role = Role & 8
  FROM GroupMembers GM 
	JOIN ipath_basys.dbo.group_member m ON m.id = Gm.Id
 WHERE (m.status & 8) > 0;





/*
 * objects (class = 'case') => Nodes
 *
 ************************************************************/
 ALTER TABLE Nodes ADD _class VARCHAR(100);

 SET IDENTITY_INSERT Nodes ON;

-- root nodes
INSERT INTO Nodes (id, OwnerId, GroupId, Status, CreatedOn, ModifiedOn, NodeType, _class)
	 SELECT id, sender_id, group_id, status, entered, modified
				, CASE WHEN class='case' THEN 1 WHEN class='folder' THEN 2 ELSE 0 END, class
		 FROM ipath_basys.dbo.objects 
		WHERE group_id > 0 AND sender_id > 0 AND (parent_id=0 or parent_id is null)
		  AND group_id in (SELECT id FROM Groups)
		  AND sender_id in (SELECT id FROM Users)


-- child nodes level 1
INSERT INTO Nodes (id, OwnerId, ParentNodeId, TopNodeId, SortNr, Status, CreatedOn, ModifiedOn, NodeType, _class)
	 SELECT id, sender_id, parent_id, parent_id, sort_nr, status, entered, modified
				, CASE WHEN class='case' THEN 1 WHEN class='folder' THEN 2 ELSE 0 END, class
		 FROM ipath_basys.dbo.objects 
		WHERE sender_id > 0 AND sender_id in (SELECT id FROM Users)
		  AND NOT ID IN (SELECT ID FROM Nodes)
		  AND parent_id IN (SELECT Id FROM Nodes)
			

-- child nodes level 2
INSERT INTO Nodes (id, OwnerId, ParentNodeId, TopNodeId, SortNr, Status, CreatedOn, ModifiedOn, NodeType, _class)
	 SELECT o.id, o.sender_id, o.parent_id, p.TopNodeId, o.sort_nr, o.status, o.entered, o.modified
				, CASE WHEN o.class='case' THEN 1 WHEN o.class='folder' THEN 2 ELSE 0 END, o.class
		 FROM ipath_basys.dbo.objects o 
		 JOIN Nodes p on p.Id = o.parent_id
		WHERE sender_id > 0 AND sender_id in (SELECT id FROM Users)
		  AND NOT o.ID IN (SELECT ID FROM Nodes)
		  AND parent_id IN (SELECT Id FROM Nodes)


-- child nodes level 3
INSERT INTO Nodes (id, OwnerId, ParentNodeId, TopNodeId, SortNr, Status, CreatedOn, ModifiedOn, NodeType, _class)
	 SELECT o.id, o.sender_id, o.parent_id, p.TopNodeId, o.sort_nr, o.status, o.entered, o.modified
				, CASE WHEN o.class='case' THEN 1 WHEN o.class='folder' THEN 2 ELSE 0 END, o.class
		 FROM ipath_basys.dbo.objects o 
		 JOIN Nodes p on p.Id = o.parent_id
		WHERE sender_id > 0 AND sender_id in (SELECT id FROM Users)
		  AND NOT o.ID IN (SELECT ID FROM Nodes)
		  AND parent_id IN (SELECT Id FROM Nodes)


-- child nodes level 4
INSERT INTO Nodes (id, OwnerId, ParentNodeId, TopNodeId, SortNr, Status, CreatedOn, ModifiedOn, NodeType, _class)
	 SELECT o.id, o.sender_id, o.parent_id, p.TopNodeId, o.sort_nr, o.status, o.entered, o.modified
				, CASE WHEN o.class='case' THEN 1 WHEN o.class='folder' THEN 2 ELSE 0 END, o.class
		 FROM ipath_basys.dbo.objects o 
		 JOIN Nodes p on p.Id = o.parent_id
		WHERE sender_id > 0 AND sender_id in (SELECT id FROM Users)
		  AND NOT o.ID IN (SELECT ID FROM Nodes)
		  AND parent_id IN (SELECT Id FROM Nodes)


-- child nodes level 5
INSERT INTO Nodes (id, OwnerId, ParentNodeId, TopNodeId, SortNr, Status, CreatedOn, ModifiedOn, NodeType, _class)
	 SELECT o.id, o.sender_id, o.parent_id, p.TopNodeId, o.sort_nr, o.status, o.entered, o.modified
				, CASE WHEN o.class='case' THEN 1 WHEN o.class='folder' THEN 2 ELSE 0 END, o.class
		 FROM ipath_basys.dbo.objects o 
		 JOIN Nodes p on p.Id = o.parent_id
		WHERE sender_id > 0 AND sender_id in (SELECT id FROM Users)
		  AND NOT o.ID IN (SELECT ID FROM Nodes)
		  AND parent_id IN (SELECT Id FROM Nodes)


-- child nodes level 6
INSERT INTO Nodes (id, OwnerId, ParentNodeId, TopNodeId, SortNr, Status, CreatedOn, ModifiedOn, NodeType, _class)
	 SELECT o.id, o.sender_id, o.parent_id, p.TopNodeId, o.sort_nr, o.status, o.entered, o.modified
				, CASE WHEN o.class='case' THEN 1 WHEN o.class='folder' THEN 2 ELSE 0 END, o.class
		 FROM ipath_basys.dbo.objects o 
		 JOIN Nodes p on p.Id = o.parent_id
		WHERE sender_id > 0 AND sender_id in (SELECT id FROM Users)
		  AND NOT o.ID IN (SELECT ID FROM Nodes)
		  AND parent_id IN (SELECT Id FROM Nodes)


-- child nodes level 7
INSERT INTO Nodes (id, OwnerId, ParentNodeId, TopNodeId, SortNr, Status, CreatedOn, ModifiedOn, NodeType, _class)
	 SELECT o.id, o.sender_id, o.parent_id, p.TopNodeId, o.sort_nr, o.status, o.entered, o.modified
				, CASE WHEN o.class='case' THEN 1 WHEN o.class='folder' THEN 2 ELSE 0 END, o.class
		 FROM ipath_basys.dbo.objects o 
		 JOIN Nodes p on p.Id = o.parent_id
		WHERE sender_id > 0 AND sender_id in (SELECT id FROM Users)
		  AND NOT o.ID IN (SELECT ID FROM Nodes)
		  AND parent_id IN (SELECT Id FROM Nodes)

		
SET IDENTITY_INSERT Nodes OFF;


-- Set Nodes Visible from status
/*
	define( "OBJECTSTAT_PUBLIC",         1 );
	define( "OBJECTSTAT_DELETED",        2 );
	define( "OBJECTSTAT_ALLOWADD",       4 );
	define( "OBJECTSTAT_COMMENTSHIDDEN", 8 );
	define( "OBJECTSTAT_HIDDEN",        16 );
*/

UPDATE Nodes SET Visibility = 1; -- set all visible first
UPDATE Nodes SET Visibility = 2 WHERE status = 1; -- status 1 -> public
UPDATE Nodes SET Visibility = 11 WHERE status = 2; -- status 2 -> deleted
UPDATE Nodes SET Visibility = 10 WHERE status = 16; -- status 16 -> hidden



select class, count(*)  from ipath_basys.dbo.objects 
 where parent_id>0 
 group by class



/*
 * objects.data/info => NodeXml
 *
 ************************************************************/
update ipath_basys.dbo.objects SET info = NULL WHERE info='';
update ipath_basys.dbo.objects SET data = NULL WHERE data='';

INSERT INTO NodeXml (NodeId, data, info, Converted)
SELECT id, ISNULL(data, '<data />'), ISNULL(info, '<info />'), 0 FROM ipath_basys.dbo.objects 
 WHERE id IN (SELECT Id FROM Nodes);

-- title
UPDATE Nodes 
   SET Title = LEFT(TRY_CONVERT(XML, x.data).value('(/data/title/node())[1]', 'nvarchar(max)'), 255)
	FROM Nodes n
	JOIN NodeXml x ON x.NodeId = n.Id;

-- subtitle
UPDATE Nodes 
   SET SubTitle = LEFT(TRY_CONVERT(XML, x.data).value('(/data/subtitle/node())[1]', 'nvarchar(max)'), 255)
	FROM Nodes n
	JOIN NodeXml x ON x.NodeId = n.Id;

-- description
UPDATE Nodes 
   SET Description = TRY_CONVERT(XML, x.data).value('(/data/description/node())[1]', 'nvarchar(max)')
	FROM Nodes n
	JOIN NodeXml x ON x.NodeId = n.Id;


-- for images/files => filename, mimetype
INSERT INTO NodeFile (NodeId, Filename, Originalname, Mimetype, IsImage)
SELECT NodeId
     , TRY_CONVERT(XML, x.data).value('(/data/filename/node())[1]', 'nvarchar(max)')
     , ISNULL(TRY_CONVERT(XML, x.data).value('(/data/origname/node())[1]', 'nvarchar(max)'), '')
		 , TRY_CONVERT(XML, x.data).value('(/data/mimetype/node())[1]', 'nvarchar(max)')
		 , 0
  FROM NodeXml x
 WHERE LEN(TRY_CONVERT(XML, x.data).value('(/data/filename/node())[1]', 'nvarchar(max)')) > 0

 UPDATE NodeFile SET IsImage=1 WHERE MimeType LIKE 'image/%'

 


/*
 * Annotations
 *
 ************************************************************/
 ALTER TABLE NodeAnnotations ADD _status int null;
 ALTER TABLE NodeAnnotations ADD _type varchar(100) null;
 
SET IDENTITY_INSERT NodeAnnotations ON;

INSERT INTO NodeAnnotations (Id, NodeId, OwnerId, _status, CreatedOn, ModifiedOn, Text, _type)
SELECT id, object_id, sender_id, status, entered, modified
     , TRY_CONVERT(XML, data).value('(/data/text/node())[1]', 'nvarchar(max)')
     , TRY_CONVERT(XML, data).value('(/data/type/node())[1]', 'nvarchar(100)')
  FROM ipath_basys.dbo.annotation
 WHERE sender_id IN (SELECT Id FROM Users)
   AND object_id IN (SELECT Id FROM Nodes)
	 AND LEN(TRY_CONVERT(XML, data).value('(/data/text/node())[1]', 'nvarchar(max)')) > 0
		
SET IDENTITY_INSERT NodeAnnotations OFF;


-- Update Visibility from Status
UPDATE NodeAnnotations SET Visibility = 1 -- Default to visible
UPDATE NodeAnnotations SET Visibility = 11 WHERE _status = 2 -- Default to visible

