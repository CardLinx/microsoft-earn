--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- ReactivatePartnerMerchantIDs.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- ReactivatePartnerMerchantIDs
--  Reactivates any partner merchant ID flagged for reactivation.
-- Parameters:
--  @merchantId: The ID assigned to the merchant in the Earn system.
--  @partnerMerchantAuthorizationIDList PartnerMerchantIDs: The list of partner merchant authorization IDs.
--  @partnerMerchantSettlementIDList PartnerMerchantIDs: The list of partner merchant settlement IDs.
-- Remarks:
--  Partner merchant ID records are deactivated instead of deleted to maintain history and referential integrity.
create procedure dbo.ReactivatePartnerMerchantIDs @merchantID int,
                                                  @partnerMerchantAuthorizationIDList PartnerMerchantIDs READONLY,
                                                  @partnerMerchantSettlementIDList PartnerMerchantIDs READONLY
as
  set nocount on;

    -- Authorizations
    if (exists(select PartnerMerchantAuthorizationIDs.Id from dbo.PartnerMerchantAuthorizationIDs
                  join @partnerMerchantAuthorizationIDList as partnerMerchantAuthorizationIDList
                    on PartnerMerchantAuthorizationIDs.[Partner] = partnerMerchantAuthorizationIDList.[Partner] and
                      PartnerMerchantAuthorizationIDs.AuthorizationID = partnerMerchantAuthorizationIDList.EventID
                  where PartnerMerchantAuthorizationIDs.MerchantId = @merchantID and Active = 0 and AddOrUpdate = 1))
    begin
      update dbo.PartnerMerchantAuthorizationIDs set Active = 1 from dbo.PartnerMerchantAuthorizationIDs
        join @partnerMerchantAuthorizationIDList as partnerMerchantAuthorizationIDList
          on PartnerMerchantAuthorizationIDs.[Partner] = partnerMerchantAuthorizationIDList.[Partner] and
            PartnerMerchantAuthorizationIDs.AuthorizationID = partnerMerchantAuthorizationIDList.EventID
        where PartnerMerchantAuthorizationIDs.MerchantId = @merchantID and Active = 0 and AddOrUpdate = 1;
    end

    -- Settlements
    if (exists(select PartnerMerchantSettlementIDs.Id from dbo.PartnerMerchantSettlementIDs
                  join @partnerMerchantSettlementIDList as partnerMerchantSettlementIDList
                    on PartnerMerchantSettlementIDs.[Partner] = partnerMerchantSettlementIDList.[Partner] and
                      PartnerMerchantSettlementIDs.SettlementID = partnerMerchantSettlementIDList.EventID
                  where PartnerMerchantSettlementIDs.MerchantId = @merchantID and Active = 0 and AddOrUpdate = 1))
    begin
      update dbo.PartnerMerchantSettlementIDs set Active = 1
        from dbo.PartnerMerchantSettlementIDs
        join @partnerMerchantSettlementIDList as partnerMerchantSettlementIDList
          on PartnerMerchantSettlementIDs.[Partner] = partnerMerchantSettlementIDList.[Partner] and
            PartnerMerchantSettlementIDs.SettlementID = partnerMerchantSettlementIDList.EventID
        where PartnerMerchantSettlementIDs.MerchantId = @merchantID and Active = 0 and AddOrUpdate = 1;
    end
GO