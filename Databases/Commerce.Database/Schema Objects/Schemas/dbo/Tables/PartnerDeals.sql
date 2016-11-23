--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE PartnerDeals
(
   PartnerId int NOT NULL
  ,PartnerDealId nvarchar(255) NOT NULL
  ,DealId int NOT NULL
  ,PartnerDealRegistrationStatusId int NOT NULL

   CONSTRAINT PKC_PartnerDeals_DealId_PartnerId PRIMARY KEY CLUSTERED (DealId, PartnerId)
  ,CONSTRAINT U_PartnerDeals_PartnerId_PartnerDealId UNIQUE (PartnerId, PartnerDealId)
	
  ,CONSTRAINT FK_PartnerDeals_DealId_Deals_Id FOREIGN KEY (DealId) REFERENCES Deals (Id)
  ,CONSTRAINT FK_PartnerDeals_PartnerId_Partners_Id FOREIGN KEY (PartnerId) REFERENCES Partners (Id)
  ,CONSTRAINT FK_PartnerDeals_PartnerDealRegistrationStatusId_PartnerDealRegistrationStatus_Id FOREIGN KEY (PartnerDealRegistrationStatusId) REFERENCES PartnerDealRegistrationStatus (Id)
)
GO
CREATE INDEX IX_PartnerId_PartnerDealId_DealId ON PartnerDeals (PartnerId, PartnerDealId, DealId)
GO