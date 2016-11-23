--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- AddOrUpdateProvider.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- AddOrUpdateProvider
--  Adds or updates a provider.
-- Parameters:
--  @globalProviderID varchar(100): The ID assigned within the wider system to this provider.
--  @name nvarchar(100): The provider's name.
-- Returns:
--  The ID of the affected provider record and whether it was newly created.
create procedure dbo.AddOrUpdateProvider @globalProviderID varchar(100),
                                         @name nvarchar(100)
as
  set nocount on;
  begin try
    begin transaction
        
        -- Gather existing information for this provider, if any.
        declare @providerId int;
        declare @existingName nvarchar(100);
        select @providerId = Providers.Id, @existingName = Name from dbo.Providers where GlobalID = @globalProviderID;

        -- If the provider has not already been added to the Earn Providers table, add it.
        declare @created bit = 0;
        if (@providerId is null)
          begin
            insert into dbo.Providers(GlobalID, Name) values (@globalProviderID, @name);
            set @providerId = scope_identity();
            set @created = 1;
          end
        -- Otherwise, update the existing entity if necessary.
        else if (@name <> @existingName)
          begin
             update dbo.Providers set Name = @name where Providers.Id = @providerId;
          end

        -- Return the id of the affected provider record.
        select @providerId as ProviderId, @created as Created;

    commit transaction
  end try
  begin catch
    -- Rollback the transaction and then re-raise the error.
    if (@@trancount > 0) rollback transaction;
    declare @errorMessage nvarchar(4000) = ERROR_MESSAGE();
    declare @errorSeverity int = ERROR_SEVERITY();
    raiserror(@errorMessage, @errorSeverity, 1)
  end catch
GO