/*
	Copyright 2017, VIA Technologies, Inc. & OLAMI Team.

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at

	http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

using System;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Security.Cryptography;

namespace Ai.Olami.Example
{
    class NluApiSample
    {
        public static string API_NAME_SEG = "seg";
        public static string API_NAME_NLI = "nli";

        private string apiBaseUrl;
        private string appKey;
        private string appSecret;

        /// <summary>
        /// Setup your authorization information to access OLAMI services.
        /// </summary>
        /// <param name="appKey">The AppKey you got from OLAMI developer console.</param>
        /// <param name="appSecret">The AppSecret you from OLAMI developer console.</param>
        public void SetAuthorization(string appKey, string appSecret)
        {
            this.appKey = appKey;
            this.appSecret = appSecret;
        }

        /// <summary>
        /// Setup localization to select service area, this is related to different server URLs or languages, etc.
        /// </summary>
        /// <param name="apiBaseURL">The URL of the API service.</param>
        public void SetLocalization(string apiBaseURL)
        {
            this.apiBaseUrl = apiBaseURL;
        }

        /// <summary>
        /// Get the NLU recognition result for your input text.
        /// </summary>
        /// <param name="apiName">The API name for 'api=xxx' HTTP parameter.</param>
        /// <param name="inputText">The text you want to recognize.</param>
        /// <returns>Recognition result</returns>
        public string GetRecognitionResult(string apiName, string inputText)
        {
            string result;
            double timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            // Prepare message to generate an MD5 digest.
            StringBuilder signMsg = new StringBuilder()
                .Append(appSecret)
                .Append("api=")
                .Append(apiName)
                .Append("appkey=")
                .Append(appKey)
                .Append("timestamp=")
                .Append(timestamp)
                .Append(appSecret);

            // Generate MD5 digest.
            MD5 md5 = MD5.Create();
            StringBuilder sign = new StringBuilder();
            byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(signMsg.ToString()));
            for (int i = 0; i < hash.Length; i++)
            {
                sign.Append(hash[i].ToString("X2"));
            }

            // Request NLU service by HTTP POST
            using (WebClient client = new WebClient())
            {
                Console.WriteLine("Sending 'POST' request to URL : " + apiBaseUrl);

                NameValueCollection apiParam = null;

                if (apiName == NluApiSample.API_NAME_SEG)
                {
                    apiParam = new NameValueCollection()
                    {
                        { "appkey", appKey },
                        { "api", NluApiSample.API_NAME_SEG },
                        { "timestamp", timestamp.ToString() },
                        { "sign", sign.ToString() },
                        { "rq", inputText }
                    };
                }
                else if (apiName == NluApiSample.API_NAME_NLI)
                {
                    apiParam = new NameValueCollection()
                    {
                        { "appkey", appKey },
                        { "api", NluApiSample.API_NAME_NLI },
                        { "timestamp", timestamp.ToString() },
                        { "sign", sign.ToString() },
                        { "rq", "{\"data_type\":\"stt\", \"data\":{\"input_type\":1,\"text\":\"" + inputText + "\"}}" }
                    };
                }

                byte[] response = client.UploadValues(apiBaseUrl, apiParam);

                result = System.Text.Encoding.UTF8.GetString(response);
            }

            return result;
        }
    }
}
