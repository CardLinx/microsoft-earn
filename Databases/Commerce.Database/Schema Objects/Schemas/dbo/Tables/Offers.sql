--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- Offers.sql
-- Copyright (c) Microsoft Corporation. All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- dbo.Offers
--  The definition of the Earn Offers table
-- Columns:
--  Id int: The identity of the offer.
--  GlobalID uniqueidentifier: The ID assigned within the wider system to this offer.
--  ProviderId int: The ID of the provider through which this offer came into the system.
--  OfferType int: The offer's type, e.g. Earn or Burn.
--  PercentBack money: The percent of the settlement amount to apply as Earn credits, or the percent of dollars spent for which
--   Earn credits can be used instead.
--  Active bit: Specifies whether this offer is currently active.
--  UtcAdded datetime: The UTC date and time at which the offer was added.
--  UtcUpdated datetime: The UTC date and time at which the offer was last updated. Populated automatically.
create table dbo.Offers
(
	Id int not null identity(1000000000, 1),
	GlobalID uniqueidentifier not null,
	ProviderId int not null,
	OfferType int not null,
	PercentBack money not null,
	Active bit not null
		constraint OffersActiveDefault default 1,
	UtcAdded datetime not null
		constraint OffersUtcAddedDefault default getutcdate(),
	UtcUpdated datetime not null
		constraint OffersUtcUpdatedDefault default getutcdate()

	constraint OffersPrimary primary key clustered(Id),
	constraint OffersGlobalIDUnique unique(GlobalID),
	constraint OfferProvider foreign key(ProviderId) references dbo.Providers(Id),
	constraint OffersOfferTypeCheck check(OfferType between -1 and 2),
	constraint OffersPercentBackCheck check(PercentBack between 1 and 100 or PercentBack = -1.00)
);

GO

-- dbo.Offers_ExistsCheck
--  The index used when determining if an offer already exists.
create nonclustered index Offers_ExistsCheck
on dbo.Offers(GlobalID, ProviderId, OfferType, PercentBack, Active, Id);
GO

-- dbo.Offers_InsertCheck
--  The index used when checking to see if new elements depending on offers should be added.
create nonclustered index Offers_InsertCheck
on dbo.Offers(Active, ProviderId, Id, OfferType, PercentBack);
GO

-- dbo.Offers_Update
--  The index used when updating an existing Offer.
create nonclustered index Offers_Update
on dbo.Offers(Id, GlobalID, ProviderId, OfferType, PercentBack, Active);
GO

-- dbo.OffersUtcUpdatedTrigger
--  The update trigger that ensures that any update to a row in the Offers table will be reflected in the UtcUpdated column.
create trigger dbo.OffersUtcUpdatedTrigger on dbo.Offers for update as
begin
    set nocount on;

    if (not update(UtcUpdated))
    begin
        update Offers set UtcUpdated = getutcdate()
        from dbo.Offers, inserted
        where Offers.Id = inserted.Id;
    end;
end;

GO