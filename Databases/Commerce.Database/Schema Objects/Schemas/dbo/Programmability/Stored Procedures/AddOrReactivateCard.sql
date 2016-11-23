--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- AddOrReactivateCard.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- AddOrReactivateCard
--  Adds or reactivates a card for the specified user.
-- Parameters:
--  @globalUserID int: The ID of the User in the wider services space who is adding the card.
--  @cardBrand int: The brand (i.e. the first digit in the card number) of the card to add.
--  @lastFourDigits char(4): The last four digits of the card to add.
--  @partnerToken varchar(100): The token assigned to the card by the partner who issued it.
--  [DEPRECATED] @fdcToken varchar(100): The FDC token assigned to the card, if any. Default is an empty string.
-- Returns:
--  * If successful, the ID of the added or reactivated card.
--  * Otherwise an exception is raised.
-- Remarks:
--  The FDC token is deprecated but can't be removed until Visa provides a solution for obtaining the token when someone else had
--   previously registered the card.
create procedure dbo.AddOrReactivateCard @globalUserID uniqueidentifier,
                                         @cardBrand int,
                                         @lastFourDigits char(4),
                                         @partnerToken varchar(100),
                                         @fdcToken varchar(100) = ''
as
  set nocount on;
  begin try
    begin transaction

        declare @cardId int;

        -- First, see if the user who's trying to add or reactivate the card had previously added it.
        declare @userId int = (select Users.Id from dbo.Users where GlobalID = @globalUserID);
        declare @userActive bit = 0;
        declare @userPreviouslyRegistered bit = 0;
        select @userActive = Active, @cardId = Id from dbo.Cards
          where CardBrand = @cardBrand and LastFourDigits = @lastFourDigits and PartnerToken = @partnerToken and UserId = @userId;
        if (@cardId is not null) set @userPreviouslyRegistered = 1;

        -- If the user had never previously registered the card, or had registered it but has since deactivated it, see if anyone else has registered it.
        declare @otherUserActive bit = 0;
        if (@userPreviouslyRegistered = 0 or @userActive = 0)
        begin
            select @otherUserActive = Active from dbo.Cards
              where CardBrand = @cardBrand and LastFourDigits = @lastFourDigits and PartnerToken = @partnerToken and UserId <> @userId
              order by Active asc;
            if (@otherUserActive is null) set @otherUserActive = 0;
        end
        
        -- If a different user account has this card currently active, throw an exception.
        if (@otherUserActive = 1)
        begin
            raiserror('CardRegisteredToDifferentUser', 16, 1);
        end
        -- Else, if the user has never previously registered the card, add a new card entity.
        else if (@userPreviouslyRegistered = 0)
        begin
            -- If the card is a MasterCard, mark it as initially unfiltered.
            declare @flags int = 0x0;
            if (@cardBrand = 5)
            begin
                set @flags = 0x1; -- unfiltered MasterCard
            end

            -- The CardBrand / PartnerCard tuple can be added multiple times, but only if the last four digits don't vary.
            if (not exists(select Cards.Id from dbo.Cards where CardBrand = @cardBrand and PartnerToken = @partnerToken and LastFourDigits <> @lastFourDigits))
            begin
                insert into dbo.Cards(UserId, CardBrand, LastFourDigits, PartnerToken, FDCToken, Flags)
                  values(@userId, @cardBrand, @lastFourDigits, @partnerToken, @fdcToken, @flags);
                set @cardId = scope_identity();
            end
            else
            begin
                raiserror('InvalidCard', 16, 1);
            end
        end
        -- Else, if the card is currently inactive, reactivate it.
        else if (@userActive = 0)
        begin
--TODO: Revert this change once Visa statement credit processing is in place, while removing FDC tokens from the DB.
--            update dbo.Cards set Active = 1 where Id = @cardId;
            update dbo.Cards
             set Active = 1,
                 FDCToken = case when FDCToken = '' then @fdcToken else FDCToken end
             where Id = @cardId;
        end
        -- Otherwise, the card is currently active in the user's account, so throw an exception.
        else
        begin
            raiserror('CardStateUnchanged', 16, 1);
        end

        -- Return the id of the new or newly reactivated card.
        select @cardId as CardId;

    commit transaction
  end try
  begin catch
    -- Rollback the transaction and then re-raise the error.
    if (@@trancount > 0) rollback transaction;
    declare @errorMessage nvarchar(4000) = ERROR_MESSAGE();
    declare @errorSeverity int = ERROR_SEVERITY();
    raiserror(@errorMessage, @errorSeverity, 1)
  end catch
GO