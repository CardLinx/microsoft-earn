--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
-- Returns the every user's earn/burn line items for every transaction and bonus Earn grant.
create function dbo.QueryEarnBurnLineItems()
returns @EarnBurnLineItems table (TransactionId uniqueidentifier,
                                  GlobalId uniqueidentifier,
                                  TransactionDate datetime2(7),
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
                                  RedeemedDealId uniqueidentifier null)
as
begin
    declare @earnBurnTransactions table (TransactionId uniqueidentifier,
                                         UserId int,
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
                                         ReviewStatusId int,
                                         RedeemedDealId uniqueidentifier null);
    declare @filteredEarnBurnTransactions table (TransactionId uniqueidentifier,
                                                 UserId int,
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
                                                 ReviewStatusId int,
                                                 RedeemedDealId uniqueidentifier null);

    --*****************************************
    -- Visa
    --*****************************************

    -- First, collate the Authorizations and RedeemedDeals records for this user.
    with matches (TransactionId, UserId, EarnCredit, BurnDebit, TransactionType, AuthPartnerTransactionId, SetPartnerTransactionId, TransactionDate, TransactionStatusId, 
                  DealSummary, DealPercent, MerchantName, TransactionAmount, Reversed, LastFourDigits, CardBrand, PermaPending, ReviewStatusId, RedeemedDealId)
    as
    (
      select AuthorizedVisaTransactions.TransactionId, AuthorizedVisaTransactions.UserId, RedeemedVisaTransactions.EarnCredit, RedeemedVisaTransactions.BurnDebit,
             AuthorizedVisaTransactions.TransactionType, AuthorizedVisaTransactions.PartnerTransactionId, RedeemedVisaTransactions.PartnerTransactionId,
             AuthorizedVisaTransactions.TransactionDate, RedeemedVisaTransactions.TransactionStatusId, AuthorizedVisaTransactions.DealSummary,
             AuthorizedVisaTransactions.DealPercent, AuthorizedVisaTransactions.MerchantName, RedeemedVisaTransactions.TransactionAmount,
             RedeemedVisaTransactions.Reversed, RedeemedVisaTransactions.LastFourDigits, RedeemedVisaTransactions.CardBrand, RedeemedVisaTransactions.PermaPending,
             RedeemedVisaTransactions.ReviewStatusId, RedeemedVisaTransactions.TransactionId
      from AuthorizedVisaTransactions with (noexpand)
       join RedeemedVisaTransactions with (noexpand)
        on RedeemedVisaTransactions.UserId = AuthorizedVisaTransactions.UserId and
        RedeemedVisaTransactions.TransactionType = AuthorizedVisaTransactions.TransactionType and
        RedeemedVisaTransactions.PartnerTransactionId = AuthorizedVisaTransactions.PartnerTransactionId
    )

    insert into @earnBurnTransactions

    -- Next, include all the matches in the results for transactions that have not ended in failure, indicating that they all have records within the RedeemedDeals table.
    select matches.TransactionId, matches.UserId, matches.TransactionDate, matches.EarnCredit, matches.BurnDebit, HasRedeemedDealRecord = 1, matches.TransactionType,
           matches.DealSummary, matches.DealPercent, matches.MerchantName, matches.TransactionAmount, matches.Reversed, matches.TransactionStatusId,
           matches.LastFourDigits, matches.CardBrand, matches.PermaPending, matches.ReviewStatusId, matches.RedeemedDealId
    from matches

    UNION ALL

    -- Then, include all transactions that are in the Authorizations table for which no corresponding RedeemedDeals record could be found. Mark these records as
    --  NOT being represented within the RedeemedDeals table.
    select AuthorizedVisaTransactions.TransactionId, AuthorizedVisaTransactions.UserId, AuthorizedVisaTransactions.TransactionDate, AuthorizedVisaTransactions.EarnCredit,
           AuthorizedVisaTransactions.BurnDebit, HasRedeemedDealRecord = 0, AuthorizedVisaTransactions.TransactionType, AuthorizedVisaTransactions.DealSummary,
           AuthorizedVisaTransactions.DealPercent, AuthorizedVisaTransactions.MerchantName, AuthorizedVisaTransactions.TransactionAmount,
           AuthorizedVisaTransactions.Reversed, AuthorizedVisaTransactions.TransactionStatusId, AuthorizedVisaTransactions.LastFourDigits,
           AuthorizedVisaTransactions.CardBrand, AuthorizedVisaTransactions.PermaPending, AuthorizedVisaTransactions.ReviewStatusId, null           
    from AuthorizedVisaTransactions with (noexpand)
     left outer join matches on matches.UserId = AuthorizedVisaTransactions.UserId and
      matches.AuthPartnerTransactionId = AuthorizedVisaTransactions.PartnerTransactionId
    where matches.UserId is null

    UNION ALL

    -- Finally, include all transactions that are in the RedeemedDeals table for which no corresponding Authorizations record could be found. Mark these records as
    --  being represented within the RedeemedDeals table.
    select RedeemedVisaTransactions.TransactionId, RedeemedVisaTransactions.UserId, RedeemedVisaTransactions.TransactionDate, RedeemedVisaTransactions.EarnCredit,
           RedeemedVisaTransactions.BurnDebit, HasRedeemedDealRecord = 1, RedeemedVisaTransactions.TransactionType, RedeemedVisaTransactions.DealSummary,
           RedeemedVisaTransactions.DealPercent, RedeemedVisaTransactions.MerchantName, RedeemedVisaTransactions.TransactionAmount, RedeemedVisaTransactions.Reversed,
           RedeemedVisaTransactions.TransactionStatusId, RedeemedVisaTransactions.LastFourDigits, RedeemedVisaTransactions.CardBrand,
           RedeemedVisaTransactions.PermaPending, RedeemedVisaTransactions.ReviewStatusId, RedeemedVisaTransactions.TransactionId
    from RedeemedVisaTransactions with (noexpand)
     left outer join matches on matches.UserId = RedeemedVisaTransactions.UserId and
      matches.SetPartnerTransactionId = RedeemedVisaTransactions.PartnerTransactionId
    where matches.UserId is null;

    --*****************************************
    -- End Visa
    --*****************************************

    --*****************************************
    -- MasterCard
    --*****************************************

    -- First, use the record matching heuristic to collate the Authorizations and RedeemedDeals records for this user.
    with likelyMatches (TransactionId, UserId, EarnCredit, BurnDebit, TransactionType, AuthBankCustomerNumber, AuthBankNetRefNumber, AuthTransactionDate,
                        SetBankCustomerNumber, SetBankNetRefNumber, SetTransactionDate, TransactionStatusId, DealSummary, DealPercent, MerchantName, TransactionAmount,
                        Reversed, LastFourDigits, CardBrand, PermaPending, ReviewStatusId, RedeemedDealId)
    as
    (
      -- No algorithm is possible with the data MasterCard sends via their API calls and files. The following is only a heuristic, but it's almost always correct.
      select AuthorizedMasterCardTransactions.TransactionId, AuthorizedMasterCardTransactions.UserId, RedeemedMasterCardTransactions.EarnCredit,
             RedeemedMasterCardTransactions.BurnDebit, AuthorizedMasterCardTransactions.TransactionType, AuthorizedMasterCardTransactions.BankCustomerNumber,
             AuthorizedMasterCardTransactions.BankNetRefNumber, AuthorizedMasterCardTransactions.TransactionDate, RedeemedMasterCardTransactions.BankCustomerNumber,
             RedeemedMasterCardTransactions.BankNetRefNumber, RedeemedMasterCardTransactions.TransactionDate, RedeemedMasterCardTransactions.TransactionStatusId,
             AuthorizedMasterCardTransactions.DealSummary, AuthorizedMasterCardTransactions.DealPercent, AuthorizedMasterCardTransactions.MerchantName,
             RedeemedMasterCardTransactions.TransactionAmount, RedeemedMasterCardTransactions.Reversed, RedeemedMasterCardTransactions.LastFourDigits, 
             RedeemedMasterCardTransactions.CardBrand, RedeemedMasterCardTransactions.PermaPending, RedeemedMasterCardTransactions.ReviewStatusId,
             RedeemedMasterCardTransactions.TransactionId
      from dbo.AuthorizedMasterCardTransactions with (noexpand)
       join dbo.RedeemedMasterCardTransactions with (noexpand) on RedeemedMasterCardTransactions.UserId = AuthorizedMasterCardTransactions.UserId and
        RedeemedMasterCardTransactions.TransactionType = AuthorizedMasterCardTransactions.TransactionType and
        RedeemedMasterCardTransactions.BankCustomerNumber = AuthorizedMasterCardTransactions.BankCustomerNumber and
        RedeemedMasterCardTransactions.BankNetRefNumber = AuthorizedMasterCardTransactions.BankNetRefNumber
    )

    insert into @earnBurnTransactions

    -- Next, include all the likely matches in the results for transactions that have not ended in failure and indicate that they all have records within the RedeemedDeals table.
    select likelyMatches.TransactionId, likelyMatches.UserId, TransactionDate = likelyMatches.AuthTransactionDate, likelyMatches.EarnCredit, likelyMatches.BurnDebit,
           HasRedeemedDealRecord = 1, likelyMatches.TransactionType, likelyMatches.DealSummary, likelyMatches.DealPercent, likelyMatches.MerchantName,
           likelyMatches.TransactionAmount, likelyMatches.Reversed, likelyMatches.TransactionStatusId, likelyMatches.LastFourDigits, likelyMatches.CardBrand,
           likelyMatches.PermaPending, likelyMatches.ReviewStatusId, likelyMatches.RedeemedDealId
    from likelyMatches

    UNION ALL

    -- Then, include all transactions that are in the Authorizations table for which no corresponding RedeemedDeals record could be found. Mark these records as
    --  NOT being represented within the RedeemedDeals table.
    select AuthorizedMasterCardTransactions.TransactionId, AuthorizedMasterCardTransactions.UserId, AuthorizedMasterCardTransactions.TransactionDate,
           AuthorizedMasterCardTransactions.EarnCredit, AuthorizedMasterCardTransactions.BurnDebit, HasRedeemedDealRecord = 0,
           AuthorizedMasterCardTransactions.TransactionType, AuthorizedMasterCardTransactions.DealSummary, AuthorizedMasterCardTransactions.DealPercent,
           AuthorizedMasterCardTransactions.MerchantName, AuthorizedMasterCardTransactions.TransactionAmount, AuthorizedMasterCardTransactions.Reversed,
           AuthorizedMasterCardTransactions.TransactionStatusId, AuthorizedMasterCardTransactions.LastFourDigits, AuthorizedMasterCardTransactions.CardBrand,
           AuthorizedMasterCardTransactions.PermaPending, AuthorizedMasterCardTransactions.ReviewStatusId, null
    from dbo.AuthorizedMasterCardTransactions with (noexpand)
     left outer join likelyMatches on likelyMatches.UserId = AuthorizedMasterCardTransactions.UserId and
      likelyMatches.AuthBankCustomerNumber = AuthorizedMasterCardTransactions.BankCustomerNumber and
      likelyMatches.AuthBankNetRefNumber = AuthorizedMasterCardTransactions.BankNetRefNumber
    where likelyMatches.UserId is null

    UNION ALL

    -- Finally, include all transactions that are in the RedeemedDeals table for which no corresponding Authorizations record could be found. Mark these records as
    --  being represented within the RedeemedDeals table.
    select RedeemedMasterCardTransactions.TransactionId, RedeemedMasterCardTransactions.UserId, RedeemedMasterCardTransactions.TransactionDate,
           RedeemedMasterCardTransactions.EarnCredit, RedeemedMasterCardTransactions.BurnDebit, HasRedeemedDealRecord = 1, RedeemedMasterCardTransactions.TransactionType,
           RedeemedMasterCardTransactions.DealSummary, RedeemedMasterCardTransactions.DealPercent, RedeemedMasterCardTransactions.MerchantName,
           RedeemedMasterCardTransactions.TransactionAmount, RedeemedMasterCardTransactions.Reversed, RedeemedMasterCardTransactions.TransactionStatusId,
           RedeemedMasterCardTransactions.LastFourDigits, RedeemedMasterCardTransactions.CardBrand, RedeemedMasterCardTransactions.PermaPending,
           RedeemedMasterCardTransactions.ReviewStatusId, RedeemedMasterCardTransactions.TransactionId
    from dbo.RedeemedMasterCardTransactions with (noexpand)
     left outer join likelyMatches on likelyMatches.UserId = RedeemedMasterCardTransactions.UserId and
      likelyMatches.SetBankCustomerNumber = RedeemedMasterCardTransactions.BankCustomerNumber and
      likelyMatches.SetBankNetRefNumber = RedeemedMasterCardTransactions.BankNetRefNumber
    where likelyMatches.UserId is null

    --*****************************************
    -- End MasterCard
    --*****************************************

    --*****************************************
    -- First Data
    --*****************************************

    insert into @earnBurnTransactions

    select TransactionId, UserId, TransactionDate, EarnCredit, BurnDebit, HasRedeemedDealRecord = 1, TransactionType, DealSummary, DealPercent, MerchantName,
           TransactionAmount, Reversed, TransactionStatusId, LastFourDigits, CardBrand, PermaPending, ReviewStatusId, TransactionId
    from dbo.FirstDataTransactions with (noexpand)

    --*****************************************
    -- End First Data
    --*****************************************

    --*****************************************
    -- Rewards and Customer Service Grants
    --*****************************************

    insert into @earnBurnTransactions

    select TransactionId, UserId, TransactionDate, EarnCredit, BurnDebit, HasRedeemedDealRecord = 0, TransactionType, DealSummary, DealPercent, MerchantName,
           TransactionAmount, Reversed, TransactionStatusId, LastFourDigits, CardBrand, 0, 0, TransactionId
    from dbo.EarnRewardsAndGrants with (noexpand);

    --*****************************************
    -- End Rewards and Customer Service Grants
    --*****************************************

    --*****************************************
    -- Begin result collation
    --*****************************************

    with noDuplicates as
    (
        select UserId, TransactionDate, EarnCredit, BurnDebit, HasRedeemedDealRecord, TransactionType, DealSummary, DealPercent, MerchantName, TransactionAmount, Reversed,
               TransactionStatusId, LastFourDigits, CardBrand, PermaPending, ReviewStatusId, count_big(*) as RecordCount
        from @earnBurnTransactions as EarnBurnTransactions
        group by UserId, TransactionDate, EarnCredit, BurnDebit, HasRedeemedDealRecord, TransactionType, DealSummary, DealPercent, MerchantName, TransactionAmount, Reversed,
                 TransactionStatusId, LastFourDigits, CardBrand, PermaPending, ReviewStatusId
    )

    insert into @filteredEarnBurnTransactions

    select earnBurnTransactions.TransactionId, NoDuplicates.UserId, noDuplicates.TransactionDate, noDuplicates.EarnCredit, noDuplicates.BurnDebit,
           noDuplicates.HasRedeemedDealRecord, noDuplicates.TransactionType, noDuplicates.DealSummary, noDuplicates.DealPercent, noDuplicates.MerchantName,
           noDuplicates.TransactionAmount, noDuplicates.Reversed, noDuplicates.TransactionStatusId, noDuplicates.LastFourDigits, noDuplicates.CardBrand,
           noDuplicates.PermaPending, noDuplicates.ReviewStatusId, earnBurnTransactions.RedeemedDealId
    from noDuplicates
      join @earnBurnTransactions as earnBurnTransactions on earnBurnTransactions.UserId = noDuplicates.UserId and 
           earnBurnTransactions.TransactionDate = noDuplicates.TransactionDate and  earnBurnTransactions.EarnCredit = noDuplicates.EarnCredit and
           earnBurnTransactions.BurnDebit = noDuplicates.BurnDebit and earnBurnTransactions.HasRedeemedDealRecord = noDuplicates.HasRedeemedDealRecord and
           earnBurnTransactions.TransactionType = noDuplicates.TransactionType and earnBurnTransactions.DealSummary = noDuplicates.DealSummary and
           earnBurnTransactions.DealPercent = noDuplicates.DealPercent and earnBurnTransactions.MerchantName = noDuplicates.MerchantName and
           earnBurnTransactions.TransactionAmount = noDuplicates.TransactionAmount and earnBurnTransactions.Reversed = noDuplicates.Reversed and
           earnBurnTransactions.TransactionStatusId = noDuplicates.TransactionStatusId and earnBurnTransactions.LastFourDigits = noDuplicates.LastFourDigits and
           earnBurnTransactions.CardBrand = noDuplicates.CardBrand and earnBurnTransactions.PermaPending = noDuplicates.PermaPending and
           earnBurnTransactions.ReviewStatusId = noDuplicates.ReviewStatusId
    where noDuplicates.RecordCount = 1
    order by TransactionStatusId asc, TransactionDate desc;

    with resolvedDuplicates as
    (
        select UserId, TransactionDate, EarnCredit, BurnDebit, HasRedeemedDealRecord, TransactionType, DealSummary, DealPercent, MerchantName, TransactionAmount, Reversed,
               TransactionStatusId, LastFourDigits, CardBrand, PermaPending, ReviewStatusId, RedeemedDealId, count_big(*) as RecordCount
        from @earnBurnTransactions as EarnBurnTransactions
        group by UserId, TransactionDate, EarnCredit, BurnDebit, HasRedeemedDealRecord, TransactionType, DealSummary, DealPercent, MerchantName, TransactionAmount, Reversed,
                 TransactionStatusId, LastFourDigits, CardBrand, PermaPending, ReviewStatusId, RedeemedDealId
    )

    insert into @filteredEarnBurnTransactions

    select TransactionId =
      (
          select top 1 TransactionId from @earnBurnTransactions earnBurnTransactions
          where earnBurnTransactions.UserId = resolvedDuplicates.UserId and earnBurnTransactions.TransactionDate = resolvedDuplicates.TransactionDate and 
                earnBurnTransactions.EarnCredit = resolvedDuplicates.EarnCredit and earnBurnTransactions.BurnDebit = resolvedDuplicates.BurnDebit and
                earnBurnTransactions.HasRedeemedDealRecord = resolvedDuplicates.HasRedeemedDealRecord and
                earnBurnTransactions.TransactionType = resolvedDuplicates.TransactionType and earnBurnTransactions.DealSummary = resolvedDuplicates.DealSummary and
                earnBurnTransactions.DealPercent = resolvedDuplicates.DealPercent and earnBurnTransactions.MerchantName = resolvedDuplicates.MerchantName and
                earnBurnTransactions.TransactionAmount = resolvedDuplicates.TransactionAmount and earnBurnTransactions.Reversed = resolvedDuplicates.Reversed and
                earnBurnTransactions.TransactionStatusId = resolvedDuplicates.TransactionStatusId and
                earnBurnTransactions.LastFourDigits = resolvedDuplicates.LastFourDigits and earnBurnTransactions.CardBrand = resolvedDuplicates.CardBrand and
                earnBurnTransactions.PermaPending = resolvedDuplicates.PermaPending and earnBurnTransactions.ReviewStatusId = resolvedDuplicates.ReviewStatusId and
                earnBurnTransactions.RedeemedDealId = resolvedDuplicates.RedeemedDealId
      ),
      resolvedDuplicates.UserId, resolvedDuplicates.TransactionDate, resolvedDuplicates.EarnCredit, resolvedDuplicates.BurnDebit, resolvedDuplicates.HasRedeemedDealRecord,
      resolvedDuplicates.TransactionType, resolvedDuplicates.DealSummary, resolvedDuplicates.DealPercent, resolvedDuplicates.MerchantName, resolvedDuplicates.TransactionAmount,
      resolvedDuplicates.Reversed, resolvedDuplicates.TransactionStatusId, resolvedDuplicates.LastFourDigits, resolvedDuplicates.CardBrand, resolvedDuplicates.PermaPending,
      resolvedDuplicates.ReviewStatusId, resolvedDuplicates.RedeemedDealId
    from resolvedDuplicates
    where resolvedDuplicates.RecordCount > 1
    order by TransactionStatusId asc, TransactionDate desc;

	-- Create the final table response, but exclude any transactions that yielded no Earn or Burn.
    insert into @EarnBurnLineItems
    select filteredEarnBurnTransactions.TransactionId, Users.GlobalId, filteredEarnBurnTransactions.TransactionDate, filteredEarnBurnTransactions.EarnCredit,
           filteredEarnBurnTransactions.BurnDebit, filteredEarnBurnTransactions.HasRedeemedDealRecord, filteredEarnBurnTransactions.TransactionType,
           filteredEarnBurnTransactions.DealSummary, filteredEarnBurnTransactions.DealPercent, filteredEarnBurnTransactions.MerchantName,
           filteredEarnBurnTransactions.TransactionAmount, filteredEarnBurnTransactions.Reversed, filteredEarnBurnTransactions.TransactionStatusId,
           filteredEarnBurnTransactions.LastFourDigits, filteredEarnBurnTransactions.CardBrand, filteredEarnBurnTransactions.PermaPending,
           filteredEarnBurnTransactions.ReviewStatusId, filteredEarnBurnTransactions.RedeemedDealId
    from @filteredEarnBurnTransactions as filteredEarnBurnTransactions
    join dbo.Users on Users.Id = filteredEarnBurnTransactions.UserId;
	
    --*****************************************
    -- End result collation
    --*****************************************

    return
end

GO
