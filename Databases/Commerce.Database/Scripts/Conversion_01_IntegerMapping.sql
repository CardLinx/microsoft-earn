--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
------------------------------------------------------------------------------------------------------
-- rename database 
ALTER DATABASE [Commerce] MODIFY NAME = [Commerce.Convert]
------------------------------------------------------------------------------------------------------

------------------------------------------------------------------------------------------------------
-- Rename objects...
------------------------------------------------------------------------------------------------------
EXECUTE sp_rename 'PartnerCards', 'Guid_PartnerCards'
EXECUTE sp_rename 'PKC_PartnerCards_CardId_PartnerId', 'Guid_PKC_PartnerCards_CardId_PartnerId'
EXECUTE sp_rename 'FK_PartnerCards_PartnerId_Partners_Id', 'Guid_FK_PartnerCards_PartnerId_Partners_Id'
EXECUTE sp_rename 'FK_PartnerCards_CardId_Cards_Id', 'Guid_FK_PartnerCards_CardId_Cards_Id'

EXECUTE sp_rename 'PartnerUsers', 'Guid_PartnerUsers'
EXECUTE sp_rename 'FK_PartnerUsers_PartnerId_Partners_Id', 'Guid_FK_PartnerUsers_PartnerId_Partners_Id'
EXECUTE sp_rename 'FK_PartnerUsers_UserId_Users_Id', 'Guid_FK_PartnerUsers_UserId_Users_Id'
EXECUTE sp_rename 'PKC_PartnerUsers_UserId_PartnerId', 'Guid_PKC_PartnerUsers_UserId_PartnerId'
EXECUTE sp_rename 'U_PartnerUsers_PartnerId_PartnerUserId', 'Guid_U_PartnerUsers_PartnerId_PartnerUserId'

EXECUTE sp_rename 'PartnerDeals', 'Guid_PartnerDeals'
EXECUTE sp_rename 'FK_PartnerDeals_DealId_Deals_Id', 'Guid_FK_PartnerDeals_DealId_Deals_Id'
EXECUTE sp_rename 'FK_PartnerDeals_PartnerDealRegistrationStatusId_PartnerDealRegistrationStatus_Id', 'Guid_FK_PartnerDeals_PartnerDealRegistrationStatusId_PartnerDealRegistrationStatus_Id'
EXECUTE sp_rename 'FK_PartnerDeals_PartnerId_Partners_Id', 'Guid_FK_PartnerDeals_PartnerId_Partners_Id'
EXECUTE sp_rename 'PKC_PartnerDeals_DealId_PartnerId', 'Guid_PKC_PartnerDeals_DealId_PartnerId'
EXECUTE sp_rename 'U_PartnerDeals_PartnerId_PartnerDealId', 'Guid_U_PartnerDeals_PartnerId_PartnerDealId'

EXECUTE sp_rename 'DealPartnerMerchantIds', 'Guid_DealPartnerMerchantIds', 'object' -- SELECT * FROM sys.objects WHERE name = 'DealPartnerMerchantIds'
EXECUTE sp_rename 'FK_DealPartnerMerchantIds_DealId_Deals_Id', 'Guid_FK_DealPartnerMerchantIds_DealId_Deals_Id'
EXECUTE sp_rename 'FK_DealPartnerMerchantIds_PartnerId_Partners_Id', 'Guid_FK_DealPartnerMerchantIds_PartnerId_Partners_Id'
EXECUTE sp_rename 'PKC_DealPartnerMerchantIds_PartnerId_PartnerMerchantId_DealId', 'Guid_PKC_DealPartnerMerchantIds_PartnerId_PartnerMerchantId_DealId'

EXECUTE sp_rename 'Cards', 'Guid_Cards'
EXECUTE sp_rename 'FK_Cards_CardBrandId_CardBrands_Id', 'Guid_FK_Cards_CardBrandId_CardBrands_Id'
EXECUTE sp_rename 'FK_Cards_UserId_Users_Id', 'Guid_FK_Cards_UserId_Users_Id'
EXECUTE sp_rename 'PKC_Cards_Id', 'Guid_PKC_Cards_Id'

EXECUTE sp_rename 'PKC_Users_Id', 'Guid_PKC_Users_Id'
EXECUTE sp_rename 'Users', 'Guid_Users'

EXECUTE sp_rename 'Deals', 'Guid_Deals'
EXECUTE sp_rename 'FK_Deals_DealStatusId_DealStatus_Id', 'Guid_FK_Deals_DealStatusId_DealStatus_Id'
EXECUTE sp_rename 'FK_Deals_MerchantNameId_MerchantNames_Id', 'Guid_FK_Deals_MerchantNameId_MerchantNames_Id'
EXECUTE sp_rename 'PKC_Deals_Id', 'Guid_PKC_Deals_Id'

EXECUTE sp_rename 'ClaimedDeals', 'Guid_ClaimedDeals', 'object'
EXECUTE sp_rename 'FK_ClaimedDeals_CardId_Card_Id', 'Guid_FK_ClaimedDeals_CardId_Card_Id'
EXECUTE sp_rename 'FK_ClaimedDeals_DealId_Deal_Id', 'Guid_FK_ClaimedDeals_DealId_Deal_Id'
EXECUTE sp_rename 'FK_ClaimedDeals_PartnerId_Partner_id', 'Guid_FK_ClaimedDeals_PartnerId_Partner_id'
EXECUTE sp_rename 'FK_ClaimedDeals_UserId_Users_Id', 'Guid_FK_ClaimedDeals_UserId_Users_Id'
EXECUTE sp_rename 'PKC_ClaimedDeals_Id', 'Guid_PKC_ClaimedDeals_Id'
EXECUTE sp_rename 'U_ClaimedDeals_PartnerId_PartnerClaimedDealId', 'Guid_U_ClaimedDeals_PartnerId_PartnerClaimedDealId'

EXECUTE sp_rename 'Authorizations', 'Guid_Authorizations'
EXECUTE sp_rename 'FK_Authorizations_ClaimedDealId_ClaimedDeals_Id', 'Guid_FK_Authorizations_ClaimedDealId_ClaimedDeals_Id'
EXECUTE sp_rename 'FK_Authorizations_PartnerId_Partner_Id', 'Guid_FK_Authorizations_PartnerId_Partner_Id'
EXECUTE sp_rename 'PKC_Authorizations_Id', 'Guid_PKC_Authorizations_Id'
EXECUTE sp_rename 'U_Authorizations_PartnerId_TransactionId', 'Guid_U_Authorizations_PartnerId_TransactionId'

EXECUTE sp_rename 'FK_RedeemedDeals_ClaimedDealId_ClaimedDeals_Id', 'Guid_FK_RedeemedDeals_ClaimedDealId_ClaimedDeals_Id'
EXECUTE sp_rename 'FK_RedeemedDeals_CreditStatusId_CreditStatus_Id', 'Guid_FK_RedeemedDeals_CreditStatusId_CreditStatus_Id'
EXECUTE sp_rename 'FK_RedeemedDeals_RedemptionEventId_RedemptionEvents_Id', 'Guid_FK_RedeemedDeals_RedemptionEventId_RedemptionEvents_Id'
EXECUTE sp_rename 'PKC_RedeemedDeals_Id', 'Guid_PKC_RedeemedDeals_Id'
EXECUTE sp_rename 'RedeemedDeals', 'Guid_RedeemedDeals'
EXECUTE sp_rename 'Trigger_RedeemedDeals_Update', 'Guid_Trigger_RedeemedDeals_Update'

--DECLARE @Name varchar(100) = 'RedeemedDeals'
--SELECT 'EXECUTE sp_rename '''+Name+''', ''Guid_'+Name+'''' FROM sys.objects WHERE Name = @Name
--UNION SELECT 'EXECUTE sp_rename '''+Constraint_Name+''', ''Guid_'+Constraint_Name+'''' FROM Information_Schema.Table_Constraints WHERE Table_Name = @Name

------------------------------------------------------------------------------------------------------
-- Run db setup from command line
--DBSetup.exe dotm XX setup Commerce xxx false dev.commerce.config
------------------------------------------------------------------------------------------------------

------------------------------------------------------------------------------------------------------
-- Insert to new schema
------------------------------------------------------------------------------------------------------
INSERT INTO Users
    (
         GlobalId
        ,AnalyticsEventId
        ,DateAddedUtc
    )
  SELECT Id
        ,AnalyticsEventId
        ,DateAddedUtc
    FROM Guid_Users

INSERT INTO Cards
    (
         GlobalId
        ,UserId
        ,NameOnCard
        ,LastFourDigits
        ,Expiration
        ,CardBrandId        
        ,DateAddedUtc
    )
  SELECT Id
        ,UserId = (SELECT Id FROM Users U WHERE U.GlobalId = C.UserId)
        ,NameOnCard
        ,LastFourDigits
        ,Expiration
        ,CardBrandId        
        ,DateAddedUtc
    FROM Guid_Cards C

INSERT INTO Deals
    (
         GlobalId
        ,MerchantId
        ,StartDate
        ,EndDate
        ,Currency
        ,Amount
        ,[Percent]
        ,MinimumPurchase
        ,Count
        ,UserLimit
        ,DiscountSummary
        ,MaximumDiscount
        ,ParentDealId
        ,DateIngestedUtc
        ,DealStatusId
        ,MerchantNameId
    )
  SELECT Id
        ,MerchantId
        ,StartDate
        ,EndDate
        ,Currency
        ,Amount
        ,[Percent]
        ,MinimumPurchase
        ,Count
        ,UserLimit
        ,DiscountSummary
        ,MaximumDiscount
        ,ParentDealId
        ,DateIngestedUtc
        ,DealStatusId
        ,MerchantNameId
    FROM Guid_Deals D

INSERT INTO ClaimedDeals
    (
         DealId
        ,UserId
        ,CardId
        ,PartnerId
        ,PartnerClaimedDealId
        ,DateClaimedUtc
    )
  SELECT DealId = (SELECT Id FROM Deals D WHERE D.GlobalId = CD.DealId)
        ,UserId = (SELECT Id FROM Users U WHERE U.GlobalId = CD.UserId)
        ,CardId = (SELECT Id FROM Cards D WHERE D.GlobalId = CD.CardId)
        ,PartnerId
        ,PartnerClaimedDealId
        ,DateClaimedUtc
    FROM Guid_ClaimedDeals CD

INSERT INTO Authorizations
    (
         Id
        ,PartnerId
        ,TransactionId
        ,TransactionDate
        ,TransactionAmount
        ,ClaimedDealId
        ,DiscountAmount
    )
  SELECT G_A.Id
        ,G_A.PartnerId
        ,TransactionId
        ,TransactionDate
        ,TransactionAmount
        ,CD.Id
        ,DiscountAmount
    --SELECT count(*)
    FROM Guid_Authorizations G_A
         JOIN Guid_ClaimedDeals G_CD ON G_CD.Id = G_A.ClaimedDealId AND G_CD.PartnerId = G_A.PartnerId
         JOIN Deals D ON D.GlobalId = G_CD.DealId
         JOIN Users U ON U.GlobalId = G_CD.UserId
         JOIN Cards C ON C.GlobalId = G_CD.CardId
         JOIN ClaimedDeals CD ON CD.DealId = D.Id AND CD.UserId = U.Id AND CD.CardId = C.Id AND CD.PartnerId = G_CD.PartnerId

INSERT INTO RedeemedDeals
    (
         Id
        ,ClaimedDealId
        ,RedemptionEventId
        ,PurchaseDateTime
        ,AuthorizationAmount
        ,Currency
        ,Reversed
        ,CreditStatusId
        ,DiscountAmount
        ,SettlementAmount
        ,AnalyticsRedemptionEventId
        ,AnalyticsSettlementEventId
        ,DateAddedUtc
        ,DateSettled
        ,DateCreditApproved
        ,LastUpdatedDateUtc
    )
  SELECT RD.Id
        ,ClaimedDealId = CD.Id
        ,RedemptionEventId
        ,PurchaseDateTime
        ,AuthorizationAmount
        ,RD.Currency
        ,Reversed
        ,CreditStatusId
        ,DiscountAmount
        ,SettlementAmount
        ,AnalyticsRedemptionEventId
        ,AnalyticsSettlementEventId
        ,RD.DateAddedUtc
        ,DateSettled
        ,DateCreditApproved
        ,LastUpdatedDateUtc
    --SELECT count(*)
    FROM Guid_RedeemedDeals RD
         JOIN Guid_ClaimedDeals G_CD ON G_CD.Id = RD.ClaimedDealId
         JOIN Deals D ON D.GlobalId = G_CD.DealId
         JOIN Users U ON U.GlobalId = G_CD.UserId
         JOIN Cards C ON C.GlobalId = G_CD.CardId
         JOIN ClaimedDeals CD ON CD.DealId = D.Id AND CD.UserId = U.Id AND CD.CardId = C.Id

INSERT INTO PartnerUsers
    (
         PartnerId
        ,PartnerUserId
        ,UserId
    )
  SELECT PartnerId
        ,PartnerUserId
        ,UserId = (SELECT Id FROM Users U WHERE U.GlobalId = PU.UserId)
    FROM Guid_PartnerUsers PU

INSERT INTO PartnerCards
    (
         PartnerId
        ,PartnerCardId
        ,CardId
        ,PartnerCardSuffix
    )
  SELECT PartnerId
        ,PartnerCardId
        ,CardId = (SELECT Id FROM Cards C WHERE C.GlobalId = PC.CardId)
        ,PartnerCardSuffix
    FROM Guid_PartnerCards PC

INSERT INTO PartnerDeals
    (
         PartnerId
        ,PartnerDealId
        ,DealId
        ,PartnerDealRegistrationStatusId
    )
  SELECT PartnerId
        ,PartnerDealId
        ,DealId = (SELECT Id FROM Deals C WHERE C.GlobalId = PD.DealId)
        ,PartnerDealRegistrationStatusId
    FROM Guid_PartnerDeals PD

INSERT INTO DealPartnerMerchantIds
    (
         PartnerId
        ,PartnerMerchantId
        ,DealId
    )
  SELECT PartnerId
        ,PartnerMerchantId
        ,DealId = (SELECT Id FROM Deals C WHERE C.GlobalId = M.DealId)
    FROM Guid_DealPartnerMerchantIds M

------------------------------------------------------------------------------------------------------
-- rename database 
ALTER DATABASE [Commerce.Convert] MODIFY NAME = [Commerce]
------------------------------------------------------------------------------------------------------

--SELECT ','+name FROM sys.columns WHERE object_id = object_id('Guid_DealPartnerMerchantIds')
--SELECT * FROM Parameters
--Operations..RestoreDatabase @dbId = 'Commerce', @useAzureTarget = 1

-- Sanity check
SELECT *
  FROM (SELECT Parent = object_name(parent_object_id), Referenced = object_name(referenced_object_id), * FROM sys.foreign_keys) A
  WHERE R LIKE 'Guid%'