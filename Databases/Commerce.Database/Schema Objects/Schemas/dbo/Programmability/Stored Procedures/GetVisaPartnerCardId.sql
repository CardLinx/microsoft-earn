--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
-- Since the card has a PAN token (currently a First Data PartnerCardId), see if a Visa ID has already been allocated for that PAN token.
--  We do this because Visa rejects registering a card that had previously been registered to a different account. This approach allows us
--  to differentiate false positives (i.e. Visa rejects a card for which we already have the Visa partner card ID somewhere in our DB)
--  from true positives (i.e. Visa rejects a card erroneously as far as we can tell.)
CREATE PROCEDURE [dbo].[GetVisaPartnerCardId]
	@panToken VARCHAR(255)
AS

DECLARE @result NVARCHAR(255);

WITH MatchingPanToken (CardId)
AS
(
    SELECT Cards.Id as CardId
	FROM dbo.Cards
	WHERE FDCToken = @panToken
)
SELECT @result = 
(
    SELECT TOP 1 PartnerToken
    FROM dbo.Cards
    JOIN MatchingPanToken on MatchingPanToken.CardId = Cards.Id
    WHERE CardBrand = 4
)

IF @result IS NULL SET @result = ''

SELECT @result AS VisaPartnerCardId
GO