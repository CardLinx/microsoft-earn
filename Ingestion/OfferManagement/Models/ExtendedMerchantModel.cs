//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using OfferManagement.DataModel;

namespace OfferManagement.Models
{
    public class ExtendedPaymentModel
    {
        public ExtendedPaymentModel(Merchant m, string paymentType)
        {
            ParentMerchant = m;
            PaymentType = paymentType;
        }

        private string ValidKey { get { return "Valid" + PaymentType; } }
        private string SyncedKey { get { return "Synced" + PaymentType; } }
        private string PartiallySyncedKey { get { return "PartiallySynced" + PaymentType; } }

        private Merchant ParentMerchant;
        private string PaymentType;

        public bool Valid {
            get
            {
                return (ParentMerchant.ExtendedAttributes.ContainsKey(ValidKey) && ParentMerchant.ExtendedAttributes[ValidKey].Equals(bool.TrueString.ToLower()));
            }
            set
            {
                ParentMerchant.ExtendedAttributes[ValidKey] = value.ToString().ToLower();
            }
        }

        public bool Invalid { get { return !Valid; } }

        public bool Synced
        {
            get
            {
                return (ParentMerchant.ExtendedAttributes.ContainsKey(SyncedKey) && ParentMerchant.ExtendedAttributes[SyncedKey].Equals(bool.TrueString.ToLower()));
            }
            set
            {
                ParentMerchant.ExtendedAttributes[SyncedKey] = value.ToString().ToLower();
            }
        }

        public bool PartiallySynced
        {
            get
            {
                return (ParentMerchant.ExtendedAttributes.ContainsKey(PartiallySyncedKey) && ParentMerchant.ExtendedAttributes[PartiallySyncedKey].Equals(bool.TrueString.ToLower()));
            }
            set
            {
                ParentMerchant.ExtendedAttributes[PartiallySyncedKey] = value.ToString().ToLower();
            }
        }

        public bool NotSynced { get { return !Synced && !PartiallySynced; } }
    }

    public class ExtendedMerchantModel
    {
        public ExtendedPaymentModel Visa { get; private set; }
        public ExtendedPaymentModel MasterCard { get; private set; }
        public ExtendedPaymentModel Amex { get; private set; }

        public Merchant Merchant { get; private set; }

        // Location
        public bool ValidLocation { get; private set; }

        public string Name { get { return Merchant.Name; } }

        public string Id { get { return Merchant.Id; } }

        public bool Active { get { return Merchant.IsActive; } }

        public ExtendedMerchantModel(Merchant merchant)
        {
            this.Merchant = merchant;

            if (Merchant.ExtendedAttributes == null)
            {
                Merchant.ExtendedAttributes = new Dictionary<string, string>();
            }

            this.Visa = new ExtendedPaymentModel(merchant, "Visa");
            this.MasterCard = new ExtendedPaymentModel(merchant, "MasterCard");
            this.Amex = new ExtendedPaymentModel(merchant, "Amex");
            Validate();
        }

        private void Validate()
        {
            bool hasVisa = false;
            bool allVisaMidsAreValid = true;
            bool visaIsSynced = true;
            bool visaIsPartiallySynced = false;

            bool hasMasterCardMids = false;
            bool hasMasterCardLocations = false;

            bool allMasterCardMidsAreValid = true;
            bool allMasterCardLocationsAreValid = true;

            bool masterCardIsSynced = true;
            bool masterCardIsPartiallySynced = false;

            bool hasAmex = false;
            bool allAmexMidsAreValid = true;
            bool amexIsSynced = true;
            bool amexIsPartiallySynced = false;


            if (Merchant.Payments != null)
            {
                foreach (Payment p in Merchant.Payments)
                {
                    switch (p.Processor)
                    {
                        case PaymentProcessor.Visa:
                            {
                                hasVisa = IsPaymentContainsVisaMids(p);
                                allVisaMidsAreValid &= (p.PaymentMids != null && hasVisa)
                                    ? AreVisaMidsValid(p)
                                    : false;

                                visaIsSynced &= p.SyncedWithCommerce;
                                visaIsPartiallySynced |= p.SyncedWithCommerce;

                                break;
                            }
                        case PaymentProcessor.MasterCard:
                            {
                                bool midsFound = IsPaymentContainsMasterCardMids(p);
                                bool locationsFound = IsPaymentContainsMasterCardLocations(p);
                                hasMasterCardMids |= midsFound;
                                hasMasterCardLocations |= locationsFound;

                                if (midsFound)
                                {
                                    allMasterCardMidsAreValid &= (p.PaymentMids != null && midsFound)
                                        ? AreMasterCardMidsValid(p)
                                        : false;
                                }
                                else if (locationsFound)
                                {
                                    allMasterCardLocationsAreValid &= (p.PaymentMids != null && locationsFound)
                                        ? AreMasterCardLocationsValid(p)
                                        : false;
                                }
                                else
                                {
                                    allMasterCardLocationsAreValid = false;
                                    allMasterCardMidsAreValid = false;
                                }

                                masterCardIsSynced &= p.SyncedWithCommerce;
                                masterCardIsPartiallySynced |= p.SyncedWithCommerce;

                                break;
                            }
                        case PaymentProcessor.Amex:
                            {
                                hasAmex = IsPaymentContainsAmexMids(p);
                                allAmexMidsAreValid &= (p.PaymentMids != null && hasAmex)
                                    ? AreAmexMidsValid(p)
                                    : false;

                                amexIsSynced &= p.SyncedWithCommerce;
                                amexIsPartiallySynced |= p.SyncedWithCommerce;

                                break;
                            }
                    }
                }
            }

            Visa.Valid = (hasVisa && allVisaMidsAreValid);
            Visa.Synced = (hasVisa && visaIsSynced);
            Visa.PartiallySynced = (hasVisa && visaIsPartiallySynced);

            MasterCard.Valid = (hasMasterCardMids && hasMasterCardLocations && allMasterCardMidsAreValid && allMasterCardLocationsAreValid);
            MasterCard.Synced = (hasMasterCardMids && hasMasterCardLocations && masterCardIsSynced);
            MasterCard.PartiallySynced = (hasMasterCardMids && hasMasterCardLocations && masterCardIsPartiallySynced);

            Amex.Valid = (hasAmex && allAmexMidsAreValid);
            Amex.Synced = (hasAmex && amexIsSynced);
            Amex.PartiallySynced = (hasAmex && amexIsPartiallySynced);

            ValidLocation = (Merchant.Location != null && Merchant.Location.Latitude != 0 && Merchant.Location.Longitude != 0);
            Merchant.ExtendedAttributes["ValidLocation"] = ValidLocation.ToString().ToLower();
        }

        // Visa Mids
        private bool IsPaymentContainsVisaMids(Payment p)
        {
            return p.PaymentMids != null
                && p.PaymentMids.ContainsKey("VisaMid")
                && p.PaymentMids.ContainsKey("VisaSid");
        }

        private bool AreVisaMidsValid(Payment p)
        {
            return !string.IsNullOrWhiteSpace(p.PaymentMids["VisaMid"])
                && !string.IsNullOrWhiteSpace(p.PaymentMids["VisaSid"]);
        }

        // MC Mids
        private bool IsPaymentContainsMasterCardMids(Payment p)
        {
            return p.PaymentMids != null
                && p.PaymentMids.ContainsKey("AcquiringICA")
                && p.PaymentMids.ContainsKey("AcquiringMid");
        }

        private bool AreMasterCardMidsValid(Payment p)
        {
            return !string.IsNullOrWhiteSpace(p.PaymentMids["AcquiringICA"])
                && !string.IsNullOrWhiteSpace(p.PaymentMids["AcquiringMid"]);
        }

        // MC Locations
        private bool IsPaymentContainsMasterCardLocations(Payment p)
        {
            return p.PaymentMids != null
                && p.PaymentMids.ContainsKey("LocationID");
        }

        private bool AreMasterCardLocationsValid(Payment p)
        {
            return !string.IsNullOrWhiteSpace(p.PaymentMids["LocationID"]);
        }

        // Amex Mids
        private bool IsPaymentContainsAmexMids(Payment p)
        {
            return p.PaymentMids != null
                && p.PaymentMids.ContainsKey("SENumber");
        }

        private bool AreAmexMidsValid(Payment p)
        {
            return !string.IsNullOrWhiteSpace(p.PaymentMids["SENumber"]);
        }
    }
}