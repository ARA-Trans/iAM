USE [iAMBridgeCare] /* Change this to the name of the DEV database */
GO

/* Increase permitted length of treatment budget list to maximum */
ALTER TABLE [dbo].[TREATMENTS] ALTER COLUMN [BUDGET] VARCHAR(MAX)