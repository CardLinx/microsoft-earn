//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Common.Utils
{
	using System.Net.Http;
    using System.Threading.Tasks;

    public static class HttpHelper
	{
        // get json
        public static async Task<T> Get<T>(string url)
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(url).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<T>().ConfigureAwait(false);
                }
                throw await response.CreateException();
            }
        }

        // post json and get another json back
        public static async Task<TOut> Post<TIn, TOut>(string url, TIn data)
        {
            using (var client = new HttpClient())
            using (var response = await client.PostAsJsonAsync(url, data).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<TOut>().ConfigureAwait(false);
                }
                throw await response.CreateException();
            }
        }

        // put json with bodyless response
        public static async Task Put<TIn>(string url, TIn data)
        {
            using (var client = new HttpClient())
            using (var response = await client.PutAsJsonAsync(url, data).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await response.CreateException();
                }
            }
        }

        // bodyless put with bodyless response
        public static async Task Put(string url)
        {
            using (var client = new HttpClient())
            using (var response = await client.PutAsync(url, null).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await response.CreateException();
                }
            }
        }

        // delete
        public static async Task Delete(string url)
        {
            using (var client = new HttpClient())
            using (var response = await client.DeleteAsync(url).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await response.CreateException();
                }
            }
        }

        private static async Task<HttpRequestException> CreateException(this HttpResponseMessage response)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return new HttpRequestStatusException(response.StatusCode, string.Format("{0}: {1}",
                response.ReasonPhrase,
                errorMessage));
        }
	}
}