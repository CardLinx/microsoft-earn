//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Lomo.Core.WebAPI
{
	using System.Net.Http;
	using System.Diagnostics.Contracts;

	using Extensions;
	using Cryptography;

	public static class HttpJsonAsync
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
		private static async Task<HttpRequestException> CreateExceptionAsync(this HttpResponseMessage response)
		{
			var errorMessage = await response.Content.ReadAsStringAsync();
			return new HttpRequestException("{0}: {1}".ExpandWith(
				response.ReasonPhrase,
				errorMessage));
		}

		[Pure]
		private static HttpClient GetHttpClient(string certificateName, AuthenticationHeaderValue authenticationHeaderValue)
		{
			var client = new HttpClient(GetHandler(certificateName));
			if (authenticationHeaderValue != null)
			{
				client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
			}
			return client;
		}
		

		// get json
		public static async Task<T> GetAsync<T>(string url, string certificateName = null, AuthenticationHeaderValue authenticationHeaderValue = null)
		{
			var response = await GetHttpClient(certificateName, authenticationHeaderValue).GetAsync(url);
			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadAsAsync<T>();
				return result;
			}

			var exception = await response.CreateExceptionAsync();
			throw exception;
		}

		public static async Task<string> GetAsyncWithStringResult(string url, string certificateName = null, AuthenticationHeaderValue authenticationHeaderValue = null)
		{
			var response = await GetHttpClient(certificateName, authenticationHeaderValue).GetAsync(url);
			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadAsStringAsync();
				return result;
			}

			var exception = await response.CreateExceptionAsync();
			throw exception;
		}


		// get json as a result of a bodyless post 
		public static async Task<TOut> PostAsync<TOut>(string url, string certificateName = null, AuthenticationHeaderValue authenticationHeaderValue = null)
		{
			var response = await GetHttpClient(certificateName, authenticationHeaderValue).PostAsync(url, null);
			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadAsAsync<TOut>();
				return result;
			}

			var exception = await response.CreateExceptionAsync();
			throw exception;
		}
		
		// post json, void response
		public static async Task PostAsync<TIn>(string url, TIn data, string certificateName = null, AuthenticationHeaderValue authenticationHeaderValue = null)
		{
			var response = await GetHttpClient(certificateName, authenticationHeaderValue).PostAsJsonAsync(url, data);
			if (response.IsSuccessStatusCode)
				return;

			var exception = await response.CreateExceptionAsync();
			throw exception;

		}

		// post json and get another json back
		public static async Task<TOut> PostAsync<TIn, TOut>(string url, TIn data, string certificateName = null, AuthenticationHeaderValue authenticationHeaderValue = null)
		{
			var response = await GetHttpClient(certificateName, authenticationHeaderValue).PostAsJsonAsync(url, data);
			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadAsAsync<TOut>();
				return result;
			}

			var exception = await response.CreateExceptionAsync();
			throw exception;
		}

		// post json and get string back. Note - about function Post<TIn, TOut> will throw exception if TOut is specified as string
		// and server returns HTML as content
		public static async Task<string> PostWithStringResultAsync<TIn>(string url, TIn data, string certificateName = null, AuthenticationHeaderValue authenticationHeaderValue = null)
		{
			var response = await GetHttpClient(certificateName, authenticationHeaderValue).PostAsJsonAsync(url, data);
			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadAsStringAsync();
				return result;
			}

			var exception = await response.CreateExceptionAsync();
			throw exception;
		}

		// delete and return json
		public static async Task<TOut> DeleteAsync<TOut>(string url, string certificateName = null, AuthenticationHeaderValue authenticationHeaderValue = null)
		{
			var response = await GetHttpClient(certificateName, authenticationHeaderValue).DeleteAsync(url);
			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadAsAsync<TOut>();
				return result;
			}

			var exception = await response.CreateExceptionAsync();
			throw exception;
		}
	}
}