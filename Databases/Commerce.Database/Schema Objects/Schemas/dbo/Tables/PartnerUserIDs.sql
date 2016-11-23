--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- PartnerUserIDs.sql
-- Copyright (c) Microsoft Corporation. All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- dbo.PartnerUserIDs
--  The definition of the Earn Partner User IDs table
-- Columns:
--  UserId int: The ID of the user to whom this PartnerUser record belongs.
--  Partner int: The partner from whom the PartnerUserId originated, i.e. 3 (American Express), 4 (Visa), or 5 (MasterCard).
--  PartnerUserID varchar(100): The ID assigned to the user by the partner.
--  Active bit not null: Specifies whether the partner user ID record is active.
--  UtcAdded datetime: The UTC date and time at which the partner user ID was added.
--  UtcUpdated datetime: The UTC date and time at which the partner user ID was last updated. Populated automatically.
-- Remarks:
--  This table will be used to house PartnerUserIDs only when those IDs are issued by the partner. When Microsoft supplies the
--   IDs, they are generated programmatically.
create table dbo.PartnerUserIDs
(
    UserId int not null,
    [Partner] int not null,
    PartnerUserID varchar(100) not null,
    Active bit not null
        constraint PartnerUserIDsActiveDefault default 1,
    UtcAdded datetime not null
        constraint PartnerUserIDsUtcAddedDefault default getutcdate(),
    UtcUpdated datetime not null
        constraint PartnerUserIDsUtcUpdatedDefault default getutcdate()

    constraint PartnerUserIDsPrimary primary key clustered(UserId, [Partner], PartnerUserID),
    constraint PartnerUserIDsUsers foreign key(UserId) references dbo.Users(Id),
    constraint PartnerUserIDsPartnerPartnerUserIDUnique unique([Partner], PartnerUserId),
    constraint PartnerUserIDsPartnerCheck check([Partner] between 3 and 5)
);

GO

-- PartnerUserIDs_Insert
--  The index used when inserting a new PartnerUserID record.
create nonclustered index PartnerUserIDs_Insert
on dbo.PartnerUserIDs(UserId, [Partner], PartnerUserID);
GO

-- PartnerUserIDs_InsertExistsCheck
--  The index used when inserting a new PartnerUserID record to check to see if a different partner user ID for the partner is already active.
create nonclustered index PartnerUserIDs_InsertExistsCheck
on dbo.PartnerUserIDs(UserId, Active, [Partner]);
GO

-- dbo.PartnerUserIDsUtcUpdatedTrigger
--  The update trigger that ensures that any update to a row in the PartnerUserIDs table will be reflected in the UtcUpdated column.
create trigger dbo.PartnerUserIDsUtcUpdatedTrigger on dbo.PartnerUserIDs for update as
begin
    set nocount on;

    if (not update(UtcUpdated))
    begin
        update PartnerUserIDs set UtcUpdated = getutcdate()
        from dbo.PartnerUserIDs, inserted
        where PartnerUserIDs.UserId = inserted.UserId and
              PartnerUserIDs.[Partner] = inserted.[Partner] and
              PartnerUserIDs.PartnerUserID = inserted.PartnerUserID;
    end
end

GO