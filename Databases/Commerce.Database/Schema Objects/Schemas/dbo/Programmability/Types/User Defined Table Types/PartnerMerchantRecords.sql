--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- PartnerMerchantRecords.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- PartnerMerchantRecords
--  A merchant as identified by our partner company, associated with our merchant entity.
-- Columns:
--  [Partner] int: The partner company from which this information originates.
--  AuthorizationID varchar(100): An ID the partner company has assigned to this merchant for authorization events.
--  SettlementID varchar(100): An ID the partner company has assigned to this merchant for settlement events.
-- Remarks:
--  This is a many-to-one relationship, where many partner company ID records can be attached to a single merchant entity in our system.
create type dbo.PartnerMerchantRecords as table
(
	[Partner] int not null,
	AuthorizationID varchar(100) not null,
	SettlementID varchar(100) not null

    primary key ([Partner], AuthorizationID, SettlementID)
)

GO