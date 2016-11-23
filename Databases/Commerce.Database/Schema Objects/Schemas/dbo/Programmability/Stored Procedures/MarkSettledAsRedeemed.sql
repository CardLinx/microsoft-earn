--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE MarkSettledAsRedeemed
	@redeemedDealIds ListOfGuids READONLY
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'MarkSettledAsRedeemed'
       ,@Rows int = 0

BEGIN TRY
		UPDATE RD
		  SET CreditStatusId = 500 -- CreditGranted
             ,DateCreditApproved = GETUTCDATE()
		  FROM dbo.RedeemedDeals AS RD
		  JOIN @redeemedDealIds AS RDIds ON RDIds.Id = RD.Id
        SET @Rows = @Rows + @@rowcount

  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Status='Info',@Milliseconds=@Milliseconds,@Rows=@Rows
END TRY
BEGIN CATCH
  IF @@trancount > 0 ROLLBACK TRANSACTION
  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Status='Error',@Milliseconds=@Milliseconds
END CATCH
GO