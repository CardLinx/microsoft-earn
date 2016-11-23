--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
-- THIS SPROC IS DEPRECATED AND WILL BE REMOVED DURING CLEANUP, AFTER FDC HAS BEEN DECOMMISSIONED. TO MY KNOWLEDGE, THIS HAS NEVER ACTUALLY RUN IN PRODUCTION.


CREATE PROCEDURE ProcessRedemptionTimeout
  @partnerId int,
  @authAmt int,
  @redemptionEventId int,
  @recommendedPartnerClaimedDealId nvarchar(255),
  @recommendedPartnerDealId nvarchar(255),
  @partnerCardId varchar(255),
  @partnerMerchantId nvarchar(255),
  @partnerOfferMerchantId nvarchar(255),
  @purchaseDateTime datetime2(7)
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'ProcessRedemptionTimeout'

BEGIN TRY
  BEGIN TRANSACTION
  EXECUTE sp_getapplock 'Commerce_ProcessRedemptionTimeout_Lock', 'exclusive'

  --TODO: Indexed view for better performance?
  DECLARE @matchingRedeemedDeals TABLE
    (
      RedeemedDealId uniqueidentifier,
      AuthorizationAmount int,
      DiscountAmount int,
      PartnerRedeemedDealId nvarchar(255),
      DealId int
    )
  INSERT INTO @matchingRedeemedDeals
    SELECT RD.Id,
           RD.AuthorizationAmount,
           RD.DiscountAmount,
           PRD.PartnerRedeemedDealId,
           CD.DealId
      FROM dbo.RedeemedDeals RD
           JOIN dbo.PartnerRedeemedDeals PRD ON PRD.RedeemedDealId = RD.Id
           JOIN dbo.TransactionLinks CD ON CD.Id = RD.ClaimedDealId
           JOIN dbo.PartnerDeals PD ON PD.DealId = CD.DealId
           JOIN dbo.Cards C ON C.Id = CD.CardId
      WHERE RD.AuthorizationAmount = @authAmt
        AND RD.RedemptionEventId = @redemptionEventId
        AND RD.PurchaseDateTime = @purchaseDateTime
        AND RD.Reversed = 0
        AND CD.PartnerId = @partnerId
        AND PD.PartnerId = @partnerId
        AND PRD.RecommendedPartnerClaimedDealId = @recommendedPartnerClaimedDealId
        AND PRD.RecommendedPartnerDealId = @recommendedPartnerDealId
        AND C.FDCToken = @partnerCardId

  -- Determine how many matching redeemed deals were found.
  DECLARE @numberMatchingRedeemedDeals int = (SELECT count(*) FROM @matchingRedeemedDeals)

  --If no matching redeemed deals were found, raise an error.
  IF (@numberMatchingRedeemedDeals = 0) RAISERROR ('MatchingRedeemedDealNotFound', 16, 1)

  -- If more than one matching redeemed deals were found, raise an error.
  IF (@numberMatchingRedeemedDeals > 1) RAISERROR ('MultipleMatchingRedeemedDealsFound', 16, 1)

  -- Get singleton redeemed deal info.
  DECLARE @redeemedDealId uniqueidentifier
  DECLARE @authorizationAmount int
  DECLARE @discountAmount int
  DECLARE @partnerRedeemedDealId nvarchar(255)
  DECLARE @dealId int
  SELECT @redeemedDealId = RedeemedDealId
        ,@authorizationAmount = AuthorizationAmount
        ,@discountAmount = DiscountAmount
        ,@partnerRedeemedDealId = PartnerRedeemedDealId
        ,@dealId = DealId
    FROM @matchingRedeemedDeals

  -- If the partner merchant ID or outlet partner merchant ID is not listed as offering the deal, throw an exception.
  IF NOT EXISTS (SELECT DealId FROM dbo.DealPartnerMerchantIds WHERE PartnerId = @partnerId AND PartnerMerchantId = @partnerMerchantId AND DealId = @dealId) 
     OR
     NOT EXISTS (SELECT DealId FROM dbo.DealPartnerMerchantIds WHERE PartnerId = @partnerId AND PartnerMerchantId = @partnerOfferMerchantId AND DealId = @dealId)
    RAISERROR ('InvalidPartnerMerchantId', 16, 1)

  -- Set credit status according to redemption event type.
  DECLARE @creditStatusId int = 0 -- AuthorizationReceived.
  IF @redemptionEventId = 1 -- RealTime; deprecated. Will be removed with the rest of the FDC implementation.
    SET @creditStatusId = 4 -- SettledAsReversed; deprecated. Will be removed with the rest of the FDC implementation.
  
  -- Update the record to indicate the redemption has been reversed.
  UPDATE dbo.RedeemedDeals
    SET Reversed = 1,
        CreditStatusId = @creditStatusId
    WHERE Id = @redeemedDealId

  -- Return information needed to convey the realized value of the deal.
  SELECT Amount
        ,[Percent]
        ,AuthorizationAmount = @authorizationAmount
        ,PartnerRedeemedDealId = @partnerRedeemedDealId
        ,DiscountAmount = @discountAmount
    FROM dbo.Deals
    WHERE Id = @dealId
  
  COMMIT TRANSACTION

  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Status='Info',@Milliseconds=@Milliseconds
END TRY
BEGIN CATCH
  IF @@trancount > 0 ROLLBACK TRANSACTION
  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Status='Error',@Milliseconds=@Milliseconds
END CATCH
GO