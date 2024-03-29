/*
   05 أبريل, 201908:47:34 م
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
CREATE TABLE dbo.trip_book
	(
	id int NOT NULL IDENTITY (1, 1),
	driver_trip_id int NOT NULL,
	rider_trip_id int NOT NULL,
	booked_at datetime NULL,
	start_at datetime NULL,
	end_at datetime NULL,
	driver_rate int NULL,
	rider_rate int NULL,
	trip_token nvarchar(100) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.trip_book ADD CONSTRAINT
	DF_trip_book_booked_at DEFAULT getDate() FOR booked_at
GO
ALTER TABLE dbo.trip_book ADD CONSTRAINT
	PK_trip_book PRIMARY KEY CLUSTERED 
	(
	id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.trip_book SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.trip_book', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.trip_book', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.trip_book', 'Object', 'CONTROL') as Contr_Per 