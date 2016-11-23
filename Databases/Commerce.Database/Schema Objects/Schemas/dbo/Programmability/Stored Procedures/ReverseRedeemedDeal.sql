--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
-- THIS SPROC IS DEPRECATED AND WILL BE REMOVED DURING CLEANUP, AFTER FDC HAS BEEN DECOMMISSIONED. TO MY KNOWLEDGE, THIS HAS NEVER ACTUALLY RUN IN PRODUCTION.


CREATE PROCEDURE ReverseRedeemedDeal
  @partnerId int,
  @partnerRedeemedDealId nvarchar(255),
  @redemptionEventId int,
  @partnerDealId nvarchar(255),
  @partnerCardId varchar(255),
  @partnerMerchantId nvarchar(255)
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'ReverseRedeemedDeal'

BEGIN TRY
  BEGIN TRANSACTION
  EXECUTE sp_getapplock 'Commerce_ReverseRedeemedDeal_Lock', 'exclusive'

  -- Find the redeemed deal record.
  DECLARE @redeemedDealId uniqueidentifier
  DECLARE @claimedDealId bigint
  DECLARE @claimedDealRedemptionEventId int
  DECLARE @authorizationAmount int
  DECLARE @discountAmount int

  SELECT @redeemedDealId = RD.Id,
         @claimedDealId = RD.ClaimedDealId,
         @claimedDealRedemptionEventId = RD.RedemptionEventId,
         @authorizationAmount = RD.AuthorizationAmount,
         @discountAmount = RD.DiscountAmount
    FROM dbo.RedeemedDeals RD
         JOIN dbo.PartnerRedeemedDeals PRD ON PRD.RedeemedDealId = RD.Id
    WHERE RD.Reversed = 0
      AND PRD.PartnerRedeemedDealId = @partnerRedeemedDealId

  -- Make sure the record exists.
  IF (@claimedDealId IS NULL) RAISERROR('RedeemedDealNotFound', 16, 1)

  -- Make sure the redemption events match
  IF (@claimedDealRedemptionEventId != @redemptionEventId) RAISERROR('RedemptionEventMismatch', 16, 1)

  -- Retrieve claimed deal information.
  DECLARE @dealId int
  DECLARE @claimedDealPartnerDealId nvarchar(255)
  DECLARE @cardId int
  DECLARE @claimedDealPartnerId int

  SELECT @dealId = CD.DealId
        ,@cardId = CD.CardId
        ,@claimedDealPartnerId = CD.PartnerId
        ,@claimedDealPartnerDealId = PD.PartnerDealId
    FROM dbo.TransactionLinks CD
         JOIN dbo.PartnerDeals PD ON PD.DealId = CD.DealId
    WHERE CD.Id = @claimedDealId
	 AND PD.PartnerId = @partnerId

  -- Make sure same partner.
  IF (@claimedDealPartnerId IS NULL OR @claimedDealPartnerId != @partnerId) RAISERROR ('PartnerMismatch', 16, 1)

  -- Make sure partner deal ID matches the actual redeemed deal.
  IF (@partnerDealId != @claimedDealPartnerDealId) RAISERROR('PartnerDealIdMismatch', 16, 1)

  -- Make sure the token matches.
  IF NOT EXISTS (SELECT * FROM dbo.Cards WHERE Cards.Id = @cardId AND FDCToken = @partnerCardId) 
    RAISERROR ('PartnerCardIdMismatch', 16, 1)
  ELSE
    -- Make sure the partnerMerchantId matches
    IF NOT EXISTS (SELECT * FROM dbo.DealPartnerMerchantIds WHERE DealId = @dealId AND PartnerMerchantId = @partnerMerchantId)
      RAISERROR('InvalidPartnerMerchantId', 16, 1)

  -- Set credit status according to redemption event type.
  DECLARE @creditStatusId int = 0 -- AuthorizationReceived
  IF @redemptionEventId = 1 -- RealTime; deprecated. Will be removed with the rest of FDC's implementation.
    SET @creditStatusId = 4 -- SettledAsReversed; deprecated. Will be removed with the rest of FDC's implementation.

  -- Update the record to indicate the redemption has been reversed.
  UPDATE dbo.RedeemedDeals
    SET Reversed = 1
       ,CreditStatusId = @creditStatusId
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