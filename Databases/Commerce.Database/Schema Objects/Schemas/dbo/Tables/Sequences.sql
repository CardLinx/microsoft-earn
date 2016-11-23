--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[Sequences]
(
	[SequenceName] NVARCHAR(255) PRIMARY KEY,
    [Seed] INT NOT NULL DEFAULT(1), 
	[Increment] INT NOT NULL DEFAULT(1), 
    [CurrentValue] INT 
)