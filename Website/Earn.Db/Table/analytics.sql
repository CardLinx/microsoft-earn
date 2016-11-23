--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--
CREATE TABLE [dbo].[analytics]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [AuthenticatedUserId] VARCHAR(70) NULL, 
    [BrowserId] UNIQUEIDENTIFIER NOT NULL, 
    [IPAddress] VARCHAR(70) NULL, 
    [SessionId] UNIQUEIDENTIFIER NOT NULL, 
    [DeviceType] VARCHAR(50) NULL, 
    [UserAgent] NTEXT NULL, 
    [PageTitle] VARCHAR(400) NULL, 
    [ServerTimeStamp] DATETIME2 NOT NULL, 
    [PageUrl] VARCHAR(1000) NULL, 
    [EventId] VARCHAR(50) NULL, 
    [EventType] VARCHAR(50) NULL, 
    [EventInfo] VARCHAR(1000) NULL, 
    [FlightId] VARCHAR(50) NULL, 
    [NewUser] BIT NULL, 
    [IsAuthenticated] BIT NULL, 
    [cmp_source] VARCHAR(50) NULL, 
    [cmp_name] VARCHAR(50) NULL, 
    [cmp_ref] VARCHAR(50) NULL
)