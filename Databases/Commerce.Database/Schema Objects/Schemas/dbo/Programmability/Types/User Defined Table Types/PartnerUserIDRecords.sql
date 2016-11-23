--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- PartnerUserIDRecords.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- PartnerUserIDRecords
--  A user as identified by our partner company, associated with our user entity.
-- Columns:
--  Partner int: The partner from whom the PartnerUserId originated, i.e. 3 (American Express), 4 (Visa), or 5 (MasterCard).
--  PartnerUserID varchar(100): The ID assigned to the user by the partner.
create type dbo.PartnerUserIDRecords as table
(
	[Partner] int not null,
	PartnerUserID varchar(100) not null

    primary key ([Partner], PartnerUserId)
)

GO