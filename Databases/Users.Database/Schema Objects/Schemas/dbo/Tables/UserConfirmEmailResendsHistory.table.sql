--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--IF object_id('UserConfirmEmailResend') IS NOT NULL DROP TABLE UserConfirmEmailResend
GO
CREATE TABLE UserConfirmEmailResendsHistory
(
	UserId				uniqueidentifier NOT NULL  
	,PartitionId		int				 NOT NULL
	,EntityType			tinyint          NOT NULL 
	,CreatedDate		datetime         NOT NULL  CONSTRAINT DF_UserConfirmEmailResend_CreatedDate DEFAULT getUTCdate()

	CONSTRAINT PKC_UserConfirmEmailResendsHistory_UserId_PartitionId PRIMARY KEY CLUSTERED (UserId, PartitionId, CreatedDate)
)
GO

GO