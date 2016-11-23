--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- AddOrUpdateMerchant.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- AddOrUpdateMerchant
--  Adds or updates a merchant, including adding, deactivating, or reactivating partner merchant info records as appropriate.
-- Parameters:
--  @globalMerchantID varchar(100): The ID assigned within the wider system to this merchant.
--  @name nvarchar(100): The merchant's name.
--  @globalProviderID varchar(100): The wider services space ID of the provider through which this merchant came into the system.
--  @partnerMerchantAuthorizationIDList PartnerMerchantIDs: The list of partner merchant authorization IDs.
--  @partnerMerchantSettlementIDList PartnerMerchantIDs: The list of partner merchant settlement IDs.
-- Returns:
--  The ID of the affected merchant record and whether it was newly created.
-- Remarks:
--  Partner merchant ID records are deactivated instead of deleted to maintain history and referential integrity.
create procedure dbo.AddOrUpdateMerchant @globalMerchantID varchar(100),
                                         @name nvarchar(100),
                                         @globalProviderID varchar(100),
                                         @partnerMerchantAuthorizationIDList PartnerMerchantIDs READONLY,
                                         @partnerMerchantSettlementIDList PartnerMerchantIDs READONLY
as
  set nocount on;
  begin try
    begin transaction

        -- If a provider with the specified global ID exists, add or update its merchant.
        declare @providerId int = (select Providers.Id from dbo.Providers where GlobalID = @globalProviderId);
        declare @created bit = 0;
        if (@providerId is not null)
          begin
            -- Gather existing information for this merchant, if any.
            declare @merchantId int;
            declare @existingName nvarchar(100);
            declare @existingProviderId int;
            select @merchantId = Merchants.Id, @existingName = Name, @existingProviderId = ProviderId from dbo.Merchants where GlobalID = @globalMerchantID;

            -- If the merchant has not already been added to the Earn Merchants table, add it.
            if (@merchantId is null)
              begin
                insert into dbo.Merchants(GlobalID, Name, ProviderId) values (@globalMerchantID, @name, @providerId);
                set @merchantId = @@identity;
                set @created = 1;
              end
            -- Otherwise, update the existing entity if necessary.
            else if (@name <> @existingName or @providerId <> @existingProviderId)
              begin
                 update dbo.Merchants set Name = @name, ProviderId = @providerId where Merchants.Id = @merchantId;
              end

            -- Process the partner merchant records.

            -- First, deactivate deleted IDs, if any.
            exec dbo.DeactivatePartnerMerchantIDs @merchantID, @partnerMerchantAuthorizationIDList, @partnerMerchantSettlementIDList;

            -- Then reactivate IDs flagged for reactivation, if any.
            exec dbo.ReactivatePartnerMerchantIDs @merchantID, @partnerMerchantAuthorizationIDList, @partnerMerchantSettlementIDList;

            -- Finally, add new IDs.
            exec dbo.AddPartnerMerchantIDs @merchantID,  @partnerMerchantAuthorizationIDList, @partnerMerchantSettlementIDList;
          end
        else
          begin
            raiserror('InvalidMerchant', 16, 1);
          end

        -- Return the id of the affected merchant record.
        select @merchantId as MerchantId, @created as Created;

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