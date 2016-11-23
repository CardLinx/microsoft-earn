--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[MSSVDistributions](
	[Id] [int] IDENTITY(0,1) NOT NULL,
	[DistributionDateUtc] [DATETIME2](7) NOT NULL,
	[UserId] [INT] NOT NULL, -- foreign key me!
	[Amount] [INT] NOT NULL,
	[Currency] [VARCHAR](5) NOT NULL,
	[VoucherExpirationUtc] [DATETIME] NOT NULL,
	[Notes] [NVARCHAR](500) NULL,
 CONSTRAINT [PrimaryKey_78FAF028-4BD5-46FE-B7F8-C9FD8A05D5B1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)