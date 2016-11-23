--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--USE FEDERATION Users (PartitionId = 0) WITH RESET, FILTERING = OFF
GO
--IF object_id('EmailUnsubscribeUrls') IS NOT NULL DROP TABLE EmailUnsubscribeUrls
CREATE TABLE EmailUnsubscribeUrls
(
	UserId      uniqueidentifier NOT NULL
   ,PartitionId int              NOT NULL
   ,CreatedDate datetime         NOT NULL  CONSTRAINT DF_EmailUnsubscribeUrls_CreatedDate DEFAULT getUTCdate()
   ,UpdatedDate datetime         NOT NULL  CONSTRAINT DF_EmailUnsubscribeUrls_UpdatedDate DEFAULT getUTCdate()
   ,UnsubscribeUrl nvarchar(max) NULL

   CONSTRAINT PKC_EmailUnsubscribeUrls_UserId_PartitionId PRIMARY KEY CLUSTERED (UserId, PartitionId)

   ,CONSTRAINT FK_EmailUnsubscribeUrls_UserId_Users FOREIGN KEY (UserId, PartitionId) REFERENCES Users (Id, PartitionId)
)
GO