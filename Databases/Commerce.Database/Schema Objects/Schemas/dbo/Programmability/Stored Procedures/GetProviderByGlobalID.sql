--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- GetProviderByGlobalID.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- GetProviderByGlobalID
--  Retrieves the provider with the specified global ID.
-- Parameters:
--  @globalProviderID varchar(100): The global ID of the provider to retrieve.
-- Returns:
--  * The provider with the specified global ID if it exists.
--  * Otherwise returns nothing.
create procedure dbo.GetProviderByGlobalID @globalProviderID varchar(100)
as
  set nocount on;

  -- First gather the provider's information.
  declare @providerId int;
  declare @name nvarchar(100);
  select @providerId = Providers.Id, @name = Name from dbo.Providers where GlobalID = @globalProviderID;

  -- If the provider existed, return its information.
  if (@providerId is not null)
    begin
      select @providerId as ProviderId, @name as Name;
    end

GO