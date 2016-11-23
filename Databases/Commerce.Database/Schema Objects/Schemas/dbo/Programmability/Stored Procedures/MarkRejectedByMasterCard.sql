--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE MarkRejectedByMasterCard
  @partnerCardId varchar(255),
  @discountAmount int,
  @dateCreditApproved date,
  @partnerReferenceNumber nvarchar(255)
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'MarkRejectedByMasterCard'

BEGIN TRY
  --NOTES: MasterCard does not have a definitive means of identifying a transaction throughout its lifecycle. There isn't even a definitive means of
  --         linking a rejected redemption back to Microsoft's earlier credit issuance. They don't even send back a merchant identifier. Because of this,
  --		 a best effort to match the exact transaction is made but in the end there is some chance the wrong redemption will be marked RejectedByPartner.
  --      This sproc returns a "select value" to indicate result, because the update should be committed even under exceptional circumstances. The middle
  --         tier will raise the proper warning for these cases.

  DECLARE @selectValue int = 1

  DECLARE @candidateRedeemedDeals table
  (
	  RedeemedDealId uniqueidentifier
  )
  
  DECLARE @creditGranted INT = 500 -- CreditGranted

  -- Get all redemptions for this card that were made for MasterCard offers, match the discount amount, partner reference number, and date the credit was approved, and are currently marked SettledAsRedeemed
  INSERT INTO @candidateRedeemedDeals
	SELECT RD.Id RedeemedDealId
	FROM dbo.RedeemedDeals RD
	     JOIN dbo.PartnerRedeemedDeals PRD ON PRD.RedeemedDealId = RD.Id
		 JOIN dbo.TransactionLinks TL ON TL.Id = RD.ClaimedDealId
		 JOIN dbo.Cards C ON C.Id = TL.CardId
	WHERE TL.PartnerId = 4
	  AND RD.CreditStatusId = @creditGranted
      AND C.PartnerToken = @partnerCardId
	  AND RD.DiscountAmount = @discountAmount
	  AND PRD.PartnerReferenceNumber = @partnerReferenceNumber
	  AND CAST(RD.DateCreditApproved AS DATE) = @dateCreditApproved

  -- If no transactions matched all criteria, begin relaxing them.
  IF (SELECT count(*) FROM @candidateRedeemedDeals) = 0
  BEGIN
    -- Note that the match, if any, is inexact. If no matches are found or more than one match is found, this value will be overwritten.
	SET @selectValue = -1

    -- Throw out the date credit approved and try again. MasterCard has indicated this is very probably not going to line up because of a number of possible issues on their end.
    INSERT INTO @candidateRedeemedDeals
	  SELECT RD.Id RedeemedDealId
      FROM dbo.RedeemedDeals RD
	       JOIN dbo.PartnerRedeemedDeals PRD ON PRD.RedeemedDealId = RD.Id
		   JOIN dbo.TransactionLinks TL ON TL.Id = RD.ClaimedDealId
		   JOIN dbo.Cards C ON C.Id = TL.CardId
	  WHERE TL.PartnerId = 4
	    AND RD.CreditStatusId = @creditGranted
	    AND C.PartnerToken = @partnerCardId
	    AND RD.DiscountAmount = @discountAmount
	    AND PRD.PartnerReferenceNumber = @partnerReferenceNumber
	    AND RD.DateCreditApproved IS NOT NULL

	-- If there still isn't a matching transaction, remove partner reference number instead.
	IF (SELECT count(*) FROM @candidateRedeemedDeals) = 0
	BEGIN
      INSERT INTO @candidateRedeemedDeals
	    SELECT RD.Id RedeemedDealId
        FROM dbo.RedeemedDeals RD
	         JOIN dbo.PartnerRedeemedDeals PRD ON PRD.RedeemedDealId = RD.Id
		     JOIN dbo.TransactionLinks TL ON TL.Id = RD.ClaimedDealId
		     JOIN dbo.Cards C ON C.Id = TL.CardId
	    WHERE TL.PartnerId = 4
	    AND RD.CreditStatusId = @creditGranted
	    AND C.PartnerToken = @partnerCardId
	    AND RD.DiscountAmount = @discountAmount
	    AND CAST(RD.DateCreditApproved AS DATE) = @dateCreditApproved

      -- If there still isn't a matching transaction, remove both date credit approved and partner reference number.
      IF (SELECT count(*) FROM @candidateRedeemedDeals) = 0
      BEGIN
        INSERT INTO @candidateRedeemedDeals
	      SELECT RD.Id RedeemedDealId
          FROM dbo.RedeemedDeals RD
	         JOIN dbo.PartnerRedeemedDeals PRD ON PRD.RedeemedDealId = RD.Id
		     JOIN dbo.TransactionLinks TL ON TL.Id = RD.ClaimedDealId
		     JOIN dbo.Cards C ON C.Id = TL.CardId
	      WHERE TL.PartnerId = 4
	        AND RD.CreditStatusId = @creditGranted
	        AND C.PartnerToken = @partnerCardId
	        AND RD.DiscountAmount = @discountAmount
	        AND RD.DateCreditApproved IS NOT NULL

        -- If there STILL isn't a maching transaction, indicate such in the select value.
        IF (SELECT count(*) FROM @candidateRedeemedDeals) = 0
		BEGIN
          SET @selectValue = 0
        END
      END
    END
  END

  -- If at least one deal was matched....
  IF @selectValue <> 0
  BEGIN
	-- Update the top matching redeemed deal.
     UPDATE dbo.RedeemedDeals
      SET CreditStatusId = 510 -- RejectedByPartner.
      WHERE Id = (SELECT top 1 RedeemedDealId FROM @candidateRedeemedDeals)

    -- If more than one redeemed deal matched the best criteria, indicate such in the select value.
    IF (SELECT count(*) FROM @candidateRedeemedDeals) > 1
    BEGIN
      SET @selectValue = 2
    END
  END

  -- Send back the value indicating the result of the operation.
  SELECT @selectValue as SelectValue

  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Status='Info',@Milliseconds=@Milliseconds
END TRY
BEGIN CATCH
  IF @@trancount > 0 ROLLBACK TRANSACTION
  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Status='Error',@Milliseconds=@Milliseconds
END CATCH
GO