/*
   08 أبريل, 201911:47:50 ص
   User: 
   Server: .
   Database: chari80_db
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.sec_sessions ADD
	device_id nvarchar(MAX) NULL
GO
ALTER TABLE dbo.sec_sessions SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.sec_sessions', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.sec_sessions', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.sec_sessions', 'Object', 'CONTROL') as Contr_Per 