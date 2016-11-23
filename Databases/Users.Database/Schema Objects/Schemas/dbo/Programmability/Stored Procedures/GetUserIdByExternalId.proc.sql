--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('GetUserIdByExternalId') IS NOT NULL DROP PROCEDURE GetUserByUserId
GO
CREATE PROCEDURE GetUserIdByExternalId
  @ExternalId nvarchar(100), @PartitionId int, @AuthProvider int
AS
set nocount on
SELECT UserId FROM dbo.ExternalUsers WHERE ExternalId = @ExternalId AND AuthProvider = @AuthProvider AND PartitionId = @PartitionId
GO