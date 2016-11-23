--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

-- Insert Partners if initial deployment.
IF ((SELECT COUNT(*) FROM [dbo].[Partners]) = 0)
BEGIN
	INSERT INTO [dbo].[Partners] (Name)
		SELECT 'FirstData'
	UNION ALL
		SELECT 'Amex'
	UNION ALL
		SELECT 'Visa'
	UNION ALL
		SELECT 'MasterCard'
END
GO

-- Insert redemption events if initial deployment.
IF ((SELECT COUNT(*) FROM [dbo].[RedemptionEvents]) = 0)
BEGIN
	INSERT INTO [dbo].[RedemptionEvents] (Name)
		SELECT 'RealTime'
	UNION ALL
		SELECT 'Settlement'
END
GO

-- Insert credit status values if initial deployment.
/*
IF ((SELECT COUNT(*) FROM [dbo].[CreditStatus]) = 0)
BEGIN
	INSERT INTO [dbo].[CreditStatus] (Name)
		SELECT 'Unprocessed'
	UNION ALL
		SELECT 'ReadyForSettlement'
	UNION ALL
		SELECT 'PendingSettledAsRedeemed'
	UNION ALL
		SELECT 'SettledAsRedeemed'
	UNION ALL
		SELECT 'SettledAsReversed'
	UNION ALL
		SELECT 'RejectedByPartner'
	UNION ALL
		SELECT 'SettlementAmountTooSmall'
    UNION ALL
        SELECT 'RejectedAfterReview'
END
GO
*/

-- Insert card types if initial deployment.
IF ((SELECT COUNT(*) FROM [dbo].[CardBrands]) = 0)
BEGIN
	INSERT INTO [dbo].[CardBrands] (Name)
		SELECT 'AmericanExpress'
	UNION ALL
		SELECT 'Visa'
	UNION ALL
		SELECT 'MasterCard'
END
GO

-- Insert referral vectors if initial deployment.
IF ((SELECT COUNT(*) FROM [dbo].[ReferralEvents]) = 0)
BEGIN
	INSERT INTO [dbo].[ReferralEvents] (Name)
		SELECT 'LinkClicked'
	UNION ALL
		SELECT 'Signup'
	UNION ALL
		SELECT 'Redemption'
END
GO

-- Insert referral vectors if initial deployment.
IF ((SELECT COUNT(*) FROM [dbo].[ReferralVectors]) = 0)
BEGIN
	INSERT INTO [dbo].[ReferralVectors] (Name)
		SELECT 'Unknown'
	UNION ALL
		SELECT 'Facebook'
	UNION ALL
		SELECT 'Twitter'
END
GO

-- Insert referrer types if initial deployment.
IF ((SELECT COUNT(*) FROM [dbo].[ReferrerTypes]) = 0)
BEGIN
	INSERT INTO [dbo].[ReferrerTypes] (Name)
		SELECT 'User'
	UNION ALL
		SELECT 'Merchant'
END
GO

-- Insert payee types if initial deployment.
IF ((SELECT COUNT(*) FROM [dbo].[PayeeTypes]) = 0)
BEGIN
	INSERT INTO [dbo].[PayeeTypes] (Name)
		SELECT 'User'
	UNION ALL
		SELECT 'Merchant'
END
GO

-- Insert reward payout status values if initial deployment.
IF ((SELECT COUNT(*) FROM [dbo].[RewardPayoutStatus]) = 0)
BEGIN
	INSERT INTO [dbo].[RewardPayoutStatus] (Name)
		SELECT 'Unprocessed'
	UNION ALL
		SELECT 'Pending'
	UNION ALL
		SELECT 'Paid'
	UNION ALL
		SELECT 'LimitReached'
	UNION ALL
		SELECT 'NoEligibleUser'
	UNION ALL
		SELECT 'Rescinded'
END
GO

-- Insert reward recipients if initial deployment.
IF ((SELECT COUNT(*) FROM [dbo].[RewardRecipients]) = 0)
BEGIN
	INSERT INTO [dbo].[RewardRecipients] (Name)
		SELECT 'Referrer'
END
GO

-- Insert reward reasons if initial deployment.
IF ((SELECT COUNT(*) FROM [dbo].[RewardReasons]) = 0)
BEGIN
	INSERT INTO [dbo].[RewardReasons] (Name)
		SELECT 'Referral'
	UNION ALL
		SELECT 'Redemption'
    UNION ALL
        SELECT 'CustomerService'
END
GO

-- Insert reward types if initial deployment.
IF ((SELECT COUNT(*) FROM [dbo].[RewardTypes]) = 0)
BEGIN
	INSERT INTO [dbo].[RewardTypes] (Name)
		SELECT 'Undefined'
	UNION ALL
		SELECT 'BingRewardPoints'
	UNION ALL
		SELECT 'StatementCredit'
	UNION ALL
		SELECT 'EarnCredit'
END
GO

IF ((SELECT COUNT(*) FROM [dbo].[Rewards]) = 0)
BEGIN
	INSERT INTO [dbo].[Rewards]
	VALUES
	(
		'34246645-73EB-4D46-BED1-039C4447E22F',
		3,
		'Referred User First Earn',
		1,
		'500'
	)

	INSERT INTO [dbo].[Rewards]
	VALUES
	(
		'B6087E16-B958-499D-A70D-64759B36592F',
		3,
		'First Earn reward',
		1,
		'TBD'
	)

	INSERT INTO [dbo].[Rewards]
	VALUES
	(
		'70F55EFF-62F1-4869-8CE0-333B169C5F06',
		2,
		'Evangelist referred user redemption reward',
		1,
		'1000'
	)

	INSERT INTO [dbo].[Rewards]
	VALUES
	(
		'FF1C9266-D16E-4AF6-AAE0-4E770858838F',
		3,
		'Customer Service Issued',
		1,
		'Amount Varies'
	)

	INSERT INTO [dbo].[Rewards]
	VALUES
	(
		'FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF',
		0,
		'DUMMY REWARD',
		0,
		'PROPERTIES'
	)

END
GO

-- TODO get actual values
-- Create Amex Offer Id sequence
IF NOT EXISTS (SELECT 1 FROM [dbo].[Sequences] WHERE SequenceName = 'AmexOfferIdSequence' )
BEGIN
	INSERT INTO [dbo].[Sequences] (SequenceName, Seed, Increment, CurrentValue)
    VALUES ('AmexOfferIdSequence', 903000000 , 1, 903000000)
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Sequences] WHERE SequenceName = 'AmexOfferRegistrationSequence' )
BEGIN
	INSERT INTO [dbo].[Sequences] (SequenceName, Seed, Increment, CurrentValue)
    VALUES ('AmexOfferRegistrationSequence', 1 , 1, 1)
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Sequences] WHERE SequenceName = 'AmexStatementCreditSequence' )
BEGIN
	INSERT INTO [dbo].[Sequences] (SequenceName, Seed, Increment, CurrentValue)
    VALUES ('AmexStatementCreditSequence', 1 , 1, 1)
END
GO

-- Insert registration status values if initial deployment.
IF ((SELECT COUNT(*) FROM [dbo].[PartnerDealRegistrationStatus]) = 0)
BEGIN
	INSERT INTO [dbo].[PartnerDealRegistrationStatus] (Name)
		SELECT 'Pending'
	UNION ALL
		SELECT 'Complete'
	UNION ALL
		SELECT 'Error'
	UNION ALL
		SELECT 'Activated'
END
GO

-- Insert deal status values if initial deployment.
IF ((SELECT COUNT(*) FROM [dbo].[DealStatus]) = 0)
BEGIN
	INSERT INTO [dbo].[DealStatus] (Name)
		SELECT 'PendingRegistration'
	UNION ALL
		SELECT 'PendingAutoLinking'
	UNION ALL
		SELECT 'AutoLinkingComplete'
	UNION ALL
		SELECT 'Activated'
END
GO

-- Insert reimbursement tender values if initial deployment.
IF ((SELECT COUNT(*) FROM [dbo].[ReimbursementTender]) = 0)
BEGIN
	INSERT INTO [dbo].[ReimbursementTender] (Name)
		SELECT 'DealCurrency'
	UNION ALL
		SELECT 'MicrosoftCsv'
	UNION ALL
		SELECT 'MicrosoftEarn'
	UNION ALL
		SELECT 'MicrosoftBurn'
END
GO

-- Insert partner merchant ID types values if initial deployment.
IF ((SELECT COUNT(*) FROM [dbo].[PartnerMerchantIdTypes]) = 0)
BEGIN
	INSERT INTO [dbo].[PartnerMerchantIdTypes] (Name)
		SELECT 'Universal'
	UNION ALL
		SELECT 'AuthorizationOnly'
	UNION ALL
		SELECT 'SettlementOnly'
END
GO

--Insert Reward progrma types
IF ((SELECT COUNT(*) FROM [dbo].[RewardProgram]) = 0)
BEGIN
	INSERT INTO [dbo].[RewardProgram] (Id,ProgramName)
	VALUES (1,'CardLinkOffers')

	INSERT INTO [dbo].[RewardProgram] (Id,ProgramName)
	VALUES (2,'EarnBurn')

END
GO

--Insert TransactionReviewStatus items.
IF ((SELECT COUNT(*) FROM [dbo].[TransactionReviewStatus]) = 0)
BEGIN
	INSERT INTO [dbo].[TransactionReviewStatus] (Name)
		SELECT 'Unnecessary'
	UNION ALL
		SELECT 'SuspiciousTransactionAmount'
	UNION ALL
		SELECT 'ResolvedAccept'
	UNION ALL
		SELECT 'ResolvedReject'
END
GO

--Map the ReimbursementTender Id to the Reward Program
IF ((SELECT COUNT(*) FROM [dbo].[RewardProgramReimbursementTenderIds]) = 0)
BEGIN
	INSERT INTO [dbo].[RewardProgramReimbursementTenderIds] (RewardProgramId,ReimbursementTenderId)
	VALUES (1,0)

	INSERT INTO [dbo].[RewardProgramReimbursementTenderIds] (RewardProgramId,ReimbursementTenderId)
	VALUES (2,2)

	INSERT INTO [dbo].[RewardProgramReimbursementTenderIds] (RewardProgramId,ReimbursementTenderId)
	VALUES (2,3)

END
GO


-- CleanupEventLog
INSERT INTO dbo.Parameters (Id, Char)
  SELECT Id = 'CleanupEventLogStatus', Char = 'Go'
    WHERE NOT EXISTS (SELECT * FROM dbo.Parameters WHERE Id = 'CleanupEventLogStatus')

INSERT INTO dbo.Parameters (Id, Number)
  SELECT Id = 'CleanupEventLogDeleteRows', Number = 1000
    WHERE NOT EXISTS (SELECT * FROM dbo.Parameters WHERE Id = 'CleanupEventLogDeleteRows')
 
INSERT INTO dbo.Parameters (Id, Number)
  SELECT Id = 'CleanupEventLogAllowedRows', Number = 1e6
    WHERE NOT EXISTS (SELECT * FROM dbo.Parameters WHERE Id = 'CleanupEventLogAllowedRows')

INSERT INTO dbo.Parameters (Id, Number)
  SELECT Id = 'CleanupEventLogRetentionDay', Number = 30
    WHERE NOT EXISTS (SELECT * FROM dbo.Parameters WHERE Id = 'CleanupEventLogRetentionDay')
GO
-- Drop table type
DECLARE @Name varchar(255) = 'DealPartnerMerchantIds'
IF EXISTS (SELECT * FROM sys.table_types WHERE name = @Name)
BEGIN
  PRINT 'Dropping Table Type '+@Name+'...'
  EXECUTE('DROP TYPE ['+@Name+']')
END
GO