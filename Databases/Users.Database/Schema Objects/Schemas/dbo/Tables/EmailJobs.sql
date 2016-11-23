--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[EmailJobs](
	[JobId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_EmailJobs_JobId]  DEFAULT (newid()),
	[UserId] [uniqueidentifier] NOT NULL,
	[PartitionId] [int] NOT NULL,
	[SubscriptionType] [nvarchar](50) NOT NULL,
	[LastRunTime] [datetime] NULL,
	[NextRunTime] [datetime] NULL,
	[CreatedDate] [datetime] NOT NULL CONSTRAINT [DF_EmailJobs_CreatedDate]  DEFAULT (getutcdate()),
	[UpdatedDate] [datetime] NOT NULL CONSTRAINT [DF_EmailJobs_UpdatedDate]  DEFAULT (getutcdate()),
 CONSTRAINT [PKC_EmailJobs_UserId_PartitionId_SubscriptionType] PRIMARY KEY CLUSTERED (UserId, PartitionId, SubscriptionType),
 CONSTRAINT [FK_EmailJobs_UserId_Users] FOREIGN KEY(UserId, PartitionId) REFERENCES Users(Id,PartitionId)
)

GO
