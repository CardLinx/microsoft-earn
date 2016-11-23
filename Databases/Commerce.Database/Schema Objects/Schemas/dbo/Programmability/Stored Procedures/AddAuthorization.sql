--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE AddAuthorization
  @authorizationId uniqueidentifier,
  @partnerId int,
  @recommendedPartnerDealId nvarchar(255) = NULL,
  @partnerCardId varchar(255),
  @partnerMerchantId nvarchar(255),
  @purchasedatetime datetime2(7),
  @authAmount int,
  @currency varchar(5),
  @transactionScopeId nvarchar(255) = NULL,
  @transactionId nvarchar(255),
  @partnerData xml = NULL,
  @timeZoneOffset int = NULL
AS
    set nocount on

    begin try
        -- If a record for this transaction has already been recorded, stop processing.
        if (exists(select Id from dbo.Authorizations where TransactionScopeId = @transactionScopeId))
            begin
                raiserror('DuplicateRecordDetected', 16, 1);
            end

        -- Gather card and user information.
        declare @cardId int;
        declare @lastFourDigits char(4);
        declare @userId int;
        declare @globalUserId uniqueidentifier;
        select @cardId = Cards.Id, @lastFourDigits = LastFourDigits, @userId = UserId, @globalUserId = GlobalId from dbo.Cards
          join dbo.Users on Users.Id = UserId
          where Active = 1 and PartnerToken = @partnerCardId;
        if (@cardId is null)
            begin
                raiserror('PartnerCardIdNotFound', 16, 1);
            end

        -- Determine the partner merchant ID type for this transaction.
        declare @partnerMerchantIdTypeId int = 0; -- All partners besides MasterCard use Universal partner merchant IDs.
        if (@partnerId = 4) set @partnerMerchantIdTypeId = 1; -- MasterCard uses AuthorizationOnly partner merchant IDs when this sproc is called.

        -- Determine applicable offer.
        declare @dealId int;
        declare @offerType int;
        declare @percentBack money;
        declare @merchantNameId int;
        declare @merchantName nvarchar(100);
        declare @discountAmount int;
        select @dealId = Offers.Id, @offerType = Offers.OfferType, @percentBack = PercentBack, @merchantNameId = Merchants.Id, @merchantName = Merchants.Name
          from dbo.Offers
            join dbo.Merchants on Merchants.ProviderId = Offers.ProviderId
            join dbo.PartnerMerchantAuthorizationIDs on PartnerMerchantAuthorizationIDs.MerchantId = Merchants.Id
        where AuthorizationID = @partnerMerchantID and [Partner] = @partnerId + 1 and Offers.Active = 1 and PartnerMerchantAuthorizationIDs.Active = 1;

        -- For Visa transactions, National providers are matched only on VMID. So, if this is a Visa transaction and no offer was found, look again using only the VMID.
        if (@dealId is null and @partnerId = 3)
          begin
            declare @delimiterPos int = charindex(';', @partnerMerchantId);
            if (@delimiterPos > 0)
              begin
                declare @vmid varchar(100) = left(@partnerMerchantId, @delimiterPos - 1);
                select @dealId = Offers.Id, @offerType = Offers.OfferType, @percentBack = PercentBack, @merchantNameId = Merchants.Id, @merchantName = Merchants.Name
                  from dbo.Offers
                    join dbo.Merchants on Merchants.ProviderId = Offers.ProviderId
                    join dbo.PartnerMerchantAuthorizationIDs on PartnerMerchantAuthorizationIDs.MerchantId = Merchants.Id
                where AuthorizationID = @vmid and [Partner] = @partnerId + 1 and Offers.Active = 1 and PartnerMerchantAuthorizationIDs.Active = 1;
              end
          end

        -- Determine Earn/Burn amount.
        if (@dealId is not null and (@offerType = 1 or (@percentBack is not null and @percentBack > 0))) -- Burn or Earn with a positive percent.
        begin
            -- Determine Earn or Burn amount.
            if (@offerType = 0) -- Earn
            begin
              set @discountAmount = @percentBack / 100 * @authAmount;
            end
            else if (@offerType = 1) -- Burn
            begin
              set @discountAmount = dbo.FundsToBurn(@cardId, @authAmount, @partnerId, null);
            end
        end
        if (@dealId is null or @discountAmount is null)
            begin
                raiserror('NoApplicableDealFound', 16, 1);
            end

        -- If the transaction is an Earn, process it as such.
        declare @transactionReviewStatus int = 0; --Unnecessary
        if (@offerType = 0) -- Earn
            begin
                -- See if the transaction needs to be flagged for review as potential fraud.
                if (@authAmount > 45000) -- $450.00
                    begin
                        set @transactionReviewStatus = 1; -- SuspiciousTransactionAmount
                    end

                -- Determine if the user is already over the yearly earning cap.
                -- NOTE: With datediff, SQL considers anything with the previous year (e.g. 12/31/2014 vs 1/1/2015) to be 1 year different(?!).
                --        Days yields a decent enough estimate, but datediff is an int operation, not a float operation. So, although a year is actually slightly less
                --        than 365.25 days, 365 days is as close as it's possible to get to an actual year. We're going with 366 days to give the user the benefit of that
                --        last approx. six hours.
                declare @rollingYearLimit int = 200000; -- $2000.00
                declare @yearEarnings int;
                select @yearEarnings = sum(EarnCredit) from dbo.GetEarnBurnLineItems(@userId)
                  where PermaPending = 0 and ReviewStatusId in (0, 2) and datediff(day, TransactionDate, getDate()) < 366; -- Review status Unnecessary or ResolvedAccept
                if (@yearEarnings >= @rollingYearLimit)
                    begin
                        raiserror('UserOverEarnLimit', 16, 1);
                    end
                -- Otherwise, determine if we need to prorate the earn amount to keep the user from going over the yearly cap.
                else if (@yearEarnings + @discountAmount > @rollingYearLimit)
                    begin
                        set @discountAmount = @rollingYearLimit - @yearEarnings;
                    end
            end

        -- Get the claimed deal ID if it exists. Otherwise create a new one if necessary.
        -- NOTE: This is deprecated and will be removed when possible.
        declare @partnerClaimedDealId nvarchar(255);
        declare @claimedDealId bigint;
        select @partnerClaimedDealId = PartnerClaimedDealId, @claimedDealId = Id from dbo.TransactionLinks
          where PartnerId = @partnerId and CardId = @cardId and UserId = @userId and DealId = @dealId;
        if (@claimedDealId is null)
            begin
                insert into dbo.TransactionLinks
                    (DealId, UserId, CardId, PartnerId)
                  values
                    (@dealId, @userId, @cardId, @partnerId);
                set @claimedDealId = scope_identity();
            end

        -- The transaction survived the gauntlet, so add a record for it.
        begin transaction
            insert into dbo.Authorizations
                (Id, PartnerId, TransactionScopeId, TransactionId, TransactionDate, TransactionAmount, ClaimedDealId, DiscountAmount, PartnerData, ReviewStatusId, MerchantNameId)
                values
                (@authorizationId, @partnerId, @transactionScopeId, @transactionId, @purchaseDateTime, @authAmount, @claimedDealId, @discountAmount, @partnerData,
                 @transactionReviewStatus, @merchantNameId);
        commit transaction

        -- If this was a transaction that Earned or Burned $0.00, throw an exception so no notification is sent out.
        if (@discountAmount < 1)
            begin
                raiserror('InsufficientFunds', 16, 1);
            end

        -- Gather offer information to return to the caller.
        declare @globalDealId uniqueidentifier;
        declare @amount int;
        declare @percent money;
        declare @minimumPurchase int;
        declare @parentDealId uniqueidentifier;
        declare @partnerDealId nvarchar(255);
        select @globalDealId = GlobalId, @amount = 0, @percent = PercentBack, @minimumPurchase = 0, @parentDealId = GlobalId, @partnerDealId = ''
          from dbo.Offers
          where Offers.Id = @dealId;
        declare @discountSummary nvarchar(255) = (select dbo.BuildDiscountSummary(@offerType, @percent));

        -- Send transaction information back to the caller.
        SELECT @cardId as CardId, @dealId as DealId, @globalUserId as GlobalUserId, @globalDealId as GlobalDealId, @partnerDealId as PartnerDealId, @currency as Currency,
               @amount as Amount, @percent as [Percent], @minimumPurchase as MinimumPurchase, @discountSummary as DiscountSummary, @merchantName as MerchantName,
               @lastFourDigits as LastFourDigits, @discountAmount as DiscountAmount, @parentDealId as ParentDealId, @offerType as ReimbursementTenderId,
               null as PartnerClaimedDealId;
    end try
    begin catch
      if @@trancount > 0 rollback transaction;
      declare @errorMessage nvarchar(4000) = ERROR_MESSAGE();
      declare @errorSeverity int = ERROR_SEVERITY();
      raiserror(@errorMessage, @errorSeverity, 1)
    end catch
GO