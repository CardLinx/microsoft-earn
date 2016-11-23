--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
-- Returns all the Earn and Burn transactions for a user
CREATE PROCEDURE [dbo].[GetEarnBurnHistory] @userId uniqueidentifier
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'GetEarnBurnHistory'

  DECLARE @intUserId INT = (SELECT Id FROM dbo.Users WHERE GlobalId = @userId)
  
  SELECT
      ReimbursementTenderId = TransactionType,
      MerchantName = MerchantName,
      DiscountSummary = DealSummary,
      [Percent] = DealPercent,
      PurchaseDateTime = TransactionDate,
      AuthorizationAmount = TransactionAmount,
      Reversed = Reversed,
      CreditStatusId = TransactionStatusId,
      DiscountAmount =
        CASE
            WHEN TransactionType = 0
        THEN
            EarnCredit
        ELSE
            BurnDebit
        END,
      LastFourDigits = LastFourDigits,
      CardBrandId = CardBrand
  FROM
    dbo.GetEarnBurnLineItems(@intUserId)
  WHERE
    (PermaPending = 0 OR TransactionStatusId = 500) AND -- CreditGranted
    ReviewStatusId in (0, 2) -- Unnecessary or ResolvedAccept
  ORDER BY
    TransactionStatusId ASC, TransactionDate DESC

SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
EXECUTE dbo.LogEvent @Process=@SP,@Status='Info',@Milliseconds=@Milliseconds

GO


