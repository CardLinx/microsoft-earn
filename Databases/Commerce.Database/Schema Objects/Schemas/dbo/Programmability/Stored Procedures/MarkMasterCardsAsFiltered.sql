--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- MarkMasterCardsAsFiltered.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- MarkMasterCardsAsFiltered
--  Marks MasterCards with the specified partner token as filtered.
-- Parameters:
--  @partnerTokens ListOfStrings READONLY: The list of partner tokens for cards to mark as filtered.
-- Returns:
--  The list of unfiltered MasterCards, if any exist.
--  Else nothing is returned.
create procedure dbo.MarkMasterCardsAsFiltered @partnerTokens ListOfStrings READONLY
as
  set nocount on;
  begin try
    begin transaction

      -- Change the flag on the newly filtered MasterCards to indicate they're now filtered.
      update dbo.Cards set Flags = Flags & 0xFFFFFFF0 | 0x2 from dbo.Cards -- 0x1 = complement of unfiltered MasterCards, 0x2 = filtered MasterCards
        with (index(CardBrand_PartnerToken_Flags))
        join @partnerTokens as PartnerTokenList on Cards.PartnerToken = PartnerTokenList.Id
        where CardBrand = 5

    commit transaction;
  end try
  begin catch
    -- Rollback the transaction and then re-raise the error.
    if (@@trancount > 0) rollback transaction;
    declare @errorMessage nvarchar(4000) = ERROR_MESSAGE();
    declare @errorSeverity int = ERROR_SEVERITY();
    raiserror(@errorMessage, @errorSeverity, 1)
  end catch
GO