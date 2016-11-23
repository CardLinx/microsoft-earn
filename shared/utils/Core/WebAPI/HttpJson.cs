//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.WebAPI
{
	using System.Net.Http;
	using System.Diagnostics.Contracts;

	using Extensions;
	using Cryptography;

	public static class HttpJson
	{
		[Pure]
		private static WebRequestHandler GetHandler(string certificateName)
		{
			var handler = new WebRequestHandler();

			// uncomment when need to disable certificate validation
			//handler.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

			if (certificateName != null)
			{
				var certificate = Certificates.ByName(certificateName);
				if (certificate == null)
					throw new HttpRequestException("Certificate [{0}] not found".ExpandWith(certificateName));
				handler.ClientCertificates.Add(certificate);
			}
			return handler;
		}

		[Pure]
		private static HttpRequestException CreateException(this HttpResponseMessage response)
		{
			var errorMessage = response.Content.ReadAsStringAsync().Result;
			return new HttpRequestException("{0}: {1}".ExpandWith(
				response.ReasonPhrase,
				errorMessage));
		}

		// get json
		public static T Get<T>(string url, string certificateName = null)
		{
			var response = new HttpClient(GetHandler(certificateName)).GetAsync(url).Result;
			if (response.IsSuccessStatusCode)
				return response.Content.ReadAsAsync<T>().Result;
			throw response.CreateException();
		}

		// get json as a result of a bodyless post 
		public static TOut Post<TOut>(string url, string certificateName = null)
		{
			var response = new HttpClient(GetHandler(certificateName)).PostAsync(url, null).Result;
			if (response.IsSuccessStatusCode)
				return response.Content.ReadAsAsync<TOut>().Result;
			throw response.CreateException();
		}

		// post json, void response
		public static void Post<TIn>(string url, TIn data, string certificateName = null)
		{
			var response = new HttpClient(GetHandler(certificateName)).PostAsJsonAsync(url, data).Result;
			if (response.IsSuccessStatusCode)
				return;
			throw response.CreateException();
		}

		// post json and get another json back
		public static TOut Post<TIn, TOut>(string url, TIn data, string certificateName = null)
		{
			var response = new HttpClient(GetHandler(certificateName)).PostAsJsonAsync(url, data).Result;
			if (response.IsSuccessStatusCode)
				return response.Content.ReadAsAsync<TOut>().Result;
			throw response.CreateException();
		}

		// post json and get string back. Note - about function Post<TIn, TOut> will throw exception if TOut is specified as string
		// and server returns HTML as content
		public static string PostWithStringResult<TIn>(string url, TIn data, string certificateName = null)
		{
			var response = new HttpClient(GetHandler(certificateName)).PostAsJsonAsync(url, data).Result;
			if (response.IsSuccessStatusCode)
				return response.Content.ReadAsStringAsync().Result;
			throw response.CreateException();
		}

		// delete and return json
		public static TOut Delete<TOut>(string url, string certificateName = null)
		{
			var response = new HttpClient(GetHandler(certificateName)).DeleteAsync(url).Result;
			if (response.IsSuccessStatusCode)
				return response.Content.ReadAsAsync<TOut>().Result;
			throw response.CreateException();
		}
	}
}