--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
EXECUTE sp_rename 'ClaimedDeals', 'TransactionLinks', 'object'
EXECUTE sp_rename 'FK_ClaimedDeals_CardId_Card_Id', 'FK_TransactionLinks_CardId_Cards_Id'
EXECUTE sp_rename 'FK_ClaimedDeals_DealId_Deal_Id', 'FK_TransactionLinks_DealId_Deals_Id'
EXECUTE sp_rename 'FK_ClaimedDeals_PartnerId_Partner_id', 'FK_TransactionLinks_PartnerId_Partners_id'
EXECUTE sp_rename 'FK_ClaimedDeals_UserId_Users_Id', 'FK_TransactionLinks_UserId_Users_Id'
EXECUTE sp_rename 'PKC_ClaimedDeals_Id', 'PKC_TransactionLinks_Id'
EXECUTE sp_rename 'DF__ClaimedDe__DateC__7B264821', 'DF_TransactionLinks_DateAdded'
EXECUTE sp_rename 'TransactionLinks.DateClaimedUtc', 'DateAdded', 'Column'
ALTER TABLE TransactionLinks DROP CONSTRAINT U_ClaimedDeals_PartnerId_PartnerClaimedDealId
ALTER TABLE TransactionLinks ALTER COLUMN PartnerClaimedDealId nvarchar(255) NULL
CREATE INDEX IX_PartnerId_PartnerClaimedDealId ON TransactionLinks (PartnerId, PartnerClaimedDealId)

------------------------------------------------------------------------------------------------------
-- Run db setup from command line
--DBSetup.exe dotm XX setup Commerce xxx false staging.commerce.config
--DBSetup.exe dotm XX setup Commerce xxx false prod.commerce.config
------------------------------------------------------------------------------------------------------

-- Run in text mode
BEGIN TRANSACTION

set nocount on
DECLARE @Deals ListOfInts
DECLARE @ClaimedDeals TABLE (CardId int, DealId int, PartnerId int PRIMARY KEY (CardId, DealId, PartnerId))
INSERT INTO @ClaimedDeals
  SELECT DISTINCT 
         CardId, DealId, PartnerId
    FROM TransactionLinks WITH (TABLOCKX)

DECLARE @DealId int, @CardId int, @PartnerId int, @StartDate datetime = getUTCdate(), @BatchId int
WHILE EXISTS (SELECT * FROM @ClaimedDeals)
BEGIN
  SELECT TOP 1 @CardId = CardId, @DealId = DealId, @PartnerId = PartnerId FROM @ClaimedDeals
  --SELECT @CardId, @DealId, @PartnerId
  
  DELETE FROM @Deals
  INSERT INTO @Deals SELECT @DealId
  
  EXECUTE RegisterDealBatch @Deals, @BatchId OUT, @returnDataSet = 0

  EXECUTE AddClaimedDeals
            @cardId = @CardId
           ,@partnerId = @PartnerId
           ,@dealBatchId = @BatchId
           ,@startDate = @StartDate
           ,@endDate = @StartDate

  DELETE FROM @ClaimedDeals WHERE CardId = @CardId AND DealId = @DealId AND PartnerId = @PartnerId
END

-- Sanity check. Should have 0 rows.
SELECT *
  FROM dbo.TransactionLinks A
       FULL OUTER JOIN dbo.ClaimedDeals B ON B.DealId = A.DealId AND B.CardId = A.CardId AND B.UserId = A.UserId AND B.PartnerId = A.PartnerId
  WHERE A.DealId IS NULL OR B.DealId IS NULL
  
--ROLLBACK TRANSACTION
--COMMIT TRANSACTION

--DECLARE @Name varchar(100) = 'ClaimedDeals'
--DECLARE @Rep varchar(100) = 'TransactionLinks'
--SELECT 'EXECUTE sp_rename '''+Name+''', '''+replace(Name,@Name,@Rep)+''', ''object''' FROM sys.objects WHERE Name = @Name
--UNION SELECT 'EXECUTE sp_rename '''+Constraint_Name+''', '''+replace(Constraint_Name,@Name,@Rep)+'''' FROM Information_Schema.Table_Constraints WHERE Table_Name = @Name
