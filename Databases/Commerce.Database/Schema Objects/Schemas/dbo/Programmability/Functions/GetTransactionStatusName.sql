--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- GetTransactionStatusName.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- GetTransactionStatusName
--  Gets the name of the transaction status for the specified enumeration value.
-- Parameters:
--  @enumValue int: The enumeration value whose name to retrieve.
-- Returns:
--  * The name of the transaction status for the specified enumeration value if successful.
--  * Else returns null.
create function dbo.GetTransactionStatusName(@enumValue int) returns varchar(100) as
  begin
    return case
             when @enumValue = 0 then 'AuthorizationReceived'
             when @enumValue = 5 then 'ClearingReceived'
             when @enumValue = 10 then 'GeneratingStatementCreditRequest'
             when @enumValue = 15 then 'SendingStatementCreditRequest'
             when @enumValue = 20 then 'StatementCreditRequested'
             when @enumValue = 25 then 'RetryingAfterGeneratingStatementCreditRequestFailure'
             when @enumValue = 500 then 'CreditGranted'
             when @enumValue = 505 then 'NoEarnBalanceToBurn'
             when @enumValue = 510 then 'RejectedByPartner'
             when @enumValue = 515 then 'SettlementAmountTooSmall'
             when @enumValue = 520 then 'RejectedAfterReview'
             when @enumValue = 525 then 'GeneratingStatementCreditRequestFailed'
             when @enumValue = 530 then 'SendingStatementCreditRequestFailed'
           end;
  end
GO