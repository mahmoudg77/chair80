/*
   08 أبريل, 201903:02:32 م
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
ALTER TABLE dbo.trip_share
	DROP CONSTRAINT FK_trip_share_tbl_accounts
GO
ALTER TABLE dbo.tbl_accounts SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tbl_accounts', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tbl_accounts', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tbl_accounts', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.trip_share
	DROP CONSTRAINT FK_trip_share_tbl_vehicles
GO
ALTER TABLE dbo.tbl_vehicles SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tbl_vehicles', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tbl_vehicles', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tbl_vehicles', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.trip_types SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.trip_types', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.trip_types', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.trip_types', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.trip_request
	DROP CONSTRAINT DF_trip_request_seats
GO
CREATE TABLE dbo.Tmp_trip_request
	(
	id int NOT NULL IDENTITY (1, 1),
	rider_id int NOT NULL,
	seats int NULL,
	start_at_date date NULL,
	end_at_date date NULL,
	trip_type_id int NULL,
	created_at datetime NULL,
	created_by int NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_trip_request SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_trip_request ADD CONSTRAINT
	DF_trip_request_seats DEFAULT ((1)) FOR seats
GO
SET IDENTITY_INSERT dbo.Tmp_trip_request ON
GO
IF EXISTS(SELECT * FROM dbo.trip_request)
	 EXEC('INSERT INTO dbo.Tmp_trip_request (id, rider_id, seats, start_at_date, trip_type_id)
		SELECT id, rider_id, seats, CONVERT(date, start_at), trip_type_id FROM dbo.trip_request WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_trip_request OFF
GO
DROP TABLE dbo.trip_request
GO
EXECUTE sp_rename N'dbo.Tmp_trip_request', N'trip_request', 'OBJECT' 
GO
ALTER TABLE dbo.trip_request ADD CONSTRAINT
	PK_trip_request PRIMARY KEY CLUSTERED 
	(
	id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.trip_request ADD CONSTRAINT
	FK_trip_request_trip_types FOREIGN KEY
	(
	trip_type_id
	) REFERENCES dbo.trip_types
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
select Has_Perms_By_Name(N'dbo.trip_request', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.trip_request', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.trip_request', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
CREATE TABLE dbo.trip_request_details
	(
	id int NOT NULL IDENTITY (1, 1),
	trip_request_id int NOT NULL,
	seats int NULL,
	start_at_date date NULL,
	start_at_time time(7) NULL,
	from_lat decimal(18, 6) NULL,
	from_lng decimal(18, 6) NULL,
	from_plc nvarchar(100) NULL,
	to_lat decimal(18, 6) NULL,
	to_long decimal(18, 6) NULL,
	to_plc nvarchar(100) NULL,
	gender_id int NULL,
	seat_cost_from decimal(18, 2) NULL,
	seat_cost_to decimal(18, 2) NULL,
	trip_direction int NULL,
	is_active bit NULL
	)  ON [PRIMARY]
GO
DECLARE @v sql_variant 
SET @v = N'1= leave , 2 = round'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'trip_request_details', N'COLUMN', N'trip_direction'
GO
ALTER TABLE dbo.trip_request_details ADD CONSTRAINT
	DF_trip_request_details_seats DEFAULT ((1)) FOR seats
GO
ALTER TABLE dbo.trip_request_details ADD CONSTRAINT
	DF_trip_request_details_trip_direction DEFAULT 1 FOR trip_direction
GO
ALTER TABLE dbo.trip_request_details ADD CONSTRAINT
	DF_trip_request_details_is_active DEFAULT 1 FOR is_active
GO
ALTER TABLE dbo.trip_request_details ADD CONSTRAINT
	PK_trip_request_details PRIMARY KEY CLUSTERED 
	(
	id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.trip_request_details ADD CONSTRAINT
	FK_trip_request_details_trip_request FOREIGN KEY
	(
	trip_request_id
	) REFERENCES dbo.trip_request
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.trip_request_details SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.trip_request_details', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.trip_request_details', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.trip_request_details', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.trip_share
	DROP CONSTRAINT FK_trip_share_tbl_genders
GO
ALTER TABLE dbo.tbl_genders SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.tbl_genders', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.tbl_genders', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.tbl_genders', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.trip_share
	DROP CONSTRAINT DF_trip_share_sates
GO
ALTER TABLE dbo.trip_share
	DROP CONSTRAINT DF_trip_share_booked_sats
GO
ALTER TABLE dbo.trip_share
	DROP CONSTRAINT DF_trip_share_sat_cost
GO
CREATE TABLE dbo.Tmp_trip_share
	(
	id int NOT NULL IDENTITY (1, 1),
	driver_id int NOT NULL,
	start_at_date date NULL,
	end_at_date date NULL,
	return_trip_id int NULL,
	vehicle_id int NULL,
	created_at datetime NULL,
	created_by int NULL,
	trip_type_id int NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_trip_share SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_trip_share ON
GO
IF EXISTS(SELECT * FROM dbo.trip_share)
	 EXEC('INSERT INTO dbo.Tmp_trip_share (id, driver_id, start_at_date, return_trip_id, vehicle_id, created_at, created_by)
		SELECT id, driver_id, CONVERT(date, start_at), return_trip_id, vehicle_id, created_at, created_by FROM dbo.trip_share WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_trip_share OFF
GO
ALTER TABLE dbo.trip_share
	DROP CONSTRAINT FK_trip_share_trip_share
GO
ALTER TABLE dbo.trip_book
	DROP CONSTRAINT FK_trip_book_trip_share
GO
DROP TABLE dbo.trip_share
GO
EXECUTE sp_rename N'dbo.Tmp_trip_share', N'trip_share', 'OBJECT' 
GO
ALTER TABLE dbo.trip_share ADD CONSTRAINT
	PK_trip_share PRIMARY KEY CLUSTERED 
	(
	id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

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
	FK_trip_share_tbl_accounts FOREIGN KEY
	(
	driver_id
	) REFERENCES dbo.tbl_accounts
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.trip_share ADD CONSTRAINT
	FK_trip_share_trip_types FOREIGN KEY
	(
	trip_type_id
	) REFERENCES dbo.trip_types
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
select Has_Perms_By_Name(N'dbo.trip_share', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.trip_share', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.trip_share', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
CREATE TABLE dbo.trip_share_details
	(
	id int NOT NULL IDENTITY (1, 1),
	trip_share_id int NOT NULL,
	seats int NULL,
	start_at_date date NULL,
	start_at_time time(7) NULL,
	from_lat decimal(18, 6) NULL,
	from_lng decimal(18, 6) NULL,
	from_plc nvarchar(100) NULL,
	to_lat decimal(18, 6) NULL,
	to_long decimal(18, 6) NULL,
	to_plc nvarchar(100) NULL,
	gender_id int NULL,
	booked_seats int NULL,
	seat_cost decimal(18, 2) NULL,
	trip_direction int NULL,
	is_active bit NULL
	)  ON [PRIMARY]
GO
DECLARE @v sql_variant 
SET @v = N'1= leave , 2 = round'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'trip_share_details', N'COLUMN', N'trip_direction'
GO
ALTER TABLE dbo.trip_share_details ADD CONSTRAINT
	DF_trip_share_details_seats DEFAULT ((1)) FOR seats
GO
ALTER TABLE dbo.trip_share_details ADD CONSTRAINT
	DF_trip_share_details_booked_seats DEFAULT ((0)) FOR booked_seats
GO
ALTER TABLE dbo.trip_share_details ADD CONSTRAINT
	DF_trip_share_details_seat_cost DEFAULT ((0)) FOR seat_cost
GO
ALTER TABLE dbo.trip_share_details ADD CONSTRAINT
	DF_trip_share_details_trip_direction DEFAULT 1 FOR trip_direction
GO
ALTER TABLE dbo.trip_share_details ADD CONSTRAINT
	DF_trip_share_details_is_active DEFAULT 1 FOR is_active
GO
ALTER TABLE dbo.trip_share_details ADD CONSTRAINT
	PK_trip_share_details PRIMARY KEY CLUSTERED 
	(
	id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.trip_share_details ADD CONSTRAINT
	FK_trip_share_details_trip_share FOREIGN KEY
	(
	trip_share_id
	) REFERENCES dbo.trip_share
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.trip_share_details SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.trip_share_details', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.trip_share_details', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.trip_share_details', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
EXECUTE sp_rename N'dbo.trip_book.driver_trip_id', N'Tmp_trip_share_details_id_5', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.trip_book.rider_trip_id', N'Tmp_trip_request_details_id_6', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.trip_book.Tmp_trip_share_details_id_5', N'trip_share_details_id', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.trip_book.Tmp_trip_request_details_id_6', N'trip_request_details_id', 'COLUMN' 
GO
ALTER TABLE dbo.trip_book ADD CONSTRAINT
	FK_trip_book_trip_share_details FOREIGN KEY
	(
	trip_share_details_id
	) REFERENCES dbo.trip_share_details
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.trip_book ADD CONSTRAINT
	FK_trip_book_trip_request_details FOREIGN KEY
	(
	trip_request_details_id
	) REFERENCES dbo.trip_request_details
	(
	id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.trip_book SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.trip_book', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.trip_book', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.trip_book', 'Object', 'CONTROL') as Contr_Per 