--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[Rewards]
(
	[Id] [uniqueidentifier] NOT NULL,
	[RewardTypeId] [int] NOT NULL,
	[Description] [nvarchar](255) NOT NULL,
	[Active] [bit] NOT NULL,
	[Properties] [nvarchar](max) NOT NULL
 CONSTRAINT [PrimaryKey_DCA16778-CB34-4990-8554-593AAFF17070] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
	CONSTRAINT [FK_Rewards_RewardType] FOREIGN KEY([RewardTypeId]) REFERENCES [dbo].[RewardTypes] ([Id])
)
GO