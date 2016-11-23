--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--

-- THIS SPROC IS DEPRECATED AND WILL BE REMOVED DURING CLEANUP, AFTER FDC HAS BEEN DECOMMISSIONED. 


CREATE PROCEDURE RegisterDealBatch 
   @dealIds ListOfInts READONLY
  ,@batchId int = NULL OUT
  ,@returnDataSet bit = 1
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'RegisterDealsBatch'
       ,@Rows int = 0
       ,@Sum bigint
       ,@Count int
       ,@LastDealId int = (SELECT TOP 1 Id FROM @dealIds ORDER BY Id DESC)
       ,@InitTrancount int = @@trancount

DECLARE @Mode varchar(100) = 'D='+convert(varchar(10),@LastDealId)
       ,@Lock varchar(100) = db_name()+'_'+@SP+'_'+convert(varchar(10),@LastDealId)

Start:

BEGIN TRY
  SET @batchId = NULL

  SELECT @Count = count(*)
        ,@Sum = sum(convert(bigint,Id))
    FROM @dealIds

  IF isnull(@Count,0) = 0 RAISERROR('Input list is empty', 18, 127)

  -- Get batch candidates
  DECLARE BatchesCursor CURSOR LOCAL FORWARD_ONLY READ_ONLY
  FOR SELECT Id FROM dbo.DealBatches WHERE Count = @Count AND SumOfIds = @Sum

  OPEN BatchesCursor
  FETCH NEXT FROM BatchesCursor INTO @BatchId

  DECLARE @Found bit = 0
  WHILE @@fetch_status = 0 AND @Found = 0
  BEGIN
    IF @Count = 1
      BEGIN
        IF EXISTS (SELECT * FROM dbo.DealBatchDetails WHERE DealBatchId = @BatchId AND DealId = @LastDealId)
          SET @Found = 1
      END
      ELSE
      BEGIN
        IF NOT EXISTS
             (SELECT *
                FROM (SELECT * FROM dbo.DealBatchDetails WHERE DealBatchId = @BatchId) A
                     FULL OUTER JOIN @dealIds B ON B.Id = A.DealId
                WHERE A.DealId IS NULL OR B.Id IS NULL
             )
          SET @Found = 1
      END

      IF @Found = 0 FETCH NEXT FROM BatchesCursor INTO @BatchId
  END

  DEALLOCATE BatchesCursor

  IF @Found = 0 SET @BatchId = NULL

  IF @BatchId IS NULL
  BEGIN
    IF @InitTrancount = 0 BEGIN TRANSACTION

    EXECUTE sp_getapplock @Lock, 'exclusive'

    IF EXISTS (SELECT * FROM dbo.DealBatches WHERE Count = @Count AND SumOfIds = @Sum)
      RAISERROR('DealBatches changed', 18, 127)

    INSERT INTO dbo.DealBatches (SumOfIds, Count) SELECT @Sum, @Count
    SET @Rows = @Rows + @@rowcount

    SET @BatchId = scope_identity()

    INSERT INTO dbo.DealBatchDetails (DealBatchId, DealId)
      SELECT @BatchId, Id
        FROM @dealIds
    SET @Rows = @Rows + @@rowcount
    
    IF @InitTrancount = 0 COMMIT TRANSACTION
  END

  IF @returnDataSet = 1
    SELECT DealBatchId = @BatchId

  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Info',@Milliseconds=@Milliseconds,@Rows=@Rows
END TRY
BEGIN CATCH
  IF @InitTrancount = 0 AND @@trancount > 0 ROLLBACK TRANSACTION
  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  IF error_message() = 'DealBatches changed'
  BEGIN
    EXECUTE dbo.LogEvent @Process=@SP,@Status='Warn',@Milliseconds=@Milliseconds,@ReraisError=0
    GOTO Start
  END
  ELSE
    EXECUTE dbo.LogEvent @Process=@SP,@Status='Error',@Milliseconds=@Milliseconds
END CATCH
GO
--INSERT INTO dbo.Parameters (Id, Char) SELECT 'RegisterDealsBatch', 'LogEvent'
--SELECT * FROM dbo.Parameters
--DECLARE @dealIds ListOfInts
--INSERT INTO @dealIds SELECT 1000000002
--EXECUTE RegisterDealBatch @dealIds
--SELECT TOP 20 * FROM EventLog ORDER BY EventDate DESC
--SELECT * FROM Deals WHERE Id = 1000000001
--SELECT * FROM DealBatches
--SELECT * FROM DealBatchDetails