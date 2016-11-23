--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE [dbo].[GetMerchantSubscriptionsByUserId]
  @UserId uniqueidentifier, @PartitionId int
AS
set nocount on	
BEGIN
		SELECT *
		FROM dbo.MerchantSubscriptions sub 
		WHERE sub.UserId = @UserId AND sub.PartitionId = @PartitionId
END
GO