--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- GetOfferByGlobalID.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- GetOfferByGlobalID
--  Retrieves the offer with the specified global ID.
-- Parameters:
--  @globalOfferID uniqueidentifier: The global ID of the offer to retrieve.
-- Returns:
--  * The offer with the specified global ID if it exists.
--  * Otherwise returns nothing.
create procedure dbo.GetOfferByGlobalID @globalOfferID uniqueidentifier
as
  set nocount on;

  select Offers.Id as OfferId, Providers.GlobalID as GlobalProviderID, OfferType, PercentBack, Active from dbo.Offers
    join dbo.Providers on Providers.Id = ProviderId
    where Offers.GlobalID = @globalOfferID;

GO