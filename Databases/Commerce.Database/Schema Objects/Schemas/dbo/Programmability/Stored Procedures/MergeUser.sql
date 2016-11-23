--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- MergeUser.sql
-- Copyright (c) Microsoft Corporation. All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- [PLANNED] AddUser
-- [DEPRECATED] MergeUser
--  Adds an Earn user account for the specified Global User ID.
-- Parameters:
--  @globalId uniqueIdentifier: The Global User ID for the new user account.
--  [DEPRECATED] @analyticsEventId uniqueIdentifier = null: The event ID assigned to the creation of this user account. Default value is null.
-- Returns:
--  If the GlobalId has not already been used to create an account, returns the int identity of the new user account.
--  [PLANNED] Else throws an error.
--  [DEPRECATED] Else returns the int Identity for the existing account.
-- Remarks:
--  The name of this stored procedure will change from MergeUser to AddUser before the national launch.
--  The @analyticsEventId parameter is deprecated and will be removed before national launch.
--  The return behavior in the error case is deprecated and will change before national launch.
create procedure MergeUser @globalId uniqueIdentifier,
                           @analyticsEventId uniqueIdentifier = null as
    begin try
      set nocount on;
      begin transaction
        -- If the Global ID has not already been used to create an Earn account, do so.
        declare @id int = (select Id from dbo.Users where GlobalId = @globalId);
        if (@id is null)
        begin
          insert into dbo.Users(GlobalId) select @globalId;
          set @id = scope_identity();
        end
      commit transaction
  
      -- Return the Earn account identity for the Global ID.
      select @Id as Id;
    end try
    begin catch
      -- Rollback the transaction and then re-raise the error.
      if (@@trancount > 0) rollback transaction;
      declare @errorMessage nvarchar(4000) = error_message();
      declare @errorSeverity int = error_severity();
      raiserror(@errorMessage, @errorSeverity, 1);
    end catch

GO