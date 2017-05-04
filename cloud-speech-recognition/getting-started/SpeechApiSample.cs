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
    class SpeechApiSample
    {
        public static string API_NAME_ASR = "asr";

        private string apiBaseUrl;
        private string appKey;
        private string appSecret;

        private CookieContainer cookies = new CookieContainer();

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
        /// Send an audio file to speech recognition service.
        /// </summary>
        /// <param name="apiName">The API name for 'api=xxx' HTTP parameter.</param>
        /// <param name="seqValue">The value of 'seq' for 'seq=xxx' HTTP parameter.</param>
        /// <param name="finished">TRUE to finish upload or FALSE to continue upload.</param>
        /// <param name="filePath">The path of the audio file you want to upload.</param>
        /// <param name="compressed">TRUE if the audio file is a Speex audio.</param>
        /// <returns>HTTP Response Result</returns>
        public string SendAudioFile(String apiName, String seqValue, Boolean finished, String filePath, Boolean compressed)
        {
            string result;

            Console.WriteLine("The path of the audio file : " + filePath);
            byte[] file = File.ReadAllBytes(filePath);

            StringBuilder httpURL = new StringBuilder(apiBaseUrl)
                .Append("?")
                .Append(GetBasicQueryString(apiName, seqValue))
                .Append("&compress=")
                .Append(compressed ? "1" : "0")
                .Append("&stop=")
                .Append(finished ? "1" : "0");

            // Send audio file to speech recognition service by HTTP POST
            Console.WriteLine("Sending 'HTTP POST' request to URL : " + httpURL.ToString());
            HttpWebRequest request = HttpWebRequest.Create(httpURL.ToString()) as HttpWebRequest;
            request.Method = "POST";
            request.KeepAlive = true;
            request.ContentType = "application/octet-stream";
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(file, 0, file.Length);
            }

            // Get cookie
            request.CookieContainer = cookies;

            // Get the response
            using (WebResponse response = request.GetResponse())
            {
                StreamReader sr = new StreamReader(response.GetResponseStream());
                result = sr.ReadToEnd();
                sr.Close();
            }

            return result;
        }

        /// <summary>
        /// Get the speech recognition result for the audio you sent.
        /// </summary>
        /// <param name="apiName">The API name for 'api=xxx' HTTP parameter.</param>
        /// <param name="seqValue">The value of 'seq' for 'seq=xxx' HTTP parameter.</param>
        /// <returns>Speech Recognition Result</returns>
        public string GetRecognitionResult(String apiName, String seqValue)
        {
            string result;

            StringBuilder httpURL = new StringBuilder(apiBaseUrl)
                .Append("?")
                .Append(GetBasicQueryString(apiName, seqValue));

            // Request speech recognition service by HTTP GET
            Console.WriteLine("Sending 'HTTP POST' request to URL : " + httpURL.ToString());
            HttpWebRequest request = HttpWebRequest.Create(httpURL.ToString()) as HttpWebRequest;
            request.Method = "GET";

            // Set cookie
            request.CookieContainer = this.cookies;

            // Get the response
            using (WebResponse response = request.GetResponse())
            {
                StreamReader sr = new StreamReader(response.GetResponseStream());
                result = sr.ReadToEnd();
                sr.Close();
            }

            return result;
        }

        /// <summary>
        /// Generate and get a basic HTTP query string
        /// </summary>
        /// <param name="apiName">the API name for 'api=xxx' HTTP parameter.</param>
        /// <param name="seqValue">the value of 'seq' for 'seq=xxx' HTTP parameter.</param>
        /// <returns>Basic query string collection</returns>
        private StringBuilder GetBasicQueryString(string apiName, string seqValue)
        {
            double timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

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

            // Assemble all the HTTP parameters you want to send
            StringBuilder query = new StringBuilder()
                .Append("&appkey=")
                .Append(appKey)
                .Append("&api=")
                .Append(apiName)
                .Append("&timestamp=")
                .Append(timestamp)
                .Append("&sign=")
                .Append(sign)
                .Append("&seq=")
                .Append(seqValue);

            return query;
        }

    }

}
