--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE DealPartnerMerchantIds
(
   PartnerId int NOT NULL
  ,PartnerMerchantId nvarchar(255) NOT NULL
  ,DealId int NOT NULL
  ,PartnerMerchantIdTypeId int NOT NULL
  ,MerchantTimeZoneId varchar(100) NULL

   CONSTRAINT PKC_DealPartnerMerchantIds_PartnerId_PartnerMerchantId_DealId PRIMARY KEY CLUSTERED (PartnerId, PartnerMerchantId, DealId)
   
  ,CONSTRAINT FK_DealPartnerMerchantIds_PartnerId_Partners_Id FOREIGN KEY (PartnerId) REFERENCES Partners (Id)
  ,CONSTRAINT FK_DealPartnerMerchantIds_DealId_Deals_Id FOREIGN KEY (DealId) REFERENCES Deals (Id)
  ,CONSTRAINT FK_DealPartnerMerchantIds_PartnerMerchantIdTypeId_PartnerMerchantIdTypes_Id FOREIGN KEY(PartnerMerchantIdTypeId) REFERENCES [dbo].[PartnerMerchantIdTypes] ([Id])
)
GO