--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE GetCardsByPartnerCardId @partnerCardId varchar(255)
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'GetCardsByPartnerCardId'

-- Return cards for the partner card id
SELECT Id
      ,GlobalUserId
      ,PerformanceUserId = UserId
   FROM (SELECT C.*
               ,GlobalUserId = (SELECT GlobalId FROM dbo.Users U WHERE U.Id = C.UserId)
                FROM dbo.Cards C
           WHERE C.PartnerToken = @partnerCardId
             AND C.Active = 1
        ) A

SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
EXECUTE dbo.LogEvent @Process=@SP,@Status='Info',@Milliseconds=@Milliseconds
GO