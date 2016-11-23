//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Database
{
	using System.Data;
	using System.Data.SqlClient;

	using Reflection;

	public class StoredProcedure
	{
		public static SqlCommand GetCommand(SqlConnection connection = null)
		{
			var type = StackFrame.GetCallingMethodDeclaringType(2);
			return new SqlCommand(type.Name)
			{
				CommandType = CommandType.StoredProcedure,
				Connection = connection
			};
		}
	}
}