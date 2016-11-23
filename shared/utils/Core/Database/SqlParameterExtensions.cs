//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Database
{
    using System;
    using System.Linq;
    using System.Data;
    using System.Reflection;
    using System.Data.SqlClient;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.SqlServer.Server;

    public static class SqlParameterExtensions
    {
        private static SqlDbType Infer(object value)
        {
            if (value == null)
                return SqlDbType.NVarChar;
            if (value is bool)
                return SqlDbType.Bit;
            if (value is byte)
                return SqlDbType.TinyInt;
            if (value is byte[])
                return SqlDbType.VarBinary;
            if (value is string)
                return SqlDbType.NVarChar;
            if (value is char)
                return SqlDbType.Char;
            if (value is long)
                return SqlDbType.BigInt;
            if (value is int)
                return SqlDbType.Int;
            if (value is short)
                return SqlDbType.SmallInt;
            if (value is ushort)
                return SqlDbType.SmallInt;
            if (value is IEnumerable<SqlDataRecord>)
                return SqlDbType.Structured;
            if (value is Guid)
                return SqlDbType.UniqueIdentifier;
            if (value is double)
                return SqlDbType.Decimal;
            if (value is decimal)
                return SqlDbType.Decimal;
            if (value is DateTime)
                return SqlDbType.DateTime;
            return SqlDbType.Variant;
        }

        public static SqlParameter Append(this SqlParameterCollection parameters, string name, PropertyInfo property, object o, ParameterDirection direction = ParameterDirection.Input)
        {
            var value = property.GetValue(o, null);
            return Append(parameters, name, value, direction);
        }

        public static SqlParameter Append(this SqlParameterCollection parameters, string name, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            return Append(parameters, name, value, Infer(value), direction);
        }

        public static SqlParameter Append(this SqlParameterCollection parameters, string name, SqlDbType sqlDbType, ParameterDirection direction = ParameterDirection.Input)
        {
            return Append(parameters, name, (object)null, sqlDbType, direction);
        }

        public static SqlParameter Append(this SqlParameterCollection parameters, string name, object value, SqlDbType sqlDbType, ParameterDirection direction = ParameterDirection.Input)
        {
            return parameters.Add(
                new SqlParameter
                {
                    ParameterName = name,
                    SqlDbType = sqlDbType,
                    Direction = direction,
                    Value = value ?? DBNull.Value,
                    Size = -1
                });
        }

		public static string Dump(this SqlParameterCollection parameters)
		{
			return parameters.Cast<SqlParameter>().Aggregate(
				new StringBuilder(),
				(acc, p) => acc.Append(p.ParameterName).Append(" = ").Append(p.Value).AppendLine(),
				acc => acc.ToString());
		}
    }
}