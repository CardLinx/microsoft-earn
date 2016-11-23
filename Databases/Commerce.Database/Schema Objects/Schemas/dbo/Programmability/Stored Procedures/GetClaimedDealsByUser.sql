--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--

-- THIS SPROC IS DEPRECATED. IT'S USED AS PART OF THE FDC CLAIMED DEALS FLOW.

CREATE PROCEDURE GetClaimedDealsByUser 
  @userId uniqueidentifier = NULL
 ,@globalUserId uniqueidentifier = NULL
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'GetClaimedDealsByUser'
       ,@Rows int = 0

SET @globalUserId = isnull(@globalUserId,@userId)

DECLARE @Mode varchar(100) = convert(varchar(36), @globalUserId)

-- Return the list of deals claimed by the user.
-- In order to keep the returned list from growing ever longer, don't return anything that's expired by more than 25 hours
--  based on UTC. This heuristic is used because at that point, the deal is expired for everyone regardless of what time
--  zone they may be in.
SELECT DISTINCT
       DealId = D.GlobalId
      ,GlobalDealId = D.GlobalId
  FROM dbo.ClaimedDeals CD
       JOIN dbo.Offers D ON D.Id = CD.DealId
       JOIN dbo.Cards C ON C.Id = CD.CardId
  WHERE CD.UserId = (SELECT Id FROM dbo.Users WHERE GlobalId = @globalUserId)
    AND D.Active = 1
    AND C.Active = 1
SET @Rows = @Rows + @@rowcount

SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
EXECUTE dbo.LogEvent @Process=@SP,@Mode=@Mode,@Status='Info',@Milliseconds=@Milliseconds,@Rows=@Rows
GO