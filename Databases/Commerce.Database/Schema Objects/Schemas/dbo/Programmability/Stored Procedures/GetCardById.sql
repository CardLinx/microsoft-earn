--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- GetCardById.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- GetCardById
--  Retrieves information for the credit card with the specified ID.
-- Parameters:
--  @cardId int: The ID of the cards whose information to retrieve.
-- Returns:
--  If a credit card with the specified ID exists, card information is returned.
--  Else nothing is returned.
create procedure dbo.GetCardById @cardId int
as
    set nocount on;
    select (select GlobalID from dbo.Users where Users.Id = UserId) as GlobalUserID, CardBrand, LastFourDigits, PartnerToken, FDCToken, Active
        from dbo.Cards where Cards.Id = @cardId and Active = 1;
GO