--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--

CREATE PROCEDURE AddClaimedDeal
  @globalUserId uniqueidentifier
 ,@cardId int
 ,@globalDealId uniqueidentifier
 ,@partnerId int
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'AddClaimedDeal'
       ,@Rows int = 0
       ,@initTrancount int = @@trancount
       ,@uId int
       ,@dId int

DECLARE @Mode varchar(100) = 'C='+convert(varchar(36),@cardId)+' D='+convert(varchar(36),@globalDealId)
       ,@Lock varchar(100) = 'Commerce_AddClaimedDeal_'+convert(varchar(36),@cardId)

BEGIN TRY
  IF @initTrancount = 0 BEGIN TRANSACTION

  EXECUTE sp_getapplock @Lock, 'exclusive'

  SET @uId = (SELECT Id FROM dbo.Users WHERE GlobalId = @globalUserId)
  SET @dId = (SELECT Id FROM dbo.Offers WHERE GlobalId = @globalDealId)

  -- Make sure the specified card belongs to the specified user.
  IF EXISTS (SELECT * FROM dbo.Cards WHERE Id = @cardId AND UserId = @uId)
  BEGIN
    -- Make sure the user has not already claimed the specified deal for the specified card.
    IF NOT EXISTS (SELECT * FROM dbo.ClaimedDeals WHERE DealId = @dId AND CardId = @cardId)
    BEGIN
      -- Determine if this is the user's first claimed deal.
      DECLARE @userFirstClaimedDeal bit = 1
      IF EXISTS (SELECT * FROM dbo.ClaimedDeals WHERE CardId = @cardId)
        SET @userFirstClaimedDeal = 0

      -- Determine if this is the first time the user claimed this deal.
      DECLARE @userNewDealClaimed bit
      IF @userFirstClaimedDeal = 1
        SET @userNewDealClaimed = 1
      ELSE
      BEGIN
        IF EXISTS (SELECT * FROM dbo.ClaimedDeals WHERE DealId = @dId AND UserId = @uId)
          SET @userNewDealClaimed = 0
        ELSE
          SET @userNewDealClaimed = 1
      END

      DECLARE @dealIds ListOfInts
      INSERT INTO @dealIds SELECT @dId
      DECLARE @batchId int
      EXECUTE dbo.RegisterDealBatch 
                @dealIds = @dealIds
               ,@batchId = @batchId OUT
               ,@returnDataSet = 0
      
      DECLARE @partnerIds ListOfInts
      INSERT INTO @partnerIds SELECT @partnerId
	  DECLARE @startDate datetime = getUTCdate()
      EXECUTE dbo.AddClaimedDeals
               @cardId = @cardId
              ,@partnerIds = @partnerIds
              ,@dealBatchId = @batchId
              ,@startDate = @startDate
              ,@endDate = @startDate

      -- Finally, return information necessary to control user notifications.
      SELECT @userFirstClaimedDeal AS UserFirstClaimedDeal,
             @userNewDealClaimed AS UserNewDealClaimed
    END
    ELSE
    BEGIN
      DECLARE @duplicateMessage char(14)
      SET @duplicateMessage = 'AlreadyClaimed'
      RAISERROR (@duplicateMessage, 16, 1)
    END
  END
  ELSE
  BEGIN
    DECLARE @mismatchMessage CHAR(23)
    SET @mismatchMessage = 'CardDoesNotBelongToUser'
    RAISERROR (@mismatchMessage, 16, 1)
  END

  IF @initTrancount = 0 COMMIT TRANSACTION

  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Info',@Milliseconds=@Milliseconds,@Rows=@Rows
END TRY
BEGIN CATCH
  IF @initTrancount = 0 AND @@trancount > 0 ROLLBACK TRANSACTION
  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Status='Error',@Milliseconds=@Milliseconds
END CATCH
GO
--DECLARE @globalDealId uniqueidentifier = (SELECT TOP 1 GlobalId FROM Deals)
--DECLARE @globalCardId uniqueidentifier
--DECLARE @globalUserId uniqueidentifier 
--SELECT TOP 1 
--       @globalCardId = GlobalId 
--      ,@globalUserId = (SELECT GlobalId FROM Users WHERE Id = UserId) 
--  FROM Cards
--EXECUTE AddClaimedDeal
--          @globalUserId
--         ,@globalCardId
--         ,@globalDealId
--         ,@partnerId = 2
--SELECT TOP 20 * FROM EventLog ORDER BY EventDate DESC