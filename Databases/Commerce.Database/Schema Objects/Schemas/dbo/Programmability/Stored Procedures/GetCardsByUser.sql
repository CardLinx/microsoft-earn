--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- GetCardsByUser.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- GetCardsByUser
--  Retrieves information for the credit cards belonging to the Earn account with the specified ID.
-- Parameters:
--  @userId int: The ID of the user whose card information to retrieve.
-- Returns:
--  If a user with the specified ID exists, information for all active cards belonging to that account is returned.
--  Else nothing is returned.
create procedure dbo.GetCardsByUser @userId int
as
    set nocount on;
    select (select GlobalID from dbo.Users where Users.Id = UserId) as GlobalUserID, Cards.Id as CardId, CardBrand, LastFourDigits, PartnerToken, FDCToken
        from dbo.Cards where UserId = @userId and Active = 1;
GO