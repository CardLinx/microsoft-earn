--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[ReferralEventRewards]
(
	[Id] [uniqueidentifier] NOT NULL,
	[ReferralTypeId] [uniqueidentifier] NOT NULL,
	[ReferralEventId] [int] NOT NULL,
	[RewardId] [uniqueidentifier] NOT NULL,
	[PerUserLimit] [int] NOT NULL
 CONSTRAINT [PrimaryKey_98BC5BEE-3B43-4D1E-B4AE-86B764550728] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
   	CONSTRAINT [FK_ReferralEventReward_ReferralTypeId] FOREIGN KEY([ReferralTypeId]) REFERENCES [dbo].[ReferralTypes] ([Id]),
	CONSTRAINT [FK_ReferralEventReward_ReferralEventId] FOREIGN KEY([ReferralEventId]) REFERENCES [dbo].[ReferralEvents] ([Id]),
	CONSTRAINT [FK_ReferralEventReward_RewardId] FOREIGN KEY([RewardId]) REFERENCES [dbo].[Rewards] ([Id])
)
GO