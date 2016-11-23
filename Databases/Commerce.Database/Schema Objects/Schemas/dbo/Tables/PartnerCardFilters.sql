--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE dbo.PartnerCardFilters
(
    PartnerCardId varchar(255) NOT NULL
   ,DateFiltered datetime NOT NULL CONSTRAINT DF_PartnerCardFilters_DateFiltered DEFAULT getUTCdate()
    
    CONSTRAINT PKC_PartnerCardFilters_PartnerCardId PRIMARY KEY CLUSTERED (PartnerCardId)
)
GO