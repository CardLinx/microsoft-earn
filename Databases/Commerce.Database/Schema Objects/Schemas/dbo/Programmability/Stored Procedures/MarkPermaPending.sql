--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
create procedure dbo.MarkPermaPending @transactionId uniqueidentifier,
                                      @tableIdentifier int,
                                      @permaPending bit as
set nocount on;

if (@tableIdentifier = 0)
begin
    update dbo.Authorizations set PermaPending = @permaPending where Id = @transactionId;
end
else if (@tableIdentifier = 1)
begin
    -- Get the RedeemedDealId that corresponds to the specified transaction ID.
    declare @redeemedDealId uniqueidentifier = (select RedeemedDealId from dbo.QueryEarnBurnLineItems() where TransactionId = @transactionId);
    update dbo.RedeemedDeals set PermaPending = @permaPending where Id = @redeemedDealId;
end

select @@rowcount as Updated;

GO