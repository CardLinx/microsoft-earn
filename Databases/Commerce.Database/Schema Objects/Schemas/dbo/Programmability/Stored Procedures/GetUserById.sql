--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- GetUserById.sql
-- Copyright (c) Microsoft Corporation. All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- GetUserById
--  Retrieves Earn account information for the account with the specified Global User ID.
-- Parameters:
--  [PLANNED] @globalId uniqueIdentifier: The Global User ID for the account whose information to retrieve.
--  [DEPRECATED] @globalUserId uniqueIdentifier: The Global User ID for the account whose information to retrieve.
-- Returns:
--  If an Earn account for the specified Global User ID exists, user account information is returned,
--  Else nothing is returned.
-- Remarks:
--  The name of the @globalUserId parameter will change to @globalId before national launch.
create procedure GetUserById @globalUserId uniqueIdentifier as

    set nocount on;
    declare @id int = (select Id from dbo.Users where GlobalId = @globalUserId);

    -- If the user exists, return its data.
    select Id as PerformanceId, GlobalId, null as AnalyticsEventId
      from dbo.Users
      where GlobalId = @globalUserId;

GO