/*
   10 مايو, 201911:05:23 م
   User: 
   Server: (local)
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
ALTER TABLE dbo.trip_book
	DROP CONSTRAINT FK_trip_book_trip_request_details
GO
ALTER TABLE dbo.trip_request_details SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.trip_request_details', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.trip_request_details', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.trip_request_details', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.trip_book
	DROP CONSTRAINT FK_trip_book_trip_share_details
GO
ALTER TABLE dbo.trip_share_details SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.trip_share_details', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.trip_share_details', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.trip_share_details', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.trip_book
	DROP CONSTRAINT DF_trip_book_booked_at
GO
CREATE TABLE dbo.Tmp_trip_book
	(
	id int NOT NULL IDENTITY (1, 1),
	trip_share_details_id int NOT NULL,
	trip_request_details_id int NOT NULL,
	booked_at datetime NULL,
	start_at datetime NULL,
	end_at datetime NULL,
	driver_rate int NULL,
	rider_rate int NULL,
	trip_token uniqueidentifier NULL,
	seats int NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_trip_book SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_trip_book ADD CONSTRAINT
	DF_trip_book_booked_at DEFAULT (getdate()) FOR booked_at
GO
SET IDENTITY_INSERT dbo.Tmp_trip_book ON
GO
IF EXISTS(SELECT * FROM dbo.trip_book)
	 EXEC('INSERT INTO dbo.Tmp_trip_book (id, trip_share_details_id, trip_request_details_id, booked_at, start_at, end_at, driver_rate, rider_rate, trip_token, seats)
		SELECT id, trip_share_details_id, trip_request_details_id, booked_at, start_at, end_at, driver_rate, rider_rate, CONVERT(uniqueidentifier, trip_token), seats FROM dbo.trip_book WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_trip_book OFF
GO
DROP TABLE dbo.trip_book
GO
EXECUTE sp_rename N'dbo.Tmp_trip_book', N'trip_book', 'OBJECT' 
GO
ALTER TABLE dbo.trip_book ADD CONSTRAINT
	PK_trip_book PRIMARY KEY CLUSTERED 
	(
	id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

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
COMMIT
select Has_Perms_By_Name(N'dbo.trip_book', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.trip_book', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.trip_book', 'Object', 'CONTROL') as Contr_Per 