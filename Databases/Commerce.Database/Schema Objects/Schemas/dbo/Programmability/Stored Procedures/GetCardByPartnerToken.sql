--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- GetCardByPartnerToken.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- GetCardByPartnerToken
--  Retrieves information for the active credit card with the specified card brand and partner token, if one exists.
-- Parameters:
--  @cardBrand int: The brand of the card whose card information to retrieve.
--  @partnerToken varchar(100): The token assigned to the card by the partner.
-- Returns:
--  If an active  credit card with the specified card brand and partner token exists, card information is returned.
--  Else nothing is returned.
create procedure dbo.GetCardByPartnerToken @cardBrand int,
                                           @partnerToken varchar(100)
as
    set nocount on;
    select Cards.Id as CardId, (select GlobalID from dbo.Users where Users.Id = UserId) as GlobalUserID, LastFourDigits, FDCToken
     from dbo.Cards where CardBrand = @cardBrand and (PartnerToken = @partnerToken or FdcToken = @partnerToken) and Active = 1
GO