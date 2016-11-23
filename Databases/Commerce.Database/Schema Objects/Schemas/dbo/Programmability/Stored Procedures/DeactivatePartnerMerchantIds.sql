--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- DeactivatePartnerMerchantIDs.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- DeactivatePartnerMerchantIDs
--  Deactivates any partner merchant ID flagged for deactivation.
-- Parameters:
--  @merchantId: The ID assigned to the merchant in the Earn system.
--  @partnerMerchantAuthorizationIDList PartnerMerchantIDs: The list of partner merchant authorization IDs.
--  @partnerMerchantSettlementIDList PartnerMerchantIDs: The list of partner merchant settlement IDs.
-- Remarks:
--  Partner merchant ID records are deactivated instead of deleted to maintain history and referential integrity.
create procedure dbo.DeactivatePartnerMerchantIDs @merchantID int,
                                                  @partnerMerchantAuthorizationIDList PartnerMerchantIDs READONLY,
                                                  @partnerMerchantSettlementIDList PartnerMerchantIDs READONLY
as
  set nocount on;

  -- NOTE: We first check to see if there are any records to update and only then do we actually update. This does mean querying twice if there are updates, but
  --        it's more performant than allowing the update and then the trigger to be called when not needed, and deactivation will be a rare event.
  --        Alternate approaches such as table variables are also more expensive.

  -- Authorizations
  if (exists(select PartnerMerchantAuthorizationIDs.Id from dbo.PartnerMerchantAuthorizationIDs
                join @partnerMerchantAuthorizationIDList as partnerMerchantAuthorizationIDList
                  on PartnerMerchantAuthorizationIDs.[Partner] = partnerMerchantAuthorizationIDList.[Partner] and
                    PartnerMerchantAuthorizationIDs.AuthorizationID = partnerMerchantAuthorizationIDList.EventID
                where PartnerMerchantAuthorizationIDs.MerchantId = @merchantId and Active = 1 and AddOrUpdate = 0))
  begin
    update dbo.PartnerMerchantAuthorizationIDs set Active = 0
      from dbo.PartnerMerchantAuthorizationIDs
        join @partnerMerchantAuthorizationIDList as partnerMerchantAuthorizationIDList
          on PartnerMerchantAuthorizationIDs.[Partner] = partnerMerchantAuthorizationIDList.[Partner] and
              PartnerMerchantAuthorizationIDs.AuthorizationID = partnerMerchantAuthorizationIDList.EventID
      where PartnerMerchantAuthorizationIDs.MerchantId = @merchantId and Active = 1 and AddOrUpdate = 0;
  end

  -- Settlements
  if (exists(select PartnerMerchantSettlementIDs.Id from dbo.PartnerMerchantSettlementIDs
                join @partnerMerchantSettlementIDList as partnerMerchantSettlementIDList
                  on PartnerMerchantSettlementIDs.[Partner] = partnerMerchantSettlementIDList.[Partner] and
                    PartnerMerchantSettlementIDs.SettlementID = partnerMerchantSettlementIDList.EventID
                where PartnerMerchantSettlementIDs.MerchantId = @merchantId and Active = 1 and AddOrUpdate = 0))
  begin
    update dbo.PartnerMerchantSettlementIDs set Active = 0
      from dbo.PartnerMerchantSettlementIDs
        join @partnerMerchantSettlementIDList as partnerMerchantSettlementIDList
          on PartnerMerchantSettlementIDs.[Partner] = partnerMerchantSettlementIDList.[Partner] and
              PartnerMerchantSettlementIDs.SettlementID = partnerMerchantSettlementIDList.EventID
      where PartnerMerchantSettlementIDs.MerchantId = @merchantId and Active = 1 and AddOrUpdate = 0;
  end
GO