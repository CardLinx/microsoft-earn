--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[RewardProgramReimbursementTenderIds]
(
	[RewardProgramId] INT NOT NULL 
	,[ReimbursementTenderId] INT NOT NULL

	
   CONSTRAINT PKC_RewardProgramReimbursementTenderIds_RewardProgramId_ReimbursementTenderId PRIMARY KEY CLUSTERED (RewardProgramId, ReimbursementTenderId)
   
  ,CONSTRAINT FK_RewardProgramReimbursementTenderIds_RewardProgramId_RewardProgram_Id FOREIGN KEY (RewardProgramId) REFERENCES RewardProgram (Id)
  ,CONSTRAINT FK_RewardProgramReimbursementTenderIds_ReimbursementTenderId_ReimbursementTender_Id FOREIGN KEY (ReimbursementTenderId) REFERENCES ReimbursementTender (Id)
)

GO