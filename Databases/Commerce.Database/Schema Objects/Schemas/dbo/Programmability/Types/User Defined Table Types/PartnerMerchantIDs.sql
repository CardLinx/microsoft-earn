--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- PartnerMerchantIDs.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- PartnerMerchantIDs
--  A merchant as identified by our partner company, associated with our merchant entity.
-- Columns:
--  [Partner] int: The partner company from which this information originates.
--  EventID varchar(100): An ID the partner company has assigned to this merchant for authorization events or settlement events.
--  AddOrUpdate bit: Specifies whether this record is to be added or updated (1) or deactivated (0).
-- Remarks:
--  This is a many-to-one relationship, where many partner company ID records can be attached to a single merchant entity in our system.
create type dbo.PartnerMerchantIDs as table
(
	[Partner] int not null,
	EventID varchar(100) not null,
	AddOrUpdate bit not null

    primary key ([Partner], EventID, AddOrUpdate)
)

GO