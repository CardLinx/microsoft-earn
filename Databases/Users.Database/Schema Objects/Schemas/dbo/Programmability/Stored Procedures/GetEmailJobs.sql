--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE [dbo].[GetEmailJobs]   @Take int = 50										
										,@FromPartitionId int = null
										,@FromUserId uniqueidentifier = null																				
AS
set nocount on
IF @FromPartitionId IS NULL OR @FromUserId IS NULL
BEGIN
	IF @FromPartitionId IS NULL AND @FromUserId IS NULL
		BEGIN
			SELECT TOP(@Take) *
			FROM dbo.ActiveEmailJobsView jobs
			WHERE 
			jobs.NextRunTime <= getUtcDate() 			
			ORDER BY jobs.UserId, jobs.PartitionId
		END
	ELSE
		BEGIN 
			RAISERROR('Both FromPartitionId and FromUserId should be null or none should be null', 18, 127)
		END
END
ELSE
BEGIN
	SELECT TOP(@Take) *
	FROM dbo.ActiveEmailJobsView jobs 
	WHERE 
		jobs.NextRunTime <= getUtcDate() 
		AND
		(jobs.UserId > @FromUserId 
			OR (jobs.UserId = @FromUserId AND jobs.PartitionId > @FromPartitionId))			
	ORDER BY jobs.UserId, jobs.PartitionId
END
GO