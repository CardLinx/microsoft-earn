//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
namespace Lomo.Core.Ftp
{
	using System.IO;
	using System.Net;
	using System.Text;
	using System.Diagnostics;
	using System.Collections.Generic;

	public class FtpClient
	{
		private readonly FtpWebRequest _request;

		public FtpClient(string uri, string userName, string password, bool useSsl = true)
		{
			_request = WebRequest.Create(uri) as FtpWebRequest;
			Debug.Assert(_request != null, "request != null");
			_request.Credentials = new NetworkCredential(userName, password);
			_request.EnableSsl = useSsl;
			_request.KeepAlive = false;
		}

		public IList<string> List()
		{
			_request.Method = WebRequestMethods.Ftp.ListDirectory;
			using (var response = _request.GetResponse())
			{
				var stream = response.GetResponseStream();
				Debug.Assert(stream != null, "stream != null");
				using (var streamReader = new StreamReader(stream))
				{
					var result = new List<string>();
					while (true)
					{
						var line = streamReader.ReadLine();
						if (line == null)
							return result;
						result.Add(line);
					}
				}
			}
		}

		public string Upload(string fileContent, Encoding encoding, bool binary = false)
		{
			_request.Method = WebRequestMethods.Ftp.UploadFile;
			_request.UseBinary = binary;
			using (var requestStream = _request.GetRequestStream())
			{
				var bytes = encoding.GetBytes(fileContent);
				requestStream.Write(bytes, 0, bytes.Length);
			}
			using (var response = _request.GetResponse() as FtpWebResponse)
			{
				Debug.Assert(response != null, "response != null");
				return response.StatusDescription;
			}
		}

		public string Download(bool binary = false)
		{
			_request.Method = WebRequestMethods.Ftp.DownloadFile;
			_request.UseBinary = binary;
			using (var response = _request.GetResponse())
			using (var stream = response.GetResponseStream())
			{
				Debug.Assert(stream != null, "stream != null");
				using (var reader = new StreamReader(stream))
					return reader.ReadToEnd();
			}
		}
	}
}