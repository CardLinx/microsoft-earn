--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE UpdatePendingPartnerRedeemedDeals
	@creditStatusId INT,
	@transactionReferenceNumbers TransactionReferenceNumbers READONLY
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'UpdatePendingPartnerRedeemedDeals'

BEGIN TRY
  BEGIN TRANSACTION
  EXECUTE sp_getapplock 'Commerce_UpdatePendingPartnerRedeemedDeals_Lock', 'exclusive'

		-- If the redemptions are being marked SettledAsRedeemed, use current date.
		DECLARE @dateCreditApproved DATETIME
		SET @dateCreditApproved = GETUTCDATE()

		UPDATE
			[dbo].[RedeemedDeals]
		SET
			CreditStatusId = @creditStatusId,
			DateCreditApproved = 
			(CASE
				 WHEN
					 @creditStatusId <> 20 -- Only change the date when moving into StatementCreditRequested State.
				 THEN
					 RedeemedDeals.DateCreditApproved
				 ELSE
					 @dateCreditApproved
			 END)
		FROM
			[dbo].[RedeemedDeals] AS RedeemedDeals
		INNER JOIN
			[dbo].[PartnerRedeemedDeals] AS PartnerRedeemedDeals
		ON
			PartnerRedeemedDeals.RedeemedDealId = RedeemedDeals.Id
		INNER JOIN
			@transactionReferenceNumbers AS TransactionReferenceNumbers
		ON
			TransactionReferenceNumbers.TransactionReferenceNumber = PartnerRedeemedDeals.TransactionReferenceNumber
  
  COMMIT TRANSACTION

  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Status='Info',@Milliseconds=@Milliseconds
END TRY
BEGIN CATCH
  IF @@trancount > 0 ROLLBACK TRANSACTION
  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Status='Error',@Milliseconds=@Milliseconds
END CATCH
GO