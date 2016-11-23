--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE DealBatchDetails
(
   DealBatchId int NOT NULL
  ,DealId int NOT NULL
   
   CONSTRAINT PKC_DealBatchDetails_DealBatchId_DealId PRIMARY KEY CLUSTERED (DealBatchId, DealId)

  ,CONSTRAINT FK_DealBatchDetails_DealId_Deals_Id FOREIGN KEY (DealId) REFERENCES Deals (Id)
  ,CONSTRAINT FK_DealBatchDetails_DealBatchId_DealsBatches_Id FOREIGN KEY (DealBatchId) REFERENCES DealBatches (Id)
)
GO
CREATE INDEX IX_DealId ON DealBatchDetails (DealId)
GO