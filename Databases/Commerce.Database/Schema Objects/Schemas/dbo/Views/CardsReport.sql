--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE VIEW CardsReport 
AS
SELECT CardId = C.Id
      ,UserId
      ,GlobalUserId = U.GlobalId
      ,LastFourDigits
      ,CardBrand = (SELECT Name FROM dbo.CardBrands CB WHERE CB.Id = C.CardBrand)
      ,C.UtcAdded
      ,Active
  FROM dbo.Cards C
       JOIN dbo.Users U ON U.Id = C.UserId
GO
--SELECT * FROM CardsReport