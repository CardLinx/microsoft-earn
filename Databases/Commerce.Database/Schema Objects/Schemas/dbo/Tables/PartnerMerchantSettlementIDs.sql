--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- PartnerMerchantSettlementIDs.sql
-- Copyright (c) Microsoft Corporation. All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- dbo.PartnerMerchantSettlementIDs
--  The definition of the Earn PartnerMerchantSettlementIDs table
-- Columns:
--  Id int: The identity of the partner merchant settlement ID record.
--  MerchantId int: The ID of the merchant as assigned by us.
--  [Partner] int: The partner company from which this settlement ID originates.
--  SettlementID varchar(100): An ID the partner company has assigned to this merchant for settlement  events.
--  Active bit: Specifies whether the partner merchant settlement ID record represents an active partner merchant.
--  UtcAdded datetime: The UTC date and time at which the partner merchant settlement ID record was added.
--  UtcUpdated datetime: The UTC date and time at which the partner merchant settlement ID recrord was last updated. Populated automatically.
-- Remarks:
--  * This is a many-to-one relationship, where many partner merchant settlements ID records can be attached to a single merchant entity in our system.
--  * An inactive partner merchant settlement ID record is not considered when processing transactions, but it remains in place for referential integrity.
create table dbo.PartnerMerchantSettlementIDs
(
    Id int not null identity(0, 1),
    MerchantId int not null,
    [Partner] int not null,
    SettlementID varchar(100) not null,
    Active bit not null
		constraint PartnerMerchantSettlementIDsActiveDefault default 1,
    UtcAdded datetime not null
        constraint PartnerMerchantSettlementIDsUtcAddedDefault default getutcdate(),
    UtcUpdated datetime not null
        constraint PartnerMerchantSettlementIDsUtcUpdatedDefault default getutcdate()

    constraint PartnerMerchantSettlementIDsPrimary primary key clustered(Id),
    constraint PartnerMerchantSettlementIDsMerchant foreign key(MerchantId) references dbo.Merchants(Id),
    constraint PartnerMerchantSettlementIDsTupleUnique unique(MerchantId, [Partner], SettlementID),
    constraint PartnerMerchantSettlementIDsPartnerCheck check([Partner] between 3 and 5)
);

GO

-- dbo.PartnerMerchantSettlementIDs_GetPartnerMerchantSettlementIDs
--  Index used in retrieving partner merchant SettlementIDs.
create nonclustered index PartnerMerchantSettlementIDs_GetPartnerMerchantSettlementIDs
on PartnerMerchantSettlementIDs(MerchantId, Id, Active, [Partner], SettlementID);
GO

-- dbo.PartnerMerchantSettlementIDs_Insert
--  Index used when inserting new records.
create nonclustered index PartnerMerchantSettlementIDs_Insert
on PartnerMerchantSettlementIDs([Partner], SettlementID);
GO

-- dbo.PartnerMerchantSettlementIDs_InsertCheck
--  Index used when inserting new checking to see if new records should be added.
create nonclustered index PartnerMerchantSettlementIDs_InsertCheck
on dbo.PartnerMerchantSettlementIDs(SettlementId, [Partner], Active, MerchantId);
GO

-- dbo.PartnerMerchantSettlementIDs_Query
-- Index used when determining whether there are records to deactivate or reactivate
create nonclustered index PartnerMerchantSettlementIDs_Query
on dbo.PartnerMerchantSettlementIDs(MerchantId, Active, [Partner], SettlementID, Id);
GO

-- dbo.PartnerMerchantSettlementIDsUtcUpdatedTrigger
--  The update trigger that ensures that any update to a row in the PartnerMerchantSettlementIDs table will be reflected in the UtcUpdated column.
create trigger dbo.PartnerMerchantSettlementIDsUtcUpdatedTrigger on dbo.PartnerMerchantSettlementIDs for update as
begin
    set nocount on;

    if (not update(UtcUpdated))
    begin
        update PartnerMerchantSettlementIDs set UtcUpdated = getutcdate()
        from dbo.PartnerMerchantSettlementIDs, inserted
        where PartnerMerchantSettlementIDs.Id = inserted.Id;
    end;
end;

GO