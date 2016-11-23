--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--

--THIS SPROC IS DEPRECATED AND WILL BE REMOVED DURING THE CLEANUP PHASE. NEED TO MAKE SHAREDDEALLOGIC USE A NO_OP

CREATE PROCEDURE MergeDeal
   @globalId uniqueidentifier
  ,@parentDealId uniqueidentifier
  ,@providerId varchar(100) = ''
  ,@merchantId varchar(100) = ''
  ,@merchantName nvarchar(100)
  ,@merchantCategory nvarchar(100)
  ,@startDate datetime2
  ,@endDate datetime2
  ,@currency varchar(5)
  ,@reimbursementTenderId int = 0
  ,@amount int
  ,@percent money
  ,@minimumPurchase int
  ,@count int
  ,@userLimit int
  ,@discountSummary nvarchar(100) = NULL
  ,@maximumDiscount int
  ,@partnerDealInfo PartnerDealInfo READONLY
  ,@dealPartnerMerchantIds DealPartnerMerchants READONLY
  ,@dealStatusId int
  -- TODO: For prototype. Fix NULL and NULL handling in perm code
  ,@dayTimeRestrictions xml = NULL
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'MergeDeal'
       ,@dId int
       ,@Rows int = 0

DECLARE @Mode varchar(100) = convert(varchar(36), @globalId)

BEGIN TRY
  BEGIN TRANSACTION
  EXECUTE sp_getapplock 'Commerce_MergeDeal', 'exclusive'

  -- Merge the merchant name
  DECLARE @merchantNameId int
  IF @merchantName IS NOT NULL
  BEGIN
    SET @merchantNameId = (SELECT Id FROM dbo.MerchantNames WHERE Name = @merchantName)

    IF @merchantNameId IS NULL
    BEGIN
      INSERT INTO MerchantNames (Name) SELECT @merchantName
      SET @Rows = @Rows + @@rowcount
      SET @merchantNameId = scope_identity()
    END
  END

  -- Merge the Deal.
  UPDATE dbo.Deals
    SET ParentDealId = @parentDealId
       ,ProviderId = @providerId
       ,MerchantId = @merchantId
       ,MerchantNameId = @merchantNameId
       ,StartDate = @startDate
       ,EndDate = @endDate
       ,Currency = @currency
       ,ReimbursementTenderId = @reimbursementTenderId
       ,Amount = @amount
       ,[Percent] = @percent
       ,MinimumPurchase = @minimumPurchase
       ,[Count] = @count
       ,UserLimit = @userLimit
       ,DiscountSummary = @discountSummary
       ,MaximumDiscount = @maximumDiscount
       ,DealStatusId = @dealStatusId
       ,DayTimeRestrictions = @dayTimeRestrictions
       ,@dId = Id
    WHERE GlobalId = @globalId
  SET @Rows = @Rows + @@rowcount
  
  IF @dId IS NULL
  BEGIN
    INSERT INTO dbo.Deals
            ( GlobalId, ParentDealId, ProviderId, MerchantId, MerchantNameId, StartDate, EndDate, Currency, ReimbursementTenderId, Amount,[Percent], MinimumPurchase, [Count], UserLimit, DiscountSummary, MaximumDiscount, DealStatusId, DayTimeRestrictions )
      SELECT @globalId,@parentDealId,@providerId, @merchantId, @merchantNameId,@startDate,@endDate,@currency,@reimbursementTenderId,@amount, @percent,@minimumPurchase, @count,@userLimit,@discountSummary,@maximumDiscount,@dealStatusId,@dayTimeRestrictions
    SET @Rows = @Rows + @@rowcount

    SET @dId = scope_identity()
  END

  -- Merge applicable PartnerDeals.
  MERGE dbo.PartnerDeals T
    USING (SELECT PartnerId, 
                  PartnerDealId,
                  PartnerDealRegistrationStatusId,
                  DealId = @dId
             FROM @partnerDealInfo
          ) S
      ON T.PartnerId = S.PartnerId
         AND T.PartnerDealId = S.PartnerDealId
         AND T.DealId = S.DealId
    WHEN MATCHED THEN 
      UPDATE -- only update the status, other data points are invariant
        SET PartnerDealRegistrationStatusId = S.PartnerDealRegistrationStatusId
    WHEN NOT MATCHED THEN
      INSERT   (  PartnerId,   PartnerDealId,   PartnerDealRegistrationStatusId,   DealId)
        VALUES (S.PartnerId, S.PartnerDealId, S.PartnerDealRegistrationStatusId, S.DealId)
  ;
  SET @Rows = @Rows + @@rowcount

  -- Delete applicable PartnerDeals.
  DELETE FROM T
    FROM dbo.PartnerDeals T
         LEFT OUTER JOIN @partnerDealInfo S
           ON T.PartnerId = S.PartnerId
              AND T.PartnerDealId = S.PartnerDealId
    WHERE T.DealId = @dId
      AND S.PartnerId IS NULL
  SET @Rows = @Rows + @@rowcount

  -- Merge applicable DealPartnerMerchantIds.
  MERGE dbo.DealPartnerMerchantIds T
    USING (SELECT PartnerId
                 ,PartnerMerchantId
                 ,DealId = @dId
                 ,PartnerMerchantIdTypeId
                 ,MerchantTimeZoneId
             FROM @dealPartnerMerchantIds
          ) S
      ON T.PartnerId = S.PartnerId
         AND T.PartnerMerchantId = S.PartnerMerchantId
         AND T.DealId = S.DealId
    WHEN NOT MATCHED THEN
      INSERT   (  PartnerId,   PartnerMerchantId,   DealId,   PartnerMerchantIdTypeId,   MerchantTimeZoneId)
        VALUES (S.PartnerId, S.PartnerMerchantId, S.DealId, S.PartnerMerchantIdTypeId, S.MerchantTimeZoneId)
    WHEN MATCHED THEN
      UPDATE SET PartnerMerchantIdTypeId = S.PartnerMerchantIdTypeId
                ,MerchantTimeZoneId = S.MerchantTimeZoneId
  ;
  SET @Rows = @Rows + @@rowcount

  -- Delete applicable DealPartnerMerchantIds.
  DELETE FROM T
    FROM dbo.DealPartnerMerchantIds T
         LEFT OUTER JOIN @dealPartnerMerchantIds S
           ON T.PartnerId = S.PartnerId
              AND T.PartnerMerchantId = S.PartnerMerchantId
    WHERE T.DealId = @dId
      AND S.PartnerId IS NULL	
  SET @Rows = @Rows + @@rowcount
  
  COMMIT TRANSACTION

  SELECT Id = @dId

  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Info',@Milliseconds=@Milliseconds,@Rows=@Rows
END TRY
BEGIN CATCH
  IF @@trancount > 0 ROLLBACK TRANSACTION
  SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
  EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Error',@Milliseconds=@Milliseconds
END CATCH
GO