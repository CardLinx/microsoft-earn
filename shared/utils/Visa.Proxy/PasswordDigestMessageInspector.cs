//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System.Net;
using System.Text;

namespace Visa.Proxy
{
    using Microsoft.Web.Services3.Security.Tokens;
    using System;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.Xml;

    /// <summary>
    /// Custom ClientMessageInspector to modify the message before it is sent
    /// and include a Hashed Password Digest UsernameToken in accordance to 
    /// the WS-I Basic Profile Security profile. The item shall be added to the
    /// SOAP request headers.
    /// </summary>
    public class PasswordDigestMessageInspector : IClientMessageInspector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordDigestMessageInspector" /> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public PasswordDigestMessageInspector(string username, string password)
        {
            Username = username;
            Password = password;
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        #region IClientMessageInspector Members

        /// <summary>
        /// The after receive reply.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <param name="correlationState">The correlation state.</param>
        /// <exception cref="NotImplementedException">This method is not implemented.</exception>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// The before send request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="channel">The channel.</param>
        /// <returns>
        /// The <see cref="object" />.
        /// </returns>
        public object BeforeSendRequest(ref Message request, System.ServiceModel.IClientChannel channel)
        {
            // Use the WSE 3.0 security token class
            var token = new UsernameToken(Username, Password, PasswordOption.SendPlainText);

            // Serialize the token to XML
            var securityToken = token.GetXml(new XmlDocument());

            // build the security header
            var securityHeader = MessageHeader.CreateHeader("Security", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", securityToken, false);
            request.Headers.Add(securityHeader);
            
            /*
            if (request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
            {
                request.Properties.Remove(HttpRequestMessageProperty.Name);
            }
            request.Properties.Add(HttpRequestMessageProperty.Name, GetAuthHeader());
            */
            
            object httpRequestMessageObject;
            if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out httpRequestMessageObject))
            {
                var httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;
                if (httpRequestMessage != null)
                {
                    httpRequestMessage.Headers[HttpRequestHeader.Authorization] = GetToken();
                }
            }
            else
            {
                request.Properties.Add(HttpRequestMessageProperty.Name, GetAuthHeader());
            }

            // complete
            return Convert.DBNull;
        }


        private HttpRequestMessageProperty GetAuthHeader()
        {
            HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty();
            httpRequestProperty.Headers[HttpRequestHeader.Authorization] = GetToken();
            return httpRequestProperty;
        }

        private string GetToken()
        {
            return "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(Username + ":" + Password));
        }

        #endregion
    }
}