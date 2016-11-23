//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Database
{
    using System;
    using System.Linq;
    using System.Data;
    using System.Data.SqlClient;
    using System.Collections.Generic;

    public static class SqlCommandExtensions
	{
		public static SqlCommand WithParameters(this SqlCommand sqlCommand, IEnumerable<SqlParameter> sqlParameters)
		{
			sqlCommand.Parameters.AddRange(sqlParameters.ToArray());
			return sqlCommand;
		}

		public static SqlCommand WithInParameter(this SqlCommand sqlCommand, SqlParameter sqlParameter, object parameterValue)
		{
			sqlParameter.Direction = ParameterDirection.Input;
			sqlParameter.Value = parameterValue;
			sqlCommand.Parameters.Add(sqlParameter);
			return sqlCommand;
		}

		public static SqlCommand WithInOutParameter(this SqlCommand sqlCommand, SqlParameter sqlParameter, object parameterValue)
		{
			sqlParameter.Direction = ParameterDirection.InputOutput;
			sqlParameter.Value = parameterValue;
			sqlCommand.Parameters.Add(sqlParameter);
			return sqlCommand;
		}

		public static SqlCommand WithOutParameter(this SqlCommand sqlCommand, SqlParameter sqlParameter)
		{
			sqlParameter.Direction = ParameterDirection.Output;
			sqlCommand.Parameters.Add(sqlParameter);
			return sqlCommand;
		}

		public static SqlCommand WithTimeout(this SqlCommand sqlCommand, TimeSpan commandTimeout)
		{
			return sqlCommand.WithTimeout((int)commandTimeout.TotalSeconds);
		}

		public static SqlCommand WithTimeout(this SqlCommand sqlCommand, int commandTimeout)
		{
			sqlCommand.CommandTimeout = commandTimeout;
			return sqlCommand;
		}

		public static Dictionary<string, object> ExecuteNonQuery(this SqlCommand sqlCommand, string connectionString)
		{
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				return sqlCommand.ExecuteNonQuery(connection);
			}
		}

		public static Dictionary<string, object> ExecuteNonQuery(this SqlCommand sqlCommand, SqlConnection connection)
		{
			sqlCommand.Connection = connection;
			sqlCommand.ExecuteNonQuery();
			return BuildParameterDictionary(sqlCommand.Parameters);
		}

		public static List<T> ExecuteReader<T>(this SqlCommand sqlCommand, string connectionString, Func<SqlDataReader, T> createT)
		{
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				return sqlCommand.ExecuteReader(connection, createT);
			}
		}

		public static List<T> ExecuteReader<T>(this SqlCommand sqlCommand, SqlConnection connection, Func<SqlDataReader, T> createT)
		{
			sqlCommand.Connection = connection;
			using (var reader = sqlCommand.ExecuteReader())
				return reader.ToList(createT);
		}

		private static Dictionary<string, object> BuildParameterDictionary(SqlParameterCollection parameters)
		{
			return parameters.Cast<SqlParameter>()
				.Where(parameter => parameter.Direction == ParameterDirection.Output || parameter.Direction == ParameterDirection.InputOutput)
				.ToDictionary(parameter => parameter.ParameterName, parameter => parameter.Value);
		}
	}
}