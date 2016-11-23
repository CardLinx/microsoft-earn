--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- BuildDiscountSummary.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- BuildDiscountSummary
--  Builds the discount summary string based on the offer type and offer percent back, when applicable.
-- Parameters:
--  @offerType int: The type of the offer, i.e. Earn (0) or Burn (1)
--  @percentBack money: The percent of the settlement amount to apply as Earn credits, or the percent of dollars spent for which
--    Earn credits can be used instead.
-- Returns:
--  The formatted discount summary string.
create function BuildDiscountSummary(@offerType int,
                                     @percentBack money)
returns nvarchar(255) with schemabinding
begin
  return case when @offerType = 0 then 'Earn ' + convert(varchar(3), cast(@percentBack as int)) + '%' else 'Save up to 100%' end;
end
GO