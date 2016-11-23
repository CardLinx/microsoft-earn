--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- Cards.sql
-- Copyright (c) Microsoft Corporation. All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- dbo.Cards
--  The definition of the Earn Cards table
-- Columns:
--  Id int: The identity of the card.
--  UserId int: The identity of the user account associated with this card entity.
--  CardBrand int: The card's brand, i.e. 3 (American Express), 4 (Visa), or 5 (MasterCard).
--  LastFourDigits char(4): The last four digits of the PAN.
--  PartnerToken varchar(100): The token assigned to the card by its associated partner, i.e. American Express, Visa, or MasterCard.
--  FDCToken varchar(100): The token assigned to the card by FDC, if any.
--  Active bit: Specifies whether this card entity is currently active within the Earn program.
--  UtcAdded datetime: The UTC date and time at which the card was added.
--  UtcUpdated datetime: The UTC date and time at which the card was last updated. Populated automatically.
-- Remarks:
--  The FDCToken column is temporary and will be removed when Visa V5 implementation is complete enough to process statement credits directly via Visa.
create table dbo.Cards
(
    Id int not null identity(1000000000, 1),
    UserId int not null,
    CardBrand int not null,
    LastFourDigits char(4) not null,
    PartnerToken varchar(100) not null
        constraint CardsPartnerTokenDefault default '',
    FDCToken varchar(100) not null
        constraint CardsFDCTokenDefault default '',
    Active bit not null
        constraint CardsActiveDefault default 1,
    Flags int not null
        constraint FlagsDefault default 0x0,
    UtcAdded datetime not null
        constraint CardsUtcAddedDefault default getutcdate(),
    UtcUpdated datetime not null
        constraint CardsUtcUpdatedDefault default getutcdate()

    constraint CardsPrimary primary key clustered(Id),
    constraint CardUser foreign key(UserId) references dbo.Users(Id),
    constraint CardsCardBrandCheck check(CardBrand between 3 and 5)
);

GO

-- dbo.CardBrand_LastFourDigits_PartnerToken_UserId_Active_Id
--  Index used in adding or updating cards.
create nonclustered index CardBrand_LastFourDigits_PartnerToken_UserId_Active_Id
on Cards(CardBrand, LastFourDigits, PartnerToken, UserId, Active, Id);
GO

-- dbo.UserId_Active_CardBrand_LastFourDigits_PartnerToken_FDCToken
--  Index used in adding or updating cards.
create nonclustered index UserId_Active_CardBrand_LastFourDigits_PartnerToken_FDCToken
on Cards(UserId, Active, CardBrand, LastFourDigits, PartnerToken, FDCToken);
GO

-- dbo.CardBrand_PartnerToken_LastFourDigits_Id
--  Index used in adding or updating cards.
create nonclustered index CardBrand_PartnerToken_LastFourDigits_Id
on Cards(CardBrand, PartnerToken, LastFourDigits, Id);
GO

-- dbo.CardBrand_PartnerToken_Active_Id_UserId_LastFourDigits_FDCToken
--  Index used in adding or updating cards.
create nonclustered index CardBrand_PartnerToken_Active_Id_UserId_LastFourDigits_FDCToken
on Cards(CardBrand, PartnerToken, Active, Id, UserId, LastFourDigits, FDCToken);
GO

-- dbo.CardBrand_Flags_PartnerToken
--  Index used in adding or updating cards.
create nonclustered index CardBrand_Flags_PartnerToken
on Cards(CardBrand, Flags, PartnerToken);
GO

-- dbo.PartnerToken_CardBrand_Flags
--  Index used in adding or updating cards.
create nonclustered index CardBrand_PartnerToken_Flags
on Cards(CardBrand, PartnerToken, Flags);
GO
     
-- dbo.CardsUtcUpdatedTrigger
--  The update trigger that ensures that any update to a row in the Cards table will be reflected in the UtcUpdated column.
create trigger dbo.CardsUtcUpdatedTrigger on dbo.Cards for update as
begin
    set nocount on;

    if (not update(UtcUpdated))
    begin
        update Cards set UtcUpdated = getutcdate()
        from dbo.Cards, inserted
        where Cards.Id = inserted.Id;
    end
end

GO

-- CardsUnique constraint.
alter table dbo.Cards add constraint CardsUnique unique (UserId, CardBrand, LastFourDigits, PartnerToken);

GO

-- Cards Flags:
-- 0x0 : none
-- 0x1 : unfiltered mastercard
-- 0x2 : filtered mastercard