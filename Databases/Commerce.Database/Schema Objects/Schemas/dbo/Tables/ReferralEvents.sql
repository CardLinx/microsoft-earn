--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[ReferralEvents]
(
	[Id] [int] NOT NULL identity(0, 1),
	[Name] [nvarchar](50) NOT NULL
 CONSTRAINT [PrimaryKey_8E40E8CE-7E52-4C3C-B7D8-19C44804C3D7] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO