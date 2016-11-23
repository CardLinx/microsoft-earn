--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- PartnerMerchantAuthorizationIDs.sql
-- Copyright (c) Microsoft Corporation. All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- dbo.PartnerMerchantAuthorizationIDs
--  The definition of the Earn PartnerMerchantAuthorizationIDs table
-- Columns:
--  Id int: The identity of the partner merchant authorization ID record.
--  MerchantId int: The ID of the merchant as assigned by us.
--  [Partner] int: The partner company from which this authorization ID originates.
--  AuthorizationID varchar(100): An ID the partner company has assigned to this merchant for authorization events.
--  Active bit: Specifies whether the partner merchant authorization ID record represents an active partner merchant.
--  UtcAdded datetime: The UTC date and time at which the partner merchant authorization ID record was added.
--  UtcUpdated datetime: The UTC date and time at which the partner merchant authorization ID recrord was last updated. Populated automatically.
-- Remarks:
--  * This is a many-to-one relationship, where many partner merchant authorizations ID records can be attached to a single merchant entity in our system.
--  * An inactive partner merchant authorization ID record is not considered when processing transactions, but it remains in place for referential integrity.
create table dbo.PartnerMerchantAuthorizationIDs
(
    Id int not null identity(0, 1),
    MerchantId int not null,
    [Partner] int not null,
    AuthorizationID varchar(100) not null,
    Active bit not null
		constraint PartnerMerchantAuthorizationIDsActiveDefault default 1,
    UtcAdded datetime not null
        constraint PartnerMerchantAuthorizationIDsUtcAddedDefault default getutcdate(),
    UtcUpdated datetime not null
        constraint PartnerMerchantAuthorizationIDsUtcUpdatedDefault default getutcdate()

    constraint PartnerMerchantAuthorizationIDsPrimary primary key clustered(Id),
    constraint PartnerMerchantAuthorizationIDsMerchant foreign key(MerchantId) references dbo.Merchants(Id),
    constraint PartnerMerchantAuthorizationIDsTupleUnique unique(MerchantId, [Partner], AuthorizationID),
    constraint PartnerMerchantAuthorizationIDsPartnerCheck check([Partner] between 3 and 5)
);

GO

-- dbo.PartnerMerchantAuthorizationIDs_GetPartnerMerchantAuthorizationIDs
--  Index used in retrieving partner merchant AuthorizationIDs.
create nonclustered index PartnerMerchantAuthorizationIDs_GetPartnerMerchantAuthorizationIDs
on dbo.PartnerMerchantAuthorizationIDs(MerchantId, Id, Active, [Partner], AuthorizationID);
GO

-- dbo.PartnerMerchantAuthorizationIDs_Insert
--  Index used when inserting new records.
create nonclustered index PartnerMerchantAuthorizationIDs_Insert
on dbo.PartnerMerchantAuthorizationIDs([Partner], AuthorizationID);
GO

-- dbo.PartnerMerchantAuthorizationIDs_InsertCheck
--  Index used when inserting new checking to see if new records should be added.
create nonclustered index PartnerMerchantAuthorizationIDs_InsertCheck
on dbo.PartnerMerchantAuthorizationIDs(AuthorizationId, [Partner], Active, MerchantId);
GO

-- dbo.PartnerMerchantAuthorizationIDs_Query
-- Index used when determining whether there are records to deactivate or reactivate.
create nonclustered index PartnerMerchantAuthorizationIDs_Query
on dbo.PartnerMerchantAuthorizationIDs(MerchantId, Active, [Partner], AuthorizationID, Id);
GO

-- dbo.PartnerMerchantAuthorizationIDsUtcUpdatedTrigger
--  The update trigger that ensures that any update to a row in the PartnerMerchantAuthorizationIDs table will be reflected in the UtcUpdated column.
create trigger dbo.PartnerMerchantAuthorizationIDsUtcUpdatedTrigger on dbo.PartnerMerchantAuthorizationIDs for update as
begin
    set nocount on;

    if (not update(UtcUpdated))
    begin
        update PartnerMerchantAuthorizationIDs set UtcUpdated = getutcdate()
        from dbo.PartnerMerchantAuthorizationIDs, inserted
        where PartnerMerchantAuthorizationIDs.Id = inserted.Id;
    end;
end;

GO