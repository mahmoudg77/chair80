/*
   08 أبريل, 201903:29:14 م
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
ALTER TABLE dbo.trip_request_details
	DROP CONSTRAINT FK_trip_request_details_trip_request
GO
ALTER TABLE dbo.trip_request SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.trip_request', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.trip_request', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.trip_request', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.trip_request_details
	DROP CONSTRAINT DF_trip_request_details_seats
GO
ALTER TABLE dbo.trip_request_details
	DROP CONSTRAINT DF_trip_request_details_trip_direction
GO
ALTER TABLE dbo.trip_request_details
	DROP CONSTRAINT DF_trip_request_details_is_active
GO
CREATE TABLE dbo.Tmp_trip_request_details
	(
	id int NOT NULL IDENTITY (1, 1),
	trip_request_id int NOT NULL,
	seats int NULL,
	start_at_date date NULL,
	start_at_time time(7) NULL,
	from_lat decimal(18, 6) NULL,
	from_lng decimal(18, 6) NULL,
	from_plc nvarchar(200) NULL,
	to_lat decimal(18, 6) NULL,
	to_lng decimal(18, 6) NULL,
	to_plc nvarchar(200) NULL,
	gender_id int NULL,
	seat_cost_from decimal(18, 2) NULL,
	seat_cost_to decimal(18, 2) NULL,
	trip_direction int NULL,
	is_active bit NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_trip_request_details SET (LOCK_ESCALATION = TABLE)
GO
DECLARE @v sql_variant 
SET @v = N'1= leave , 2 = round'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_trip_request_details', N'COLUMN', N'trip_direction'
GO
ALTER TABLE dbo.Tmp_trip_request_details ADD CONSTRAINT
	DF_trip_request_details_seats DEFAULT ((1)) FOR seats
GO
ALTER TABLE dbo.Tmp_trip_request_details ADD CONSTRAINT
	DF_trip_request_details_trip_direction DEFAULT ((1)) FOR trip_direction
GO
ALTER TABLE dbo.Tmp_trip_request_details ADD CONSTRAINT
	DF_trip_request_details_is_active DEFAULT ((1)) FOR is_active
GO
SET IDENTITY_INSERT dbo.Tmp_trip_request_details ON
GO
IF EXISTS(SELECT * FROM dbo.trip_request_details)
	 EXEC('INSERT INTO dbo.Tmp_trip_request_details (id, trip_request_id, seats, start_at_date, start_at_time, from_lat, from_lng, from_plc, to_lat, to_lng, to_plc, gender_id, seat_cost_from, seat_cost_to, trip_direction, is_active)
		SELECT id, trip_request_id, seats, start_at_date, start_at_time, from_lat, from_lng, from_plc, to_lat, to_lng, to_plc, gender_id, seat_cost_from, seat_cost_to, trip_direction, is_active FROM dbo.trip_request_details WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_trip_request_details OFF
GO
ALTER TABLE dbo.trip_book
	DROP CONSTRAINT FK_trip_book_trip_request_details
GO
DROP TABLE dbo.trip_request_details
GO
EXECUTE sp_rename N'dbo.Tmp_trip_request_details', N'trip_request_details', 'OBJECT' 
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
COMMIT
select Has_Perms_By_Name(N'dbo.trip_request_details', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.trip_request_details', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.trip_request_details', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
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