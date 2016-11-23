--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE AddFilteredMasterCards
  @partnerCardIds ListOfStrings READONLY
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'AddFilteredMasterCards'
       ,@Rows int = 0

BEGIN TRY
  BEGIN TRANSACTION
  EXECUTE sp_getapplock 'Commerce_AddFilteredMasterCards_Lock', 'exclusive'

	--Add filter date for all specified MasterCard cards.
	INSERT INTO dbo.PartnerCardFilters
	  SELECT PCIds.Id
			,GETUTCDATE()
		FROM @partnerCardIds PCIds
			 LEFT OUTER JOIN dbo.PartnerCardFilters PCF
			   ON PCF.PartnerCardId = convert(varchar(255),PCIds.Id) -- need to convert to the type of PK
		WHERE PCF.PartnerCardId IS NULL
    SET @Rows = @Rows + @@rowcount

  COMMIT TRANSACTION

  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Status='Info',@Milliseconds=@Milliseconds,@Rows=@Rows
END TRY
BEGIN CATCH
  IF @@trancount > 0 ROLLBACK TRANSACTION
  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Status='Error',@Milliseconds=@Milliseconds
END CATCH
GO