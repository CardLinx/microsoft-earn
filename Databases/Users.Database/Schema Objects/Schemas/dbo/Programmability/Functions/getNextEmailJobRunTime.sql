--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE FUNCTION [dbo].[getNextEmailJobRunTime](@ScheduleType nvarchar(50))
RETURNS datetime
AS
BEGIN
  DECLARE @NextRunTime datetime  
  SELECT @NextRunTime = CASE @ScheduleType
						    WHEN 'Daily' THEN DATEADD(hh, 18, DATEADD(dd, DATEDIFF(dd, -1, GETUTCDATE()), 0))
			                WHEN 'Weekly' THEN DATEADD(hh, 18, DATEADD(wk, DATEDIFF(wk, 0, GETUTCDATE()), 7)) -- RETURN THE FOLLOWING MONDAY
			                WHEN 'Monthly' THEN DATEADD(HH, 18, DATEADD(mm, 1, DATEADD(mm, DATEDIFF(mm, 0, GETUTCDATE()), 0))) -- RETURN THE BEGINNING DAY OF FOLLOWING MONTH
					        END
  RETURN @NextRunTime;
END


GO

