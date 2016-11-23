//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// // <summary>
// // Specialized list for Analytics Items
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------


using Newtonsoft.Json;

namespace AnalyticsClient
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Linq;

    using Lomo.Logging;

    /// <summary>
    /// A list of items 
    /// </summary>
    public class AnalyticsItemList : List<AnalyticsItem>
    {
        #region Methods
        
        /// <summary>
        /// Parse a list of items form an Xml String
        /// </summary>
        /// <param name="str">
        /// the input string
        /// </param>
        /// <returns>
        /// a list of items
        /// </returns>
        public static AnalyticsItemList Deserialize(string str)
        {
            try
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                return JsonConvert.DeserializeObject<AnalyticsItemList>(str, serializerSettings);

            }
            catch (Exception e)
            {
                Log.Verbose("Original String:{0}; Error: {1}", str, e);
                //Don't do anything this is expected until all the systems will migrate to json
            }
            // Backward compatability to xml
            var list = new AnalyticsItemList();
            try
            {
                XElement document = XElement.Parse(str);
                foreach (XElement node in document.Elements())
                {
                    AnalyticsItem item = AnalyticsItem.Parse(node);
                    if (item != null)
                    {
                        list.Add(item);
                    }
                }
            }
            catch (Exception e)
            {
                // log and ignore the invalid message
                Log.Error(e, "Original String:{0}", str);
            }

            return list;
        }

        public string ToJsonString()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(this, Formatting.None, serializerSettings);
        }

        #endregion
    }
}