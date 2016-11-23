//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using Newtonsoft.Json;
using System.Linq;

namespace Lomo.Commerce.DataAccess
{
    using Lomo.Commerce.Context;
    using Lomo.Commerce.DataContracts.Extensions;
    using Lomo.Commerce.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    /// <summary>
    /// Represents operations on objects within the data store.
    /// </summary>
    public abstract class CommerceOperations
    {
        /***** BEGIN TEMPORARY METHODS REQUIRED FOR PHYSICAL MICROSOFT STORE FDC -> VISA/MC SWITCH *****/

        /// <summary>
        ///  Reject all FDC calls for the physical Microsoft Stores.
        /// </summary>
        /// <returns>
        /// * True if the transaction can proceed.
        /// * False if the filter removed the transaction.
        /// </returns>
        protected bool PhysStoreFilter()
        {
            string partnerMerchantId = (string)Context[Key.PartnerMerchantId];
            return partnerMerchantId != "433023601886" &&
                   partnerMerchantId != "433023605887" &&
                   partnerMerchantId != "433023612883" &&
                   partnerMerchantId != "433023626883" &&
                   partnerMerchantId != "433023672887" &&
                   partnerMerchantId != "433023683884" &&
                   partnerMerchantId != "433023689881" &&
                   partnerMerchantId != "433023691887" &&
                   partnerMerchantId != "433024710884" &&
                   partnerMerchantId != "433024716881" &&
                   partnerMerchantId != "433024717889" &&
                   partnerMerchantId != "433024729884";
        }

        /***** END TEMPORARY METHODS REQUIRED FOR PHYSICAL MICROSOFT STORE FDC -> VISA/MC SWITCH *****/

        /// <summary>
        /// Executes the described SQL stored procedure against the specified database.
        /// </summary>
        /// <param name="storedProcedureName">
        /// The name of the stored procedure to run.
        /// </param>
        /// <param name="parameters">
        /// Parameters to add to the stored procedure.
        /// </param>
        /// <param name="loader">
        /// The method to call to load results from the stored procedure.
        /// </param>
        /// <returns>
        /// * ResultCode.Success if successful.
        /// * Else returns appropriate error ResultCode.
        /// </returns>
        protected ResultCode SqlProcedure(string storedProcedureName,
                                          Dictionary<string, object> parameters,
                                          Action<SqlDataReader> loader)
        {
            object scalarExecutionResult;
            return SqlProcedure(storedProcedureName, parameters, loader, out scalarExecutionResult);
        }

        /// <summary>
        /// Executes the described SQL stored procedure against the specified database.
        /// </summary>
        /// <param name="storedProcedureName">
        /// The name of the stored procedure to run.
        /// </param>
        /// <param name="parameters">
        /// Parameters to add to the stored procedure.
        /// </param>
        /// <returns>
        /// * ResultCode.Success if successful.
        /// * Else returns appropriate error ResultCode.
        /// </returns>
        protected ResultCode SqlProcedure(string storedProcedureName,
                                          Dictionary<string, object> parameters)
        {
            object scalarExecutionResult;
            return SqlProcedure(storedProcedureName, parameters, null, out scalarExecutionResult);
        }

        /// <summary>
        /// Executes the described SQL stored procedure against the specified database.
        /// </summary>
        /// <param name="storedProcedureName">
        /// The name of the stored procedure to run.
        /// </param>
        /// <param name="parameters">
        /// Parameters to add to the stored procedure.
        /// </param>
        /// <param name="loader">
        /// The method to call to load results from the stored procedure.
        /// </param>
        /// <param name="scalarExecutionResult">
        /// Receives the result of the result of executing a scalar stored precedure.
        /// </param>
        /// <returns>
        /// * ResultCode.Success if successful.
        /// * Else returns appropriate error ResultCode.
        /// </returns>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities",
         Justification = "Parameter storedProcedureName does not come from user input and the command type is set to stored procedure.")]
        private ResultCode SqlProcedure(string storedProcedureName,
                                        Dictionary<string, object> parameters,
                                        Action<SqlDataReader> loader,
                                        out object scalarExecutionResult)
        {
            ResultCode result;

            scalarExecutionResult = null;
            SqlConnection sqlConnection = null;
            SqlDataReader sqlDataReader = null;

            try
            {
                // Prepare the connection.
                sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["CommerceEntities"].ConnectionString + Context.ConnectionStringSuffix);
                OpenSqlConnection(sqlConnection, () =>  { sqlConnection.Open(); });
                using (SqlCommand sqlCommand = new SqlCommand("dbo." + storedProcedureName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        foreach (string key in parameters.Keys)
                        {
                            if (parameters[key] is DateTime)
                            {
                                SqlParameter sqlParameter = new SqlParameter(key, SqlDbType.DateTime2);
                                sqlParameter.Value = parameters[key];
                                sqlCommand.Parameters.Add(sqlParameter);
                            }
                            else
                            {
                                sqlCommand.Parameters.Add(new SqlParameter(key, parameters[key]));
                            }
                        }
                    }

                    Stopwatch sprocTimer = new Stopwatch();
                    try
                    {
                        string parametersString = null;
                        // Execute the stored procedure.
                        if (parameters != null && parameters.Any())
                        {
                            try
                            {
                                parametersString = JsonConvert.SerializeObject(parameters);
                            }
                            catch
                            {
                                //ignore any error in serialization   
                            }
                        }

                        Context.Log.Verbose("Executing {0} stored procedure with parameters {1}.", storedProcedureName, parametersString);
                        sprocTimer.Start();
                        if (loader != null)
                        {
                            sqlDataReader = sqlCommand.ExecuteReader();
                        }
                        else
                        {
                            scalarExecutionResult = sqlCommand.ExecuteScalar();
                        }
                        sprocTimer.Stop();
                        Context.PerformanceInformation.Add(storedProcedureName, String.Format("{0} ms",
                                                         sprocTimer.ElapsedMilliseconds));
                        result = ResultCode.Success;
                    }
                    catch (SqlException ex)
                    {
                        // Stop the timer and extract the result code.
                        sprocTimer.Stop();
                        int delimiterIndex = ex.Message.IndexOf(":", StringComparison.OrdinalIgnoreCase); // handles the case when format is not as custom generated
                        string errorMessage = ex.Message;
                        if (delimiterIndex > 0)
                        {
                            errorMessage = errorMessage.Substring(0, delimiterIndex);
                        }
                        if (Enum.TryParse<ResultCode>(errorMessage, out result) == true)
                        {
                            // AlreadyClaimed is a very common result and does not indicate an error state.
                            if (result != ResultCode.AlreadyClaimed)
                            {
                                Context.Log.Warning("{0} completed with error ResultCode {1}.", storedProcedureName, result);
                            }
                            else
                            {
                                Context.Log.Verbose("{0} completed with error ResultCode {1}.", storedProcedureName, result);
                            }
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                if (result == ResultCode.Success)
                {
                    // Load the results.
                    if (loader != null)
                    {
                        loader(sqlDataReader);
                    }

                    Context.Log.Verbose("{0} completed successfully.", storedProcedureName);
                }
            }
            finally
            {
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                }
                if (sqlDataReader != null)
                {
                    sqlDataReader.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the context in which operations will be performed.
        /// </summary>
        public CommerceContext Context { get; set; }

        /// <summary>
        /// Opens the specified SQL connection, retrying as configured if needed.
        /// </summary>
        /// <param name="sqlConnection">
        /// The SQL connection to open.
        /// </param>
        /// <param name="openAction">
        /// The action to perform when opening the connection.
        /// </param>
        internal void OpenSqlConnection(SqlConnection sqlConnection,
                                        Action openAction)
        {
            // Open the SQL connection, retrying if needed.
            int tryCount = 0;
            int maxRetries = Context.Config.MaxSqlConnectionRetries;
            int retryLatency = Context.Config.InitialSqlConnectionRetryLatency;

            do
            {
                try
                {
                    tryCount++;
                    openAction();
                }
                catch(SqlException ex)
                {
                    if (tryCount <= maxRetries && ex.Number == ConnectionTimeoutExpired)
                    {
                        Context.Log.Warning("Timeout opening connection to database with connection string {0}.",
                                            sqlConnection.ConnectionString);
                    }
                    else
                    {
                        throw;
                    }
                }

                // If a rety is needed, wait a short but increasingly lengthy time before proceeding.
                if (sqlConnection.State != ConnectionState.Open && tryCount <= maxRetries)
                {
                    Context.Log.Verbose("Waiting {0} milliseconds before retrying SQL connection.", retryLatency);
                    Thread.Sleep(retryLatency);
                    retryLatency *= 2;
                }
            }
            while (sqlConnection.State != ConnectionState.Open && tryCount <= maxRetries);
        }

        /// <summary>
        /// The error code for SqlException 0x80131904, Connection Timeout Expired.
        /// </summary>
        /// <remarks>
        /// * This values comes from System.Data.SqlClient.TdsEnums equivalent for TIMEOUT_EXPIRED.
        /// * We can't leverage this directly because TdsEnums is an internal class.
        /// </remarks>
        private const int ConnectionTimeoutExpired = -2;
    }
}