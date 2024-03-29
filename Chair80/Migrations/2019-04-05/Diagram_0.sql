/*
   05 أبريل, 201908:32:08 م
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
ALTER TABLE dbo.tbl_genders SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tbl_genders', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tbl_genders', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tbl_genders', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_vehicles SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tbl_vehicles', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tbl_vehicles', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tbl_vehicles', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.tbl_accounts SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tbl_accounts', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tbl_accounts', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tbl_accounts', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
CREATE TABLE dbo.trip_share
	(
	id int NOT NULL IDENTITY (1, 1),
	account_id int NOT NULL,
	shared_sats int NULL,
	start_at datetime NULL,
	from_lat decimal(18, 6) NULL,
	from_long decimal(18, 6) NULL,
	from_plc nvarchar(100) NULL,
	to_lat decimal(18, 6) NULL,
	to_long decimal(18, 6) NULL,
	to_plc nvarchar(100) NULL,
	gender_id int NULL,
	booked_sats int NULL,
	return_trip_id int NULL,
	is_driver bit NULL,
	vehicle_id int NULL,
	sat_cost decimal(18, 2) NULL,
	created_at datetime NULL,
	created_by int NULL
	)  ON [PRIMARY]
GO
DECLARE @v sql_variant 
SET @v = N'1 = driver , 0 = rider'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'trip_share', N'COLUMN', N'is_driver'
GO
ALTER TABLE dbo.trip_share ADD CONSTRAINT
	DF_trip_share_sates DEFAULT 1 FOR shared_sats
GO
ALTER TABLE dbo.trip_share ADD CONSTRAINT
	DF_trip_share_booked_sats DEFAULT 0 FOR booked_sats
GO
ALTER TABLE dbo.trip_share ADD CONSTRAINT
	DF_trip_share_sat_cost DEFAULT 0 FOR sat_cost
GO
ALTER TABLE dbo.trip_share ADD CONSTRAINT
	PK_trip_share PRIMARY KEY CLUSTERED 
	(
	id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.trip_share ADD CONSTRAINT
	FK_trip_share_tbl_accounts FOREIGN KEY
	(
	account_id
	) REFERENCES dbo.tbl_accounts
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.trip_share ADD CONSTRAINT
	FK_trip_share_tbl_vehicles FOREIGN KEY
	(
	vehicle_id
	) REFERENCES dbo.tbl_vehicles
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.trip_share ADD CONSTRAINT
	FK_trip_share_trip_share FOREIGN KEY
	(
	return_trip_id
	) REFERENCES dbo.trip_share
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.trip_share ADD CONSTRAINT
	FK_trip_share_tbl_genders FOREIGN KEY
	(
	gender_id
	) REFERENCES dbo.tbl_genders
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.trip_share SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.trip_share', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.trip_share', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.trip_share', 'Object', 'CONTROL') as Contr_Per 