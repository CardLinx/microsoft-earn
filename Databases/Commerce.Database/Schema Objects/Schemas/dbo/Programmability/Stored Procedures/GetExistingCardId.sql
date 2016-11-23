--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE GetExistingCardId
  @userId uniqueidentifier = NULL,
  @globalUserId uniqueidentifier = NULL,
  --TODO: When the V1 APIs are removed, remove old parameter set.
  @nameOnCard nvarchar(100) = NULL,
  @lastFourDigits char(4),
  @expiration datetime2(7),
  @cardBrandId int
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'GetExistingCardId'
       ,@Rows int = 0

SET @globalUserId = isnull(@globalUserId, @userId)

DECLARE @Mode varchar(100) = convert(varchar(36),@globalUserId)

SELECT Id
    ,Active
FROM dbo.Cards
WHERE UserId = (SELECT Id FROM dbo.Users WHERE GlobalId = @globalUserId)
    AND LastFourDigits = @lastFourDigits
    AND CardBrand = @cardBrandId
ORDER BY Active DESC
SET @Rows = @Rows + @@rowcount

SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Info',@Milliseconds=@Milliseconds,@Rows=@Rows
GO
--EXECUTE GetExistingCardId NULL, NULL, NULL, NULL, NULL
--SELECT * FROM EventLog WHERE Process = 'GetExistingCardId' ORDER BY EventDate DESC