/*
   22 فبراير, 201909:18:20 ص
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
ALTER TABLE dbo.tbl_accounts SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tbl_accounts', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tbl_accounts', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tbl_accounts', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_vehicles SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tbl_vehicles', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tbl_vehicles', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tbl_vehicles', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
CREATE TABLE dbo.tbl_drivers_vechiles_rel
	(
	id int NOT NULL IDENTITY (1, 1),
	driver_id int NULL,
	vechile_id int NULL,
	created_at datetime NULL,
	created_by datetime NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.tbl_drivers_vechiles_rel ADD CONSTRAINT
	PK_tbl_drivers_vechiles_rel PRIMARY KEY CLUSTERED 
	(
	id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tbl_drivers_vechiles_rel ADD CONSTRAINT
	FK_Table_1_vechile FOREIGN KEY
	(
	vechile_id
	) REFERENCES dbo.tbl_vehicles
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tbl_drivers_vechiles_rel ADD CONSTRAINT
	FK_Table_1_driver FOREIGN KEY
	(
	driver_id
	) REFERENCES dbo.tbl_accounts
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tbl_drivers_vechiles_rel SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tbl_drivers_vechiles_rel', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tbl_drivers_vechiles_rel', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tbl_drivers_vechiles_rel', 'Object', 'CONTROL') as Contr_Per 