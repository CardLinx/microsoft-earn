--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
-- Enrolls the specified user's cards (of specified brand only) into the specified reward programs.
CREATE PROCEDURE [dbo].[EnrollCardsInRewardPrograms]
  @userGlobalId uniqueidentifier,
  @rewardPrograms int, 
  @cardBrandIds ListOfInts READONLY
AS
-- RewardPrograms is deprecated. This will be removed when the DAL is refactored.
GO