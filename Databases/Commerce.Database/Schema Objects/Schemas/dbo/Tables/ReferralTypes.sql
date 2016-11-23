--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[ReferralTypes]
(
	[Id] [uniqueidentifier] NOT NULL,
	[Sequence] [int] identity(10000, 1) NOT NULL,
	[Code] [nvarchar](255) NULL,
	[ReferrerId] [uniqueidentifier] NOT NULL,
	[ReferrerTypeId] [int] NOT NULL,
	[ReferralVectorId] [int] NOT NULL,
	[RewardRecipientId] [int] NOT NULL,
	[CreatedDateUtc] [datetime2](7) NOT NULL DEFAULT GETUTCDATE()
 CONSTRAINT [PrimaryKey_BF2F3532-7158-426E-BE90-3D38D93A9007] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
    CONSTRAINT [UC_ReferralType_ReferrerId_ReferrerTypeId_ReferralVectorId_RewardRecipientId] UNIQUE ([ReferrerId], [ReferrerTypeId], [ReferralVectorId], [RewardRecipientId]),
	CONSTRAINT [FK_ReferralType_ReferrerType] FOREIGN KEY([ReferrerTypeId]) REFERENCES [dbo].[ReferrerTypes] ([Id]),
	CONSTRAINT [FK_ReferralType_ReferralVector] FOREIGN KEY([ReferralVectorId]) REFERENCES [dbo].[ReferralVectors] ([Id]),
	CONSTRAINT [FK_ReferralType_RewardRecipient] FOREIGN KEY([RewardRecipientId]) REFERENCES [dbo].[RewardRecipients] ([Id])
)
GO