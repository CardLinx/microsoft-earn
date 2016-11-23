//
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using Azure.Utils;
using Lomo.Commerce.AmexClient;
using Utilities;

namespace AcsToken
{
    class Program
    {
        //************************* Payment Auth **************************************
        const string QaEnvironmentBaseAddress = "https://TODO_YOUR_INT_DOMAIN_HERE";
        const string PaymentAuthEndpoint = "/api/commerce/amex/auth";

        //************************* OAUTH **************************************
        const string QaOAuthBaseAddress = "https://TODO_YOUR_INT_AUTH_DOMAIN_HERE";
        const string OAuthEndpoint = "/v2/OAuth2-13";

        const string GrantType = "client_credentials";
        //--- INT ----
        //const string ClientId = "TODO_YOUR_CLIENT_ID_HERE";
        //const string PrimarySecret = "TODO_YOUR_SECRET_HERE";
        //const string SecondarySecret = "TODO_YOUR_SECRET_HERE";

        //---PROD---
        const string ClientId = "TODO_YOUR_CLIENT_ID_HERE";
        const string PrimarySecret = "TODO_YOUR_SECRET_HERE";

        //-----------------------------Amex QA---------------------------------------------
        // documented post payload did not work
        const string AmexOAuthPayload = "grant_type=client_credentials&app_spec_info=Apigee&guid_type=privateguid";

        //const string AmexOAuthUri = "https://api.qa.americanexpress.com/apiplatform/v2/oauth/token/mac";
        //const string AmexSyncUri = "https://api.qa.americanexpress.com/v3/smartoffers/sync";
        //const string AmexUnSyncUri = "https://api.qa.americanexpress.com/v3/smartoffers/unsync";
        //const string AmexClientId = "TODO_AMEX_CLIENT_ID_HERE";
        //const string AmexPrimarySecret = "TODO_AMEX_SECRET_HERE";

        //-----------------------------Amex Prod---------------------------------------------
        const string AmexOAuthUri = "https://api.americanexpress.com/apiplatform/v2/oauth/token/mac";
        const string AmexSyncUri = "https://api.americanexpress.com/v3/smartoffers/sync";
        const string AmexUnSyncUri = "https://api.americanexpress.com/v3/smartoffers/unsync";
        private const string AmexClientId = "TODO_AMEX_CLIENT_ID_HERE";
        const string AmexPrimarySecret = "TODO_AMEX_SECRET_HERE";

        static void Main(string[] args)
        {
            /*
           // Get OAuth2 SWT from Azure Access Control Service
           Console.WriteLine("Getting access token from ACS for client id: {0}\n", ClientId);
           string acsToken = GetTokenFromACS();

           if (string.IsNullOrWhiteSpace(acsToken))
           {
               Console.WriteLine("Failed to get access token from ACS for client id: {0}\n", ClientId);
           }
           else
           {
               Console.WriteLine("access_token: {0}\n", acsToken);
               // Call MSFT hosted payment authorization endpoint.
               CallMSFTAuthEndpoint(acsToken);
           }
            */

            // Get OAuth2 SWT from AMEX
            //Console.WriteLine("\ngetting Amex oauth token\n");
            //AmexOAuth2TokenResponse tokenResponse = GetTokenFromAmex();
            //Console.WriteLine("\ninvoking Amex sync\n");
            //CallAmexSyncEndpoint(tokenResponse);
            //Console.WriteLine("\ninvoking Amex unsync\n");
            //CallAmexUnSyncEndpoint(tokenResponse);


            //****************************Staging Test****************************

            // Merchant Registration
            //SendMerchantRegistrationFile();
            //SendMerchantRegistrationFileToWorker();

            //ProcessMerchantRegistrationResponseFiles();

            //ProcessMerchantRegistrationResponseFilesFromDisk();

            // Transaction Log
            //ProcessTransactionLogFiles();

            //ProcessTransactionLogFilesFromDisk();

            // Statement of Credit
            //SendStatementofCreditFile();

            //ProcessStatementofCreditResponseFiles();


            //****************************Production Test****************************

            SendMerchantRegistrationFileToWorker();

            //SendStatementCreditToUser();

            Console.ReadLine();
        }

        static string GetTokenFromACS()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                //httpClient.BaseAddress = new Uri(AmexQaOAuthBaseAddress);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                HttpContent httpContent = new FormUrlEncodedContent(
                  new List<KeyValuePair<string, string>>
                  {
                      new KeyValuePair<string, string>("grant_type", GrantType),
                      //new KeyValuePair<string, string>("scope", QaEnvironmentBaseAddress + PaymentAuthEndpoint),
                      new KeyValuePair<string, string>("scope", "https://TODO_YOUR_DOMAIN_HERE/api/commerce/amex/auth"),
                      new KeyValuePair<string, string>("client_id", ClientId),
                      new KeyValuePair<string, string>("client_secret", PrimarySecret)
                  }
                );

                try
                {
                    HttpResponseMessage response = httpClient.PostAsync("https://TODO_PROD_AUTH_DOMAIN_HERE/v2/OAuth2-13", httpContent).Result;
                    //HttpResponseMessage response = httpClient.PostAsync(QaOAuthBaseAddress + OAuthEndpoint, httpContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        OAuth2TokenResponse tokenResponse = (OAuth2TokenResponse)new DataContractJsonSerializer(typeof(OAuth2TokenResponse)).ReadObject(response.Content.ReadAsStreamAsync().Result);
                        return tokenResponse.Token;
                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return null;
        }

        static void CallMSFTAuthEndpoint(string token)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                //httpClient.BaseAddress = new Uri("http://localhost:81");
                //httpClient.BaseAddress = new Uri(QaEnvironmentBaseAddress);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // Bearer - case sensitive
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                string request = @"{
                    transaction_time: '2016-05-27 20:30:15',
                    transaction_id: '004035',
                    transaction_amount: '10.88',
                    cm_alias: '1234567890abcdef1234567890abcdef',
                    merchant_number: '1234567890', 
                    offer_id: '000000'
                }"; // TODO REAL_MERCHANT_NUMBER AND CM_ALIAS

                HttpContent httpContent = new StringContent(request, Encoding.UTF8, "application/json");
                try
                {
                    Console.WriteLine("Request: {0}\n", request);
                    //HttpResponseMessage response = httpClient.PostAsync(PaymentAuthEndpoint, httpContent).Result;
                    HttpResponseMessage response = httpClient.PostAsync("http://localhost:81/api/commerce/amex/auth", httpContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Response: {0}\n", response.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode + " " + response.Content.ReadAsStringAsync().Result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static AmexOAuth2TokenResponse GetTokenFromAmex()
        {
            string timestamp = ((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
            string nonce = string.Format("{0}:AMEX", timestamp);
            //Format = client_id \n timestamp \n timestamp:AMEX \n mac
            string baseString = string.Format("{0}\n{1}\n{2}\n{3}\n", AmexClientId, timestamp, nonce, GrantType);

            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(AmexPrimarySecret)))
            {
                string mac = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(baseString)));

                // Format = MAC id="client_id",ts="timestamp",nonce="timestamp:AMEX",mac="HMACSHA256 hash"
                StringBuilder authenticationHeader = new StringBuilder();
                authenticationHeader.Append("MAC id=\"");
                authenticationHeader.Append(AmexClientId);
                authenticationHeader.Append("\",ts=\"");
                authenticationHeader.Append(timestamp);
                authenticationHeader.Append("\",nonce=\"");
                authenticationHeader.Append(nonce);
                authenticationHeader.Append("\",mac=\"");
                authenticationHeader.Append(mac);
                authenticationHeader.Append("\"");

                Console.WriteLine("Oauth Authentication Header: {0}", authenticationHeader);
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Add("Authentication", authenticationHeader.ToString());
                    httpClient.DefaultRequestHeaders.Add("X-AMEX-API-KEY", AmexClientId);
                    HttpContent httpContent = new StringContent(AmexOAuthPayload);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    try
                    {
                        HttpResponseMessage response = httpClient.PostAsync(AmexOAuthUri, httpContent).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                            AmexOAuth2TokenResponse tokenResponse = (AmexOAuth2TokenResponse)new DataContractJsonSerializer(typeof(AmexOAuth2TokenResponse)).ReadObject(response.Content.ReadAsStreamAsync().Result);
                            return tokenResponse;
                        }
                        else
                        {
                            Console.WriteLine(response.StatusCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return null;
        }

        //For adding card
        static void CallAmexSyncEndpoint(AmexOAuth2TokenResponse tokenResponse)
        {
            if (tokenResponse == null)
            {
                Console.WriteLine("Invalid token response");
            }
            else
            {
                string timestamp = ((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
                string nonce = string.Format("{0}:AMEX", timestamp);
                //Format = timestamp \n timestamp:AMEX \n method \n path \n port \n
                UriBuilder uri = new UriBuilder(AmexSyncUri);
                string baseString = string.Format("{0}\n{1}\nPOST\n{2}\n{3}\n{4}\n\n", timestamp, nonce, Regex.Replace(HttpUtility.UrlEncode(uri.Path.ToLowerInvariant()), @"%[a-f0-9]{2}", c => c.Value.ToUpper()), uri.Host.ToLowerInvariant(), uri.Port);
                //string baseString = string.Format("{0}\n{1}\nPOST\n%2Fv3%2Fsmartoffers%2Fsync\napi.qa.americanexpress.com\n443\n\n", timestamp, nonce);

                Console.WriteLine("baseString: {0}", baseString);
                using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(tokenResponse.MacKey)))
                {
                    string mac = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(baseString)));
                    // Format = MAC id="access_token",ts="timestamp",nonce="timestamp:AMEX",mac="HMACSHA256 hash"
                    //StringBuilder authorizationHeader = new StringBuilder();
                    //authorizationHeader.Append("MAC id=\"");
                    //authorizationHeader.Append(tokenResponse.Token);
                    //authorizationHeader.Append("\",ts=\"");
                    //authorizationHeader.Append(timestamp);
                    //authorizationHeader.Append("\",nonce=\"");
                    //authorizationHeader.Append(nonce);
                    //authorizationHeader.Append("\",mac=\"");
                    //authorizationHeader.Append(mac);
                    //authorizationHeader.Append("\"");

                    string authorizationHeader = string.Format("MAC id=\"{0}\",ts=\"{1}\",nonce=\"{2}\",mac=\"{3}\"", tokenResponse.Token, timestamp, nonce, mac);

                    Console.WriteLine("Authorization header: {0}", authorizationHeader);
                    string apiRequestPayload = "{" + "\"msgId\": \"HS256\","
                        + "\"partnerId\": \"AAAA1234\","
                        + "\"distrChan\": \"9999\","
                        + "\"cardNbr\": \"312345678901234\","
                        + "\"cmAlias1\": \"1234567890abcdef1234567890abcdef\"}" + "";

                    // card 2
                    var request = new AmexCardSyncRequest
                    {
                        CardNumber = "312345678901234",
                        CardToken1 = "1234567890abcdef1234567890abcdef"
                    };

                    //HttpClient httpClient = new HttpClient();
                    //HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, AmexSyncEndpoint);
                    //requestMessage.Headers.Add("Authorization", authorizationHeader.ToString());
                    //requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    //requestMessage.Headers.Add("X-AMEX-ACCESS-KEY", AmexClientId);
                    //requestMessage.Content = new StringContent(apiRequestPayload);
                    //HttpResponseMessage response = httpClient.SendAsync(requestMessage).Result;
                    //Console.WriteLine("Response code: {0}", response.StatusCode);

                    using (HttpClient httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("MAC", authorizationHeader.ToString());
                        //httpClient.DefaultRequestHeaders.Add("Authentication", authorizationHeader.ToString());
                        httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader.ToString());
                        httpClient.DefaultRequestHeaders.Add("X-AMEX-API-KEY", AmexClientId);
                        //httpClient.DefaultRequestHeaders.Add("X-AMEX-ACCESS-KEY", AmexClientId);
                        //httpClient.DefaultRequestHeaders.Add("X-AMEX-MSG-ID", Guid.NewGuid().ToString());
                        HttpContent httpContent = new StringContent(apiRequestPayload);
                        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        try
                        {
                            //HttpResponseMessage response = httpClient.PostAsync(AmexSyncUri, httpContent).Result;
                            HttpResponseMessage response = httpClient.PostAsJsonAsync(AmexSyncUri, request).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                Console.WriteLine(response.StatusCode);
                                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                            }
                            else
                            {
                                Console.WriteLine(response.StatusCode);
                                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }
        
        //For removing card
        static void CallAmexUnSyncEndpoint(AmexOAuth2TokenResponse tokenResponse)
        {
            if (tokenResponse == null)
            {
                Console.WriteLine("Invalid token response");
            }
            else
            {
                string timestamp = ((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
                string nonce = string.Format("{0}:AMEX", timestamp);
                //Format = timestamp \n timestamp:AMEX \n method \n path \n port \n
                UriBuilder uri = new UriBuilder(AmexUnSyncUri);
                string baseString = string.Format("{0}\n{1}\nPOST\n{2}\n{3}\n{4}\n\n", timestamp, nonce, Regex.Replace(HttpUtility.UrlEncode(uri.Path.ToLowerInvariant()), @"%[a-f0-9]{2}", c => c.Value.ToUpper()), uri.Host.ToLowerInvariant(), uri.Port);
                //string baseString = string.Format("{0}\n{1}\nPOST\n%2Fv3%2Fsmartoffers%2Fsync\napi.qa.americanexpress.com\n443\n\n", timestamp, nonce);

                Console.WriteLine("baseString: {0}", baseString);
                using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(tokenResponse.MacKey)))
                {
                    string mac = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(baseString)));
                    // Format = MAC id="access_token",ts="timestamp",nonce="timestamp:AMEX",mac="HMACSHA256 hash"
                    //StringBuilder authorizationHeader = new StringBuilder();
                    //authorizationHeader.Append("MAC id=\"");
                    //authorizationHeader.Append(tokenResponse.Token);
                    //authorizationHeader.Append("\",ts=\"");
                    //authorizationHeader.Append(timestamp);
                    //authorizationHeader.Append("\",nonce=\"");
                    //authorizationHeader.Append(nonce);
                    //authorizationHeader.Append("\",mac=\"");
                    //authorizationHeader.Append(mac);
                    //authorizationHeader.Append("\"");

                    string authorizationHeader = string.Format("MAC id=\"{0}\",ts=\"{1}\",nonce=\"{2}\",mac=\"{3}\"", tokenResponse.Token, timestamp, nonce, mac);

                    Console.WriteLine("Authorization header: {0}", authorizationHeader);
                    string apiRequestPayload = "{" + "\"msgId\": \"HS256\","
                        + "\"partnerId\": \"AAAA0030\","
                        + "\"distrChan\": \"9994\","
                        // + "\"cardNbr\": \"373459999999999\","
                        + "\"cmAlias1\": \"0beadcceec7a43c5a0e6a54c54a0d525\"}" + "";

                    var request = new AmexCardUnSyncRequest
                    {
                        CardToken1 = "89536311e6f246119fb2a448d74f4877"
                    };

                    //HttpClient httpClient = new HttpClient();
                    //HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, AmexSyncEndpoint);
                    //requestMessage.Headers.Add("Authorization", authorizationHeader.ToString());
                    //requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    //requestMessage.Headers.Add("X-AMEX-ACCESS-KEY", AmexClientId);
                    //requestMessage.Content = new StringContent(apiRequestPayload);
                    //HttpResponseMessage response = httpClient.SendAsync(requestMessage).Result;
                    //Console.WriteLine("Response code: {0}", response.StatusCode);

                    using (HttpClient httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Accept.Clear();
                        //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("MAC", authorizationHeader.ToString());
                        //httpClient.DefaultRequestHeaders.Add("Authentication", authorizationHeader.ToString());
                        httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader.ToString());
                        httpClient.DefaultRequestHeaders.Add("X-AMEX-API-KEY", AmexClientId);
                        //httpClient.DefaultRequestHeaders.Add("X-AMEX-ACCESS-KEY", AmexClientId);
                        //httpClient.DefaultRequestHeaders.Add("X-AMEX-MSG-ID", Guid.NewGuid().ToString());
                        HttpContent httpContent = new StringContent(apiRequestPayload);
                        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        try
                        {
                            //HttpResponseMessage response = httpClient.PostAsync(AmexUnSyncUri, httpContent).Result;
                            HttpResponseMessage response = httpClient.PostAsJsonAsync(AmexUnSyncUri, request).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                Console.WriteLine(response.StatusCode);
                                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                            }
                            else
                            {
                                Console.WriteLine(response.StatusCode);
                                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }

        //Adding a new MID
        private static void SendMerchantRegistrationFileToWorker()
        {
            Console.WriteLine("Preparing merchant registration file.");
            AzureBlob merchantRegistrationAzureBlob = new AzureBlob(WebConfigurationManager.AppSettings["CommerceStorageConnectionString"]);

            List<OfferRegistrationDetail> offerRegistrationDetails = new List<OfferRegistrationDetail>
            {
                //new OfferRegistrationDetail()
                //{
                //    ActionCode = OfferRegistrationActionCodeType.Add, // Need to update if already added Merchant
                //    MerchantName = "PETSMART INC 2055",
                //    MerchantNumber = "5461297830",
                //    MerchantEndDate = new DateTime(2026, 06, 03),
                //    MerchantId = "95f6a179-2a5f-48eb-8488-19dc10976739",
                //    OfferName = "Get 5% back in Earn Credits",
                //    MerchantStartDate = new DateTime(2016, 06, 03)
                //},
                //new OfferRegistrationDetail()
                //{
                //    ActionCode = OfferRegistrationActionCodeType.Add, // Need to update if already added Merchant
                //    MerchantName = "PETSMART INC 3005",
                //    MerchantNumber = "5465810323",
                //    MerchantEndDate = new DateTime(2026, 06, 03),
                //    MerchantId = "19eadc50-dbdf-4a94-b1e3-29b40d70b333",
                //    OfferName = "Get 5% back in Earn Credits",
                //    MerchantStartDate = new DateTime(2016, 06, 03)
                //}

                new OfferRegistrationDetail()
                {
                    ActionCode = OfferRegistrationActionCodeType.Add, // Need to update if already added Merchant
                    MerchantName = "TEST MERCHANT",
                    MerchantNumber = "1234567890",
                    MerchantEndDate = new DateTime(2026, 06, 03),
                    MerchantId = "4fc64cb3-d5d2-4fb7-92cf-123456789012",
                    OfferName = "Microsoft Earn",
                    MerchantStartDate = new DateTime(2016, 06, 03)
                },
            };

            foreach (var detail in offerRegistrationDetails)
            {
                string blobName = "ToBeProcessed/" + detail.MerchantNumber + "-" + GuidUtility.GenerateShortGuid() + ".txt";
                byte[] contentBytes = Encoding.ASCII.GetBytes(detail.BuildFileDetailRecord());
                MemoryStream ms = new MemoryStream(contentBytes)
                {
                    Position = 0
                };

                Task task = merchantRegistrationAzureBlob.UploadBlobFromStreamAsync("amex-offer-registrationrecords", blobName, ms);
                Task.WaitAny(task);
            }
        }

        private static void SendMerchantRegistrationFile()
        {
            Console.WriteLine("Preparing merchant registration file.");
            //"D|A|AAAA0030|1.0|                              |               |1020218384|000000000|Buy One Get One Free|||USD|My Burgers Inc||04/01/2016|12/31/2017||||||||||||        "
            string detailRecord1 = OfferRegistrationRecordBuilder.BuildAddOffer("95f6a179-2a5f-48eb-8488-19dc10976739", "Get 5% back in Earn Credits", new DateTime(2016, 06, 03), new DateTime(2026, 06, 03), "5461297830", "PETSMART INC 2055");

            string detailRecord2 = OfferRegistrationRecordBuilder.BuildAddOffer("19eadc50-dbdf-4a94-b1e3-29b40d70b333", "Get 5% back in Earn Credits", new DateTime(2016, 06, 03), new DateTime(2026, 06, 03), "5465810323", "PETSMART INC 3005");

            string file = OfferRegistrationFileBuilder.Build(
                new Collection<string>() { detailRecord1, detailRecord2 },
                8,
                DateTime.UtcNow);

            // MSF_AXP_mer_reg_ yymmdd_hhmmss.txt
            string dateformatted = DateTime.UtcNow.ToString("yyMMdd_hhmmss");
            MemoryStream stream = new MemoryStream();
            SftpClient sftpClient = new SftpClient("BRCTST", "amex123", "fsgatewaytest.aexp.com");
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(file);
                writer.Flush();
                stream.Position = 0;

                Console.WriteLine("Uploading file to sftp server.");
                sftpClient.UploadFileAsync("MSF_AXP_mer_reg_" + dateformatted + ".txt", stream, "inbox").Wait();
                Console.WriteLine("Upload complete.");
            }
        }

        private static void ProcessMerchantRegistrationResponseFiles()
        {
            SftpClient sftpClient = new SftpClient("BRCTST", "amex123", "fsgatewaytest.aexp.com");

            // check the response 
            Console.WriteLine("Reading all response files");
            string[] responseFileNames = sftpClient.DirectoryListAsync("MSF_AXP_MER_REG_RESP_", "outbox").Result;
            if (responseFileNames == null || responseFileNames.Length == 0)
            {
                Console.WriteLine("No files to process on sftp server.");
            }
            else
            {
                Console.WriteLine("Found {0} files\n {1} \n", responseFileNames.Length, string.Join("\n", responseFileNames));
                foreach (string fileName in responseFileNames)
                {
                    Console.WriteLine("Downloading file: {0}", fileName);

                    using (MemoryStream stream = new MemoryStream())
                    {
                        sftpClient.DownloadFileAsync(fileName, stream, "outbox").Wait();
                        stream.Position = 0;
                        //await ResponseFileBlobClient.UploadAsync(stream, fileName).ConfigureAwait(false);
                        // writing it to the disk
                        FileStream fileStream = File.Create(@"C:\Users\myuser\Desktop\Amex Test\" + fileName, (int)stream.Length);
                        byte[] bytesInStream = new byte[stream.Length];
                        stream.Read(bytesInStream, 0, bytesInStream.Length);
                        fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                        fileStream.Close();
                    }

                    // parsing the file
                    Console.WriteLine("Parsing the response file: {0}", fileName);
                    OfferRegistrationResponseFile responseFile = new OfferRegistrationResponseFile();
                    string recordType;
                    OfferRegistrationResponseHeader header = null;
                    OfferRegistrationResponseTrailer trailer = null;
                    Collection<OfferRegistrationResponseDetail> detailRecords = new Collection<OfferRegistrationResponseDetail>();

                    using (StreamReader reader = new StreamReader(@"C:\Users\myuser\Desktop\Amex Test\" + fileName))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            recordType = line.Substring(0, 1);
                            switch (recordType)
                            {
                                case "H":
                                    header = new OfferRegistrationResponseHeader(line);
                                    break;
                                case "D":
                                    detailRecords.Add(new OfferRegistrationResponseDetail(line));
                                    break;
                                case "T":
                                    trailer = new OfferRegistrationResponseTrailer(line);
                                    break;
                            }
                        }
                    }

                    // verify integrity
                    if (trailer.TrailerCount != detailRecords.Count)
                    {
                        Console.WriteLine("Number of Records suggested by trailer Amex Offer Registration Response file \"{0}\" do not match.", fileName);
                    }

                    responseFile.Header = header;
                    responseFile.Trailer = trailer;
                    foreach (OfferRegistrationResponseDetail detailRecord in detailRecords)
                    {
                        responseFile.ResponseRecords.Add(detailRecord);
                    }

                    Console.WriteLine("Successfully parsed the response file: {0}", fileName);
                }
            }
        }

        private static void ProcessMerchantRegistrationResponseFilesFromDisk()
        {

            string fileName = "MSF_AXP_MER_REG_RESP_160503141328.csv";
            // parsing the file
            Console.WriteLine("Parsing the response file: {0}", fileName);
            OfferRegistrationResponseFile responseFile = new OfferRegistrationResponseFile();
            string recordType;
            OfferRegistrationResponseHeader header = null;
            OfferRegistrationResponseTrailer trailer = null;
            Collection<OfferRegistrationResponseDetail> detailRecords = new Collection<OfferRegistrationResponseDetail>();

            using (StreamReader reader = new StreamReader(@"C:\Users\myuser\Desktop\Amex Test\" + fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    recordType = line.Substring(0, 1);
                    switch (recordType)
                    {
                        case "H":
                            header = new OfferRegistrationResponseHeader(line);
                            break;
                        case "D":
                            detailRecords.Add(new OfferRegistrationResponseDetail(line));
                            break;
                        case "T":
                            trailer = new OfferRegistrationResponseTrailer(line);
                            break;
                    }
                }
            }


            // verify integrity
            if (trailer.TrailerCount != detailRecords.Count)
            {
                Console.WriteLine("Number of Records suggested by trailer Amex Offer Registration Response file \"{0}\" do not match.", fileName);
            }

            responseFile.Header = header;
            responseFile.Trailer = trailer;
            foreach (OfferRegistrationResponseDetail detailRecord in detailRecords)
            {
                responseFile.ResponseRecords.Add(detailRecord);
            }

            Console.WriteLine("Successfully parsed the response file: {0}", fileName);
        }

        private static void ProcessTransactionLogFiles()
        {
            SftpClient sftpClient = new SftpClient("BRCTST", "amex123", "fsgatewaytest.aexp.com");

            // check the response 
            Console.WriteLine("Reading all transaction log files");
            string[] transactionFileNames = sftpClient.DirectoryListAsync("AXP_MSF_TLOG", "outbox").Result;
            if (transactionFileNames == null || transactionFileNames.Length == 0)
            {
                Console.WriteLine("No files to process on sftp server.");
            }
            else
            {
                Console.WriteLine("Found {0} files\n {1} \n", transactionFileNames.Length, string.Join("\n", transactionFileNames));
                foreach (string fileName in transactionFileNames)
                {
                    Console.WriteLine("Downloading file: {0}", fileName);

                    using (MemoryStream stream = new MemoryStream())
                    {
                        sftpClient.DownloadFileAsync(fileName, stream, "outbox").Wait();
                        stream.Position = 0;
                        //await ResponseFileBlobClient.UploadAsync(stream, fileName).ConfigureAwait(false);
                        // writing it to the disk
                        FileStream fileStream = File.Create(@"C:\Users\myuser\Desktop\Amex Test\" + fileName, (int)stream.Length);
                        byte[] bytesInStream = new byte[stream.Length];
                        stream.Read(bytesInStream, 0, bytesInStream.Length);
                        fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                        fileStream.Close();
                    }

                    // parsing the file
                    Console.WriteLine("Parsing the response file: {0}", fileName);
                    string recordType;
                    TransactionLogFile transactionLogFile = new TransactionLogFile();
                    TransactionLogHeader header = null;
                    TransactionLogTrailer trailer = null;
                    Collection<TransactionLogDetail> detailRecords = new Collection<TransactionLogDetail>();

                    using (StreamReader reader = new StreamReader(@"C:\Users\myuser\Desktop\Amex Test\" + fileName))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            recordType = line.Substring(0, 1);
                            switch (recordType)
                            {
                                case "H":
                                    header = new TransactionLogHeader(line);
                                    break;
                                case "D":
                                    detailRecords.Add(new TransactionLogDetail(line));
                                    break;
                                case "T":
                                    trailer = new TransactionLogTrailer(line);
                                    break;
                            }
                        }

                        // verify integrity
                        if (trailer.TrailerCount != detailRecords.Count)
                        {
                            Console.WriteLine("Number of Records suggested by trailer Amex TransactionLog file \"{0}\" do not match.", fileName);
                        }

                        transactionLogFile.Header = header;
                        transactionLogFile.Trailer = trailer;
                        foreach (TransactionLogDetail detailRecord in detailRecords)
                        {
                            transactionLogFile.TransactionLogRecords.Add(detailRecord);
                        }

                        Console.WriteLine("Successfully parsed the response file: {0}", fileName);
                    }
                }
            }
        }

        private static void ProcessTransactionLogFilesFromDisk()
        {
            string fileName = "MSF_TLOG.TXT";
            // parsing the file
            Console.WriteLine("Parsing the transaction log file: {0}", fileName);
            string recordType;
            TransactionLogFile transactionLogFile = new TransactionLogFile();
            TransactionLogHeader header = null;
            TransactionLogTrailer trailer = null;
            Collection<TransactionLogDetail> detailRecords = new Collection<TransactionLogDetail>();

            using (StreamReader reader = new StreamReader(@"C:\Users\myuser\Desktop\Amex Test\" + fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    recordType = line.Substring(0, 1);
                    switch (recordType)
                    {
                        case "H":
                            header = new TransactionLogHeader(line);
                            break;
                        case "D":
                            detailRecords.Add(new TransactionLogDetail(line));
                            break;
                        case "T":
                            trailer = new TransactionLogTrailer(line);
                            break;
                    }
                }

                // verify integrity
                if (trailer.TrailerCount != detailRecords.Count)
                {
                    Console.WriteLine("Number of Records suggested by trailer Amex TransactionLog file \"{0}\" do not match.", fileName);
                }

                transactionLogFile.Header = header;
                transactionLogFile.Trailer = trailer;
                foreach (TransactionLogDetail detailRecord in detailRecords)
                {
                    transactionLogFile.TransactionLogRecords.Add(detailRecord);
                }

                Console.WriteLine("Successfully parsed the response file: {0}", fileName);
            }
        }

        private static void SendStatementofCreditFile()
        {
            string fileName = "AXP_MSF_TLOG_160526140323.txt";
            // parsing the file
            Console.WriteLine("Parsing the transaction log file: {0}", fileName);
            string recordType;
            TransactionLogFile transactionLogFile = new TransactionLogFile();
            TransactionLogHeader header = null;
            TransactionLogTrailer trailer = null;
            Collection<TransactionLogDetail> detailRecords = new Collection<TransactionLogDetail>();

            using (StreamReader reader = new StreamReader(@"C:\Users\myuser\Desktop\Amex Test\" + fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    recordType = line.Substring(0, 1);
                    switch (recordType)
                    {
                        case "H":
                            header = new TransactionLogHeader(line);
                            break;
                        case "D":
                            detailRecords.Add(new TransactionLogDetail(line));
                            break;
                        case "T":
                            trailer = new TransactionLogTrailer(line);
                            break;
                    }
                }

                // verify integrity
                if (trailer.TrailerCount != detailRecords.Count)
                {
                    Console.WriteLine("Number of Records suggested by trailer Amex TransactionLog file \"{0}\" do not match.", fileName);
                }

                transactionLogFile.Header = header;
                transactionLogFile.Trailer = trailer;
                foreach (TransactionLogDetail detailRecord in detailRecords)
                {
                    transactionLogFile.TransactionLogRecords.Add(detailRecord);
                }

                Console.WriteLine("Preparing statement credit file.");
                if (transactionLogFile.TransactionLogRecords.Count > 0)
                {
                    StatementCreditFile file = new StatementCreditFile()
                    {
                        Header = new StatementCreditHeader()
                        {
                            Date = DateTime.UtcNow,
                            SequenceNumber = transactionLogFile.Header.SequenceNumber
                        }
                    };

                    decimal totalAmount = 0;
                    foreach (var item in transactionLogFile.TransactionLogRecords)
                    {
                        totalAmount += item.TransactionAmount;
                        StatementCreditDetail detail = new StatementCreditDetail()
                        {
                            CampaignName = "My Burgers Inc",
                            CardToken = item.CardToken.Trim(),
                            DiscountAmount = (decimal)item.TransactionAmount / 10,
                            OfferId = "000000000",
                            StatementDescriptor = "Test Credit",
                            TransactionId = item.TransactionId.ToString(CultureInfo.InvariantCulture)
                        };

                        file.StatementCreditRecords.Add(detail);
                    }

                    file.Trailer = new StatementCreditTrailer()
                    {
                        TrailerAmount = (decimal)totalAmount / 10,
                        TrailerCount = file.StatementCreditRecords.Count
                    };

                    StringBuilder fileBuilder = new StringBuilder();
                    fileBuilder.Append(file.Header.BuildFileHeader());
                    fileBuilder.Append("\n");
                    foreach (StatementCreditDetail statementCreditRecord in file.StatementCreditRecords)
                    {
                        fileBuilder.Append(statementCreditRecord.BuildFileDetailRecord());
                        fileBuilder.Append("\n");
                    }
                    fileBuilder.Append(file.Trailer.BuildFileTrailer());

                    // MSF_AXP_stmt_crdt_yymmdd_hhmmss.txt
                    string statementcreditFile = "MSF_AXP_stmt_crdt_" + DateTime.UtcNow.ToString("yyMMdd_hhmmss") + ".txt";
                    //MemoryStream stream = new MemoryStream();
                    //SftpClient sftpClient = new SftpClient("BRCTST", "amex123", "fsgatewaytest.aexp.com");
                    //using (StreamWriter writer = new StreamWriter(stream))
                    //{
                    //    writer.Write(fileBuilder.ToString());
                    //    writer.Flush();
                    //    stream.Position = 0;

                    //    Console.WriteLine("Uploading file to sftp server.");
                    //    sftpClient.UploadFileAsync(statementcreditFile, stream, "inbox").Wait();
                    //    Console.WriteLine("Upload complete.");
                    //}

                    using (MemoryStream stream = new MemoryStream())
                    {
                        StreamWriter writer = new StreamWriter(stream);
                        writer.Write(fileBuilder.ToString());
                        writer.Flush();
                        stream.Position = 0;
                        // writing it to the disk
                        FileStream fileStream = File.Create(@"C:\Users\myuser\Desktop\Amex Test\" + statementcreditFile, (int)stream.Length);
                        byte[] bytesInStream = new byte[stream.Length];
                        stream.Read(bytesInStream, 0, bytesInStream.Length);
                        fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                        fileStream.Close();
                    }
                }

                Console.WriteLine("Successfully sent statement credit file.");
            }
        }

        private static void ProcessStatementofCreditResponseFiles()
        {
            SftpClient sftpClient = new SftpClient("BRCTST", "amex123", "fsgatewaytest.aexp.com");

            // check the response 
            Console.WriteLine("Reading all response files");
            string[] responseFileNames = sftpClient.DirectoryListAsync("AXP_MSF_STMT_CRDT_ACK", "outbox").Result;
            if (responseFileNames == null || responseFileNames.Length == 0)
            {
                Console.WriteLine("No files to process on sftp server.");
            }
            else
            {
                Console.WriteLine("Found {0} files\n {1} \n", responseFileNames.Length, string.Join("\n", responseFileNames));
                foreach (string fileName in responseFileNames)
                {
                    Console.WriteLine("Downloading file: {0}", fileName);

                    using (MemoryStream stream = new MemoryStream())
                    {
                        sftpClient.DownloadFileAsync(fileName, stream, "outbox").Wait();
                        stream.Position = 0;
                        // writing it to the disk
                        FileStream fileStream = File.Create(@"C:\Users\myuser\Desktop\Amex Test\" + fileName, (int)stream.Length);
                        byte[] bytesInStream = new byte[stream.Length];
                        stream.Read(bytesInStream, 0, bytesInStream.Length);
                        fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                        fileStream.Close();
                    }

                    Console.WriteLine("Downloaded file: {0}", fileName);
                }
            }
        }

        private static void SendStatementCreditToUser()
        {
            Console.WriteLine("Preparing statement credit file.");

            StatementCreditFile file = new StatementCreditFile()
            {
                Header = new StatementCreditHeader()
                {
                    Date = DateTime.UtcNow,
                    SequenceNumber = 2
                }
            };

            decimal totalAmount = (decimal) 1.50;
            file.StatementCreditRecords.Add(new StatementCreditDetail()
            {
                CampaignName = "Microsoft Earn",
                CardToken = "99fad43401e74cfbb5215ec447dc4adb",
                DiscountAmount = totalAmount,
                OfferId = "000000000",
                StatementDescriptor = "Earn Credit Reversal Test",
                TransactionId = "000272786"
            });


            file.Trailer = new StatementCreditTrailer()
            {
                TrailerAmount = totalAmount,
                TrailerCount = file.StatementCreditRecords.Count
            };

            StringBuilder fileBuilder = new StringBuilder();
            fileBuilder.Append(file.Header.BuildFileHeader());
            fileBuilder.Append("\n");
            foreach (StatementCreditDetail statementCreditRecord in file.StatementCreditRecords)
            {
                fileBuilder.Append(statementCreditRecord.BuildFileDetailRecord());
                fileBuilder.Append("\n");
            }
            fileBuilder.Append(file.Trailer.BuildFileTrailer());

            string statementcreditFile = "MSF_AXP_stmt_crdt_" + DateTime.UtcNow.ToString("yyMMdd_hhmmss") + ".txt";
            MemoryStream stream = new MemoryStream();
            //SftpClient sftpClient = new SftpClient("BRCTST", "amex123", "fsgatewaytest.aexp.com");
            SftpClient sftpClient = new SftpClient("BRCPRD", "Amex123", "fsgateway.aexp.com");
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(fileBuilder.ToString());
                writer.Flush();
                stream.Position = 0;

                Console.WriteLine("Uploading file to sftp server.");
                sftpClient.UploadFileAsync(statementcreditFile, stream, "inbox").Wait();
                Console.WriteLine("Upload complete.");
            }

            //using (MemoryStream stream = new MemoryStream())
            //{
            //    StreamWriter writer = new StreamWriter(stream);
            //    writer.Write(fileBuilder.ToString());
            //    writer.Flush();
            //    stream.Position = 0;
            //    // writing it to the disk
            //    FileStream fileStream = File.Create(@"C:\Users\myuser\Desktop\Amex Test\" + statementcreditFile, (int)stream.Length);
            //    byte[] bytesInStream = new byte[stream.Length];
            //    stream.Read(bytesInStream, 0, bytesInStream.Length);
            //    fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            //    fileStream.Close();
            //}

            Console.WriteLine("Successfully sent statement credit file.");
        }
    }
}