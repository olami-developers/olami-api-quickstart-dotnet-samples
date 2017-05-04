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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ai.Olami.Example
{
    class QuickstartTest
    {
        static void Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("\n\n[Error] Missing args! Usage:\n");
                Console.WriteLine(" - args[0]: api_url");
                Console.WriteLine(" - args[1]: your_app_key");
                Console.WriteLine(" - args[2]: your_app_secret");
                Console.WriteLine(" - args[3]: your_audio_file");
                Console.WriteLine(" - args[4]: compress_flag=[0|1]");
                Console.WriteLine("\n\n");
                Environment.Exit(0);
            }

            string url = args[0];
            string appKey = args[1];
            string appSecret = args[2];
            string filePath = args[3];
            Boolean compressed = args[4].Equals("1");

            string responseString = null;

            SpeechApiSample speechApi = new SpeechApiSample();
            speechApi.SetLocalization(url);
            speechApi.SetAuthorization(appKey, appSecret);

            // Start sending audio file for recognition
            Console.WriteLine("\n---------- Test Speech Recognition API, seq=nli,seg ----------\n");
            Console.WriteLine("Send audio file...:\n");
            responseString = speechApi.SendAudioFile(SpeechApiSample.API_NAME_ASR, "nli,seg", true, filePath, compressed);
            Console.WriteLine("\nResult:\n" + responseString);
            
            // We just check the state by a lazy way :P , you should do it by JSON.
            if (!responseString.ToLower().Contains("error"))
            {
                Console.WriteLine("\n----- Get Recognition Result -----\n");
                System.Threading.Thread.Sleep(1000);

                // Try to get result until the end of the recognition is complete
                while (true)
                {
                    responseString = speechApi.GetRecognitionResult(SpeechApiSample.API_NAME_ASR, "nli,seg");
                    Console.WriteLine("\nResult:\n" + responseString);
                    // Well, check by lazy way...again :P , do it by JSON please.
                    if (!responseString.ToLower().Contains("\"final\":true"))
                    {
                        Console.WriteLine("*** The recognition is not yet complete. ***\n");
                        if (responseString.ToLower().Contains("error")) break;
                        System.Threading.Thread.Sleep(2000);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            
            Console.WriteLine("\n\n");
        }
    }
}
