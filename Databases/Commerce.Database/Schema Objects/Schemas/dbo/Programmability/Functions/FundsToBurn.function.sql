--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--IF object_id('FundsToBurn') IS NOT NULL DROP FUNCTION FundsToBurn
--GO

-- Determines the amount of earned funds, if any, that can be burned in this transaction.
-- @cardId: The ID of the card whose earned funds are to be burned.
-- @authAmount: The amount of the authorization or settlement. Burn can be up to this amount, but no more than available earned funds.
-- If there are earned funds to burn, returns the amount to burn. Otherwise returns 0.
CREATE FUNCTION FundsToBurn (@cardId int,
                             @authAmount int,
                             @partnerId int,
                             @purchaseDate datetime)
RETURNS INT
BEGIN
  DECLARE @result INT = 0

  -- Determine the user who owns the card.
  DECLARE @userId INT = (SELECT UserId FROM dbo.Cards where Id = @cardId)

  -- Determine if this is an Authorization or a Settlement call.
  -- NOTE: This is a hacky solution, but SelectBestDeal is a gnarly sproc that I don't want to change if possible. When we implement the
  --       new CLO legacy-free implementation, this will be more straight forward.
  DECLARE @authCall BIT = 1;
  IF @partnerId IN (3, 4) AND @purchaseDate IS NOT NULL
  BEGIN
    SET @authCall = 0
  END

  -- If this not an Authorization call, try to find a matching authorization on which to base the burn amount.
  DECLARE @matchFound BIT = 0
  IF @authCall = 0
  BEGIN
    SELECT TOP 1 @result = BurnDebit FROM dbo.GetEarnBurnLineItems(@userId) AS LineItems
    WHERE TransactionStatusId = 0 AND --AuthorizationReceived.
          TransactionType = 1 AND --Burn
          TransactionAmount = @authAmount AND -- This assumption will no longer hold true when/if restaurants or gas stations are included in Burn offers.
          LineItems.CardBrand = @partnerId + 1 AND
          LineItems.PermaPending = 0 AND LineItems.ReviewStatusId in (0, 2)
    SET @matchFound = @@rowcount
  END

  -- If this is an Authorization call or a Settlement call but no matching record could be found, determine available funds through a tally.
  IF @authCall = 1 OR (@authCall = 0 AND @matchFound = 0)
  BEGIN
      -- Determine the remaining Burn balance for all cards ever registered to the user for the Earn program.
      -- In order to guard against overdraft, Burns are counted when still in pending states, but Earns are only counted for settled transactions.
      DECLARE @availableFunds INT =
      (
        SELECT SUM(EarnCredit) - SUM(BurnDebit)
        FROM dbo.GetEarnBurnLineItems(@userId)
        WHERE (TransactionType = 0 and TransactionStatusId = 500) or          -- Earn transactions are only included if they're in the CreditGranted State.
              (TransactionType = 1 and ((PermaPending = 0 and ReviewStatusId in (0, 2)) or TransactionStatusId = 500))
                                                                            -- Burn transactions are always included if 1) BOTH the PermaPending flag is not set AND
                                                                            --  the ReviewStatusId flag is set to Unnecessary or ResolvedAccept,
                                                                            --   OR 2) the transaction is in CreditGranted State.
      )
      IF @availableFunds IS NULL OR @availableFunds < 0 SET @availableFunds = 0

      -- Return the lesser of the remaining Burn balance or the entire authorization amount.
      IF @availableFunds < @authAmount
        SET @result = @availableFunds
      ELSE
        SET @result = @authAmount
  END

  return @result
END
GO