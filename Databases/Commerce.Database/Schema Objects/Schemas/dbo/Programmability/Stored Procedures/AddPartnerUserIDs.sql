--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
---------------------------------------------------------------------------------------------------------------------------------
-- AddPartnerUserIDs.sql
-- Copyright (c) Microsoft Corporation.  All rights reserved.  
---------------------------------------------------------------------------------------------------------------------------------

-- AddPartnerUserIDs
--  Adds new partner user IDs unless a different Active ID is already present.
-- Parameters:
--  @userId: The ID assigned to the user in the Earn system.
--  @partnerUserIDRecords PartnerUserIDRecords: The list of partner user IDs.
-- Remarks:
--  Partner user ID records are deactivated instead of deleted to maintain history and referential integrity.
create procedure dbo.AddPartnerUserIDs @userID int,
                                       @partnerUserIDRecords PartnerUserIDRecords READONLY
as
  set nocount on;

    insert into dbo.PartnerUserIDs(UserId, [Partner], PartnerUserID)
      select @userId, partnerUserIDRecords.[Partner], partnerUserIDRecords.PartnerUserID
          from @partnerUserIDRecords as partnerUserIDRecords
            left outer join dbo.PartnerUserIDs
              on PartnerUserIDs.[Partner] = partnerUserIDRecords.[Partner] and
                  PartnerUserIDs.PartnerUserID = partnerUserIDRecords.PartnerUserID
      where PartnerUserIDs.[Partner] is null and not exists(select PartnerUserIDs.[Partner] from dbo.PartnerUserIDs
                                                              where PartnerUserIDs.UserId = @userID and Active = 1 and
                                                                    [Partner] = partnerUserIDRecords.[Partner]);
GO