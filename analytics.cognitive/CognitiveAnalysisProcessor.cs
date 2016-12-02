using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace analytics.cognitive
{
    public class CognitiveAnalysisProcessor
    {
        string _azureEndpoint;
        string _key;
        int _numLanguages;
        public CognitiveAnalysisProcessor(string azureEndpoint, string key, int numLanguages)
        {
            _azureEndpoint = azureEndpoint;
            _key = key;
            _numLanguages = numLanguages;
        }

        public async void ProcessText(string text, Action<string> callback)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_azureEndpoint);

                // Request headers.
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _key);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Request body. Insert your text data here in JSON format.
                string data = "{\"documents\":[";
               // foreach (var item in text)
                {
                    data = data + "{\"id\":\"1\",\"text\":" + JsonConvert.ToString(text) + "}";
                }
                data = data + "]}";

                byte[] byteData = Encoding.UTF8.GetBytes(data);

                // Detect key phrases:
                var uri = "text/analytics/v2.0/keyPhrases";
                var response = await CallEndpoint(client, uri, byteData);
                callback("\nDetect key phrases response for "+ data + ":\n" + response);

                // Detect language:
                var queryString = HttpUtility.ParseQueryString(string.Empty);
                queryString["numberOfLanguagesToDetect"] = _numLanguages.ToString(CultureInfo.InvariantCulture);
                uri = "text/analytics/v2.0/languages?" + queryString;
                response = await CallEndpoint(client, uri, byteData);
                callback("\nDetect language response for " + data + ":\n" + response);

                // Detect sentiment:
                uri = "text/analytics/v2.0/sentiment";
                response = await CallEndpoint(client, uri, byteData);
                callback("\nDetect sentiment response for " + data + ":\n" + response);
            }
        }

        static async Task<String> CallEndpoint(HttpClient client, string uri, byte[] byteData)
        {
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(uri, content);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
