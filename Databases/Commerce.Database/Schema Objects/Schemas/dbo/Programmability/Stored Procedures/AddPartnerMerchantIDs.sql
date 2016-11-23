--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- AddPartnerMerchantIDs.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- AddPartnerMerchantIDs
--  Adds new partner merchant IDs.
-- Parameters:
--  @merchantId: The ID assigned to the merchant in the Earn system.
--  @partnerMerchantAuthorizationIDList PartnerMerchantIDs: The list of partner merchant authorization IDs.
--  @partnerMerchantSettlementIDList PartnerMerchantIDs: The list of partner merchant settlement IDs.
-- Remarks:
--  Partner merchant ID records are deactivated instead of deleted to maintain history and referential integrity.
create procedure dbo.AddPartnerMerchantIDs @merchantID int,
                                           @partnerMerchantAuthorizationIDList PartnerMerchantIDs READONLY,
                                           @partnerMerchantSettlementIDList PartnerMerchantIDs READONLY
as
  set nocount on;

    -- Authorizations
    insert into dbo.PartnerMerchantAuthorizationIDs(MerchantId, [Partner], AuthorizationID)
      select distinct @merchantId, partnerMerchantAuthorizationIDList.[Partner], partnerMerchantAuthorizationIDList.EventID
          from @partnerMerchantAuthorizationIDList as partnerMerchantAuthorizationIDList
            left outer join dbo.PartnerMerchantAuthorizationIDs
              on PartnerMerchantAuthorizationIDs.[Partner] = partnerMerchantAuthorizationIDList.[Partner] and
                  PartnerMerchantAuthorizationIDs.AuthorizationID = partnerMerchantAuthorizationIDList.EventID
      where AddOrUpdate = 1 and (PartnerMerchantAuthorizationIDs.Id is null or PartnerMerchantAuthorizationIDs.MerchantId <> @merchantId);

    -- Settlements
    insert into dbo.PartnerMerchantSettlementIDs(MerchantId, [Partner], SettlementID)
      select distinct @merchantId, partnerMerchantSettlementIDList.[Partner], partnerMerchantSettlementIDList.EventID
          from @partnerMerchantSettlementIDList as partnerMerchantSettlementIDList
            left outer join dbo.PartnerMerchantSettlementIDs
              on PartnerMerchantSettlementIDs.[Partner] = partnerMerchantSettlementIDList.[Partner] and
                  PartnerMerchantSettlementIDs.SettlementID = partnerMerchantSettlementIDList.EventID
      where AddOrUpdate = 1 and (PartnerMerchantSettlementIDs.Id is null or PartnerMerchantSettlementIDs.MerchantId <> @merchantId);
GO