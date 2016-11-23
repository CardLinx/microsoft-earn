--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE RemoveCardFromRewardPrograms
  @cardId int
 ,@globalUserId uniqueidentifier
 ,@rewardPrograms int
AS
set nocount on
-- RewardPrograms is deprecated. This sproc will be removed when the DAL is refactored.
    declare @userId int = (select Id from dbo.Users where GlobalId = @globalUserId);

    declare @cardOwnerId int;
    declare @active bit;
    select @cardOwnerId = UserId, @active = Active from dbo.Cards
     where Id = @cardId;

    if (@cardOwnerId <> @userId) RAISERROR('CardDoesNotBelongToUser', 16, 1)

    update dbo.Cards set Active = 0 where Id = @cardId;
GO