--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
create procedure dbo.ResolveTransactionReview @transactionId uniqueidentifier,
                                              @tableIdentifier int,
                                              @accept bit as
set nocount on;

if (@tableIdentifier = 0)
begin
    if (@accept = 1)
    begin
        update dbo.Authorizations set ReviewStatusId = 2 where Id = @transactionId;
    end
    else
    begin
        update dbo.Authorizations
        set ReviewStatusId = 3, PermaPending = 1 -- ResolvedReject, flag PermaPending
        where Id = @transactionId;
    end
end
else if (@tableIdentifier = 1)
begin
    -- Get the RedeemedDealId that corresponds to the specified transaction ID and determine if it was an Earn transaction.
    declare @redeemedDealId uniqueidentifier;
    declare @earnCredit int;
    select @redeemedDealId = RedeemedDealId, @earnCredit = EarnCredit
    from dbo.QueryEarnBurnLineItems()
    where TransactionId = @transactionId;

    if (@accept = 1)
    begin
        -- Because there is no futher processing for Earn transactions, mark them with CreditStatus SettledAsRedeemed. Otherwise, leave the credit status unchanged
        --  so the pipeline will pick the transaction up again.
        update dbo.RedeemedDeals
        set ReviewStatusId = 2, CreditStatusId = (case when @earnCredit > 0 then 500 else CreditStatusId end) -- ResolvedAccept, CreditGranted -OR- current status.
        where Id = @redeemedDealId;
    end
    else
    begin
        update dbo.RedeemedDeals
        set ReviewStatusId = 3, CreditStatusId = 520 -- ResolvedReject, RejectedAfterReview
        where Id = @redeemedDealId;
    end
end

select @@rowcount as Updated;

GO