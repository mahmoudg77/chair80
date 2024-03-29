/*
   22 فبراير, 201909:26:53 ص
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
ALTER TABLE dbo.sec_users
	DROP CONSTRAINT DF_sec_users_agree
GO
ALTER TABLE dbo.sec_users
	DROP COLUMN agree
GO
ALTER TABLE dbo.sec_users SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.sec_users', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.sec_users', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.sec_users', 'Object', 'CONTROL') as Contr_Per 