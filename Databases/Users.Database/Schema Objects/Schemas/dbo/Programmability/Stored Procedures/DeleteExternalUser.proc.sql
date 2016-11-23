--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
----USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('DeleteExternalUser') IS NOT NULL DROP PROCEDURE DeleteExternalUser
GO
CREATE PROCEDURE DeleteExternalUser
	@ExternalId nvarchar(100), @PartitionId int, @AuthProvider tinyint 

AS
set nocount on

DELETE From dbo.ExternalUsers 
WHERE 
	ExternalId = @ExternalId 
	AND AuthProvider = @AuthProvider 
	AND PartitionId  = @PartitionId

GO