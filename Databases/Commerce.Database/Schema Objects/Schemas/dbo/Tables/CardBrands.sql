--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[CardBrands](
	[Id] [int] IDENTITY(3,1) NOT NULL,
	[Name] [nvarchar](25) NOT NULL,
 CONSTRAINT [PrimaryKey_2405938C-6D00-42AD-9C55-38D087ACCA52] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON), 
    CONSTRAINT [UC_CardBrands_Name] UNIQUE ([Name])
)