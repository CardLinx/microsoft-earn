--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE PROCEDURE GetNextValueFromSequence
	@SequenceName NVARCHAR(255)
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'GetNextValueFromSequence'

	EXECUTE sp_getapplock 'Commerce_Sequences_Lock', 'exclusive'

    DECLARE @NewVal INT

    UPDATE [dbo].[Sequences]
    SET @NewVal = CurrentValue = CurrentValue + Increment
    WHERE SequenceName = @SequenceName
     
    IF @@rowcount = 0 
	BEGIN
		PRINT 'Sequence does not exist'
		RETURN
	END
 
    SELECT @NewVal AS NextValue

SET @Milliseconds = datediff(millisecond,@st,getUTCdate())
EXECUTE dbo.LogEvent @Process=@SP,@Status='Info',@Milliseconds=@Milliseconds
GO