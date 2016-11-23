--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--

--THIS SPROC IS DEPRECATED AND WILL BE REMOVED DURING THE CLEANUP PHASE. MIDDLE TIER NEEDS TO BE UPDATED TO MATCH.

CREATE PROCEDURE GetDealById 
   @globalId uniqueidentifier
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'GetDealById'
       ,@Rows int = 0
       ,@dId int

DECLARE @Mode varchar(100) = convert(varchar(36),@globalId)

SET @dId = (SELECT Id FROM dbo.Deals WHERE GlobalId = @globalId) 

-- Return deal information.
SELECT D.Id
      ,D.GlobalId
      ,D.GlobalID AS ParentDealId
      ,D.ProviderId
      ,M.Id AS MerchantId
      ,null as MerchantCategory
      ,MerchantName = M.Name
      ,cast('2013-1-1' as datetime) AS StartDate
      ,cast('2099-1-1' as datetime) AS EndDate
      ,'USD' AS Currency
      ,D.OfferType + 2 AS ReimbursementTenderId
      ,0 AS Amount
      ,D.PercentBack AS [Percent]
      ,0 AS MinimumPurchase
      ,0 AS [Count]
      ,0 AS UserLimit
      ,(select dbo.BuildDiscountSummary(D.OfferType, D.PercentBack)) AS DiscountSummary
      ,0 AS MaximumDiscount
      ,4 AS DealStatusId
      ,NULL AS DayTimeRestrictions
      ,NULL AS MerchantCategory
  FROM dbo.Offers D
       JOIN dbo.Merchants M on M.ProviderId = D.ProviderId
  WHERE D.Id = @dId
SET @Rows = @Rows + @@rowcount

-- Partner deals are deprecated, but we still want to return a result so that the function succeeds in loading the next result set.
SELECT PartnerId
      ,PartnerDealId
      ,PartnerDealRegistrationStatusId
  FROM dbo.PartnerDeals
  WHERE DealId = 1100001786
SET @Rows = @Rows + @@rowcount

-- Return the partner merchant IDs for the partner deals if any exist.
SELECT [Partner] - 1 as PartnerId
       ,AuthorizationID AS PartnerMerchantId
       ,PartnerMerchantIdTypeId = CASE WHEN [Partner] = 5 THEN 1 ELSE 0 END
       ,NULL AS MerchantTimeZoneId
  FROM dbo.PartnerMerchantAuthorizationIDs
  JOIN dbo.Merchants on Merchants.Id = PartnerMerchantAuthorizationIDs.MerchantId
  JOIN dbo.Offers on Offers.ProviderId = Merchants.ProviderId
  WHERE Offers.Id = @dId

UNION

SELECT [Partner] - 1 as PartnerId
       ,SettlementID AS PartnerMerchantId
       ,PartnerMerchantIdTypeId = CASE WHEN [Partner] = 5 THEN 2 ELSE 0 END
       ,NULL AS MerchantTimeZoneId
  FROM dbo.PartnerMerchantSettlementIDs
  JOIN dbo.Merchants on Merchants.Id = PartnerMerchantSettlementIDs.MerchantId
  JOIN dbo.Offers on Offers.ProviderId = Merchants.ProviderId
  WHERE Offers.Id = @dId
SET @Rows = @Rows + @@rowcount

SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Info',@Milliseconds=@Milliseconds,@Rows=@Rows
GO