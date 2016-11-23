--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- AddOrUpdateUser.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- AddOrUpdateUser
--  Adds or updates a user, including adding, deactivating, or reactivating partner user records as appropriate.
-- Parameters:
--  @globalUserID uniqueidentifier: The user's ID within the wider services space.
--  @partnerUserIDRecords PartnerUserIDRecords READONLY
-- Returns:
--  * If successful, the ID of the newly created or updated user account.
--  * Otherwise an exception is raised.
create procedure dbo.AddOrUpdateUser @globalUserID uniqueidentifier,
                                     @partnerUserIDRecords PartnerUserIDRecords READONLY
as
  set nocount on;
  begin try
    begin transaction

    -- If the user has not already been added to the Earn Users table, add it.
    declare @userId int;
    select @userId = Users.Id from dbo.Users where GlobalID = @globalUserID;
    declare @created bit = 0;
    if (@userId is null)
      begin
        insert into dbo.Users(GlobalID) values (@globalUserID);
        set @userId = scope_identity();
        set @created = 1;
      end
    -- Otherwise, since there are no rows to update, don't do anything.

    -- Process the partner user records.

    -- First, deactivate missing IDs, if any.
    exec dbo.DeactivatePartnerUserIDs @userId, @partnerUserIDRecords;

    -- Then reactivate previously missing IDs that are present again, if any.
    exec dbo.ReactivatePartnerUserIDs @userId, @partnerUserIDRecords;

    -- Finally, add new IDs.
    exec dbo.AddPartnerUserIDs @userId, @partnerUserIDRecords;

    -- Return the ID of the new user account.
    select UserId = @userId, @created as Created;
      
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