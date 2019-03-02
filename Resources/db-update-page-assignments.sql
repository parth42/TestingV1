USE [triyo-prod];

IF OBJECT_ID(N'dbo.tblAssignedDocs', N'U') IS NOT NULL
	BEGIN
		EXEC sp_rename 'tblAssignedDocs', 'tblAssignedWordPages';
		EXEC sp_rename 'tblAssignedWordPages.AssignedDocsID', 'AssignedWordPageID', 'COLUMN';
	END

IF COL_LENGTH('dbo.tblAssignedWordPages', 'IsPublished') IS NULL
	BEGIN
		ALTER TABLE dbo.tblAssignedWordPages ADD IsPublished bit NULL
	END

IF COL_LENGTH('dbo.tblAssignedWordPages', 'ReviewRequested') IS NULL
	BEGIN
		ALTER TABLE dbo.tblAssignedWordPages ADD ReviewRequested bit NULL
	END

IF COL_LENGTH('dbo.tblAssignedWordPages', 'Ticks') IS NULL
	BEGIN
		ALTER TABLE dbo.tblAssignedWordPages ADD Ticks varchar(100) NULL
	END


IF COL_LENGTH('dbo.tblAssignedWordPages', 'PageRemarks') IS NULL
	BEGIN
		ALTER TABLE dbo.tblAssignedWordPages ADD PageRemarks varchar(500) NULL 
	END								 

IF COL_LENGTH('dbo.tblAssignmentMembers', 'CanPublish') IS NULL
	BEGIN
		ALTER TABLE dbo.tblAssignmentMembers ADD CanPublish bit NULL;
	END

IF COL_LENGTH('dbo.tblAssignments', 'IsEntireDocument') IS NULL
	BEGIN
		ALTER TABLE dbo.tblAssignments ADD IsEntireDocument bit NULL;
	END