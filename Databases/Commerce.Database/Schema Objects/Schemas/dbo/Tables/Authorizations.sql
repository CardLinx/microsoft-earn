--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE Authorizations
(
   Id uniqueidentifier NOT NULL
  ,PartnerId int NOT NULL
  ,TransactionScopeId nvarchar(255) NULL
  ,TransactionId nvarchar(255) NOT NULL
  ,TransactionDate datetime2 NOT NULL
  ,TransactionAmount int NOT NULL
  ,ClaimedDealId bigint NOT NULL
  ,DiscountAmount int NOT NULL
  ,PartnerData xml NULL
  ,DateAdded datetime NOT NULL CONSTRAINT DF_Authorizations_DateAdded DEFAULT getUTCdate()
  ,PermaPending BIT NOT NULL DEFAULT 0
  ,ReviewStatusId INT NOT NULL DEFAULT 0
  ,MerchantNameId INT NOT NULL

   CONSTRAINT PKC_Authorizations_Id PRIMARY KEY CLUSTERED (Id)
  ,CONSTRAINT U_Authorizations_PartnerId_TransactionId UNIQUE (PartnerId, TransactionId)
  ,CONSTRAINT FK_Authorizations_PartnerId_Partners_Id FOREIGN KEY (PartnerId) REFERENCES Partners (Id)
  ,CONSTRAINT FK_Authorizations_MerchantNameId_Merchants_Id FOREIGN KEY (MerchantNameId) REFERENCES Merchants (Id)
  ,CONSTRAINT FK_Authorizations_ClaimedDealId_TransactionLinks_Id FOREIGN KEY (ClaimedDealId) REFERENCES TransactionLinks (Id)
)
GO
CREATE INDEX IX_ClaimedDealId ON Authorizations (ClaimedDealId)
GO