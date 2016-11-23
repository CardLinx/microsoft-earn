--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TYPE PartnerCardInfo AS TABLE
(
    PartnerId int
   ,PartnerCardId nvarchar(255)
   ,PartnerCardSuffix char(2)

    PRIMARY KEY (PartnerId, PartnerCardId)
)
GO