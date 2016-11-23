--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
USE [Earn.Reporting]
GO

/****** Object:  View [dbo].[EarnUsersView]    Script Date: 3/3/2016 9:57:21 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[EarnUsersView] AS

SELECT 
	Users.MsId AS Puid, 
	Users.Email, 
	ISNULL(Commerce.ActiveCardsCount, 0) AS ActiveCardsCount, 
	Commerce.UtcAdded AS EnrolledDate,
	(SELECT Max(v) FROM (VALUES (Commerce.UtcAdded), (Commerce.LastEarnTransactionDate), (Commerce.LastBurnTransactionDate), (Commerce.LastCardAddedDate)) AS value(v)) AS LastActivityDate,
	NULL AS LastWebsiteVisitDate,
	Commerce.EarnBalance,
	Commerce.EarnTransactionsCount,
	Commerce.LastEarnTransactionDate,
	Commerce.BurnTransactionsCount,
	Commerce.LastBurnTransactionDate,
	ISNULL(Commerce.PendingReferralsCount, 0) AS PendingReferralsCount,
	ISNULL(Commerce.PaidReferralsCount, 0) AS PaidReferralsCount,
	Commerce.LastPaidReferralDate,
	'https://earn.microsoft.com/?borrfc=' + Commerce.ReferralCode AS ReferralLink,
	NULL AS GeoState,
	NULL AS GeoCity,	
	NULL AS GeoLatitude,
	NULL AS GeoLongitude
	--(SELECT Max(v) FROM (VALUES (Commerce.LastVisaTransactionDate), (Commerce.LastMcTransactionDate), (Commerce.LastMcTransactionDate)) AS value(v)) as LastTransactionDate
FROM [Users].[Lomo.Users].[dbo].Users Users WITH (NOLOCK)
RIGHT OUTER JOIN  
	(SELECT 
		U.GlobalId AS Id, 
		U.UtcAdded AS UtcAdded,
		C.ActiveCardsCount,
		C.LastCardAddedDate,
		SUM(ISNULL(VIB.BurnCount, 0) + ISNULL(MCB.BurnCount, 0) + ISNULL(FDB.BurnCount, 0)) AS BurnTransactionsCount,
		SUM(ISNULL(VIB.Burn, 0) + ISNULL(MCB.Burn, 0) + ISNULL(FDB.Burn, 0)) AS BurnTotal,
		(SELECT Max(v) FROM (VALUES (VIB.TransactionDate), (MCB.TransactionDate), (FDB.TransactionDate)) AS value(v)) as LastBurnTransactionDate,
		SUM(ISNULL(VIE.EarnCount, 0) + ISNULL(MCE.EarnCount, 0)) AS EarnTransactionsCount,
		SUM(ISNULL(VIE.Earn, 0) + ISNULL(MCE.Earn, 0)) AS EarnTotal,
		(SELECT Max(v) FROM (VALUES (VIE.TransactionDate), (MCE.TransactionDate), (RWE.TransactionDate)) AS value(v)) as LastEarnTransactionDate,
		SUM(ISNULL(VIE.Earn, 0) + ISNULL(MCE.Earn, 0) + ISNULL(RWE.Earn, 0) - ISNULL(VIB.Burn, 0) - ISNULL(MCB.Burn, 0) - ISNULL(FDB.Burn, 0)) AS EarnBalance,
		RPD.PaidReferralsCount,
		RPD.LastPaidReferralDate,
		RPD.ReferralCode,
		RPN.PendingReferralsCount	
	FROM [Commerce].[Commerce].[dbo].Users U WITH (NOLOCK)	
	LEFT OUTER JOIN (SELECT UserId, COUNT(UserId) AS ActiveCardsCount, MAX(UtcAdded) AS LastCardAddedDate FROM [Commerce].[Commerce].[dbo].Cards WITH (NOLOCK) WHERE Active = 1 GROUP BY UserId) C ON C.UserId = U.Id	
	LEFT OUTER JOIN (SELECT UserId, MAX(TransactionDate) AS TransactionDate, SUM(BurnDebit) AS Burn, COUNT(UserId) AS BurnCount FROM [Commerce].[Commerce].[dbo].RedeemedVisaTransactions WITH (NOEXPAND) WHERE BurnDebit <> 0 GROUP BY UserId) VIB ON VIB.UserId = U.Id
	LEFT OUTER JOIN (SELECT UserId, MAX(TransactionDate) AS TransactionDate, SUM(EarnCredit) AS Earn, COUNT(UserId) AS EarnCount FROM [Commerce].[Commerce].[dbo].RedeemedVisaTransactions WITH (NOEXPAND) WHERE EarnCredit <> 0 GROUP BY UserId) VIE ON VIE.UserId = U.Id
	LEFT OUTER JOIN (SELECT UserId, MAX(TransactionDate) AS TransactionDate, SUM(BurnDebit) AS Burn, COUNT(UserId) AS BurnCount FROM [Commerce].[Commerce].[dbo].RedeemedMasterCardTransactions WITH (NOEXPAND) WHERE BurnDebit <> 0 GROUP BY UserId) MCB ON MCB.UserId = U.Id
	LEFT OUTER JOIN (SELECT UserId, MAX(TransactionDate) AS TransactionDate, SUM(EarnCredit) AS Earn, COUNT(UserId) AS EarnCount FROM [Commerce].[Commerce].[dbo].RedeemedMasterCardTransactions WITH (NOEXPAND) WHERE EarnCredit <> 0 GROUP BY UserId) MCE ON MCE.UserId = U.Id
	LEFT OUTER JOIN (SELECT UserId, MAX(TransactionDate) AS TransactionDate, SUM(BurnDebit) AS Burn, COUNT(UserId) AS BurnCount FROM [Commerce].[Commerce].[dbo].FirstDataTransactions WITH (NOEXPAND) WHERE BurnDebit <> 0 GROUP BY UserId) FDB ON FDB.UserId = U.Id
	--LEFT OUTER JOIN (SELECT UserId, MAX(TransactionDate) AS TransactionDate, SUM(EarnCredit) AS Earn FROM [Commerce].[Commerce].[dbo].FirstDataTransactions WITH (NOEXPAND) WHERE EarnCredit <> 0 GROUP BY UserId) FDE ON FDE.UserId = U.Id	
	LEFT OUTER JOIN (SELECT UserId, MAX(TransactionDate) AS TransactionDate, SUM(EarnCredit) AS Earn FROM [Commerce].[Commerce].[dbo].EarnRewardsAndGrants WITH (NOEXPAND) WHERE EarnCredit <> 0 GROUP BY UserId) RWE ON RWE.UserId = U.Id	
	LEFT OUTER JOIN (SELECT A.PayeeId, Count(A.PayeeId) AS PaidReferralsCount, MAX(A.PayoutFinalizedDateUtc) AS LastPaidReferralDate, B.code AS ReferralCode from [Commerce].[Commerce].[dbo].RewardPayouts A
						JOIN [Commerce].[Commerce].[dbo].ReferralTypes B on A.PayeeId = B.ReferrerId AND A.RewardId = '34246645-73EB-4D46-BED1-039C4447E22F' AND A.RewardReasonId = 0 AND A.RewardPayoutStatusId = 2
						GROUP by A.PayeeId, B.code)	RPD ON RPD.PayeeId = U.GlobalId 
	LEFT OUTER JOIN (SELECT A.PayeeId, Count(A.PayeeId) AS PendingReferralsCount from [Commerce].[Commerce].[dbo].RewardPayouts A
						WHERE A.RewardId = '34246645-73EB-4D46-BED1-039C4447E22F' AND RewardReasonId = 0 AND A.RewardPayoutStatusId = 0
						GROUP by A.PayeeId)	RPN ON RPN.PayeeId = U.GlobalId 
	GROUP BY U.GlobalId, U.UtcAdded, 
		C.ActiveCardsCount, C.LastCardAddedDate, 
		VIB.TransactionDate, MCB.TransactionDate, FDB.TransactionDate, 
		VIE.TransactionDate, MCE.TransactionDate, RWE.TransactionDate, 
		RPD.PaidReferralsCount, RPD.LastPaidReferralDate, RPD.ReferralCode,
		RPN.PendingReferralsCount) 
Commerce ON Commerce.Id = Users.Id
WHERE Users.Email IS NOT NULL AND Users.MsId IS NOT NULL AND Users.MsId NOT LIKE 'fb-%'


GO

