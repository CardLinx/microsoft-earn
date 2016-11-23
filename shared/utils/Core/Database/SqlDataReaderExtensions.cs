//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public static class SqlDataReaderExtensions
    {
        public static T ToType<T>(this SqlDataReader reader, string fieldName)
        {
            object data = reader[fieldName];
            if (data == DBNull.Value)
            {
                var result = default(T);
                if (result == null)
                    return result;
                throw new Exception("DBNull returned where value type is expected");
            }
            return (T)data;
        }

		public static List<T> ToList<T>(this SqlDataReader reader, Func<SqlDataReader, T> createT)
		{
			var result = new List<T>();
			while (reader.Read())
				result.Add(createT(reader));
			return result;
		}
    }
}