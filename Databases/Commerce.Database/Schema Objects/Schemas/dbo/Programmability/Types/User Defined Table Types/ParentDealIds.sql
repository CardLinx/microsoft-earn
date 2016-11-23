--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TYPE [dbo].[ParentDealIds] AS TABLE
(
	ParentDealId UNIQUEIDENTIFIER NOT NULL,
	UnconstrainedDiscounts BIT NOT NULL
)