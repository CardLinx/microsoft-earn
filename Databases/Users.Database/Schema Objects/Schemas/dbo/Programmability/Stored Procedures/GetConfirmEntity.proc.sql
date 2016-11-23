--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
----USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('GetConfirmEntity') IS NOT NULL DROP PROCEDURE GetConfirmEntity
CREATE PROCEDURE GetConfirmEntity
	@UserIdHash			nvarchar(128),
	@PartitionId		int,
	@EntityType			tinyint
	
AS
set nocount on

	SELECT TOP(1) * 
	From ConfirmationCodes c
	WHERE c.UserIdHash = @UserIdHash AND 
		  c.PartitionId = @PartitionId AND
		  c.EntityType = @EntityType
	ORDER BY c.CreatedDate desc
GO