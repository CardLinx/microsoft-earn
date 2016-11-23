//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.WebAPI
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Diagnostics;
    using System.Security.Cryptography.X509Certificates;
    using Cryptography;
    using Functional;
    using Logging;
    using System.Web;
    using System.Threading.Tasks;

    public static class HttpRequestMessageExtensions
    {
        public static HttpResponseMessage Execute<TResult>(
            this HttpRequestMessage request,
            Closure<TResult> closure,
            string certificateName = null)
        {
            return request.ExecuteAsync<TResult>(() => Task.FromResult<TResult>(closure()), certificateName).Result;
        }

        public static async Task<HttpResponseMessage> ExecuteAsync<TResult>(
            this HttpRequestMessage request,
            Closure<Task<TResult>> closure,
			string certificateName = null)
        {
            var sw = new Stopwatch();
            sw.Start();
            try
            {
#if NO_CERT_CHECK 
#else
				if (certificateName != null)
				{
					var remoteCert = request.GetClientCertificate();
					if (remoteCert == null)
						throw new Exception("No certificate found in the request");
					var localCert = Certificates.ByName(certificateName, StoreLocation.CurrentUser);
					if (localCert == null)
						throw new Exception("No certificate found in the store");
					if (remoteCert.Thumbprint != localCert.Thumbprint)
						throw new Exception("Certificate thumbprints do not match");
				}
#endif
                var result = request.CreateResponse(HttpStatusCode.OK, await closure());
                result.SetMediaType();
                sw.Stop();
                Log.Information(BuildLogMessage(request, sw));
                return result;
            }
            catch (HttpException e)
            {
                // Enable callers to define their own Http status code (e.g. 404 BadRequest)
                sw.Stop();
                Log.Error(BuildLogMessage(request, sw, e));

                var httpStatus = HttpStatusCode.InternalServerError;
                if (Enum.IsDefined(typeof(HttpStatusCode), e.GetHttpCode()))
                {
                    httpStatus = (HttpStatusCode)e.GetHttpCode();
                }

                var result = request.CreateResponse(httpStatus, e.Message);
                result.Content.Headers.ContentType.MediaType = "text/html";
                return result;
            }
            catch (Exception e)
            {
                sw.Stop();
                Log.Error(BuildLogMessage(request, sw, e));
                var result = request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
                result.Content.Headers.ContentType.MediaType = "text/html";
                return result;
            }
        }

        private static void SetMediaType(this HttpResponseMessage response)
        {
            const string FMT = "FMT";
            const string JSON = "JSON";
            var mediaType = "text/html";
            var query = response.RequestMessage.GetQueryNameValuePairs().ToDictionary(x => x.Key.ToUpperInvariant(), x => x.Value);
            if (query.ContainsKey(FMT))
                if (query[FMT].ToUpperInvariant() == JSON)
                    mediaType = "application/json";
            response.Content.Headers.ContentType.MediaType = mediaType;
        }

        private static string BuildLogMessage(HttpRequestMessage request, Stopwatch sw)
        {
            return string.Format(
                "{0} {1} {2:N2} ms",
                request.Method.Method,
                request.RequestUri,
                sw.Elapsed.TotalMilliseconds);
        }

        private static string BuildLogMessage(HttpRequestMessage request, Stopwatch sw, Exception e)
        {
            return string.Format(
                "{0} {1}\r\n{2}",
                BuildLogMessage(request, sw),
                e.Message,
                e.StackTrace);
        }
    }
}