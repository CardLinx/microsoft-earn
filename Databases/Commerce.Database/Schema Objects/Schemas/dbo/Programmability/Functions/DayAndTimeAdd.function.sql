--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--IF object_id('DayAndTimeAdd') IS NOT NULL DROP FUNCTION DayAndTimeAdd
GO
CREATE FUNCTION DayAndTimeAdd (@DayAndTime int, @Minutes int, @Add7Days bit)
RETURNS int
BEGIN
  DECLARE @Day int = @DayAndTime / 10000
  DECLARE @Hour int = (@DayAndTime - @Day * 10000) / 100
  DECLARE @Minute int = @DayAndTime - @Day * 10000 - @Hour * 100
  DECLARE @Date datetime = dateadd(minute,@Minute + @Minutes,dateadd(hour,@Hour,convert(datetime, @Day + 5 + 7)))
  RETURN dbo.ToDayAndTime(@Date, @Add7Days)
END
GO