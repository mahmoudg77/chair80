/*
   06 أبريل, 201907:55:54 م
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
ALTER TABLE dbo.sec_sessions
	DROP CONSTRAINT FK_sec_sessions_sec_users
GO
ALTER TABLE dbo.sec_users_roles
	DROP CONSTRAINT FK_sec_users_roles_sec_users
GO
ALTER TABLE dbo.sec_users SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.sec_users', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.sec_users', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.sec_users', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.sec_users_roles ADD CONSTRAINT
	FK_sec_users_roles_sec_users FOREIGN KEY
	(
	user_id
	) REFERENCES dbo.sec_users
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.sec_users_roles SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.sec_users_roles', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.sec_users_roles', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.sec_users_roles', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.sec_sessions ADD CONSTRAINT
	FK_sec_sessions_sec_users FOREIGN KEY
	(
	user_id
	) REFERENCES dbo.sec_users
	(
	id
	) ON UPDATE  CASCADE 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.sec_sessions SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.sec_sessions', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.sec_sessions', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.sec_sessions', 'Object', 'CONTROL') as Contr_Per 