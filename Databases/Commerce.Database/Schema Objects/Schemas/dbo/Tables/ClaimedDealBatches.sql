--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE ClaimedDealBatches
(
   CardId int NOT NULL
  ,PartnerId int NOT NULL
  ,DealBatchId int NOT NULL
  ,StartDate datetime NOT NULL CONSTRAINT DF_ClaimedDealBatches_StartDate DEFAULT getUTCdate() 
  ,EndDate datetime NOT NULL CONSTRAINT DF_ClaimedDealBatches_EndDate DEFAULT 0 

   CONSTRAINT PKC_ClaimedDealBatches_CardId_PartnerId_DealBatchId PRIMARY KEY CLUSTERED (CardId, PartnerId, DealBatchId)

  ,CONSTRAINT FK_ClaimedDealBatches_DealBatchId_DealBatches_Id FOREIGN KEY (DealBatchId) REFERENCES DealBatches (Id)
  ,CONSTRAINT FK_ClaimedDealBatches_CardId_Cards_Id FOREIGN KEY (CardId) REFERENCES Cards (Id)
  ,CONSTRAINT FK_ClaimedDealBatches_PartnerId_Partners_id FOREIGN KEY (PartnerId) REFERENCES Partners (Id)
)
GO