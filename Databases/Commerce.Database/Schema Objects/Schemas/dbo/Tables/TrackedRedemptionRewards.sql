--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[TrackedRedemptionRewards]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RedeemedDealId] [uniqueidentifier] NOT NULL,
	[RewardPayoutId] [uniqueidentifier] NOT NULL
 CONSTRAINT [PKC_TrackedRedemptionRewards_Id] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
	CONSTRAINT [FK_TrackedRedemptionRewards_RedeemedDeals] FOREIGN KEY([RedeemedDealId]) REFERENCES [dbo].[RedeemedDeals] ([Id]),
	CONSTRAINT [FK_TrackedRedemptionRewards_RewardPayouts] FOREIGN KEY([RewardPayoutId]) REFERENCES [dbo].[RewardPayouts] ([Id]),
	CONSTRAINT U_TrackedRedemptionRewards_RedeemedDealId_RewardPayoutId UNIQUE (RedeemedDealId, RewardPayoutId)
)
GO