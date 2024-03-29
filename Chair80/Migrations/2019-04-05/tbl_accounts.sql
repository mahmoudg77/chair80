/*
   06 أبريل, 201907:54:14 م
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
	DROP CONSTRAINT FK_sec_users_tbl_accounts
GO
ALTER TABLE dbo.tbl_vehicles
	DROP CONSTRAINT FK_tbl_vehicles_tbl_accounts
GO
ALTER TABLE dbo.tbl_drivers_vehicles_rel
	DROP CONSTRAINT FK_tbl_drivers_vehicles_rel_driver
GO
ALTER TABLE dbo.trip_share
	DROP CONSTRAINT FK_trip_share_tbl_accounts
GO
ALTER TABLE dbo.tbl_accounts SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tbl_accounts', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tbl_accounts', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tbl_accounts', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.trip_share ADD CONSTRAINT
	FK_trip_share_tbl_accounts FOREIGN KEY
	(
	account_id
	) REFERENCES dbo.tbl_accounts
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.trip_share SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.trip_share', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.trip_share', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.trip_share', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_drivers_vehicles_rel ADD CONSTRAINT
	FK_tbl_drivers_vehicles_rel_driver FOREIGN KEY
	(
	driver_id
	) REFERENCES dbo.tbl_accounts
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.tbl_drivers_vehicles_rel SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tbl_drivers_vehicles_rel', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tbl_drivers_vehicles_rel', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tbl_drivers_vehicles_rel', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_vehicles ADD CONSTRAINT
	FK_tbl_vehicles_tbl_accounts FOREIGN KEY
	(
	owner_id
	) REFERENCES dbo.tbl_accounts
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.tbl_vehicles SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tbl_vehicles', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tbl_vehicles', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tbl_vehicles', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.sec_users ADD CONSTRAINT
	FK_sec_users_tbl_accounts FOREIGN KEY
	(
	id
	) REFERENCES dbo.tbl_accounts
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.sec_users SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.sec_users', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.sec_users', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.sec_users', 'Object', 'CONTROL') as Contr_Per 