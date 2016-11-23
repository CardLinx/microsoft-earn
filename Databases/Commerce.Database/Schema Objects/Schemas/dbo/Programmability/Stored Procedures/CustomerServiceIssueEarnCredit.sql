--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE [dbo].[CustomerServiceIssueEarnCredit]
    @userId UNIQUEIDENTIFIER,
    @amount INT,
    @explanation NVARCHAR(100),
    @issuedBy NVARCHAR(100)
AS
    -- Each credit is limited to a maximum of $10. Amount is in pennies.
    IF @amount > 1000
        RAISERROR('Credit too large.', 16, 1)
    ELSE IF @amount < -1000
        RAISERROR('Debit too large.', 16, 1)

    INSERT INTO dbo.RewardPayouts
    (
        Id,
        RewardId,
        RewardReasonId,
        PayeeId,
        PayeeTypeId,
        RewardPayoutStatusId,
        PayoutScheduledDateUtc,
        PayoutFinalizedDateUtc,
        AgentId,
        AgentTypeId,
        Amount,
        Explanation,
        IssuedBy
    )
    VALUES
    (
        NEWID(),
        'FF1C9266-D16E-4AF6-AAE0-4E770858838F', -- Customer Service Issued
        2, -- CustomerService
        @userId,
        0, -- User
        2, -- Paid
        GETUTCDATE(),
        GETUTCDATE(),
        NULL,
        NULL,
        @amount,
        @explanation,
        @issuedBy
    )
GO
