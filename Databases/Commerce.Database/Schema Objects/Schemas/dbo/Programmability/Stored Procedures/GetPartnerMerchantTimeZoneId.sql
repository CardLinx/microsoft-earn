--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--

--THIS SPROC IS DEPRECATED AND WILL BE REMOVED DURING THE CLEANUP PHASE. FEATURE WAS NEVER ACTUALLY USED.

CREATE PROCEDURE GetPartnerMerchantTimeZoneId
   @partnerId int
  ,@partnerMerchantId nvarchar(100)
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'GetPartnerMerchantTimeZoneId'
       ,@Rows int = 0
       ,@Mode varchar(100) = convert(varchar(10),@partnerId)+@partnerMerchantId

BEGIN TRY
  SELECT TOP 1 MerchantTimeZoneId 
    FROM dbo.DealPartnerMerchantIds
    WHERE PartnerId = @PartnerId
      AND PartnerMerchantId = @PartnerMerchantId
  SET @Rows = @Rows + @@rowcount

  IF @Rows = 0 RAISERROR('PartnerMerchantNotFound', 16, 1)

  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Info',@Milliseconds=@Milliseconds,@Rows=@Rows
END TRY
BEGIN CATCH
  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Error',@Milliseconds=@Milliseconds
END CATCH
GO