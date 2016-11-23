--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
-- Returns the user's earn/burn line items for every transaction and bonus Earn grant.
create function dbo.GetEarnBurnLineItems(@userId int)
returns @UserEarnBurnLineItems table (TransactionDate datetime2(7),
                                      EarnCredit int,
                                      BurnDebit int,
                                      HasRedeemedDealRecord bit,
                                      TransactionType int,
                                      DealSummary nvarchar(255),
                                      DealPercent money,
                                      MerchantName nvarchar(255),
                                      TransactionAmount int,
                                      Reversed bit,
                                      TransactionStatusId int,
                                      LastFourDigits char(4),
                                      CardBrand int,
                                      PermaPending bit,
                                      ReviewStatusId int,
                                      RecordCount int)
as
begin
    declare @userEarnBurnTransactions table (UserId int,
                                             TransactionDate datetime2(7),
                                             EarnCredit int,
                                             BurnDebit int,
                                             HasRedeemedDealRecord bit,
                                             TransactionType int,
                                             DealSummary nvarchar(255),
                                             DealPercent int,
                                             MerchantName nvarchar(255),
                                             TransactionAmount int,
                                             Reversed bit,
                                             TransactionStatusId int,
                                             LastFourDigits char(4),
                                             CardBrand int,
                                             PermaPending bit,
                                             ReviewStatusId int);

    --*****************************************
    -- Visa
    --*****************************************

    -- First, collate the Authorizations and RedeemedDeals records for this user.
    with matches (UserId, EarnCredit, BurnDebit, TransactionType, AuthPartnerTransactionId, SetPartnerTransactionId, TransactionDate, TransactionStatusId, 
                  DealSummary, DealPercent, MerchantName, TransactionAmount, Reversed, LastFourDigits, CardBrand, PermaPending, ReviewStatusId)
    as
    (
      select AuthorizedVisaTransactions.UserId, RedeemedVisaTransactions.EarnCredit, RedeemedVisaTransactions.BurnDebit, AuthorizedVisaTransactions.TransactionType,
             AuthorizedVisaTransactions.PartnerTransactionId, RedeemedVisaTransactions.PartnerTransactionId, AuthorizedVisaTransactions.TransactionDate,
             RedeemedVisaTransactions.TransactionStatusId, AuthorizedVisaTransactions.DealSummary, AuthorizedVisaTransactions.DealPercent,
             AuthorizedVisaTransactions.MerchantName, RedeemedVisaTransactions.TransactionAmount, RedeemedVisaTransactions.Reversed,
             RedeemedVisaTransactions.LastFourDigits, RedeemedVisaTransactions.CardBrand, RedeemedVisaTransactions.PermaPending, RedeemedVisaTransactions.ReviewStatusId
      from AuthorizedVisaTransactions with (noexpand)
       join RedeemedVisaTransactions with (noexpand)
        on RedeemedVisaTransactions.UserId = AuthorizedVisaTransactions.UserId and
        RedeemedVisaTransactions.TransactionType = AuthorizedVisaTransactions.TransactionType and
        RedeemedVisaTransactions.PartnerTransactionId = AuthorizedVisaTransactions.PartnerTransactionId
      where AuthorizedVisaTransactions.UserId = @userId
    )

    insert into @userEarnBurnTransactions

    -- Next, include all the matches in the results for transactions that have not ended in failure, indicating that they all have records within the RedeemedDeals table.
    select matches.UserId, matches.TransactionDate, matches.EarnCredit, matches.BurnDebit, HasRedeemedDealRecord = 1, matches.TransactionType, matches.DealSummary,
           matches.DealPercent, matches.MerchantName, matches.TransactionAmount, matches.Reversed, matches.TransactionStatusId, matches.LastFourDigits,
           matches.CardBrand, matches.PermaPending, matches.ReviewStatusId
    from matches
    where matches.UserId = @userId and matches.TransactionStatusId <= 500 -- CreditGranted, or an earlier (non-terminal) state.

    UNION ALL

    -- Then, include all transactions that are in the Authorizations table for which no corresponding RedeemedDeals record could be found. Mark these records as
    --  NOT being represented within the RedeemedDeals table.
    select AuthorizedVisaTransactions.UserId, AuthorizedVisaTransactions.TransactionDate, AuthorizedVisaTransactions.EarnCredit, AuthorizedVisaTransactions.BurnDebit, HasRedeemedDealRecord = 0,
           AuthorizedVisaTransactions.TransactionType, AuthorizedVisaTransactions.DealSummary, AuthorizedVisaTransactions.DealPercent,
           AuthorizedVisaTransactions.MerchantName, AuthorizedVisaTransactions.TransactionAmount, AuthorizedVisaTransactions.Reversed,
           AuthorizedVisaTransactions.TransactionStatusId, AuthorizedVisaTransactions.LastFourDigits, AuthorizedVisaTransactions.CardBrand, AuthorizedVisaTransactions.PermaPending,
           AuthorizedVisaTransactions.ReviewStatusId
    from AuthorizedVisaTransactions with (noexpand)
     left outer join matches on matches.UserId = AuthorizedVisaTransactions.UserId and
      matches.AuthPartnerTransactionId = AuthorizedVisaTransactions.PartnerTransactionId
    where AuthorizedVisaTransactions.UserId = @userId and datediff(day, AuthorizedVisaTransactions.TransactionDate, getdate()) < 7
      and matches.UserId is null

    UNION ALL

    -- Finally, include all transactions that are in the RedeemedDeals table for which no corresponding Authorizations record could be found. Mark these records as
    --  being represented within the RedeemedDeals table.
    select RedeemedVisaTransactions.UserId, RedeemedVisaTransactions.TransactionDate, RedeemedVisaTransactions.EarnCredit, RedeemedVisaTransactions.BurnDebit, HasRedeemedDealRecord = 1,
           RedeemedVisaTransactions.TransactionType, RedeemedVisaTransactions.DealSummary, RedeemedVisaTransactions.DealPercent, RedeemedVisaTransactions.MerchantName,
           RedeemedVisaTransactions.TransactionAmount, RedeemedVisaTransactions.Reversed, RedeemedVisaTransactions.TransactionStatusId,
           RedeemedVisaTransactions.LastFourDigits, RedeemedVisaTransactions.CardBrand, RedeemedVisaTransactions.PermaPending, RedeemedVisaTransactions.ReviewStatusId
    from RedeemedVisaTransactions with (noexpand)
     left outer join matches on matches.UserId = RedeemedVisaTransactions.UserId and
      matches.SetPartnerTransactionId = RedeemedVisaTransactions.PartnerTransactionId
    where RedeemedVisaTransactions.UserId = @userId and (matches.TransactionStatusId is null or matches.TransactionStatusId <= 500) -- CreditGranted, or an earlier (non-terminal) state.
      and matches.UserId is null;

    --*****************************************
    -- End Visa
    --*****************************************

    --*****************************************
    -- MasterCard
    --*****************************************

    -- First, use the record matching heuristic to collate the Authorizations and RedeemedDeals records for this user.
    with likelyMatches (UserId, EarnCredit, BurnDebit, TransactionType, AuthBankCustomerNumber, AuthBankNetRefNumber, AuthTransactionDate,
                        SetBankCustomerNumber, SetBankNetRefNumber, SetTransactionDate, TransactionStatusId, DealSummary, DealPercent, MerchantName, TransactionAmount,
                        Reversed, LastFourDigits, CardBrand, PermaPending, ReviewStatusId)
    as
    (
      -- No algorithm is possible with the data MasterCard sends via their API calls and files. The following is only a heuristic, but it's almost always correct.
      select AuthorizedMasterCardTransactions.UserId, RedeemedMasterCardTransactions.EarnCredit, RedeemedMasterCardTransactions.BurnDebit,
             AuthorizedMasterCardTransactions.TransactionType, AuthorizedMasterCardTransactions.BankCustomerNumber,
             AuthorizedMasterCardTransactions.BankNetRefNumber, AuthorizedMasterCardTransactions.TransactionDate, RedeemedMasterCardTransactions.BankCustomerNumber,
             RedeemedMasterCardTransactions.BankNetRefNumber, RedeemedMasterCardTransactions.TransactionDate, RedeemedMasterCardTransactions.TransactionStatusId,
             AuthorizedMasterCardTransactions.DealSummary, AuthorizedMasterCardTransactions.DealPercent, AuthorizedMasterCardTransactions.MerchantName,
             RedeemedMasterCardTransactions.TransactionAmount, RedeemedMasterCardTransactions.Reversed, RedeemedMasterCardTransactions.LastFourDigits, 
             RedeemedMasterCardTransactions.CardBrand, RedeemedMasterCardTransactions.PermaPending, RedeemedMasterCardTransactions.ReviewStatusId
      from dbo.AuthorizedMasterCardTransactions with (noexpand)
       join dbo.RedeemedMasterCardTransactions with (noexpand) on RedeemedMasterCardTransactions.UserId = AuthorizedMasterCardTransactions.UserId and
        RedeemedMasterCardTransactions.TransactionType = AuthorizedMasterCardTransactions.TransactionType and
        RedeemedMasterCardTransactions.BankCustomerNumber = AuthorizedMasterCardTransactions.BankCustomerNumber and
        RedeemedMasterCardTransactions.BankNetRefNumber = AuthorizedMasterCardTransactions.BankNetRefNumber
      where AuthorizedMasterCardTransactions.UserId = @userId
    )

    insert into @userEarnBurnTransactions

    -- Next, include all the likely matches in the results for transactions that have not ended in failure and indicate that they all have records within the RedeemedDeals table.
    select likelyMatches.UserId, TransactionDate = likelyMatches.AuthTransactionDate, likelyMatches.EarnCredit, likelyMatches.BurnDebit, HasRedeemedDealRecord = 1,
           likelyMatches.TransactionType, likelyMatches.DealSummary, likelyMatches.DealPercent, likelyMatches.MerchantName, likelyMatches.TransactionAmount,
           likelyMatches.Reversed, likelyMatches.TransactionStatusId, likelyMatches.LastFourDigits, likelyMatches.CardBrand, likelyMatches.PermaPending,
           likelyMatches.ReviewStatusId
    from likelyMatches
    where likelyMatches.UserId = @userId and likelyMatches.TransactionStatusId <= 500 -- CreditGranted, or an earlier (non-terminal) state.

    UNION ALL

    -- Then, include all transactions that are in the Authorizations table for which no corresponding RedeemedDeals record could be found. Mark these records as
    --  NOT being represented within the RedeemedDeals table.
    select AuthorizedMasterCardTransactions.UserId, AuthorizedMasterCardTransactions.TransactionDate, AuthorizedMasterCardTransactions.EarnCredit, AuthorizedMasterCardTransactions.BurnDebit,
           HasRedeemedDealRecord = 0, AuthorizedMasterCardTransactions.TransactionType, AuthorizedMasterCardTransactions.DealSummary,
           AuthorizedMasterCardTransactions.DealPercent, AuthorizedMasterCardTransactions.MerchantName, AuthorizedMasterCardTransactions.TransactionAmount,
           AuthorizedMasterCardTransactions.Reversed, AuthorizedMasterCardTransactions.TransactionStatusId, AuthorizedMasterCardTransactions.LastFourDigits,
           AuthorizedMasterCardTransactions.CardBrand, AuthorizedMasterCardTransactions.PermaPending, AuthorizedMasterCardTransactions.ReviewStatusId
    from dbo.AuthorizedMasterCardTransactions with (noexpand)
     left outer join likelyMatches on likelyMatches.UserId = AuthorizedMasterCardTransactions.UserId and
      likelyMatches.AuthBankCustomerNumber = AuthorizedMasterCardTransactions.BankCustomerNumber and
      likelyMatches.AuthBankNetRefNumber = AuthorizedMasterCardTransactions.BankNetRefNumber
    where AuthorizedMasterCardTransactions.UserId = @userId and datediff(day, AuthorizedMasterCardTransactions.TransactionDate, getdate()) < 7
      and likelyMatches.UserId is null

    UNION ALL

    -- Finally, include all transactions that are in the RedeemedDeals table for which no corresponding Authorizations record could be found. Mark these records as
    --  being represented within the RedeemedDeals table.
    select RedeemedMasterCardTransactions.UserId, RedeemedMasterCardTransactions.TransactionDate, RedeemedMasterCardTransactions.EarnCredit, RedeemedMasterCardTransactions.BurnDebit, HasRedeemedDealRecord = 1,
           RedeemedMasterCardTransactions.TransactionType, RedeemedMasterCardTransactions.DealSummary, RedeemedMasterCardTransactions.DealPercent,
           RedeemedMasterCardTransactions.MerchantName, RedeemedMasterCardTransactions.TransactionAmount, RedeemedMasterCardTransactions.Reversed,
           RedeemedMasterCardTransactions.TransactionStatusId, RedeemedMasterCardTransactions.LastFourDigits, RedeemedMasterCardTransactions.CardBrand,
           RedeemedMasterCardTransactions.PermaPending, RedeemedMasterCardTransactions.ReviewStatusId
    from dbo.RedeemedMasterCardTransactions with (noexpand)
     left outer join likelyMatches on likelyMatches.UserId = RedeemedMasterCardTransactions.UserId and
      likelyMatches.SetBankCustomerNumber = RedeemedMasterCardTransactions.BankCustomerNumber and
      likelyMatches.SetBankNetRefNumber = RedeemedMasterCardTransactions.BankNetRefNumber
    where RedeemedMasterCardTransactions.UserId = @userId and (likelyMatches.TransactionStatusId is null or likelyMatches.TransactionStatusId <= 500) -- CreditGranted, or an earlier (non-terminal) state.
      and likelyMatches.UserId is null;

    --*****************************************
    -- End MasterCard
    --*****************************************
		
	--*****************************************
    -- Amex
    --*****************************************

	with amexMatches (UserId, EarnCredit, BurnDebit, TransactionType, AuthPartnerTransactionId, SetPartnerTransactionId, TransactionDate, TransactionStatusId, 
                  DealSummary, DealPercent, MerchantName, TransactionAmount, Reversed, LastFourDigits, CardBrand, PermaPending, ReviewStatusId)
    as
    (
      select AuthorizedAmexTransactions.UserId, RedeemedAmexTransactions.EarnCredit, RedeemedAmexTransactions.BurnDebit, AuthorizedAmexTransactions.TransactionType,
             AuthorizedAmexTransactions.PartnerTransactionId, RedeemedAmexTransactions.PartnerTransactionId, AuthorizedAmexTransactions.TransactionDate,
             RedeemedAmexTransactions.TransactionStatusId, AuthorizedAmexTransactions.DealSummary, AuthorizedAmexTransactions.DealPercent,
             AuthorizedAmexTransactions.MerchantName, RedeemedAmexTransactions.TransactionAmount, RedeemedAmexTransactions.Reversed,
             RedeemedAmexTransactions.LastFourDigits, RedeemedAmexTransactions.CardBrand, RedeemedAmexTransactions.PermaPending, RedeemedAmexTransactions.ReviewStatusId
      from AuthorizedAmexTransactions with (noexpand)
       join RedeemedAmexTransactions with (noexpand)
        on RedeemedAmexTransactions.UserId = AuthorizedAmexTransactions.UserId and
        RedeemedAmexTransactions.TransactionType = AuthorizedAmexTransactions.TransactionType and
		RedeemedAmexTransactions.CardId = AuthorizedAmexTransactions.CardId
        --RedeemedAmexTransactions.PartnerTransactionId = AuthorizedAmexTransactions.PartnerTransactionId
      where AuthorizedAmexTransactions.UserId = @userId
    )

    insert into @userEarnBurnTransactions

    -- Next, include all the amexMatches in the results for transactions that have not ended in failure, indicating that they all have records within the RedeemedDeals table.
    select amexMatches.UserId, amexMatches.TransactionDate, amexMatches.EarnCredit, amexMatches.BurnDebit, HasRedeemedDealRecord = 1, amexMatches.TransactionType, amexMatches.DealSummary,
           amexMatches.DealPercent, amexMatches.MerchantName, amexMatches.TransactionAmount, amexMatches.Reversed, amexMatches.TransactionStatusId, amexMatches.LastFourDigits,
           amexMatches.CardBrand, amexMatches.PermaPending, amexMatches.ReviewStatusId
    from amexMatches
    where amexMatches.UserId = @userId and amexMatches.TransactionStatusId <= 500 -- CreditGranted, or an earlier (non-terminal) state.

    UNION ALL

    -- Then, include all transactions that are in the Authorizations table for which no corresponding RedeemedDeals record could be found. Mark these records as
    --  NOT being represented within the RedeemedDeals table.
    select AuthorizedAmexTransactions.UserId, AuthorizedAmexTransactions.TransactionDate, AuthorizedAmexTransactions.EarnCredit, AuthorizedAmexTransactions.BurnDebit, HasRedeemedDealRecord = 0,
           AuthorizedAmexTransactions.TransactionType, AuthorizedAmexTransactions.DealSummary, AuthorizedAmexTransactions.DealPercent,
           AuthorizedAmexTransactions.MerchantName, AuthorizedAmexTransactions.TransactionAmount, AuthorizedAmexTransactions.Reversed,
           AuthorizedAmexTransactions.TransactionStatusId, AuthorizedAmexTransactions.LastFourDigits, AuthorizedAmexTransactions.CardBrand, AuthorizedAmexTransactions.PermaPending,
           AuthorizedAmexTransactions.ReviewStatusId
    from AuthorizedAmexTransactions with (noexpand)
     left outer join amexMatches on amexMatches.UserId = AuthorizedAmexTransactions.UserId and
      amexMatches.AuthPartnerTransactionId = AuthorizedAmexTransactions.PartnerTransactionId
    where AuthorizedAmexTransactions.UserId = @userId and datediff(day, AuthorizedAmexTransactions.TransactionDate, getdate()) < 7
      and amexMatches.UserId is null

    UNION ALL

    -- Finally, include all transactions that are in the RedeemedDeals table for which no corresponding Authorizations record could be found. Mark these records as
    --  being represented within the RedeemedDeals table.
    select RedeemedAmexTransactions.UserId, RedeemedAmexTransactions.TransactionDate, RedeemedAmexTransactions.EarnCredit, RedeemedAmexTransactions.BurnDebit, HasRedeemedDealRecord = 1,
           RedeemedAmexTransactions.TransactionType, RedeemedAmexTransactions.DealSummary, RedeemedAmexTransactions.DealPercent, RedeemedAmexTransactions.MerchantName,
           RedeemedAmexTransactions.TransactionAmount, RedeemedAmexTransactions.Reversed, RedeemedAmexTransactions.TransactionStatusId,
           RedeemedAmexTransactions.LastFourDigits, RedeemedAmexTransactions.CardBrand, RedeemedAmexTransactions.PermaPending, RedeemedAmexTransactions.ReviewStatusId
    from RedeemedAmexTransactions with (noexpand)
     left outer join amexMatches on amexMatches.UserId = RedeemedAmexTransactions.UserId and
      amexMatches.SetPartnerTransactionId = RedeemedAmexTransactions.PartnerTransactionId
    where RedeemedAmexTransactions.UserId = @userId and (amexMatches.TransactionStatusId is null or amexMatches.TransactionStatusId <= 500) -- CreditGranted, or an earlier (non-terminal) state.
      and amexMatches.UserId is null;
	  
	--*****************************************
    -- End Amex
    --*****************************************

    --*****************************************
    -- First Data
    --*****************************************

    insert into @userEarnBurnTransactions

    select UserId, TransactionDate, EarnCredit, BurnDebit, HasRedeemedDealRecord = 1, TransactionType, DealSummary, DealPercent, MerchantName, TransactionAmount,
           Reversed, TransactionStatusId, LastFourDigits, CardBrand, PermaPending, ReviewStatusId
    from dbo.FirstDataTransactions with (noexpand)
    where UserId = @userId

    --*****************************************
    -- End First Data
    --*****************************************

    --*****************************************
    -- Rewards and Customer Service Grants
    --*****************************************

    declare @globalUserId uniqueIdentifier = (select Users.GlobalId from dbo.Users where Users.Id = @userId);

    insert into @userEarnBurnTransactions

    select UserId, TransactionDate, EarnCredit, BurnDebit, HasRedeemedDealRecord = 0, TransactionType, DealSummary, DealPercent, MerchantName, TransactionAmount,
           Reversed, TransactionStatusId, LastFourDigits, CardBrand, 0, 0
    from dbo.EarnRewardsAndGrants with (noexpand)
    where GlobalUserId = @globalUserId

    --*****************************************
    -- End Rewards and Customer Service Grants
    --*****************************************

    insert into @UserEarnBurnLineItems
    select TransactionDate, EarnCredit, BurnDebit, HasRedeemedDealRecord, TransactionType, DealSummary, DealPercent, MerchantName, TransactionAmount, Reversed,
           TransactionStatusId, LastFourDigits, CardBrand, PermaPending, ReviewStatusId, count_big(*) as RecordCount
    from @userEarnBurnTransactions as userEarnBurnTransactions
	where userEarnBurnTransactions.EarnCredit <> 0 or userEarnBurnTransactions.BurnDebit <> 0
    group by TransactionDate, EarnCredit, BurnDebit, HasRedeemedDealRecord, TransactionType, DealSummary, DealPercent, MerchantName, TransactionAmount, Reversed,
             TransactionStatusId, LastFourDigits, CardBrand, PermaPending, ReviewStatusId
    order by TransactionStatusId asc, TransactionDate desc;

    return
end

GO