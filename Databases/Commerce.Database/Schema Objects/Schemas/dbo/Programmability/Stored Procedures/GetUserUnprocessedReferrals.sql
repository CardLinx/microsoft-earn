--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE GetUserUnprocessedReferrals
	@userId NVARCHAR(100),
	@referralEventId INT
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'GetUserUnprocessedReferrals'

    -- First, get the referralTypeCode from the userId.
	DECLARE @referralTypeCode NVARCHAR(255)
    SELECT
        @referralTypeCode = ReferralTypes.Code
    FROM
        [dbo].[ReferralTypes] AS ReferralTypes
    INNER JOIN
        [dbo].[RewardPayouts] AS RewardPayouts
    ON
        RewardPayouts.PayeeId = ReferralTypes.ReferrerId
    INNER JOIN
        [dbo].[Referrals] AS Referrals
     ON
        Referrals.RewardPayoutId = RewardPayouts.Id
    INNER JOIN
        [dbo].[ReferralEventRewards] AS ReferralEventRewards
    ON
        ReferralEventRewards.Id = Referrals.ReferralEventRewardId
    INNER JOIN
        [dbo].[Users] AS Users
    ON
        Users.GlobalId = Referrals.ReferredUserId
    WHERE
        Referrals.ReferredUserId = @userId
    AND
        ReferralEventRewards.ReferralEventId = @referralEventId

	SELECT
		RewardPayouts.Id,
		Rewards.RewardTypeId,
		Rewards.Properties,
		RewardPayouts.PayeeId,
		RewardPayouts.PayeeTypeId
	FROM
		[dbo].[Rewards] AS Rewards
	INNER JOIN
		[dbo].[ReferralEventRewards] AS ReferralEventRewards
	ON
		ReferralEventRewards.RewardId = Rewards.Id
	INNER JOIN
		[dbo].[ReferralTypes] AS ReferralTypes
	ON
		ReferralTypes.Id = ReferralEventRewards.ReferralTypeId
	INNER JOIN
		[dbo].[RewardPayouts] AS RewardPayouts
	ON
		RewardPayouts.RewardId = Rewards.Id
	INNER JOIN
		[dbo].[Referrals] AS Referrals
	ON
		Referrals.RewardPayoutId = RewardPayouts.Id
	WHERE
		ReferralTypes.Code = @referralTypeCode
	AND
		ReferralEventRewards.ReferralEventId = @referralEventId
	AND
		Referrals.ReferredUserId = @userId
	AND
		RewardPayouts.RewardPayoutStatusId = 0 -- Unprocessed

SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
EXECUTE dbo.LogEvent @Process=@SP,@Status='Info',@Milliseconds=@Milliseconds
GO