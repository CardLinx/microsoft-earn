--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- ReactivatePartnerUserIDs.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- ReactivatePartnerUserIDs
--  Reactivates any partner user ID that was previously missing but is now present again, if any, unless a different Active ID
--   is already present.
-- Parameters:
--  @userId: The ID assigned to the user in the Earn system.
--  @partnerUserIDRecords PartnerUserIDRecords: The list of partner user IDs.
-- Remarks:
--  Partner user ID records are deactivated instead of deleted to maintain history and referential integrity.
create procedure dbo.ReactivatePartnerUserIDs @userId int,
                                              @partnerUserIDRecords PartnerUserIDRecords READONLY
as
  set nocount on;

  -- NOTE: We first check to see if there are any records to update and only then do we actually update. This does mean querying twice if there are updates, but
  --        it's more performant than allowing the update and then the trigger to be called when not needed, and deactivation will be a rare event.
  --        Alternate approaches such as table variables are also more expensive.

  -- Ensure that we don't update a record and cause two IDs to be active at the same time....
  if (exists(select PartnerUserIDs.UserId from dbo.PartnerUserIDs
               join @partnerUserIDRecords as partnerUserIDRecords
                  on PartnerUserIDs.[Partner] = partnerUserIDRecords.[Partner] and
                    PartnerUserIDs.PartnerUserID = partnerUserIDRecords.PartnerUserID
                where PartnerUserIDs.UserId = @userId and Active = 0 and
                      not exists(select PartnerUserIDs.UserId from dbo.PartnerUserIDs
                                  join @partnerUserIDRecords as partnerUserIDRecords
                                  on PartnerUserIDs.[Partner] = partnerUserIDRecords.[Partner]
                                 where PartnerUserIDs.UserId = @userId and Active = 1)))
    begin
      update dbo.PartnerUserIDs set Active = 1 from dbo.PartnerUserIDs
               join @partnerUserIDRecords as partnerUserIDRecords
                  on PartnerUserIDs.[Partner] = partnerUserIDRecords.[Partner] and
                    PartnerUserIDs.PartnerUserID = partnerUserIDRecords.PartnerUserID
                where PartnerUserIDs.UserId = @userId and Active = 0 and
                      not exists(select PartnerUserIDs.UserId from dbo.PartnerUserIDs
                                  join @partnerUserIDRecords as partnerUserIDRecords
                                  on PartnerUserIDs.[Partner] = partnerUserIDRecords.[Partner]
                                 where PartnerUserIDs.UserId = @userId and Active = 1);
    end

GO