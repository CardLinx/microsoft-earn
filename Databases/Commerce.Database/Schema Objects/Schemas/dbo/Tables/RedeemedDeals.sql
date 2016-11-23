--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE RedeemedDeals
(
   Id uniqueidentifier NOT NULL
  ,ClaimedDealId bigint NOT NULL
  ,RedemptionEventId int NOT NULL
  ,PurchaseDateTime datetime2 NOT NULL
  ,AuthorizationAmount int NOT NULL
  ,Currency varchar(5) NOT NULL
  ,Reversed bit NOT NULL
  ,CreditStatusId int NOT NULL
  ,DiscountAmount int NOT NULL
  ,SettlementAmount int NOT NULL
  ,AnalyticsRedemptionEventId uniqueidentifier NULL
  ,AnalyticsSettlementEventId uniqueidentifier NULL
  ,DateAdded datetime NOT NULL CONSTRAINT DF_RedeemedDeals_DateAdded DEFAULT getUTCdate()
  ,DateSettled datetime NULL
  ,DateCreditApproved datetime NULL
  ,LastUpdatedDateUtc datetime NOT NULL DEFAULT getUTCdate()
  ,PermaPending BIT NOT NULL DEFAULT 0
  ,ReviewStatusId INT NOT NULL DEFAULT 0
  ,MerchantNameId INT NOT NULL
  ,UtcReachedTerminalState [datetime] NULL

   CONSTRAINT PKC_RedeemedDeals_Id PRIMARY KEY CLUSTERED (Id)
  ,CONSTRAINT FK_RedeemedDeals_ClaimedDealId_TransactionLinks_Id FOREIGN KEY (ClaimedDealId) REFERENCES TransactionLinks (Id)
  ,CONSTRAINT FK_RedeemedDeals_RedemptionEventId_RedemptionEvents_Id FOREIGN KEY (RedemptionEventId) REFERENCES RedemptionEvents (Id)
  ,CONSTRAINT FK_RedeemedDeals_MerchantNameId_MerchantNames_Id FOREIGN KEY (MerchantNameId) REFERENCES MerchantNames (Id)
  ,CONSTRAINT RedeemedDealsCreditStatusIdCheck check(CreditStatusId in (0, 4, 5, 10, 15, 20, 25, 500, 505, 510, 515, 520, 525, 530)) -- 4 is deprecated and will be removed along with the rest of the FDC code.
)
GO
CREATE INDEX IX_ClaimedDealId_RedemptionEventId_PurchaseDateTime_AuthorizationAmount_Currency ON RedeemedDeals (ClaimedDealId,RedemptionEventId,PurchaseDateTime,AuthorizationAmount,Currency)
GO

CREATE TRIGGER [dbo].[Trigger_RedeemedDeals_Update] ON [dbo].[RedeemedDeals] FOR UPDATE
AS
BEGIN
  SET NOCOUNT ON
  IF NOT UPDATE(UtcReachedTerminalState) AND NOT UPDATE(LastUpdatedDateUtc)
  BEGIN
    UPDATE RD
      SET LastUpdatedDateUtc = GETUTCDATE(),
          UtcReachedTerminalState =
          CASE
              WHEN DELETED.CreditStatusId < 500 AND INSERTED.CreditStatusId >= 500
          THEN
              GETUTCDATE()
          ELSE
              RD.UtcReachedTerminalState
          END
      FROM dbo.RedeemedDeals RD JOIN INSERTED ON INSERTED.Id = RD.Id JOIN DELETED ON DELETED.Id = INSERTED.Id
  END
END

GO

CREATE TRIGGER [dbo].[Trigger_RedeemedDeals_Insert] ON [dbo].[RedeemedDeals] FOR INSERT
AS
BEGIN
  SET NOCOUNT ON

  BEGIN
    UPDATE RD
      SET UtcReachedTerminalState =
          CASE
              WHEN INSERTED.CreditStatusId >= 500
          THEN
              GETUTCDATE()
          ELSE
              RD.UtcReachedTerminalState
          END
      FROM dbo.RedeemedDeals RD JOIN INSERTED ON INSERTED.Id = RD.Id
  END

END

GO