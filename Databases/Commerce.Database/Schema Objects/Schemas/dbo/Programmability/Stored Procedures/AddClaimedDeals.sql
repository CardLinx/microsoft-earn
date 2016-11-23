--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE AddClaimedDeals
  @cardId int
 ,@partnerIds ListOfInts READONLY
 ,@dealBatchId int
 ,@startDate datetime
 ,@endDate datetime
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'AddClaimedDeals'
       ,@Rows int = 0
       ,@InitTrancount int = @@trancount

DECLARE @Mode varchar(100) = 'C='+convert(varchar(10),@cardId)+' P='+convert(varchar(10),(SELECT TOP 1 Id FROM @partnerIds))+' B='+convert(varchar(10),@dealBatchId)
       ,@Lock varchar(100) = db_name()+'_'+@SP+'_'+convert(varchar(10),@cardId)

BEGIN TRY
  IF @InitTrancount = 0 BEGIN TRANSACTION
  
  EXECUTE sp_getapplock @Lock, 'exclusive'

  INSERT INTO dbo.ClaimedDealBatches 
         (  CardId, PartnerId,  DealBatchId,  StartDate,  EndDate )
    SELECT @cardId,        Id, @dealBatchId, @startDate, @endDate
      FROM @partnerIds P
      WHERE NOT EXISTS (SELECT * FROM dbo.ClaimedDealBatches C WHERE C.CardId = @cardId AND C.PartnerId = P.Id AND C.DealBatchId = @dealBatchId)
  SET @Rows = @@rowcount

  IF @InitTrancount = 0 COMMIT TRANSACTION

  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Info',@Milliseconds=@Milliseconds,@Rows=@Rows
END TRY
BEGIN CATCH
  IF @InitTrancount = 0 AND @@trancount > 0 ROLLBACK TRANSACTION
  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Error',@Milliseconds=@Milliseconds
END CATCH
GO
--INSERT INTO dbo.Parameters (Id, Char) SELECT 'AddClaimedDeals', 'LogEvent'
--DECLARE @dealIds ListOfInts
--INSERT INTO @dealIds SELECT 1000000002
--EXECUTE RegisterDealsBatch @dealIds
--EXECUTE AddClaimedDeals 
-- @cardId = 1000000001
--,@partnerId = 1
--,@dealsBatchId = 1
--,@startDate = '2014-01-01'
--SELECT TOP 20 * FROM EventLog ORDER BY EventDate DESC
--SELECT * FROM Cards WHERE Id = 1000000001
--DROP TABLE DealsBatchDetails
--DROP TABLE ClaimedDealsBatches
--DROP TABLE DealsBatches