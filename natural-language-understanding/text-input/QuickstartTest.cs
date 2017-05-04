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
			    Console.WriteLine(" - args[3]: your_text_input");
                Console.WriteLine("\n\n");
                Environment.Exit(0);
            }

            string url = args[0];
            string appKey = args[1];
            string appSecret = args[2];
            string inputText = args[3];

            NluApiSample nluApi = new NluApiSample();
            nluApi.SetLocalization(url);
            nluApi.SetAuthorization(appKey, appSecret);
            
		    Console.WriteLine("\n---------- Test NLU API, api=seg ----------\n");
            Console.WriteLine("\nResult:\n\n" + nluApi.GetRecognitionResult(NluApiSample.API_NAME_SEG, inputText));
		
		    Console.WriteLine("\n---------- Test NLU API, api=nli ----------\n");
            Console.WriteLine("\nResult:\n\n" + nluApi.GetRecognitionResult(NluApiSample.API_NAME_NLI, inputText));

            Console.WriteLine("\n\n");
        }
    }
}
