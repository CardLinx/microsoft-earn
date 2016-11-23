--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- Marchants.sql
-- Copyright (c) Microsoft Corporation. All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- dbo.Merchants
--  The definition of the Earn Merchants table
-- Columns:
--  Id int: The identity of the merchant.
--  GlobalID varchar(100): The ID assigned within the wider system to this merchant.
--  ProviderId int: The ID of the provider through which this merchant came into the system.
--  Name nvarchar(100): The merchant's name.
--  UtcAdded datetime: The UTC date and time at which the merchant was added.
--  UtcUpdated datetime: The UTC date and time at which the merchant was last updated. Populated automatically.
-- Remarks:
--  The merchant's name should be formatted for end user consumption.
create table dbo.Merchants
(
	Id int not null identity(0, 1),
	GlobalID varchar(100) not null,
	Name nvarchar(100) not null,
  ProviderId int not null,
	UtcAdded datetime not null
		constraint MerchantsUtcAddedDefault default getutcdate(),
	UtcUpdated datetime not null
		constraint MerchantsUtcUpdatedDefault default getutcdate()

	constraint MerchantsPrimary primary key clustered(Id),
	constraint MerchantProvider foreign key(ProviderId) references dbo.Providers(Id),
	constraint MerchantsGlobalIDUnique unique(GlobalID)
)

GO

-- dbo.Merchants_GlobalID_Id_Name
--  Index used in adding or updating merchants.
create nonclustered index Merchants_GlobalID_Id_Name_ProviderId
on Merchants(GlobalId, Id, Name, ProviderId);
GO

-- dbo.MerchantsUtcUpdatedTrigger
--  The update trigger that ensures that any update to a row in the Merchants table will be reflected in the UtcUpdated column.
create trigger dbo.MerchantsUtcUpdatedTrigger on dbo.Merchants for update as
begin
    set nocount on;

    if (not update(UtcUpdated))
    begin
        update Merchants set UtcUpdated = getutcdate()
        from dbo.Merchants, inserted
        where Merchants.Id = inserted.Id;
    end;
end;

GO