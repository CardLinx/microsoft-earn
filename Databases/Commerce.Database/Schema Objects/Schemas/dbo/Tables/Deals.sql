--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE Deals
(
   Id int NOT NULL IDENTITY(1000000000,1)
  ,GlobalId uniqueidentifier NOT NULL
  ,ProviderId varchar(100) NOT NULL
  ,MerchantId varchar(100) NOT NULL
  ,MerchantCategory varchar(100) NOT NULL
  ,StartDate datetime2 NOT NULL
  ,EndDate datetime2 NOT NULL
  ,Currency varchar(5) NOT NULL
  ,ReimbursementTenderId int NOT NULL DEFAULT 0
  ,Amount int NOT NULL
  ,[Percent] money NOT NULL
  ,MinimumPurchase int NOT NULL
  ,Count int NOT NULL
  ,UserLimit int NOT NULL
  ,DiscountSummary nvarchar(100) NULL
  ,MaximumDiscount int NOT NULL
  ,ParentDealId uniqueidentifier NULL
  ,DateIngestedUtc datetime NULL DEFAULT GETUTCDATE()
  ,DealStatusId int NOT NULL
  ,MerchantNameId int NULL	
  ,DayTimeRestrictions xml NULL
  
   CONSTRAINT PKC_Deals_Id PRIMARY KEY CLUSTERED (Id)
  ,CONSTRAINT U_Deals_GlobalId UNIQUE (GlobalId)
  ,CONSTRAINT FK_Deals_ReimbursementTenderId_ReimbursementTender_Id FOREIGN KEY (ReimbursementTenderId) REFERENCES ReimbursementTender (Id)
  ,CONSTRAINT FK_Deals_DealStatusId_DealStatus_Id FOREIGN KEY (DealStatusId) REFERENCES DealStatus (Id)
  ,CONSTRAINT FK_Deals_MerchantNameId_MerchantNames_Id FOREIGN KEY (MerchantNameId) REFERENCES MerchantNames (Id)
)
GO