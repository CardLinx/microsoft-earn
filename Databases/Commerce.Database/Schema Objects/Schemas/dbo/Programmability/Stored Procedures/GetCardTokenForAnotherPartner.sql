--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE GetCardTokenForAnotherPartner
	@partnerIdFrom INT,
	@partnerIdTo INT,
	@cardTokenFrom NVARCHAR(255)
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'GetCardTokenForAnotherPartner'


	SELECT DISTINCT
		Cards.FDCToken AS PartnerCardId
	FROM 
        dbo.Cards
	WHERE 
		Cards.PartnerToken = @cardTokenFrom


SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
EXECUTE dbo.LogEvent @Process=@SP,@Status='Info',@Milliseconds=@Milliseconds
GO