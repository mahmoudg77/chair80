/*
   22 فبراير, 201908:50:01 م
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
ALTER TABLE dbo.tbl_drivers_vehicles_rel
	DROP CONSTRAINT FK_tbl_drivers_vechiles_rel_driver
GO
ALTER TABLE dbo.tbl_accounts SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tbl_accounts', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tbl_accounts', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tbl_accounts', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
EXECUTE sp_rename N'dbo.tbl_drivers_vehicles_rel.vechile_id', N'Tmp_vehicle_id', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.tbl_drivers_vehicles_rel.Tmp_vehicle_id', N'vehicle_id', 'COLUMN' 
GO
ALTER TABLE dbo.tbl_drivers_vehicles_rel ADD CONSTRAINT
	FK_tbl_drivers_vehicles_rel_driver FOREIGN KEY
	(
	driver_id
	) REFERENCES dbo.tbl_accounts
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tbl_drivers_vehicles_rel SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tbl_drivers_vehicles_rel', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tbl_drivers_vehicles_rel', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tbl_drivers_vehicles_rel', 'Object', 'CONTROL') as Contr_Per 