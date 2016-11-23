--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--IF object_id('ConfirmationCodes') IS NOT NULL DROP TABLE ConfirmationCodes
GO
CREATE TABLE ConfirmationCodes
  (
	 UserIdHash			nvarchar(128)	 NOT NULL  
	,PartitionId		int              NOT NULL
    ,EntityId			nvarchar(100)    NOT NULL
    ,EntityType			tinyint          NOT NULL -- 1 for email, 2 for phone
	,UserId				uniqueidentifier NOT NULL  
    ,Code				int				 NOT NULL  
	,RetryCount			int				 NOT NULL  
    ,CreatedDate		datetime         NOT NULL  CONSTRAINT DF_Confirmations_CreatedDate DEFAULT getUTCdate()
    ,UpdatedDate		datetime         NOT NULL  CONSTRAINT DF_Confirmations_UpdatedDate DEFAULT getUTCdate()
	,ExpiredDate		datetime		 NOT NULL

	CONSTRAINT PKC_UserIdHash_PartitionId_EntityType_CreatedDate PRIMARY KEY CLUSTERED (UserIdHash, PartitionId, EntityType, CreatedDate)
  ) 
  --FEDERATED ON (PartitionId = PartitionId)
GO

GO