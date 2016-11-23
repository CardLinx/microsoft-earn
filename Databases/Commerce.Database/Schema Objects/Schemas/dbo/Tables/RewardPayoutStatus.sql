--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[RewardPayoutStatus]
(
	[Id] [int] NOT NULL identity(0, 1),
	[Name] [nvarchar](50) NOT NULL
 CONSTRAINT [PrimaryKey_DB36B5AB-9C2B-432D-9FF0-BDC9D9C72F1D] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO