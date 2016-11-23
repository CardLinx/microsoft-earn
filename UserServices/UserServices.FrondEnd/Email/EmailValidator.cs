//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
// <summary>
//   The email validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UserServices.FrondEnd.Email
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    using Lomo.Logging;

    /// <summary>
    /// The email validator.
    /// </summary>
    public class EmailValidator
    {
        #region Public Methods and Operators

        /// <summary>
        /// Validates the email format
        /// </summary>
        /// <param name="emailAddress">
        /// The email address to validate.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsValidEmailFormat(string emailAddress)
        {
            try
            {
                bool invalid = false;
                if (string.IsNullOrEmpty(emailAddress))
                {
                    return false;
                }

                // Use IdnMapping class to convert Unicode domain names.
                emailAddress = Regex.Replace(emailAddress, @"(@)(.+)$", (match) => DomainMapper(match, out invalid));
                if (invalid)
                {
                    return false;
                }

                // Return true if strIn is in valid e-mail format.
                return Regex.IsMatch(
                    emailAddress, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$", RegexOptions.IgnoreCase);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error while validating email address");
                return false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The domain mapper.
        /// </summary>
        /// <param name="match">
        /// The match.
        /// </param>
        /// <param name="invalid">
        /// The invalid.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string DomainMapper(Match match, out bool invalid)
        {
            invalid = false;

            // IdnMapping class with default property values.
            var idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }

            return match.Groups[1].Value + domainName;
        }

        #endregion
    }
}