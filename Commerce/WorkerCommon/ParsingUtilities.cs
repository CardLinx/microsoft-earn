//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Commerce.WorkerCommon
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Text.RegularExpressions;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Logging;

    /// <summary>
    /// Contains utility methods used during the parsing of First Data files.
    /// </summary>
    public class ParsingUtilities
    {
        /// <summary>
        /// Initializes a new instance of the ParsingUtilities class.
        /// </summary>
        /// <param name="recordType">
        /// The type of record being parsed to place in the log.
        /// </param>
        /// <param name="recordNumber">
        /// The number of the record being parsed to place in the log.
        /// </param>
        /// <param name="fileName">
        /// The name of the file for which the record being parsed originated to place in the log.
        /// </param>
        /// <param name="timeFieldLength">
        /// The length of the time field.
        /// </param>
        /// <param name="log">
        /// The CommerceLog object through which log entries can be made.
        /// </param>
        /// <exception cref="ArgumentExeception">
        /// The value of parameter timeFieldLength must be at least 4.
        /// </exception>
        public ParsingUtilities(string recordType,
                                int recordNumber,
                                string fileName,
                                int timeFieldLength,
                                CommerceLog log)
        {
            if (timeFieldLength < 4)
            {
                throw new ArgumentException("The value of parameter timeFieldLength must be at least 4.", "timeFieldLength");
            }

            RecordType = recordType;
            RecordNumber = recordNumber;
            FileName = fileName;
            TimeFieldLength = timeFieldLength;
            Log = log;
        }

        /// <summary>
        /// Verifies the specified string in the specified record.
        /// </summary>
        /// <param name="record">
        /// The record text being parsed.
        /// </param>
        /// <param name="recordPosition">
        /// The current position of the record as it's parsed.
        /// </param>
        /// <param name="fieldName">
        /// The name of the field whose value to verify to place in the log.
        /// </param>
        /// <param name="expectedValue">
        /// The value the string is expected to contain.
        /// </param>
        /// <param name="fieldLength">
        /// The length of the field to verify.
        /// </param>
        /// <param name="recordValid">
        /// A value that indicates whether the record being parsed is so far valid.
        /// </param>
        /// <returns>
        /// * True if the string is valid.
        /// * Else returns false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter record cannot be null.
        /// </exception>
        public bool VerifyString(string record,
                                 ref int recordPosition,
                                 string fieldName,
                                 string expectedValue,
                                 int fieldLength,
                                 bool recordValid)
        {
            return VerifyString(record, ref recordPosition, fieldName, expectedValue, fieldLength, true, recordValid);
        }

        /// <summary>
        /// Verifies the specified string in the specified record.
        /// </summary>
        /// <param name="record">
        /// The record text being parsed.
        /// </param>
        /// <param name="recordPosition">
        /// The current position of the record as it's parsed.
        /// </param>
        /// <param name="fieldName">
        /// The name of the field whose value to verify to place in the log.
        /// </param>
        /// <param name="expectedValue">
        /// The value the string is expected to contain.
        /// </param>
        /// <param name="fieldLength">
        /// The length of the field to verify.
        /// </param>
        /// <param name="logReceivedValue">
        /// A value that indicates whether the received value should be added to the log.
        /// </param>
        /// <param name="recordValid">
        /// A value that indicates whether the record being parsed is so far valid.
        /// </param>
        /// <returns>
        /// * True if the string is valid.
        /// * Else returns false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter record cannot be null.
        /// </exception>
        public bool VerifyString(string record,
                                 ref int recordPosition,
                                 string fieldName,
                                 string expectedValue,
                                 int fieldLength,
                                 bool logReceivedValue,
                                 bool recordValid)
        {
            if (record == null)
            {
                throw new ArgumentNullException("record", "Parameter record cannot be null.");
            }

            if (recordValid == true)
            {
                if (record.Length >= recordPosition + fieldLength)
                {
                    string receivedValue = record.Substring(recordPosition, fieldLength);
                    if (receivedValue == expectedValue)
                    {
                        recordPosition += fieldLength;
                    }
                    else
                    {
                        string formattedReceivedValue = String.Empty;
                        if (logReceivedValue == true)
                        {
                            formattedReceivedValue = String.Format(" but received \"{0}\"", receivedValue);
                        }
                        Log.Warning("Error parsing \"{0}\" record #{1} from file \"{2}\". Expected value \"{3}\" in field " +
                                    "\"{4}\"{5}.", (int)ResultCode.ExpectedValueNotFound, RecordType, RecordNumber, FileName,
                                    expectedValue, fieldName, formattedReceivedValue);
                        recordValid = false;
                    }
                }
                else
                {
                    LogUnexpectedEndOfRecord();
                    recordValid = false;
                }
            }

            return recordValid;
        }

        /// <summary>
        /// Populates the specified string from the given record.
        /// </summary>
        /// <param name="record">
        /// The record text being parsed.
        /// </param>
        /// <param name="recordPosition">
        /// The current position of the record as it's parsed.
        /// </param>
        /// <param name="field">
        /// The field to populate.
        /// </param>
        /// <param name="fieldLength">
        /// The length of the field to populate.
        /// </param>
        /// <param name="recordValid">
        /// A value that indicates whether the record being parsed is so far valid.
        /// </param>
        /// <returns>
        /// * True if the string was populated.
        /// * Else returns false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter record cannot be null.
        /// </exception>
        public bool PopulateString(string record,
                                   ref int recordPosition,
                                   out string field,
                                   int fieldLength,
                                   bool recordValid)
        {
            if (record == null)
            {
                throw new ArgumentNullException("record", "Parameter record cannot be null.");
            }

            field = null;

            if (recordValid == true)
            {
                if (record.Length >= recordPosition + fieldLength)
                {
                    field = record.Substring(recordPosition, fieldLength).Trim(' ');
                    recordPosition += fieldLength;
                }
                else
                {
                    LogUnexpectedEndOfRecord();
                    recordValid = false;
                }
            }

            return recordValid;
        }

        /// <summary>
        /// Populates the specified DateTime from the given record.
        /// </summary>
        /// <param name="record">
        /// The record text being parsed.
        /// </param>
        /// <param name="recordPosition">
        /// The current position of the record as it's parsed.
        /// </param>
        /// <param name="fieldName">
        /// The name of the field whose value to verify to place in the log.
        /// </param>
        /// <param name="field">
        /// The field to populate.
        /// </param>
        /// <param name="includeTime">
        /// Specifies whether to include a time from the record when creating the DateTime.
        /// </param>
        /// <param name="timeIncludesColons">
        /// Specifies whether the time in the record includes colons.
        /// </param>
        /// <param name="recordValid">
        /// A value that indicates whether the record being parsed is so far valid.
        /// </param>
        /// <returns>
        /// * True if the date was populated.
        /// * Else returns false.
        /// </returns>
        /// <remarks>
        /// Then includeTime is false, time is set to midnight.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Parameter record cannot be null.
        /// </exception>
        public bool PopulateDateTime(string record,
                                     ref int recordPosition,
                                     string fieldName,
                                     out DateTime field,
                                     bool includeTime,
                                     bool timeIncludesColons,
                                     bool recordValid)
        {
            if (record == null)
            {
                throw new ArgumentNullException("record", "Parameter record cannot be null.");
            }

            field = DateTime.MinValue;

            if (recordValid == true)
            {
                int fieldLength = DateFieldLength;
                if (includeTime == true)
                {
                    fieldLength += TimeFieldLength;
                }

                if (record.Length >= recordPosition + fieldLength)
                {
                    // Build a standard string representation of the date.
                    string receivedValue = record.Substring(recordPosition, fieldLength);
                    recordPosition += fieldLength;
                    StringBuilder normalizedDateTime = new StringBuilder(receivedValue.Substring(0, 4));
                    normalizedDateTime.Append("/");
                    normalizedDateTime.Append(receivedValue.Substring(4, 2));
                    normalizedDateTime.Append("/");
                    normalizedDateTime.Append(receivedValue.Substring(6, 2));
                    if (includeTime == true)
                    {
                        normalizedDateTime.Append(" ");
                        if (timeIncludesColons == true)
                        {
                            normalizedDateTime.Append(receivedValue.Substring(8, 8));
                        }
                        else
                        {
                            normalizedDateTime.Append(receivedValue.Substring(8, 2));
                            normalizedDateTime.Append(":");
                            normalizedDateTime.Append(receivedValue.Substring(10, 2));
                            normalizedDateTime.Append(":");
                            normalizedDateTime.Append(receivedValue.Substring(12, 2));
                        }
                    }

                    // Attempt to parse the date.
                    if (DateTime.TryParse(normalizedDateTime.ToString(), out field) == false)
                    {
                        Log.Warning("Error parsing \"{0}\" record #{1} from file \"{2}\". Received value \"{3}\" in field " +
                                    "\"{4}\" is not properly formatted.", (int)ResultCode.InvalidValue, RecordType, RecordNumber,
                                    FileName, receivedValue, fieldName);
                        recordValid = false;
                    }
                }
                else
                {
                    LogUnexpectedEndOfRecord();
                    recordValid = false;
                }
            }

            return recordValid;
        }

        /// <summary>
        /// Populates the specified DateTime from the given record.
        /// </summary>
        /// <param name="record">
        /// The record text being parsed.
        /// </param>
        /// <param name="recordPosition">
        /// The current position of the record as it's parsed.
        /// </param>
        /// <param name="fieldName">
        /// The name of the field whose value to verify to place in the log.
        /// </param>
        /// <param name="date">
        /// The existing date to which to append the time.
        /// </param>
        /// <param name="recordValid">
        /// A value that indicates whether the record being parsed is so far valid.
        /// </param>
        /// <returns>
        /// * True if the date was populated.
        /// * Else returns false.
        /// </returns>
        /// <remarks>
        /// Then includeTime is false, time is set to midnight.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Parameter record cannot be null.
        /// </exception>
        public bool PopulateTimeForExistingDate(string record,
                                                ref int recordPosition,
                                                string fieldName,
                                                ref DateTime date,
                                                bool recordValid)
        {
            if (record == null)
            {
                throw new ArgumentNullException("record", "Parameter record cannot be null.");
            }

            if (recordValid == true)
            {
                if (record.Length >= recordPosition + TimeFieldLength)
                {
                    // Get the time string from the record.
                    string receivedValue = record.Substring(recordPosition, TimeFieldLength);
                    recordPosition += TimeFieldLength;

                    // Build a dummy date string.
                    StringBuilder dummyDateString = new StringBuilder("2014/02/10 ");
                    if (String.IsNullOrWhiteSpace(receivedValue) == false)
                    {
                        if (receivedValue.Contains(":") == true)
                        {
                            dummyDateString.Append(receivedValue);
                        }
                        else
                        {
                            dummyDateString.Append(receivedValue.Substring(0, 2));
                            dummyDateString.Append(':');
                            dummyDateString.Append(receivedValue.Substring(2, 2));
                            if (TimeFieldLength > 4)
                            {
                                dummyDateString.Append(':');
                                dummyDateString.Append(receivedValue.Substring(4, 2));
                            }
                        }

                        // Attempt to parse the dummy date.
                        DateTime dummyDate;
                        if (DateTime.TryParse(dummyDateString.ToString(), out dummyDate) == true)
                        {
                            // Append the time to the existing date.
                            date = new DateTime(date.Year, date.Month, date.Day, dummyDate.Hour, dummyDate.Minute, dummyDate.Second);
                        }
                        else
                        {
                            Log.Warning("Error parsing \"{0}\" record #{1} from file \"{2}\". Received value \"{3}\" in field " +
                                        "\"{4}\" is not properly formatted.", (int)ResultCode.InvalidValue, RecordType, RecordNumber,
                                        FileName, receivedValue, fieldName);
                            recordValid = false;
                        }
                    }
                }
                else
                {
                    LogUnexpectedEndOfRecord();
                    recordValid = false;
                }
            }

            return recordValid;
        }

        /// <summary>
        /// Populates the specified long from the given record.
        /// </summary>
        /// <param name="record">
        /// The record text being parsed.
        /// </param>
        /// <param name="recordPosition">
        /// The current position of the record as it's parsed.
        /// </param>
        /// <param name="fieldName">
        /// The name of the field whose value to verify to place in the log.
        /// </param>
        /// <param name="field">
        /// The field to populate.
        /// </param>
        /// <param name="fieldLength">
        /// The length of the field to populate.
        /// </param>
        /// <param name="recordValid">
        /// A value that indicates whether the record being parsed is so far valid.
        /// </param>
        /// <returns>
        /// * True if the long was populated.
        /// * Else returns false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter record cannot be null.
        /// </exception>
        public bool PopulateLong(string record,
                                 ref int recordPosition,
                                 string fieldName,
                                 out long field,
                                 int fieldLength,
                                 bool recordValid)
        {
            if (record == null)
            {
                throw new ArgumentNullException("record", "Parameter record cannot be null.");
            }

            field = Int64.MinValue;

            if (recordValid == true)
            {
                if (record.Length >= recordPosition + fieldLength)
                {
                    // Extract the potential long value from the record.
                    string receivedValue = record.Substring(recordPosition, fieldLength);
                    recordPosition += fieldLength;

                    // Attempt to parse the long.
                    if (Int64.TryParse(receivedValue.ToString(), out field) == false)
                    {
                        Log.Warning("Error parsing \"{0}\" record #{1} from file \"{2}\". Received value \"{3}\" in field " +
                                    "\"{4}\" is not properly formatted.", (int)ResultCode.InvalidValue, RecordType, RecordNumber,
                                    FileName, receivedValue, fieldName);
                        recordValid = false;
                    }
                }
                else
                {
                    LogUnexpectedEndOfRecord();
                    recordValid = false;
                }
            }

            return recordValid;
        }

        /// <summary>
        /// Populates the specified decimal from the given record.
        /// </summary>
        /// <param name="record">
        /// The record text being parsed.
        /// </param>
        /// <param name="recordPosition">
        /// The current position of the record as it's parsed.
        /// </param>
        /// <param name="fieldName">
        /// The name of the field whose value to verify to place in the log.
        /// </param>
        /// <param name="field">
        /// The field to populate.
        /// </param>
        /// <param name="fieldLength">
        /// The length of the field to populate.
        /// </param>
        /// <param name="recordValid">
        /// A value that indicates whether the record being parsed is so far valid.
        /// </param>
        /// <returns>
        /// * True if the decimal was populated.
        /// * Else returns false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Parameter record cannot be null.
        /// </exception>
        public bool PopulateDecimal(string record,
                                    ref int recordPosition,
                                    string fieldName,
                                    out decimal field,
                                    int fieldLength,
                                    bool recordValid)
        {
            if (record == null)
            {
                throw new ArgumentNullException("record", "Parameter record cannot be null.");
            }

            field = Int64.MinValue;

            if (recordValid == true)
            {
                if (record.Length >= recordPosition + fieldLength)
                {
                    // Extract the potential decimal value from the record.
                    string receivedValue = record.Substring(recordPosition, fieldLength);
                    recordPosition += fieldLength;

                    // If the value is negative, strip the sign before trying to parse into a decimal.
                    string positiveValue = receivedValue.Replace("-", String.Empty);

                    // Attempt to parse the decimal.
                    if (Decimal.TryParse(positiveValue, out field) == false)
                    {
                        Log.Warning("Error parsing \"{0}\" record #{1} from file \"{2}\". Received value \"{3}\" in field " +
                                    "\"{4}\" is not properly formatted.", (int)ResultCode.InvalidValue, RecordType, RecordNumber,
                                    FileName, receivedValue, fieldName);
                        recordValid = false;
                    }

                    // If the value was negative, adjust the parsed value accordingly.
                    if (receivedValue != positiveValue)
                    {
                        field *= -1;
                    }
                }
                else
                {
                    LogUnexpectedEndOfRecord();
                    recordValid = false;
                }
            }

            return recordValid;
        }

        /// <summary>
        /// Validates the end of the record.
        /// </summary>
        /// <param name="record">
        /// The record text being parsed.
        /// </param>
        /// <param name="recordPosition">
        /// The current position of the record as it's parsed.
        /// </param>
        /// <param name="fillerLength">
        /// The expected length of the filler at the end of the record.
        /// </param>
        /// <param name="recordValid">
        /// A value that indicates whether the record being parsed is so far valid.
        /// </param>
        /// <remarks>
        /// The end of the record is validated, but if validation fails, the error is only logged.
        /// </remarks>
        public void VerifyRecordEnd(string record,
                                    ref int recordPosition,
                                    int fillerLength,
                                    bool checkRecordEndMarker,
                                    bool recordValid)
        {
            if (recordValid == true)
            {
                // Populate a string from the filler.
                string filler;
                recordValid = PopulateString(record, ref recordPosition, out filler, fillerLength, recordValid);
                if (recordValid == true)
                {
                    // Ensure the filler contained only spaces.
                    if (filler.Length > 0)
                    {
                        Log.Warning("Error parsing \"{0}\" record #{1} from file \"{2}\". Record filler portion contained " +
                                    "characters other than spaces.", (int)ResultCode.ExpectedValueNotFound, RecordType,
                                    RecordNumber, FileName);
                        recordValid = false;
                    }
                    
                    // Ensure the record ends with the expected marker.
                    if (checkRecordEndMarker == true)
                    {
                        VerifyString(record, ref recordPosition, "record end marker", RecordEndMarker, RecordEndMarkerLength, recordValid);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a warning to the log that indicates the record ended unexpectedly.
        /// </summary>
        private void LogUnexpectedEndOfRecord()
        {
            Log.Warning("Error parsing \"{0}\" record #{1} from file \"{2}\". Unexpected end of record detected.",
                        (int)ResultCode.UnexpectedEndOfRecord, RecordType, RecordNumber, FileName);
        }

        /// <summary>
        /// Gets or sets the CommerceLog object through which log entries can be made.
        /// </summary>
        private CommerceLog Log { get; set; }

        /// <summary>
        /// Gets or sets the type of record being parsed.
        /// </summary>
        private string RecordType { get; set; }

        /// <summary>
        /// Gets or sets the number of the record being parsed.
        /// </summary>
        private int RecordNumber { get; set; }

        /// <summary>
        /// Gets or sets the name of the file from which the record originated.
        /// </summary>
        private string FileName { get; set; }

        /// <summary>
        /// Gets or sets the length of a time field.
        /// </summary>
        private int TimeFieldLength { get; set; }

        /// <summary>
        /// The length of a date field.
        /// </summary>
        private const int DateFieldLength = 8;

        /// <summary>
        /// The length of the record end marker.
        /// </summary>
        private const int RecordEndMarkerLength = 1;

        /// <summary>
        /// The marker of the end of a record.
        /// </summary>
        private string RecordEndMarker = new String(new char[] { (char)65533 });
    }
}