//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Logging
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Security;
    using System.Security.Principal;
    using System.Text;

    /// <summary>
    /// <para>Provides exception formatting when not using the Exception Handling Application Block.</para>
    /// </summary>
    public class ExceptionFormatter
    {
        /// <summary>
        /// Name of the additional information entry that holds the Header.
        /// </summary>
        private const string Header = "Exception";

        /// <summary>
        /// Exception Summary header
        /// </summary>
        private const string ExceptionSummary = "Exception Summary";

        /// <summary>
        /// Exception Details header
        /// </summary>
        private const string ExceptionDetails = "Details";

        /// <summary>
        /// Exception type header
        /// </summary>
        private const string ExceptionType = "Type";

        /// <summary>
        /// Exception trace header
        /// </summary>
        private const string ExceptionStackTraceDetails = "Trace";

        /// <summary>
        /// Section seperator
        /// </summary>
        private const string SectionSeparator = "=================================================";

        /// <summary>
        /// Additional info collection
        /// </summary>
        private readonly NameValueCollection additionalInfo;

        /// <summary>
        /// application name
        /// </summary>
        private readonly string applicationName;

        /// <summary>
        /// Initializes a new instance of the ExceptionFormatter class.
        /// </summary>
        public ExceptionFormatter()
            : this(new NameValueCollection(), string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExceptionFormatter class.
        /// with the additional information and the application name.
        /// </summary>
        /// <param name="additionalInfo">
        /// <para>
        /// The additional information to log.
        /// </para>
        /// </param>
        /// <param name="applicationName">
        /// <para>
        /// The application name.
        /// </para>
        /// </param>
        public ExceptionFormatter(NameValueCollection additionalInfo, string applicationName)
        {
            this.additionalInfo = additionalInfo;
            this.applicationName = applicationName;
        }

        /// <summary>
        /// <para>
        /// Get the formatted message to be logged.
        /// </para>
        /// </summary>
        /// <param name="exception">
        /// <para>
        /// The exception object whose information should be written to log file.
        /// </para>
        /// </param>
        /// <returns>
        /// <para>
        /// The formatted message.
        /// </para>
        /// </returns>
        public string GetMessage(Exception exception)
        {
            try
            {
                var eventInformation = new StringBuilder();
                this.CollectAdditionalInfo();

                // Record the contents of the AdditionalInfo collection.
                eventInformation.AppendFormat("{0}\n\n", this.additionalInfo.Get(Header));

                eventInformation.AppendFormat("\n{0} {1}:\n{2}", ExceptionSummary, this.applicationName, SectionSeparator);

                foreach (string info in this.additionalInfo)
                {
                    if (info != Header)
                    {
                        eventInformation.AppendFormat("\n--> {0}", this.additionalInfo.Get(info));
                    }
                }

                if (exception != null)
                {
                    Exception currException = exception;
                    do
                    {
                        eventInformation.AppendFormat("\n\n{0}\n{1}", ExceptionDetails, SectionSeparator);
                        eventInformation.AppendFormat("\n{0}: {1}", ExceptionType, currException.GetType().FullName);

                        ReflectException(currException, eventInformation);

                        // Record the StackTrace with separate label.
                        if (currException.StackTrace != null)
                        {
                            eventInformation.AppendFormat("\n\n{0} \n{1}", ExceptionStackTraceDetails, SectionSeparator);
                            eventInformation.AppendFormat("\n{0}", currException.StackTrace);
                        }

                        // Reset the temp exception object and iterate the counter.
                        currException = currException.InnerException;
                    }
                    while (currException != null);
                }

                return eventInformation.ToString();
            }
            catch (Exception)
            {
                return Convert.ToString(exception);
            }
        }

        /// <summary>
        /// Reflect the exception
        /// </summary>
        /// <param name="currException">current exception</param>
        /// <param name="strEventInfo">event info</param>
        private static void ReflectException(Exception currException, StringBuilder strEventInfo)
        {
            PropertyInfo[] arrPublicProperties = currException.GetType().GetProperties();
            foreach (PropertyInfo propinfo in arrPublicProperties)
            {
                // Do not log information for the InnerException or StackTrace. This information is 
                // captured later in the process.
                if (propinfo.Name != "InnerException" && propinfo.Name != "StackTrace")
                {
                    if (propinfo.CanRead && propinfo.GetIndexParameters().Length == 0)
                    {
                        object propValue = null;

                        try
                        {
                            propValue = propinfo.GetValue(currException, null);
                        }
                        catch (TargetInvocationException)
                        {
                            propValue = "Access Failed";
                        }

                        if (propValue == null)
                        {
                            strEventInfo.AppendFormat("\n{0}: NULL", propinfo.Name);
                        }
                        else
                        {
                            ProcessAdditionalInfo(propinfo, propValue, strEventInfo);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Process additional info
        /// </summary>
        /// <param name="propinfo">property info</param>
        /// <param name="propValue">property value</param>
        /// <param name="stringBuilder">string builder</param>
        private static void ProcessAdditionalInfo(PropertyInfo propinfo, object propValue, StringBuilder stringBuilder)
        {
            // Loop through the collection of AdditionalInformation if the exception type is a BaseApplicationException.
            if (propinfo.Name == "AdditionalInformation")
            {
                if (propValue != null)
                {
                    // Cast the collection into a local variable.
                    NameValueCollection currAdditionalInfo = (NameValueCollection)propValue;

                    // Check if the collection contains values.
                    if (currAdditionalInfo.Count > 0)
                    {
                        stringBuilder.AppendFormat("\nAdditionalInformation:");

                        // Loop through the collection adding the information to the string builder.
                        for (int i = 0; i < currAdditionalInfo.Count; i++)
                        {
                            stringBuilder.AppendFormat("\n{0}: {1}", currAdditionalInfo.GetKey(i), currAdditionalInfo[i]);
                        }
                    }
                }
            }
            else
            {
                // Otherwise just write the ToString() value of the property.
                stringBuilder.AppendFormat("\n{0}: {1}", propinfo.Name, propValue);
            }
        }

        /// <summary>
        /// Get windows identity
        /// </summary>
        /// <returns>the identity</returns>
        private static string GetWindowsIdentity()
        {
            try
            {
                return WindowsIdentity.GetCurrent().Name;
            }
            catch (SecurityException)
            {
                return "Permission Denied";
            }
        }

        /// <summary>
        /// Get machine name
        /// </summary>
        /// <returns>the machine name</returns>
        private static string GetMachineName()
        {
            try
            {
                return Environment.MachineName;
            }
            catch (SecurityException)
            {
                return "Permission Denied";
            }
        }

        /// <summary>
        /// Add additional 'environment' information. 
        /// </summary>
        private void CollectAdditionalInfo()
        {
            if (this.additionalInfo["MachineName:"] != null)
            {
                return;
            }

            this.additionalInfo.Add("MachineName:", "MachineName: " + GetMachineName());
            this.additionalInfo.Add("FullName:", "FullName: " + Assembly.GetExecutingAssembly().FullName);
            this.additionalInfo.Add("AppDomainName:", "AppDomainName: " + AppDomain.CurrentDomain.FriendlyName);
            this.additionalInfo.Add("WindowsIdentity:", "WindowsIdentity: " + GetWindowsIdentity());
        }
    }
}