--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
-- Generic table to store application specific parameters
-- DROP TABLE Parameters
GO
CREATE TABLE Parameters 
  (
     Id          varchar(100)   NOT NULL
    ,Date        datetime       NULL
    ,Number      float          NULL
    ,Char        varchar(255)   NULL
    
    ,UpdatedDate datetime       NULL
    ,UpdatedBy   nvarchar(255)  NULL
    
     CONSTRAINT PKC_Parameters_Id PRIMARY KEY CLUSTERED (Id)
  )
GO
CREATE TRIGGER ParametersInsUpd ON Parameters
FOR INSERT, UPDATE
AS
BEGIN
  UPDATE A
    SET UpdatedDate = getUTCdate() 
       ,UpdatedBy = left(system_user, 100)
    FROM dbo.Parameters A 
        JOIN Inserted B 
            ON B.Id = A.Id
  RETURN
END
GO