--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE VIEW dbo.RedemptionReport
AS
SELECT RD.Id AS TransactionId,
       P.Name AS PartnerName,
       PRD.PartnerMerchantId,
       CB.Name AS CardBrand,
       C.LastFourDigits,
       D.GlobalID AS DealId,
       D.Id AS DiscountId,
       GlobalDiscountId = D.GlobalId,
       RD.PurchaseDateTime AS AuthorizationDateTimeLocal,
       AuthorizationDateTimeUtc = RD.DateAdded,
       RD.DateSettled AS SettlementDate,
       RD.DateCreditApproved AS CreditApprovalDateTimeUtc,
       'USD' AS Currency,
       RD.AuthorizationAmount,
       RD.SettlementAmount,
       RD.DiscountAmount,
       (select dbo.GetTransactionStatusName(CreditStatusId)) AS CreditStatus,
       CASE 
         WHEN CreditStatusId < 500 THEN 'Pending' -- Any non-terminal State.
         WHEN CreditStatusId = 500 THEN 'Succeeded' -- Only CreditGranted (success) terminal State. Other terminal States are not returned.
         ELSE 'Failed'
       END AS CurrentState,
       LastUpdatedDateUtc,
       RD.UtcReachedTerminalState,
       U.GlobalID as GlobalUserID,
       D.ProviderId
  FROM dbo.RedeemedDeals RD
       JOIN dbo.PartnerRedeemedDeals PRD ON PRD.RedeemedDealId = RD.Id
       JOIN dbo.TransactionLinks CD ON CD.Id = RD.ClaimedDealId
       JOIN dbo.Cards C ON C.Id = CD.CardId
       JOIN dbo.Users U ON U.Id = CD.UserId
       JOIN dbo.CardBrands CB ON CB.Id = C.CardBrand
       JOIN dbo.Partners P ON P.Id = CD.PartnerId
       JOIN dbo.Offers D ON D.Id = CD.DealId
GO