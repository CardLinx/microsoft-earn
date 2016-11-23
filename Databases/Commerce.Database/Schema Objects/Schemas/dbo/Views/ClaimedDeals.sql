--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE VIEW ClaimedDeals 
AS
SELECT CD.PartnerId
      ,CD.CardId
      ,B.DealId
      ,C.UserId
  FROM dbo.ClaimedDealBatches CD
       JOIN dbo.DealBatchDetails B ON B.DealBatchId = CD.DealBatchId
       LEFT OUTER JOIN dbo.Cards C ON C.Id = CD.CardId
  WHERE CD.EndDate <> 0
GO