--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--IF object_id('ToDayAndTime') IS NOT NULL DROP FUNCTION ToDayAndTime
GO
CREATE FUNCTION ToDayAndTime (@Date datetime, @Add7Days bit)
RETURNS int
BEGIN
  RETURN (datepart(weekday,@Date) + CASE WHEN @Add7Days = 1 THEN 7 ELSE 0 END) * 10000 + datepart(hour,@Date) * 100 + datepart(minute,@Date)
END
GO