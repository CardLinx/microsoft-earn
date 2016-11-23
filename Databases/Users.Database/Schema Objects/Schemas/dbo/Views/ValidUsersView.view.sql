--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE VIEW ValidUsersView
AS	
SELECT * FROM Users usr WHERE usr.IsSuppressed = 0;
GO