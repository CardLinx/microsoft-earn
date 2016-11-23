--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE VIEW dbo.MSSVPayouts
AS

SELECT Users.GlobalId AS UserId,
	   DistributionDateUtc,
	   CONVERT(MONEY, Amount / 100.0) AS Amount
FROM dbo.MSSVDistributions
INNER JOIN dbo.Users ON Users.Id = MSSVDistributions.UserId

GO