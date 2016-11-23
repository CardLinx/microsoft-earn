--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- GetMerchantByGlobalID.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- GetMerchantByGlobalID
--  Retrieves the merchant with the specified global ID.
-- Parameters:
--  @globalMerchantID varchar(100): The global ID of the merchant to retrieve.
--  @includePartnerMerchantIDs bit: Specifies whether to return the partner merchant IDs as well.
-- Returns:
--  * The merchant with the specified global ID if it exists.
--  * Otherwise returns nothing.
create procedure dbo.GetMerchantByGlobalID @globalMerchantID varchar(100),
									                         @includePartnerMerchantIDs bit
as
  set nocount on;

  -- First gather our internal merchant information.
  declare @merchantId int;
  declare @name nvarchar(100);
  declare @globalProviderID varchar(100);
  select @merchantId = Merchants.Id, @name = Merchants.Name, @globalProviderID = Providers.GlobalID from dbo.Merchants
    join dbo.Providers on Providers.Id = ProviderId
    where Merchants.GlobalID = @globalMerchantID;

  -- If the merchant existed, continue retrieving it.
  if (@merchantId is not null)
    begin
      -- Return our internal information.
      select @merchantId as MerchantId, @name as Name, @globalProviderID as GlobalProviderID
    
      -- Then return partner IDs for the merchant.
      if (@includePartnerMerchantIDs = 1)
        begin
          -- Authorization IDs.
          select [Partner], AuthorizationID from dbo.PartnerMerchantAuthorizationIDs
            where MerchantId = @merchantId and Active = 1
            order by Id;

          -- Settlement IDs.
          select [Partner], SettlementID from dbo.PartnerMerchantSettlementIDs
            where MerchantId = @merchantId and Active = 1
            order by Id;
        end
    end

GO