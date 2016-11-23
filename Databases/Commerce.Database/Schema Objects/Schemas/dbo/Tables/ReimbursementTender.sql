--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[ReimbursementTender](
	[Id] [int] IDENTITY(0,1) NOT NULL,
	[Name] [nvarchar](25) NOT NULL,
 CONSTRAINT [PrimaryKey_22928CE1-A3D6-401E-A579-A0CD0E94E6F0] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON), 
    CONSTRAINT [UC_ReimbursementTender_Name] UNIQUE ([Name])
)