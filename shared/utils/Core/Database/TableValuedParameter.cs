//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Database
{
	using System.Linq;
    using System.Data;
    using System.Data.SqlClient;
    using System.Collections.Generic;

    using Microsoft.SqlServer.Server;

    public abstract class TableValuedParameter<T>
    {
        protected abstract SqlMetaData[] MetaData { get; }
        protected abstract object[] GetValues(T t);

        public SqlParameter ToSqlParameter(string name, IList<T> data)
        {
	        var result = new SqlParameter(name, SqlDbType.Structured);
	        if (data != null && data.Count > 0)
				result.Value = data.Select(x =>
				{
					var rec = new SqlDataRecord(MetaData);
					var values = GetValues(x);
					for (int i = 0; i < rec.FieldCount; i++) // verbose, but helps with debugging
					{
						rec.SetValue(i, values[i]);
					}
					return rec;
				}).ToList();
            return result;
        }
    }
}