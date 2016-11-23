--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[RewardPayouts]
(
	[Id] [uniqueidentifier] NOT NULL,
	[RewardId] [uniqueidentifier] NOT NULL,
	[RewardReasonId] [int] NOT NULL,
	[PayeeId] [uniqueidentifier] NOT NULL,
	[PayeeTypeId] [int] NOT NULL,
	[RewardPayoutStatusId] [int] NOT NULL,
	[PayoutScheduledDateUtc] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [PayoutFinalizedDateUtc] [datetime2](7) NULL,
	[AgentId] NVARCHAR(100) NULL,
	[AgentTypeId] [int] NULL,
	[Amount] INT NOT NULL DEFAULT 0,
    [Explanation] NVARCHAR(100) NULL,
    [IssuedBy] VARCHAR(100) NULL
 CONSTRAINT [PrimaryKey_E9937A5B-3022-46CA-892B-50E92C378788] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
    CONSTRAINT [FK_RewardPayouts_Reward] FOREIGN KEY([RewardId]) REFERENCES [dbo].[Rewards] ([Id]),
	CONSTRAINT [FK_RewardPayouts_RewardReason] FOREIGN KEY([RewardReasonId]) REFERENCES [dbo].[RewardReasons] ([Id]),
	CONSTRAINT [FK_RewardPayouts_PayeeType] FOREIGN KEY([PayeeTypeId]) REFERENCES [dbo].[PayeeTypes] ([Id]),
	CONSTRAINT [FK_RewardPayouts_RewardPayoutStatus] FOREIGN KEY([RewardPayoutStatusId]) REFERENCES [dbo].[RewardPayoutStatus] ([Id])
)
GO