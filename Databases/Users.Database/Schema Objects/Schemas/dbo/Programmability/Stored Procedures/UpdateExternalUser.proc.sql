--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
----USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('UpdateExternalUser') IS NOT NULL DROP PROCEDURE SuppressUser
GO
CREATE PROCEDURE UpdateExternalUser
  @NewUserId uniqueidentifier, @ExternalId nvarchar(100), @PartitionId int, @AuthProvider tinyint 

AS
set nocount on
IF NOT EXISTS (SELECT * FROM dbo.ExternalUsers WHERE ExternalId = @ExternalId AND AuthProvider = @AuthProvider AND PartitionId  = @PartitionId) 
  RAISERROR('External User Does not exist',18,127)

UPDATE dbo.ExternalUsers
  SET UserId = @NewUserId
     ,UpdatedDate = getUTCdate()
  WHERE ExternalId = @ExternalId AND AuthProvider = @AuthProvider AND PartitionId  = @PartitionId

SELECT * FROM dbo.ExternalUsers WHERE ExternalId = @ExternalId AND AuthProvider = @AuthProvider AND PartitionId  = @PartitionId
GO