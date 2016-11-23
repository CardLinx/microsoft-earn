--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE VIEW EmailSubscriptionsCurrentView
AS
	SELECT 
		es.UserId, es.PartitionId, es.LocationId, es.IsActive, es.SubscriptionType, usr.Email, es.CreatedDate, es.UpdatedDate, Version = 0
	FROM
		 dbo.EmailSubscriptions es INNER JOIN dbo.ValidUsersView usr ON es.UserId = usr.Id
GO