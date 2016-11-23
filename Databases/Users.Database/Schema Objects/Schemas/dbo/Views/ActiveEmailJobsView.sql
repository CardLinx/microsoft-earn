--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE VIEW [dbo].[ActiveEmailJobsView]
AS
	SELECT 
		jobs.JobId, jobs.UserId, jobs.PartitionId, jobs.SubscriptionType, jobs.LastRunTIme, jobs.NextRunTime, jobs.CreatedDate, jobs.UpdatedDate
	FROM
		 dbo.EmailJobs jobs INNER JOIN dbo.ValidUsersView usr ON jobs.UserId = usr.Id
		 WHERE jobs.NextRunTime is not null
GO