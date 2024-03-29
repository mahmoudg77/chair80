/*
   08 أبريل, 201902:13:27 م
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
CREATE TABLE dbo.trip_types
	(
	id int NOT NULL IDENTITY (1, 1),
	name nvarchar(100) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.trip_types ADD CONSTRAINT
	PK_trip_types PRIMARY KEY CLUSTERED 
	(
	id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.trip_types SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.trip_types', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.trip_types', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.trip_types', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
CREATE TABLE dbo.trip_request
	(
	id int NOT NULL IDENTITY (1, 1),
	rider_id int NOT NULL,
	seats int NULL,
	start_at datetime NULL,
	from_lat decimal(18, 6) NULL,
	from_long decimal(18, 6) NULL,
	from_plc nvarchar(100) NULL,
	to_lat decimal(18, 6) NULL,
	to_long decimal(18, 6) NULL,
	to_plc nvarchar(100) NULL,
	gender_id int NULL,
	seat_cost_from decimal(18, 2) NULL,
	seat_cost_to decimal(18, 2) NULL,
	trip_type_id int NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.trip_request ADD CONSTRAINT
	DF_trip_request_seats DEFAULT 1 FOR seats
GO
ALTER TABLE dbo.trip_request ADD CONSTRAINT
	PK_trip_request PRIMARY KEY CLUSTERED 
	(
	id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.trip_request SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.trip_request', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.trip_request', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.trip_request', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
EXECUTE sp_rename N'dbo.trip_share.account_id', N'Tmp_driver_id_2', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.trip_share.shared_seats', N'Tmp_seats_3', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.trip_share.booked_sats', N'Tmp_booked_seats_4', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.trip_share.Tmp_driver_id_2', N'driver_id', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.trip_share.Tmp_seats_3', N'seats', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.trip_share.Tmp_booked_seats_4', N'booked_seats', 'COLUMN' 
GO
ALTER TABLE dbo.trip_share SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.trip_share', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.trip_share', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.trip_share', 'Object', 'CONTROL') as Contr_Per 