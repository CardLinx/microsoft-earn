--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--

-- THIS IS DEPRECATED AND WILL BE REMOVED AS PART OF CLEANUP. WAS ONLY USED BY AMEX. MAKE SURE AMEX JOBS HAVE BEEN REMOVED.


CREATE PROCEDURE GetDealByPartnerDealId
  @partnerDealId nvarchar(255),
  @partnerId int
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'GetDealByPartnerDealId'
       ,@Rows int = 0
       ,@Mode varchar(100) = convert(varchar(10),@partnerId) + convert(varchar(100),@partnerDealId)

SELECT Id = DealId
      ,GlobalId
      ,DealId = GlobalId
  FROM (SELECT *, GlobalId = (SELECT GlobalId FROM dbo.Deals D WHERE D.Id = PD.DealId)
          FROM dbo.PartnerDeals PD
          WHERE PartnerId = @partnerId
           AND PartnerDealId = @partnerDealId
       ) A
SET @Rows = @Rows + @@rowcount

SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Info',@Milliseconds=@Milliseconds,@Rows=@Rows
GO