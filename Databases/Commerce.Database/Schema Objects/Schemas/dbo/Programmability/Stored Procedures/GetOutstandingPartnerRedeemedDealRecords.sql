--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE GetOutstandingPartnerRedeemedDealRecords
	@partnerId INT
AS
set nocount on;
DECLARE @st datetime = getUTCdate()
	   ,@Milliseconds int
	   ,@SP varchar(100) = 'GetOutstandingPartnerRedeemedDealRecords';

	declare @outstandingTransactions table (RedeemedDealId uniqueidentifier,
											PartnerMerchantId nvarchar(255),
											PartnerDealId nvarchar(255),
											PartnerReferenceNumber nvarchar(255),
											PartnerCardId varchar(100),
											DiscountAmount int,
											PurchaseDateTime datetime2 ,
											TransactionReferenceNumber int,
											MerchantName nvarchar(100),
											MerchantId varchar(100),
											SettlementAmount int,
											GlobalId uniqueidentifier,
											ParentDealId uniqueidentifier,
											PartnerData xml,
											ReimbursementTenderId int,
											GlobalUserId uniqueidentifier,
											PartnerRedeemedDealScopeId nvarchar(255)
											);

	EXECUTE sp_getapplock 'GetOutstandingPartnerRedeemedDealRecords_Lock', 'exclusive'
	
	--find all transactions for the partner for which clearing message is received and insert them into  @outstandingTransactions
	INSERT @outstandingTransactions
	SELECT
	RD.Id AS RedeemedDealId,
	PartnerRD.PartnerMerchantId
	,'' AS PartnerDealId
	,PartnerRD.PartnerReferenceNumber
	,CASE WHEN @partnerId = 1 THEN C.FDCToken ELSE C.PartnerToken END AS PartnerCardId
	,RD.DiscountAmount
	,RD.PurchaseDateTime
	,PartnerRD.TransactionReferenceNumber
	,MN.Name AS MerchantName
	,MN.Id AS MerchantId
	,RD.SettlementAmount
	,D.GlobalId
	,D.GlobalId AS ParentDealId
	,PartnerRD.PartnerData
	,D.OfferType + 2 AS ReimbursementTenderId
	,U.GlobalID AS GlobalUserId
	,PartnerRD.PartnerRedeemedDealScopeId AS PartnerRedeemedDealScopeId
	FROM dbo.PartnerRedeemedDeals PartnerRD
	JOIN dbo.RedeemedDeals RD ON RD.Id = PartnerRD.RedeemedDealId
	JOIN dbo.TransactionLinks CD ON CD.Id = RD.ClaimedDealId
	JOIN dbo.Offers D ON D.Id = CD.DealId
	JOIN dbo.Merchants MN ON MN.Id = RD.MerchantNameId
	JOIN dbo.Cards C ON C.Id = CD.CardId
	JOIN dbo.Users U ON CD.UserId = U.Id
	WHERE CD.PartnerId = @partnerId
	AND D.OfferType = 1 -- Burn
	AND RD.CreditStatusId = 5 -- ClearingReceived
	AND RD.ReviewStatusId IN (0, 2) -- Unnecessary or ResolvedAccept
	-- TODO - remove the following FDCToken check when we remove FDC calls
	-- AND ((@partnerId = 3 and C.FDCToken <> '') or @partnerId <> 3) -- Visa cards must have FDC Token to have FDC issue a statement credit.
	AND RD.DiscountAmount > 0; -- Must have an actual burn coming. (This stops us from spamming our partners with dead transactions, and stops them from spamming us in turn.)

	-- update CreditStatusId in RedeemedDeals table and set to GeneratingStatementCreditRequest for all rows selected. This will prevent other process from selecting the same rows again
	UPDATE RD
	SET RD.CreditStatusId = 10    --GeneratingStatementCreditRequest
	FROM dbo.RedeemedDeals RD
	JOIN @outstandingTransactions OT
	ON RD.Id = OT.RedeemedDealId;

	
SELECT * FROM @outstandingTransactions ORDER BY PartnerMerchantId;
	
SET @Milliseconds = datediff(millisecond,@st,getUTCdate());
EXECUTE dbo.LogEvent @Process=@SP,@Status='Info',@Milliseconds=@Milliseconds;

GO