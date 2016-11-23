//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Microsoft.HolMon.Common.Utils
{
    using System.Runtime.Remoting.Metadata.W3cXsd2001;

    /// <summary>
    /// Helper class to convert bytes to a hex string and back.
    /// </summary>
    public class BytesHexConverter
    {
        public static byte[] ToBytes(string value)
        {
            return SoapHexBinary.Parse(value).Value;
        }

        public static string ToHexString(byte[] value)
        {
            return new SoapHexBinary(value).ToString();
        }
    }
}