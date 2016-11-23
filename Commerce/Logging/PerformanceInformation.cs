//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Utilities;

    /// <summary>
    /// Stores and collates analytics information added during API calls.
    /// </summary>
    public class PerformanceInformation
    {
        /// <summary>
        /// Initializes a new instance of the PerformanceInformation class.
        /// </summary>
        public PerformanceInformation()
        {
            PerformanceInformationList = new List<string>();
        }

        /// <summary>
        /// Add to the performance information the specified label and its associated information.
        /// </summary>
        /// <param name="label">
        /// The label to prepend to the specified information. 
        /// </param>
        /// <param name="information">
        /// The information to add to performance. 
        /// </param>
        public void Add(string label, string information)
        {
            PerformanceInformationList.Add(String.Format("; {0}: {1}", label, information));
        }

        /// <summary>
        /// Builds a string that contains all performance information added during the API call.
        /// </summary>
        /// <returns>
        /// The collated performance information.
        /// </returns>
        public string Collate()
        {
            StringBuilder stringBuilder = new StringBuilder("Server ID: ");
            stringBuilder.Append(RequestInformationExtensions.ServerId);

            foreach (string item in PerformanceInformationList)
            {
                stringBuilder.Append(item);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets the performance information collection.
        /// </summary>
        private List<string> PerformanceInformationList { get; set; }
    }
}