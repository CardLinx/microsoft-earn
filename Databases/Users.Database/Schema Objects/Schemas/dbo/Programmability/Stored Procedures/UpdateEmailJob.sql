--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE [dbo].[UpdateEmailJob]
	@JobId uniqueidentifier
AS
set nocount on
SET XACT_ABORT ON -- close the transaction in case of client timeouts

DECLARE @ScheduleType nvarchar(50)
DECLARE @UserId uniqueidentifier

BEGIN TRANSACTION

EXECUTE sp_getapplock 'UpdateEmailJob', 'exclusive' 

SELECT @UserId = UserId FROM dbo.EmailJobs WHERE JobId = @JobId	
SELECT @ScheduleType = ScheduleType from dbo.ValidMerchantUsersView WHERE UserId = @UserId

UPDATE dbo.EmailJobs
		SET
		LastRunTime = getUtcDate(),
		NextRunTime = dbo.getNextEmailJobRunTime(@ScheduleType)
		WHERE JobId = @JobId	

COMMIT TRANSACTION
GO