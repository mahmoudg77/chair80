/*
   22 فبراير, 201908:02:16 م
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
ALTER TABLE dbo.tbl_vehicles
	DROP CONSTRAINT FK_tbl_vehicles_tbl_accounts
GO
ALTER TABLE dbo.tbl_accounts SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tbl_accounts', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tbl_accounts', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tbl_accounts', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_vehicles
	DROP CONSTRAINT DF_tbl_vehicles_created_at
GO
CREATE TABLE dbo.Tmp_tbl_vehicles
	(
	id int NOT NULL IDENTITY (1, 1),
	model nvarchar(100) NOT NULL,
	color nvarchar(50) NULL,
	capacity int NOT NULL,
	owner_id int NOT NULL,
	created_at datetime NULL,
	created_by int NOT NULL,
	license_no nvarchar(50) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_tbl_vehicles SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_tbl_vehicles ADD CONSTRAINT
	DF_tbl_vehicles_created_at DEFAULT (getdate()) FOR created_at
GO
SET IDENTITY_INSERT dbo.Tmp_tbl_vehicles ON
GO
IF EXISTS(SELECT * FROM dbo.tbl_vehicles)
	 EXEC('INSERT INTO dbo.Tmp_tbl_vehicles (id, model, color, capacity, owner_id, created_at, created_by)
		SELECT id, model, color, capacity, owner_id, created_at, created_by FROM dbo.tbl_vehicles WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_tbl_vehicles OFF
GO
ALTER TABLE dbo.tbl_drivers_vechiles_rel
	DROP CONSTRAINT FK_tbl_drivers_vechiles_rel_vechile
GO
DROP TABLE dbo.tbl_vehicles
GO
EXECUTE sp_rename N'dbo.Tmp_tbl_vehicles', N'tbl_vehicles', 'OBJECT' 
GO
ALTER TABLE dbo.tbl_vehicles ADD CONSTRAINT
	PK_tbl_vehicles PRIMARY KEY CLUSTERED 
	(
	id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.tbl_vehicles ADD CONSTRAINT
	FK_tbl_vehicles_tbl_accounts FOREIGN KEY
	(
	owner_id
	) REFERENCES dbo.tbl_accounts
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tbl_vehicles', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tbl_vehicles', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tbl_vehicles', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_drivers_vechiles_rel ADD CONSTRAINT
	FK_tbl_drivers_vechiles_rel_vechile FOREIGN KEY
	(
	vechile_id
	) REFERENCES dbo.tbl_vehicles
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.tbl_drivers_vechiles_rel SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tbl_drivers_vechiles_rel', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tbl_drivers_vechiles_rel', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tbl_drivers_vechiles_rel', 'Object', 'CONTROL') as Contr_Per 