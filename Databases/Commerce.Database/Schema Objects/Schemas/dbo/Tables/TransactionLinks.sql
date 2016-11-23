--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE TransactionLinks
(
   Id bigint NOT NULL IDENTITY(1,1)
  ,DealId int NOT NULL
  ,UserId int NOT NULL
  ,CardId int NOT NULL
  ,PartnerId int NOT NULL
  ,PartnerClaimedDealId nvarchar(255) NULL
  ,DateAdded datetime NOT NULL CONSTRAINT DF_TransactionLinks_DateAdded DEFAULT getUTCdate()

   CONSTRAINT PKC_TransactionLinks_Id PRIMARY KEY CLUSTERED (Id)

  ,CONSTRAINT FK_TransactionLinks_DealId_Offers_Id FOREIGN KEY (DealId) REFERENCES Offers (Id)
  ,CONSTRAINT FK_TransactionLinks_UserId_Users_Id FOREIGN KEY (UserId) REFERENCES Users (Id)
  ,CONSTRAINT FK_TransactionLinks_CardId_Cards_Id FOREIGN KEY (CardId) REFERENCES Cards (Id)
  ,CONSTRAINT FK_TransactionLinks_PartnerId_Partners_id FOREIGN KEY (PartnerId) REFERENCES Partners (Id)
)
GO
CREATE INDEX IX_DealId_UserId_CardId ON TransactionLinks (DealId, UserId, CardId)
GO
CREATE INDEX IX_PartnerId_PartnerClaimedDealId ON TransactionLinks (PartnerId, PartnerClaimedDealId)
GO