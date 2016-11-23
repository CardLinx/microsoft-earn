--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
--run this manually on Commerce DB in INT environment

use master
go

-- login should be created in master db
--sys.server_principals does not work in Azure SQL
--IF NOT EXISTS  (SELECT name  FROM master.sys.server_principals WHERE name = 'dotmExec')
IF NOT EXISTS  (select * from sys.sql_logins WHERE name = 'dotmExec')
BEGIN
	CREATE LOGIN dotmExec WITH PASSWORD = 'TODO_SET_PASSWORD_HERE';
END


-- following statements should be executed in commerce int db
use commerce
go

IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = 'dotmExec' and Type = 'S')
BEGIN
	CREATE USER dotmExec FOR LOGIN dotmExec;
END


if not exists (select * from sys.database_principals where name='db_execproc' and Type = 'R')
begin
	CREATE ROLE db_execproc AUTHORIZATION dotmExec;
	GRANT EXECUTE TO db_execproc;
	GRANT EXECUTE ON SCHEMA::dbo TO db_execproc;
end


execute sp_addrolemember @rolename = 'db_execproc', @membername = 'dotmExec';