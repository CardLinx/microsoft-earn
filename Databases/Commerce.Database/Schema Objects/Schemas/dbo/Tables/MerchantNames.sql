--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[MerchantNames]
(
	[Id] [int] NOT NULL IDENTITY(0,1),
	[Name] [nvarchar](100) NOT NULL
 CONSTRAINT [PrimaryKey_6121AD33-E67B-44BA-B630-E6081D09C707] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
    CONSTRAINT [UC_MerchantNames_Name] UNIQUE ([Name])
)
GO