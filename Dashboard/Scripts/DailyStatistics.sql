--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
USE [Earn.Reporting]
GO

/****** Object:  UserDefinedFunction [dbo].[DailyStatistics]    Script Date: 3/3/2016 9:57:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[DailyStatistics] (@day DATE) 
RETURNS @t TABLE (
    Name VARCHAR(50),
    Count VARCHAR(50)) AS
BEGIN

    DECLARE @from DATETIME = DATEADD(d, -0, DATEDIFF(d, 0, @day));
    DECLARE @to DATETIME = DATEADD(d, 1, DATEDIFF(d, 0, @day))

    INSERT @t
		SELECT 
			'New Users',
			COUNT(*)			
        FROM [Commerce].[Commerce].[dbo].Users WITH (NOLOCK)
        WHERE UtcAdded BETWEEN @from and @to   

	INSERT @t
		SELECT 
			'New Cards',
			COUNT(*)			
        FROM [Commerce].[Commerce].[dbo].Cards WITH (NOLOCK)
        WHERE UtcAdded BETWEEN @from and @to  
		
	INSERT @t
		SELECT 
			'Earn #',
			SUM(Earns.CNT)	
			FROM 
			(
				SELECT COUNT(*)	AS CNT	
				FROM [Commerce].[Commerce].[dbo].AuthorizedVisaTransactions WITH (NOEXPAND)
				WHERE TransactionDate BETWEEN @from and @to AND EarnCredit <> 0
				UNION
				SELECT COUNT(*)	AS CNT	
				FROM [Commerce].[Commerce].[dbo].AuthorizedMasterCardTransactions WITH (NOEXPAND)
				WHERE TransactionDate BETWEEN @from and @to AND EarnCredit <> 0
			) AS Earns	

	INSERT @t
		SELECT 
			'Earn $',
			'$'+ CAST(CAST(SUM(Earns.EarnCredit)/100.0 AS NUMERIC(36,2))AS VARCHAR(10))             
			FROM 
			(
				SELECT EarnCredit	
				FROM [Commerce].[Commerce].[dbo].AuthorizedVisaTransactions WITH (NOEXPAND)
				WHERE TransactionDate BETWEEN @from and @to AND EarnCredit <> 0
				UNION
				SELECT EarnCredit	
				FROM [Commerce].[Commerce].[dbo].AuthorizedMasterCardTransactions WITH (NOEXPAND)
				WHERE TransactionDate BETWEEN @from and @to AND EarnCredit <> 0
			) AS Earns	

	INSERT @t
		SELECT 
			'Burn #',
			SUM(Burns.CNT)	
			FROM 
			(
				SELECT COUNT(*)	AS CNT	
				FROM [Commerce].[Commerce].[dbo].AuthorizedVisaTransactions WITH (NOEXPAND)
				WHERE TransactionDate BETWEEN @from and @to AND BurnDebit <> 0
				UNION
				SELECT COUNT(*)	AS CNT	
				FROM [Commerce].[Commerce].[dbo].AuthorizedMasterCardTransactions WITH (NOEXPAND)
				WHERE TransactionDate BETWEEN @from and @to AND BurnDebit <> 0
			) AS Burns	

	INSERT @t
		SELECT 
			'Burn $',
			'$'+ CAST(CAST(SUM(Burns.BurnDebit)/100.0 AS NUMERIC(36,2))AS VARCHAR(10))             
			FROM 
			(
				SELECT BurnDebit	
				FROM [Commerce].[Commerce].[dbo].AuthorizedVisaTransactions WITH (NOEXPAND)
				WHERE TransactionDate BETWEEN @from and @to AND BurnDebit <> 0
				UNION
				SELECT BurnDebit	
				FROM [Commerce].[Commerce].[dbo].AuthorizedMasterCardTransactions WITH (NOEXPAND)
				WHERE TransactionDate BETWEEN @from and @to AND BurnDebit <> 0
			) AS Burns	

    RETURN;
END;

GO

