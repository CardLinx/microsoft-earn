--
-- Copyright (c) Microsoft Corporation. All rights reserved. 
-- Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
--

CREATE PROCEDURE ProcessPartnerRedeemedDealRecord
   @partnerId int
  ,@partnerRedeemedDealId nvarchar(255)
  ,@currency varchar(5)
  ,@purchaseDateTime datetime2(7)
  ,@redemptionEventId int
  ,@settlementAmount int = 0
  ,@discountAmount int
  ,@reversed bit
  ,@partnerDealId nvarchar(255)
  ,@partnerUserId nvarchar(255)
  ,@partnerCardSuffix char(2)
  ,@partnerMerchantId nvarchar(255)
  ,@partnerReferenceNumber nvarchar(255)
  ,@timeZoneOffset int = NULL
AS
set nocount on
DECLARE @st datetime = getUTCdate()
       ,@Milliseconds int
       ,@SP varchar(100) = 'ProcessPartnerRedeemedDealRecord'
  -- This is entirely deprecated but cannot be removed until FDC is no longer used for anything. (We don't need this because we never accept FDC deal anymore.)
GO