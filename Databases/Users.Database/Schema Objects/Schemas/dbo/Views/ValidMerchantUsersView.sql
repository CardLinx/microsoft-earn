--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE VIEW [dbo].[ValidMerchantUsersView]
AS
	SELECT 
		ms.UserId, ms.PartitionId, ms.SubscriptionType, ms.IsActive, ms.Preferences, ms.CreatedDate, ms.UpdatedDate, ms.ScheduleType
	FROM
		 dbo.MerchantSubscriptions ms INNER JOIN dbo.ValidUsersView usr ON ms.UserId = usr.Id
GO