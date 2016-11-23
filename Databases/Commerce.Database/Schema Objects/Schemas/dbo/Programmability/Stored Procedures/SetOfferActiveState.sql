--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- SetOfferActiveState.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- SetOfferActiveState
--  Sets the specified offer to the specified Active state if it's not in that state already.
-- Parameters:
--  @globalID uniqueidentifier: The ID assigned within the wider system to this offer.
--  @activeState bit: The Active state to which to set the offer.
create procedure dbo.SetOfferActiveState @globalID uniqueidentifier,
                                         @activeState bit
as
  set nocount on;
  begin try
    begin transaction

      -- Determine if the specified offer exists in the Earn system.
      declare @offerId int = (select Offers.Id from dbo.Offers where GlobalID = @globalID);

      -- If an offer with the specifed global ID exists within the Earn system, attempt to update its Active state.
      if (@offerId is not null)
        begin
          -- If the Offer is not already in the specified Active state, set it to that state.
          if ((select Active from dbo.Offers where Offers.Id = @offerId) <> @activeState)
            begin
              update dbo.Offers set Active = @activeState where Offers.Id = @offerId;
            end
          -- Otherwise, let the caller know that no change was needed.
          else
            begin
              raiserror('OfferUnchanged', 16, 1);
            end
        end
      -- Otherwise raise an exception.
      else
        begin
          raiserror('OfferNotFound', 16, 1);
        end

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