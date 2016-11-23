--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TYPE [dbo].[PartnerDealInfo] AS TABLE
(
	PartnerId INT, 
	PartnerDealId NVARCHAR(255),
	PartnerDealRegistrationStatusId INT,
	DealId UniqueIdentifier
)