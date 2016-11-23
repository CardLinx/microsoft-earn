--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('GetUserByUserId') IS NOT NULL DROP PROCEDURE GetUserByUserId
GO
CREATE PROCEDURE GetUserByUserId
  @Id uniqueidentifier, @PartitionId int
AS
set nocount on
SELECT * FROM dbo.Users WHERE Id = @Id AND PartitionId = @PartitionId
GO