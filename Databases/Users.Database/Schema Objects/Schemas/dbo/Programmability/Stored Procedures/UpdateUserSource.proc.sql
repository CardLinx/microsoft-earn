--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('UpdateUserSource') IS NOT NULL DROP PROCEDURE UpdateUserSource
GO
CREATE PROCEDURE UpdateUserSource
  @Id uniqueidentifier, @PartitionId int, @Email nvarchar(100), @Source nvarchar(50)
AS
set nocount on
DECLARE @lock varchar(100) = 'Users_'+convert(varchar(50),@Id)

EXECUTE sp_getapplock @lock, 'exclusive' -- this prevents racing conditions on id

IF NOT EXISTS (SELECT * From dbo.Users Where Email = @Email AND  Id = @Id AND PartitionId = @PartitionId) 
  RAISERROR('User Does not exist',18,127)

UPDATE dbo.Users
  SET Source = @Source
     ,UpdatedDate = getUTCdate()
  WHERE Email = @Email AND Id = @Id AND PartitionId = @PartitionId

SELECT * FROM dbo.Users WHERE Id = @Id AND PartitionId = @PartitionId AND Email = @Email
GO