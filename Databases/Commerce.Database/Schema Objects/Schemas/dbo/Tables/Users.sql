--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- Users.sql
-- Copyright (c) Microsoft Corporation. All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- dbo.Users
--  The definition of the Earn User Account table.
-- Columns:
--  Id int: The identity of the user account.
--  GlobalID uniqueIdentifier: The GUID ID assigned within the wider system to the user for whom the account was created.
--  UtcAdded datetime: The UTC date and time at which the user account was created. Populated automatically.
--  UtcUpdated datetime: The UTC date and time at which the user account was last updated. Populated automatically.
create table dbo.Users
(
  Id int not null identity(1000000000, 1),
  GlobalID uniqueIdentifier not null,
  UtcAdded datetime not null
   constraint UsersUtcAddedDefault default getutcdate(),
  UtcUpdated datetime not null
   constraint UsersUtcUpdatedDefault default getutcdate()

  constraint UsersPrimary primary key clustered(Id),
  constraint UsersGlobalIDUnique unique(GlobalID)
);

GO

-- dbo.UsersUtcUpdatedTrigger
--  The update trigger that ensures that any update to a row in the Users table will be reflected in the UtcUpdated column.
create trigger dbo.UsersUtcUpdatedTrigger on dbo.Users for update as
begin
  set nocount on;

  if (not update(UtcUpdated))
  begin
    update Users set UtcUpdated = getutcdate()
      from dbo.Users, inserted
      where Users.Id = inserted.Id;
  end
end

GO