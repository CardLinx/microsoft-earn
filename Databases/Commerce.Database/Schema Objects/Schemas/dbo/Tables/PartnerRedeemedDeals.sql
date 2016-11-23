--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[PartnerRedeemedDeals](
	[TransactionReferenceNumber] [int] IDENTITY(0,1) NOT NULL,
	[RedeemedDealId] [uniqueidentifier] NOT NULL,
    [PartnerRedeemedDealScopeId] [nvarchar](255) NULL,
	[PartnerRedeemedDealId] [nvarchar](255) NOT NULL,
	[PartnerReferenceNumber] [nvarchar](255) NOT NULL,
	[PartnerMerchantId] [nvarchar](255) NOT NULL,
	[RecommendedPartnerDealId] [nvarchar](255) NULL,
	[RecommendedPartnerClaimedDealId] [nvarchar](255) NULL,
	[PartnerData] xml NULL
 CONSTRAINT [PrimaryKey_25D3C2E7-B882-4A42-9D9A-92B659B89449] PRIMARY KEY CLUSTERED 
(
	[TransactionReferenceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON), 
	CONSTRAINT [FK_PartnerRedeemedDeals_RedeemedDeals] FOREIGN KEY([RedeemedDealId]) REFERENCES [dbo].[RedeemedDeals] ([Id]),
    CONSTRAINT [UC_PartnerRedeemedDeals_RedeemedDealId] UNIQUE ([RedeemedDealId]),
    CONSTRAINT [UC_PartnerRedeemedDeals_PartnerRedeemedDealId] UNIQUE ([PartnerRedeemedDealId])
)