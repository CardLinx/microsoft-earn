--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TYPE DealPartnerMerchants AS TABLE
(
	PartnerId INT, 
	PartnerMerchantId NVARCHAR(255),
	PartnerMerchantIdTypeId INT NULL,
	MerchantTimeZoneId varchar(100) NULL
)