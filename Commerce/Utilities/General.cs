//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.Utilities
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides general utility methods used throughout the service.
    /// </summary>
    public static class General
    {
        /// <summary>
        /// Deserialize json into an object.
        /// </summary>
        /// <typeparam name="TObjectType">
        /// The type to deserialize the json string to.
        /// </typeparam>
        /// <param name="value">
        /// The string value to deserialize into an object.
        /// </param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public static TObjectType DeserializeJson<TObjectType>(string value)
        {
            return JsonConvert.DeserializeObject<TObjectType>(value);
        }

        /// <summary>
        /// Serializes the object into json.
        /// </summary>
        /// <typeparam name="TObjectType">
        /// The type of object to serialize.
        /// </typeparam>
        /// <param name="obj">
        /// The object to serialize.
        /// </param>
        /// <returns>
        /// The object, serialized into json.
        /// </returns>
        public static string SerializeJson<TObjectType>(TObjectType obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        /// <summary>
        /// Determines whether the two DateTime structs are comparable, i.e. close enough to be the same for Commerce services
        /// purposes.
        /// </summary>
        /// <param name="first">
        /// The first date to compare.
        /// </param>
        /// <param name="second">
        /// The second date to compare.
        /// </param>
        /// <returns>
        /// * True if the two DateTimes are comparable.
        /// * Else returns false.
        /// </returns>
        public static bool DateTimesComparable(DateTime first,
                                               DateTime second)
        {
            return Math.Abs((first - second).TotalSeconds) < 1;
        }

        /// <summary>
        /// Returns utc offset for a given time in specified time zone (day light saving time aware)
        /// </summary>
        /// <param name="utcValue">utc date time to find offset for</param>
        /// <param name="timeZoneId">the time zone id</param>
        /// <returns>the offset. format example: for -7 hours returns -700</returns>
        public static int? GetTimeZoneOffset(DateTime utcValue, string timeZoneId)
        {
            if (timeZoneId != null)
            {
                DateTime local = TimeZoneInfo.ConvertTimeFromUtc(utcValue, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
                int offset = (local - utcValue).Hours * 100 + (local - utcValue).Minutes;
                return offset;    
            }
            
            return null;
        }

        /// <summary>
        /// Converts integer to guid format
        /// </summary>
        /// <param name="value">input integer</param>
        /// <returns>guid from int</returns>
        public static Guid GuidFromInteger(int value)
        {
            string str = string.Concat("00000000-0000-0000-0000-", value.ToString().PadLeft(12, '0'));
            return new Guid(str);
        }

        /// <summary>
        /// Converts guid to integer format
        /// </summary>
        /// <param name="value">the guid</param>
        /// <returns>int from guid</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "integer", Justification = "Name correctly reflects the meaning.")]
        public static int IntegerFromGuid(Guid value)
        {
            string str = value.ToString().Split(new[] { '-' })[4];
            return int.Parse(str);
        }

        /// <summary>
        /// Converts 2 integers to hex string.
        /// Note: This logic will break when first int reaches its max value.
        /// </summary>
        /// <param name="first">
        /// The 1st integer value.
        /// </param>
        /// <param name="second">
        /// The 2nd integer value.
        /// </param>
        /// <returns>
        /// The hex string.
        /// </returns>
        /// <remarks>
        /// This operation is reversable. See HexStringToTwoIntegers.
        /// </remarks>
        public static string TwoIntegersToHexString(int first, int second)
        {
            long str = ((long)first * int.MaxValue) + second;
            return str.ToString("X");
        }

        /// <summary>
        /// Converts hex string to 2 integers.
        /// Note: This logic will break when first int reaches its max value.
        /// </summary>
        /// <param name="input">
        /// The hex string.
        /// </param>
        /// <returns>
        /// The tuple with 2 integers.
        /// </returns>
        public static Tuple<int, int> HexStringToTwoIntegers(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length != 16 || Regex.IsMatch(input, "[^a-fA-F0-9]"))
            {
                return null;
            }

            long num = long.Parse(input, NumberStyles.HexNumber);
            int first = (int)(num / int.MaxValue);
            int second = (int)(num - first * int.MaxValue);
            return new Tuple<int, int>(first, second);
        }

        /// <summary>
        /// Generates a short representation of the specified Guid.
        /// </summary>
        /// <param name="source">
        /// The Guid to convert to a short representation.
        /// </param>
        /// <returns>
        /// The short representation of the specified Guid.
        /// </returns>
        /// <remarks>
        /// This operation is not reversable.
        /// </remarks>
        public static string GenerateShortGuid(Guid source)
        {
            long accumulator = 1;
            foreach (byte b in source.ToByteArray())
            {
                accumulator *= ((int)b + 1);
            }

            return String.Format("{0:x}", accumulator - DateTime.Now.Ticks);
        }

        /// <summary>
        /// Generates a short representation of a new Guid.
        /// </summary>
        /// <returns>
        /// The short representation of a new Guid.
        /// </returns>
        /// <remarks>
        /// This operation is not reversable.
        /// </remarks>
        public static string GenerateShortGuid()
        {
            return GenerateShortGuid(Guid.NewGuid());
        }

        /// <summary>
        /// Determines if the specified client certificate is valid based on its serial number and a list of valid serial
        /// numbers.
        /// </summary>
        /// <param name="certificate">
        /// The certificate whose validity to check.
        /// </param>
        /// <param name="validSerialNumbers">
        /// The list of serial numbers for valid certificates.
        /// </param>
        /// <returns>
        /// * True if the specified certificate is valid.
        /// * Else returns false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// * Parameter certificate cannot be null.
        /// -OR-
        /// * Parameter validSerialNumbers cannot be null.
        /// </exception>
        public static bool IsPresentedCertValid(HttpClientCertificate certificate,
                                                Collection<string> validSerialNumbers)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate", "Parameter certificate cannot be null.");
            }

            if (validSerialNumbers == null)
            {
                throw new ArgumentNullException("validSerialNumbers", "Parameter validSerialNumbers cannot be null.");
            }

            bool result = false;

            // Certificate is valid if its serial number matches one of the listed valid serial numbers, regardless of case, or
            // if it is null, empty, or whitespace and one of the valid serial numbers is also null, empty, or whitespace.
            foreach (string serialNumber in validSerialNumbers)
            {
                if (String.Equals(certificate.SerialNumber, serialNumber, StringComparison.OrdinalIgnoreCase) == true ||
                    (String.IsNullOrWhiteSpace(certificate.SerialNumber) == true &&
                     String.IsNullOrWhiteSpace(serialNumber) == true))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieve a certificate via its thumbprint
        /// It is assumed to be in LocalMachine store
        /// </summary>
        /// <param name="thumbprint">
        /// Thumbprint of the certificate
        /// </param>
        /// <returns>
        /// X509 Certificate or Null
        /// </returns>
        public static X509Certificate2 RetrieveCertificateByThumbprint(string thumbprint)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

            X509Certificate2Enumerator enumerator = certCollection.GetEnumerator();

            X509Certificate2 cert = null;

            while (enumerator.MoveNext())
            {
                cert = enumerator.Current;
            }
            store.Close();

            return cert;
        }

        /// <summary>
        /// Gets a value indicating whether the process is running in Azure.
        /// </summary>
        public static bool RunningInAzure
        {
            get
            {
                if (runningInAzureSet == false)
                {
                    runningInAzure = RoleEnvironment.IsAvailable;
                    runningInAzureSet = true;
                }

                return runningInAzure;
            }
        }
        private static bool runningInAzure = false;
        private static bool runningInAzureSet = false;
        
        /// <summary>
        /// The name of the Commerce log source.
        /// </summary>
        public const string CommerceLogSource = "LomoCommerce";
    }
}