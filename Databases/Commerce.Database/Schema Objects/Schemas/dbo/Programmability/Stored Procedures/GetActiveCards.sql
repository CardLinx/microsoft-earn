--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE GetActiveCards
  @rewardPrograms int = 2147483647
 ,@partnerId int = NULL
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'GetActiveCards'
	
SELECT Id
      ,UserId
      ,GlobalUserId
   FROM (SELECT *
               ,GlobalUserId = (SELECT GlobalId FROM dbo.Users U WHERE U.Id = C.UserId)
           FROM dbo.Cards C
           WHERE Active = 1
             AND (@partnerId IS NULL OR (@partnerId = 1 AND FDCToken is not null) OR EXISTS(SELECT CardBrand FROM dbo.Cards Cards WHERE Cards.Id = C.Id AND Cards.CardBrand = @partnerId + 1))
        ) C

SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
EXECUTE dbo.LogEvent @Process=@SP,@Status='Info',@Milliseconds=@Milliseconds
GO