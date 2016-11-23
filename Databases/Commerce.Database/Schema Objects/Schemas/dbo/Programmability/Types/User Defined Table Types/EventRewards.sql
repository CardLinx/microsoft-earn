--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TYPE [dbo].[EventRewards] AS TABLE
(
	[Id] [uniqueidentifier] NOT NULL,
	[ReferralEventId] [int] NOT NULL,
	[RewardId] [uniqueidentifier] NOT NULL,
	[PerUserLimit] [int] NOT NULL
)