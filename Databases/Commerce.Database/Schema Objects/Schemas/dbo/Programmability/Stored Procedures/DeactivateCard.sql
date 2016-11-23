--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- DeactivateCard.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- DeactivateCard
--  Deactivates the specified card.
-- Parameters:
--  @userId int: The ID of the user whose card to deactivate.
--  @cardId int: The ID of the card to deactivate.
-- Remarks:
--  If the card is not currently registered to the specified user, an exception is thrown.
create procedure dbo.DeactivateCard @userId int,
                                    @cardId int
as
  set nocount on;
  begin try
    begin transaction

      -- If the specified user does not currently own the card, throw an exception.
      declare @currentOwner int = (select UserId from dbo.Cards where Cards.Id = @cardId and Active = 1);
      if (@currentOwner is null or @currentOwner <> @userId)
      begin
          raiserror('CardDoesNotBelongToUser', 16, 1);
      end
      -- Otherwise, deactivate the card.
      else
      begin
        update dbo.Cards set Active = 0 where Cards.Id = @cardId
      end

    commit transaction;
  end try
  begin catch
    -- Rollback the transaction and then re-raise the error.
    if (@@trancount > 0) rollback transaction;
    declare @errorMessage nvarchar(4000) = ERROR_MESSAGE();
    declare @errorSeverity int = ERROR_SEVERITY();
    raiserror(@errorMessage, @errorSeverity, 1)
  end catch;
GO