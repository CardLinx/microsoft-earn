--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[Referrals]
(
	[Id] [uniqueidentifier] NOT NULL,
	[ReferralEventRewardId] [uniqueidentifier] NOT NULL,
	[RewardPayoutId] [uniqueidentifier] NOT NULL,
	[ReferralDateUtc] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
	[ReferredUserId] [nvarchar](100) NOT NULL,
 CONSTRAINT [PrimaryKey_818D7A16-4401-4473-9719-AA7B2303293D] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
	CONSTRAINT [FK_Referral_ReferralEventReward] FOREIGN KEY([ReferralEventRewardId]) REFERENCES [dbo].[ReferralEventRewards] ([Id]),
	CONSTRAINT [FK_Referral_RewardPayouts] FOREIGN KEY([RewardPayoutId]) REFERENCES [dbo].[RewardPayouts] ([Id])
)
GO