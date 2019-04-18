--***************************************************
--Monitoring Data Change with SqlDependency using C#
--***************************************************

--## Check Service Broker Enabled / Disbled for database.
--USE [test]
--GO
--SELECT NAME, IS_BROKER_ENABLED FROM SYS.DATABASES


--## Enable / Disable Service Broker for existing database.
--USE [test]
--GO
--ALTER DATABASE test SET ENABLE_BROKER
--ALTER DATABASE test SET DISABLE_BROKER

--## Create sample table in test database
--USE [test]
--GO
--CREATE TABLE [dbo].[Table1](
--	[SampleId] [bigint] IDENTITY(1,1) NOT NULL,
--	[SampleName] [nvarchar](50) NOT NULL,
--	[SampleCategory] [nvarchar](50) NOT NULL,
--	[SampleDateTime] [datetime] NOT NULL,
--	[IsSampleProcessed] [bit] NOT NULL)
-- ON [PRIMARY];

--## Create stored procedure for query notification
--USE [test]
--GO
--CREATE PROCEDURE uspGetSampleInformation
--AS
--BEGIN
--	SELECT
--		[SampleId],
--		[SampleName],
--		[SampleCategory],
--		[SampleDateTime],
--		[IsSampleProcessed]
--	FROM
--		[dbo].[SampleTable01];
--END

--## Create QUEUE and SERVICE for the query
--USE [test]
--GO
--CREATE QUEUE QueueSampleInformationDataChange
--CREATE SERVICE ServiceSampleInformationDataChange
--	ON QUEUE QueueSampleInformationDataChange
--	([http://schemas.microsoft.com/SQL/Notifications/PostQueryNotification]);

--GRANT SUBSCRIBE QUERY NOTIFICATIONS TO YourSqlUserName;

--## Give authorization for the Sql user to test database
--ALTER AUTHORIZATION ON DATABASE::test TO YourSqlUserName;

--## Grand send permission on Service broker service you created for user that will be used to run the client application
--GRANT SEND ON SERVICE:: ServiceSampleInformationDataChange TO [YourMachineNameOrDomain\UserName];

--## Grand send permission on Service broker queue you created for user that will be used to run the client application
--GRANT RECEIVE ON dbo.QueueSampleInformationDataChange TO [YourMachineNameOrDomain\UserName];

--## Use this query to see the current queue subscriptions
--SELECT * FROM SYS.DM_QN_SUBSCRIPTIONS