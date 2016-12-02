using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace social.facebook
{
    public class FacebookProfileProcessor
    {
        private string _appID;
        private string _appSecret;

        public FacebookProfileProcessor(string appID, string appSecret)
        {
            _appID = appID;
            _appSecret = appSecret;
        }

        public string ProcessNode(string nodeID, Action<string> processor)
        {
            try
            {
                Facebook.FacebookClient client = new Facebook.FacebookClient(_appID + "|" + _appSecret);
                var res = client.Get(nodeID) as JsonObject;
                if (res != null)
                {
                    var friends = client.Get(nodeID + "/friends") as JsonObject;
                    dynamic feed = client.Get(nodeID + "/feed");
                    foreach (var post in feed["data"])
                    {
                        if (post.type == "video")
                            processor(post.name);
                        else if (post.type == "status")
                            processor(post.message ?? post.story);
                        else if (post.type == "photo")
                            processor(post.message ?? post.story);
                        else if (post.type == "link")
                            processor(post.message ?? post.caption);
                        else
                            processor(post.type);

                    }
                    return res.ToString();
                }
                return "-1";
            }
            catch
            {
                //todo:log this
                throw;
            }
        }
    }
}
