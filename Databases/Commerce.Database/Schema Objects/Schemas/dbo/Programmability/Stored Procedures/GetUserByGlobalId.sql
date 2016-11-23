--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- GetUserByGlobalId.sql
-- Copyright (c) Microsoft Corporation. All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- GetUserByGlobalId
--  Retrieves Earn account information for the account with the specified Global User ID.
-- Parameters:
--  @globalId uniqueIdentifier: The Global User ID for the account whose information to retrieve.
--  @includePartnerUserIDs bit: Specifies whether to return the partner user IDs as well.
-- Returns:
--  If an Earn account for the specified Global User ID exists, user account information is returned.
--  Else nothing is returned.
create procedure GetUserByGlobalId @globalId uniqueIdentifier,
                                   @includePartnerUserIDs bit = 0
as
    set nocount on;

    -- First gather our internal user information.
    declare @userId int = (select Id from dbo.Users where GlobalId = @globalId);

  -- If the user existed, continue retrieving it.
    if (@userId is not null)
    begin
      -- Return our internal information.
        select Id from dbo.Users where GlobalId = @globalId;
  
      -- Then return partner IDs for the user.
      if (@includePartnerUserIDs = 1)
        begin
          select [Partner], PartnerUserID from dbo.PartnerUserIDs
            where UserId = @userId and Active = 1;
        end
    end
GO