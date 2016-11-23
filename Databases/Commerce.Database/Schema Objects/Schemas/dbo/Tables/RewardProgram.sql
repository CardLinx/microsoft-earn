--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[RewardProgram]
(
	[Id] INT NOT NULL 
	,ProgramName nvarchar(100) NOT NULL

	CONSTRAINT PKC_RewardProgram_Id PRIMARY KEY CLUSTERED (Id)
)

GO