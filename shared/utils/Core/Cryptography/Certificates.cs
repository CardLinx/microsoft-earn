//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Cryptography
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Security.Cryptography.X509Certificates;

	public class Certificates
	{
        private static readonly Dictionary<string, X509Certificate2> _certificates = new Dictionary<string, X509Certificate2>(StringComparer.OrdinalIgnoreCase);

	    [Pure]
	    public static X509Certificate2 ByName(
			string certificateName, 
			StoreLocation location = StoreLocation.LocalMachine,
			X509FindType findType = X509FindType.FindBySubjectName,
			bool validOnly = true)
	    {
	        if (certificateName == null)
	            throw new ArgumentNullException("certificateName");

		    lock (_certificates)
		    {
			    if (!_certificates.ContainsKey(certificateName))
			    {
				    var store = new X509Store(StoreName.My, location);
				    store.Open(OpenFlags.ReadOnly);
				    try
				    {
					    var certificate = store.Certificates.
							Find(findType, certificateName, validOnly).
							OfType<X509Certificate2>().
							FirstOrDefault();
					    _certificates.Add(certificateName, certificate);
				    }
				    finally
				    {
					    store.Close();
				    }
			    }
				return _certificates[certificateName];
		    }
	    }
	}
}