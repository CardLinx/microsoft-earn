--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- GetUnfilteredMasterCards.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- GetUnfilteredMasterCards
--  Retrieves the partner tokens for all unfiltered MasterCards
-- Returns:
--  The list of unfiltered MasterCards, if any exist.
--  Else nothing is returned.
create procedure dbo.GetUnfilteredMasterCards
as
    set nocount on;
    select PartnerToken from dbo.Cards where CardBrand = 5 and (Flags & 0x1) = 0x1 -- 5 = MasterCard, 0x1 = unfiltered MasterCard
GO