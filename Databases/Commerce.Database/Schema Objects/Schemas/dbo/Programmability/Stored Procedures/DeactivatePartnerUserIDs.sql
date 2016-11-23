--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- DeactivatePartnerUserIDs.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- DeactivatePartnerUserIDs
--  Deactivates any partner user ID that no longer appears in the ID list.
-- Parameters:
--  @userId: The ID assigned to the user in the Earn system.
--  @partnerUserIDRecords PartnerUserIDRecords: The list of partner user IDs.
-- Remarks:
--  Partner user ID records are deactivated instead of deleted to maintain history and referential integrity.
create procedure dbo.DeactivatePartnerUserIDs @userID int,
                                              @partnerUserIDRecords PartnerUserIDRecords READONLY
as
  set nocount on;

  -- NOTE: We first check to see if there are any records to update and only then do we actually update. This does mean querying twice if there are updates, but
  --        it's more performant than allowing the update and then the trigger to be called when not needed, and deactivation will be a rare event.
  --        Alternate approaches such as table variables are also more expensive.

  if (exists(select PartnerUserIDs.UserId from dbo.PartnerUserIDs
                left outer join @partnerUserIDRecords as partnerUserIDRecords
                  on PartnerUserIDs.[Partner] = partnerUserIDRecords.[Partner] and
                    PartnerUserIDs.PartnerUserID = partnerUserIDRecords.PartnerUserID
                where PartnerUserIDs.UserId = @userId and Active = 1 and partnerUserIDRecords.PartnerUserID is null))
  begin
    update dbo.PartnerUserIDs set Active = 0
      from dbo.PartnerUserIDs
        left outer join @partnerUserIDRecords as partnerUserIDRecords
          on PartnerUserIDs.[Partner] = partnerUserIDRecords.[Partner] and
            PartnerUserIDs.PartnerUserID = partnerUserIDRecords.PartnerUserID
        where PartnerUserIDs.UserId = @userId and Active = 1 and partnerUserIDRecords.PartnerUserID is null;
  end
GO