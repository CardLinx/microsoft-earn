--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--

-- THIS SPROC IS DEPRECATED. IT'S USED AS PART OF THE FDC CLAIMED DEALS FLOW.

CREATE PROCEDURE GetActiveDealIds
  @excludeEarn bit = 0
 ,@partnerId int = 0
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'GetActiveDealIds'

-- Return the list id of deals currently active for the given constraints.
-- That means all deals that have end date in future.
SELECT Id = GlobalId
      ,GlobalId
      ,PerformanceId = Id
  FROM dbo.Offers D
  WHERE Active = 1
    AND (@excludeEarn = 0 OR OfferType > 0) -- 0 = Earn, so only include Burn offers.
    AND (@partnerId = 0 OR EXISTS(SELECT PartnerId FROM dbo.PartnerDeals PD WHERE PD.DealId = D.Id AND PD.PartnerId = @partnerId))

SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
EXECUTE dbo.LogEvent @Process=@SP,@Status='Info',@Milliseconds=@Milliseconds
GO