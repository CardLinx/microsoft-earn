--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE PartnerCards 
(
   PartnerId int NOT NULL
  ,PartnerCardId varchar(255) NOT NULL
  ,CardId int NOT NULL
  ,PartnerCardSuffix char(2) NOT NULL

   CONSTRAINT PKC_PartnerCards_CardId_PartnerId PRIMARY KEY CLUSTERED (CardId, PartnerId)
  ,CONSTRAINT FK_PartnerCards_PartnerId_Partners_Id FOREIGN KEY (PartnerId) REFERENCES Partners (Id)
  ,CONSTRAINT FK_PartnerCards_CardId_Cards_Id FOREIGN KEY (CardId) REFERENCES Cards (Id)
)
GO
CREATE INDEX IX_PartnerId_PartnerCardId_CardId ON dbo.PartnerCards (PartnerId, PartnerCardId, CardId)
GO