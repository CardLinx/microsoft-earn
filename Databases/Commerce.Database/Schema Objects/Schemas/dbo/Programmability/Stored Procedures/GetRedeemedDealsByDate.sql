--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--

CREATE PROCEDURE [dbo].[GetRedeemedDealsByDate]
    @startDateTimeInclusive datetime,
    @endDateTimeExclusive datetime
AS
SET NOCOUNT ON
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'GetRedeemedDealsByDate'

    SELECT DISTINCT        
        MN.Name as MerchantName,
        RD.Id as RedemptionId,
        RD.PurchaseDateTime as TransactionDate,
        RD.AuthorizationAmount as TransactionAmount,
        C.LastFourDigits as CardLastFourDigits,
        C.CardBrand as CardBrand,
        D.GlobalId as DealId,
        D.ProviderId as ProviderId
    FROM dbo.TransactionLinks CD
    JOIN dbo.Offers D ON D.Id = CD.DealId
    JOIN dbo.Cards C ON C.Id = CD.CardId
    JOIN dbo.RedeemedDeals RD ON RD.ClaimedDealId = CD.Id
    JOIN dbo.Merchants MN ON MN.Id = RD.MerchantNameId
    WHERE RD.DateAdded >= @startDateTimeInclusive AND RD.DateAdded < @endDateTimeExclusive AND D.OfferType = 0 -- Earn

SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
EXECUTE dbo.LogEvent @Process=@SP,@Status='Info',@Milliseconds=@Milliseconds

GO