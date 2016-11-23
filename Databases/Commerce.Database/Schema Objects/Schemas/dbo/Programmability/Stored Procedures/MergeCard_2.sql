--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE MergeCard_2
  @globalUserId uniqueidentifier = NULL,
  @nameOnCard nvarchar(100),
  @lastFourDigits char(4),
  @expiration datetime2,
  @cardBrandId int, 
  @partnerCardInfo PartnerCardInfo READONLY,
  @rewardPrograms int = 1
AS
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'MergeCard_2'
       ,@Rows int = 0
       ,@userId int

DECLARE @id INT, @ExistingCardRewardPrograms INT, @Active BIT

SELECT @id = Id, @active = Active
    FROM dbo.Cards
    WHERE UserId = (SELECT Id FROM dbo.Users WHERE GlobalId = @globalUserId)
      AND LastFourDigits = @lastFourDigits
      AND CardBrand = @cardBrandId
      ORDER BY Active DESC

IF @active = 0
BEGIN
    SET @ExistingCardRewardPrograms = 0
END
ELSE
BEGIN
    SET @ExistingCardRewardPrograms = 2
END

DECLARE @Mode varchar(100) = convert(varchar(36),isnull(@id,0))+' '+convert(varchar(36),@globalUserId)
       ,@LockUser varchar(100) = 'Commerce_MergeCard_'+convert(varchar(36),@globalUserId)
       ,@LockCard varchar(100) = 'Commerce_MergeCard_'+convert(varchar(36),isnull(@id,0))

BEGIN TRY
  BEGIN TRANSACTION

  EXECUTE sp_getapplock @LockUser, 'exclusive'
  IF isnull(@id,0) <> 0 -- Don't lock on new card
    EXECUTE sp_getapplock @LockCard, 'exclusive'
        
  SET @userId = (SELECT Id FROM dbo.Users WHERE GlobalId = @globalUserId)

  DECLARE @CardRegisteredToDifferentUserForSameRewardProgram BIT
  DECLARE @UserToWhomCardIsRegistered INT
  
  -- The merge card operation could end up doing one of the following:
  --201 -> Card added
  --200 -> Card updated
  --409 -> Card registered to different user.
  DECLARE @OperationStatus INT

  -- We don't want two users to register the same card for the same rewards program.
  -- Let us check if the card is already registered to a different user under same program.
        SELECT @UserToWhomCardIsRegistered = u.Id FROM dbo.Cards c
        INNER JOIN dbo.Users u ON c.UserId = u.Id
        INNER JOIN dbo.PartnerCards pc ON pc.CardId = c.Id        
        INNER JOIN @partnerCardInfo pci ON pci.PartnerCardId = pc.PartnerCardId AND pc.CardId = c.Id AND pci.PartnerId = pc.PartnerId
        WHERE 
        c.Active = 1 AND 
        c.CardBrand = @cardBrandId AND 
        c.LastFourDigits = @lastFourDigits AND
        u.Id <> @userId

    IF ISNULL(@UserToWhomCardIsRegistered, @userId) = @userId
    BEGIN
        SET @CardRegisteredToDifferentUserForSameRewardProgram = 0
    END
    ELSE
    BEGIN
        SET @CardRegisteredToDifferentUserForSameRewardProgram = 1
    END
    
    IF(@CardRegisteredToDifferentUserForSameRewardProgram = 0)
    BEGIN
      IF isnull(@id,0) = 0
      BEGIN
        INSERT INTO dbo.Cards 
                ( UserId,  LastFourDigits,  CardBrand, UtcAdded)
          SELECT @userId, @lastFourDigits, @cardBrandId, getUTCdate()
        SET @Rows = @Rows + @@rowcount

        SET @id = scope_identity()
        SET @OperationStatus = 201
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
        SET @OperationStatus = 200
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
            AND T.CardId = S.CardId
            AND T.PartnerCardSuffix = S.PartnerCardSuffix
        WHEN MATCHED AND T.PartnerCardId <> S.PartnerCardId THEN
            UPDATE SET T.PartnerCardId = S.PartnerCardId -- TODO: Need to know if the partner id can ever change.
        WHEN NOT MATCHED THEN
            INSERT (PartnerId, PartnerCardId, CardId, PartnerCardSuffix) VALUES (S.PartnerId, S.PartnerCardId, S.CardId, S.PartnerCardSuffix);
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
  
      -- Ensure any partner filtering jobs are reset if user is migrating to Earn from CLO.
      IF @ExistingCardRewardPrograms IN (0, 1)
      BEGIN
        DELETE FROM dbo.PartnerCardFilters WHERE PartnerCardId IN
        (
          SELECT PCF.PartnerCardId FROM dbo.PartnerCardFilters PCF
          JOIN dbo.PartnerCards PC ON PC.PartnerCardId = PCF.PartnerCardId
          WHERE PC.CardId = @id
        )
      END
  END
  ELSE
  BEGIN
    SET @id = -1
    SET @OperationStatus = 409
  END

  COMMIT TRANSACTION

  SELECT OperationStatus = @OperationStatus, Id = @id

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