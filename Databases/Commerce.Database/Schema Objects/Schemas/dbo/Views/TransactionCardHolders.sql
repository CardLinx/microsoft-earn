--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE VIEW TransactionCardHolders
AS
SELECT DISTINCT
       TransactionId = Info.Id
      ,Info.LastUpdatedDateUtc
      ,C.UserId
      ,GlobalUserId = U.GlobalId
      ,CardCurrentlyActive = C.Active
FROM (SELECT DISTINCT
             RD.Id
            ,RD.LastUpdatedDateUtc
            ,C.CardBrand - 1 as PartnerId
            ,C.PartnerToken as PartnerCardId
        FROM dbo.Cards C
             JOIN dbo.TransactionLinks CD ON CD.CardId = C.Id
             JOIN dbo.RedeemedDeals RD ON RD.ClaimedDealId = CD.Id
      ) Info
      JOIN dbo.Cards C ON C.CardBrand = Info.PartnerId + 1 AND C.PartnerToken = Info.PartnerCardId
      JOIN dbo.Users U ON U.Id = C.UserId
GO