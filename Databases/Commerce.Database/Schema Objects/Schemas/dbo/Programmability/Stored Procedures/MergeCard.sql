--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE MergeCard
  @id int = NULL,
  @globalUserId uniqueidentifier = NULL,
  @nameOnCard nvarchar(100),
  @lastFourDigits char(4),
  @expiration datetime2,
  @cardBrandId int, 
  @partnerCardInfo PartnerCardInfo READONLY
AS
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'MergeCard'
       ,@Rows int = 0
       ,@userId int

DECLARE @Mode varchar(100) = convert(varchar(36),isnull(@id,0))+' '+convert(varchar(36),@globalUserId)
       ,@LockUser varchar(100) = 'Commerce_MergeCard_'+convert(varchar(36),@globalUserId)
       ,@LockCard varchar(100) = 'Commerce_MergeCard_'+convert(varchar(36),isnull(@id,0))

BEGIN TRY
  BEGIN TRANSACTION

  EXECUTE sp_getapplock @LockUser, 'exclusive'
  IF isnull(@id,0) <> 0 -- Don't lock on new card
    EXECUTE sp_getapplock @LockCard, 'exclusive'

  SET @userId = (SELECT Id FROM dbo.Users WHERE GlobalId = @globalUserId)

  IF isnull(@id,0) = 0
  BEGIN
    INSERT INTO dbo.Cards 
            ( UserId,  LastFourDigits,  CardBrand,  UtcAdded )
      SELECT @userId,  @lastFourDigits, @cardBrandId, getUTCdate()
    SET @Rows = @Rows + @@rowcount

    SET @id = scope_identity()
  END
  ELSE
  BEGIN
    -- Merge the Card.
    UPDATE dbo.Cards
      SET UserId = @userId
         ,LastFourDigits = @lastFourDigits
         ,CardBrand = @cardBrandId
         ,Active = 1
      WHERE Id = @id
    SET @Rows = @Rows + @@rowcount
  END

  -- Merge applicable PartnerCards.
  MERGE dbo.PartnerCards T
    USING (SELECT PartnerId, 
                  PartnerCardId,
                  CardId = @id,
                  PartnerCardSuffix
             FROM @partnerCardInfo
          ) S
    ON T.PartnerId = S.PartnerId
       AND T.PartnerCardId = S.PartnerCardId
       AND T.CardId = S.CardId
       AND T.PartnerCardSuffix = S.PartnerCardSuffix
    WHEN NOT MATCHED THEN
      INSERT   (  PartnerId,   PartnerCardId,   CardId,   PartnerCardSuffix)
        VALUES (S.PartnerId, S.PartnerCardId, S.CardId, S.PartnerCardSuffix);
  SET @Rows = @Rows + @@rowcount

  -- Delete applicable PartnerCards.
  DELETE FROM T
    FROM dbo.PartnerCards T
         LEFT OUTER JOIN @partnerCardInfo S
           ON T.PartnerId = S.PartnerId
              AND T.PartnerCardId = S.PartnerCardId
              AND T.PartnerCardSuffix = S.PartnerCardSuffix
    WHERE T.CardId = @id
      AND S.PartnerId IS NULL
  SET @Rows = @Rows + @@rowcount
  
  COMMIT TRANSACTION

  SELECT Id = @id

  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Info',@Milliseconds=@Milliseconds,@Rows=@Rows
END TRY
BEGIN CATCH
  IF @@trancount > 0 ROLLBACK TRANSACTION
  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Error',@Milliseconds=@Milliseconds
END CATCH
GO
--DECLARE @PartnerCardInfo AS PartnerCardInfo
--EXECUTE MergeCard NULL, NULL, NULL, NULL, NULL, NULL, @PartnerCardInfo
--SELECT TOP 1 * FROM EventLog WHERE Process = 'MergeCard' ORDER BY EventDate DESC