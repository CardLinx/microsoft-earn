--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE DealBatches
(
   Id int NOT NULL IDENTITY(1,1)
  ,SumOfIds bigint NOT NULL
  ,Count int NOT NULL
  ,RegisteredDate datetime NOT NULL CONSTRAINT DF_DealBatches_RegisteredDate DEFAULT getUTCdate()

   CONSTRAINT PKC_DealBatches_Id PRIMARY KEY CLUSTERED (Id)
)
GO
CREATE INDEX IX_SumOfIds_Count ON DealBatches (SumOfIds, Count)
GO