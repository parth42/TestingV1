--Update Menu item positions and add new Admin tab
UPDATE dbo.tblMenuItem
SET SortOrder = 1
WHERE MenuItem in ('User');

UPDATE dbo.tblMenuItem
SET SortOrder = 2
WHERE MenuItem in ('Role');

UPDATE dbo.tblMenuItem
SET SortOrder = 3
WHERE MenuItem = 'Role Privileges';

UPDATE dbo.tblMenuItem
SET SortOrder = 4
WHERE MenuItem = 'Section';

UPDATE dbo.tblMenuItem
SET SortOrder = 5
WHERE MenuItem = 'Company';

UPDATE dbo.tblMenuItem
SET SortOrder = 6
WHERE MenuItem = 'Log Activity';

declare @UserMenuItemID int;
select @UserMenuItemID = MenuItemID from dbo.tblMenuItem where MenuItem = 'User';

--UPDATE dbo.tblMenuItem
--set ParentID = @UserMenuItemID
--WHERE MenuItem in ('Role','Role Privileges');

IF NOT EXISTS 
(select * from dbo.tblMenuItem where MenuItem = 'Admin')
BEGIN
	INSERT INTO dbo.tblMenuItem (MenuItem, MenuItemController, MenuItemView, SortOrder, ParentID, IsActive) 
	VALUES ('Admin',' ', ' ',9, 0, 1);
END

UPDATE dbo.tblMenuItem
set SortOrder = 10
where MenuItem = 'Admin';

--Add missing role privileges for new Admin menu item (set only for super admins)
declare @AdminMenuItemID int;
select @AdminMenuItemID = MenuItemID from dbo.tblMenuItem where MenuItem = 'Admin';

UPDATE dbo.tblMenuItem
set ParentID = @AdminMenuItemID
WHERE MenuItem in ('User','Company','LogActivity','Section','Role','Role Privileges');

declare @listOfRolePrivilegeIDs table(idx int identity(1,1),id int);

insert into @listOfRolePrivilegeIDs 
select distinct rp.RoleID 
from dbo.tblRolePrivilages rp
left join dbo.tblRole r on r.RoleID = rp.RoleID
where IsAdminRole = 1;

declare @i int;
declare @cnt int;
declare @id int;

select @i = min(idx) - 1 from @listOfRolePrivilegeIDs;
select @cnt = max(idx) from @listOfRolePrivilegeIDs;

while @i < @cnt
begin
     select @i = @i + 1
	 select @id = id from @listOfRolePrivilegeIDs where idx = @i
	 select @id
     IF NOT EXISTS 
	(select * from dbo.tblRolePrivilages where MenuItemID = @AdminMenuItemID and RoleID = @id)
	 BEGIN
	 INSERT INTO dbo.tblRolePrivilages(MenuItemID,ViewPermission,UserID,CreatedBy,[Add],Edit,[Delete],Detail,IsActive,RoleID) 
		VALUES (@AdminMenuItemID,1,1,1,1,1,1,1,1,@id);
	 END
end
