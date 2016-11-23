//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.FirstDataClient
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a First Data Acknowledgment file.
    /// </summary>
    public class Acknowledgment
    {
        /// <summary>
        /// Gets the Acknowledgment file's Detail Acknowledgment records.
        /// </summary>
        public Collection<DetailAcknowledgment> DetailAcknowledgments
        {
            get
            {
                return detailAcknowledgments;
            }
        }

        /// <summary>
        /// Gets the Acknowledgment file's General Acknowledgment records.
        /// </summary>
        public Collection<GeneralAcknowledgment> GeneralAcknowledgments
        {
            get
            {
                return generalAcknowledgments;
            }
        }

        /// <summary>
        /// backup collection for DetailAcknowledgments public property
        /// </summary>
        private Collection<DetailAcknowledgment> detailAcknowledgments = new Collection<DetailAcknowledgment>();

        /// <summary>
        /// backup collection for GeneralAcknowledgment public property
        /// </summary>
        private Collection<GeneralAcknowledgment> generalAcknowledgments = new Collection<GeneralAcknowledgment>();
    }
}