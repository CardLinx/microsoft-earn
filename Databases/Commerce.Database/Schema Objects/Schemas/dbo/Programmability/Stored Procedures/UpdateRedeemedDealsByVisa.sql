--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE UpdateRedeemedDealsByVisa
	@creditStatusId INT,
	@partnerRedeemedDealScopeId nvarchar(255),
	@dateCreditApproved datetime
AS
set nocount on
DECLARE @st datetime = getUTCdate()
	   ,@Milliseconds int
	   ,@SP varchar(100) = 'UpdateRedeemedDealsByVisa'

BEGIN TRY
  BEGIN TRANSACTION
	EXECUTE sp_getapplock 'Commerce_UpdateRedeemedDealsByVisa_Lock', 'exclusive'

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
		-- assuming that @partnerRedeemedDealScopeId is unique and it will only update Visa transactions
		WHERE PartnerRedeemedDeals.PartnerRedeemedDealScopeId = @partnerRedeemedDealScopeId
  
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