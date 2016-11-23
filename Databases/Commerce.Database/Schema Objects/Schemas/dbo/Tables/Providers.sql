--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- Providers.sql
-- Copyright (c) Microsoft Corporation. All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- dbo.Providers
--  The definition of the Earn providers table
-- Columns:
--  Id int: The identity of the provider.
--  GlobalID varchar(100): The ID assigned within the wider system to this provider.
--  Name nvarchar(100): The provider's name.
--  UtcAdded datetime: The UTC date and time at which the provider was added.
--  UtcUpdated datetime: The UTC date and time at which the provider was last updated. Populated automatically.
-- Remarks:
--  The provider's name should be formatted for end user consumption.
create table dbo.Providers
(
	Id int not null identity(0, 1),
	GlobalID varchar(100) not null,
	Name nvarchar(100) not null,
	UtcAdded datetime not null
		constraint ProvidersUtcAddedDefault default getutcdate(),
	UtcUpdated datetime not null
		constraint ProvidersUtcUpdatedDefault default getutcdate()

	constraint ProvidersPrimary primary key clustered(Id),
	constraint ProvidersGlobalIDUnique unique(GlobalID)
)

GO

-- dbo.Providers_GlobalID_Id_Name
--  Index used in adding or updating providers.
create nonclustered index Providers_GlobalID_Id_Name
on Providers(GlobalId, Id, Name);
GO

-- dbo.ProvidersUtcUpdatedTrigger
--  The update trigger that ensures that any update to a row in the Providers table will be reflected in the UtcUpdated column.
create trigger dbo.ProvidersUtcUpdatedTrigger on dbo.Providers for update as
begin
    set nocount on;

    if (not update(UtcUpdated))
    begin
        update Providers set UtcUpdated = getutcdate()
        from dbo.Providers, inserted
        where Providers.Id = inserted.Id;
    end;
end;

GO