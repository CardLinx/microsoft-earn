--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[PartnerDealRegistrationStatus]
(
	[Id] [int] NOT NULL identity(1, 1),
	[Name] [nvarchar](50) NOT NULL
 CONSTRAINT [PrimaryKey_8188566C-C040-442D-AF4C-3E87058F7E8B] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO