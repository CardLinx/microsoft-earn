//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Schema.Offers
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using ProtoBuf;

    /// <summary>
    /// Specifies the conversion criteria for an offer
    /// </summary>
    [DataContract]
    [ProtoContract]
    public class Conversion
    {
        /// <summary>
        /// Get or set conversion segment which contains the expression to determine conversion
        /// Expression may be like "IsO365User == true"
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "segment_id")]
        [ProtoMember(1)]

        public int SegmentId { get; set;}

        /// <summary>
        /// Get or set the Microsoft store line offer ids. If we are running a special promotion in 
        /// store like Work and Play bundle then these bundle are assigned special codes which can use 
        /// to determine who brought that bundle
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "store_line_offer_ids")]
        [ProtoMember(2)]
        public IList<string> StoreLineOfferIds { get; set; }


        /// <summary>
        /// Get or set Epiphany campaign id. Epiphany is email provider we use to send offer emails.
        /// It identifies the Epiphany campaign id in their dashboard 
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "epiphany_campaign_id")]
        [ProtoMember(3)]
        public string EpiphanyCampaignId { get; set; }

        /// <summary>
        /// Get or set Epiphany treatment code. Epiphany is email provider we use to send offer emails.
        /// If this is an email offer then this code identifies the email template in Epipahny campaign dashboard. (http://osd-bici-de/#CampaignPerformance/MRT/CampaignID/376065)
        /// If a campaign consist of 3 offers then for each offer there is a separate treatment code in Epiphany system and each treamtment code has its own email template
        /// When we get analytics logs (email sent, open, click) from Epipahny we get the campaign id and treatment code which help us identifiy the logs are for which campaign 
        /// and for which offer within the campaign
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "epiphany_treatment_code")]
        [ProtoMember(4)]
        public string EpiphanyTreatmentCode { get; set; }
        
        /// <summary>
        /// Get or set Epiphany pid code. In Epiphany system each campaign is divided into tactics. Each tactic has its own PID code. There should be one to one
        /// correspondence between offer, tactic, pid, email template. PID identified an email template which is used to send email to the user. This email template
        /// may have some dynamic paramters like reward code, reward amount. The value of these dynamic parameters comes from a flat file (tab or csv delimited) which is 
        /// dropped in a Epiphany file share. The name of this file should match PID code. This is how Epiphany determine which template to use
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "epiphany_pid")]
        [ProtoMember(5)]
        public string EpiphanyPid { get; set; }

        /// <summary>
        /// Get or set schema of PID file which contains dynamic paramter values for ANIDs which is substituted in the email template by Epiphany system
        /// PID file can have a schema like the following. This schema will be different for different templates. The value of these field can come from offer
        /// meta data or email template specific metadata or Epiphany defined system parameters. Epiphany defined system parameters can be like user name which Epiphany
        /// substitute with actual user name
        /// Anid    RewardCode  RewardAmount
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "epiphany_pid_file_column_names")]
        [ProtoMember(6)]
        public IList<string> EpiphanyPidFileColumnNames { get; set; }
        
        /// <summary>
        /// Get or set the number of days in which user should convert (e.g. become O365 user) after receiving email, for attribution and reward 
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "days_to_convert_after_impression")]
        [ProtoMember(7)]
        public string DaysToConvertAfterImpression { get; set; }

    }
}