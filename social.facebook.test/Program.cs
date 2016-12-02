using analytics.cognitive;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social.facebook.test
{
    class Program
    {
        static void Main(string[] args)
        {            
            FacebookProfileProcessor facebookProcessor = new FacebookProfileProcessor(
                ConfigurationManager.AppSettings["appId"], 
                ConfigurationManager.AppSettings["appSecret"]);

            CognitiveAnalysisProcessor cognitiveProcessor = new CognitiveAnalysisProcessor(
                ConfigurationManager.AppSettings["azureCognitiveEndpoint"],
                ConfigurationManager.AppSettings["azureCognitiveServiceKey"], 
                1);

            facebookProcessor.ProcessNode(ConfigurationManager.AppSettings["facebookNode"], (e) => {
                cognitiveProcessor.ProcessText(e, (f) => {
                    Console.WriteLine(f);
                });
            });
            Console.ReadLine();
        }
    }
}
