/*
   08 أبريل, 201903:28:19 م
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
EXECUTE sp_rename N'dbo.trip_request_details.to_long', N'Tmp_to_lng_7', 'COLUMN' 
GO
EXECUTE sp_rename N'dbo.trip_request_details.Tmp_to_lng_7', N'to_lng', 'COLUMN' 
GO
ALTER TABLE dbo.trip_request_details SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.trip_request_details', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.trip_request_details', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.trip_request_details', 'Object', 'CONTROL') as Contr_Per 