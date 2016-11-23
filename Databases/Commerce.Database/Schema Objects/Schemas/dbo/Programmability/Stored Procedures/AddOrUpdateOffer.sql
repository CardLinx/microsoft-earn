--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- AddOrUpdateOffer.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- AddOrUpdateOffer
--  Adds or updates an offer.
-- Parameters:
--  @globalOfferID uniqueidentifier: The ID assigned within the wider system to this offer.
--  @globalProviderID varchar(100): The wider services space ID of the provider through which this offer came into the system.
--  @offerType int: The offer's type, e.g. Earn or Burn.
--  @percentBack money: The percent of the settlement amount to apply as Earn credits, or the percent of dollars spent for which
--   Earn credits can be used instead.
--  @active bit: Specifies whether this offer is currently active.
-- Returns:
--  * The ID of the affected offer record and whether it was newly created.
--  * Else returns nothing.
-- Remarks:
--  * Offers are deactivated instead of deleted to maintain history and referential integrity.
create procedure dbo.AddOrUpdateOffer @globalOfferID uniqueidentifier,
                                      @globalProviderID varchar(100),
                                      @offerType int,
                                      @percentBack money,
                                      @active bit
as
  set nocount on;
  begin try
    begin transaction

        declare @offerId int;

        -- If a provider with the specified global ID exists, add or update its offer.
        declare @providerId int = (select Providers.Id from dbo.Providers where GlobalID = @globalProviderId);
        declare @created bit = 0;
        if (@providerId is not null)
          begin
            -- If the offer has not already been added to the Earn Offers table, add it.
            select @offerId = Offers.Id from dbo.Offers where GlobalID = @globalOfferID;
            if (@offerId is null)
              begin
                insert into dbo.Offers(GlobalID, ProviderID, OfferType, PercentBack, Active)
                  values (@globalOfferID, @providerId, @offerType, @percentBack, @active);
                set @offerId = @@identity;
                set @created = 1;
              end
            -- Otherwise, update the existing entity.
            else
              begin
                -- NOTE: We first check to see if there are any records to update and only then do we call update. This does mean querying twice if there are updates, but
                --        it's more performant than allowing the update and then the trigger to be called when not needed. Alternate approaches such as table variables are
                --        also more expensive.
                if (not exists(select Offers.Id from dbo.Offers
                  where GlobalID = @globalOfferID and ProviderID = @providerId and OfferType = @offerType and PercentBack = @percentBack and Active = @active))
                  begin
                      update dbo.Offers set GlobalID = @globalOfferID, ProviderID = @providerId, OfferType = @offerType, PercentBack = @percentBack, Active = @active
                        where Offers.Id = @offerId;
                  end
              end
          end
        else
          begin
            raiserror('InvalidDeal', 16, 1);
          end

        -- Return the id of the affected offer record.
        select @offerId as OfferId, @created as Created;

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