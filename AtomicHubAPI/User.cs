using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using xNet;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace AtomicHubAPI
{
    public static class User
    {
        public static dynamic GetProfileDescription(string name = "")
        {
            WebRequest request = WebRequest.Create("https://wax.pink.gg/v1/chain/get_table_rows");
            request.Method = "POST";
            string postData = "{\"code\":\"atomhubtools\",\"table\":\"acctexts\",\"scope\":\"" + name + "\",\"lower_bound\":\"description\",\"upper_bound\":\"description\",\"limit\":1,\"json\":true}\"";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            request.ContentType = "text/plain;charset=UTF-8";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            dynamic resp = "";
            using (dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                resp = responseFromServer;
            }
            response.Close();
            dynamic dsc = JObject.Parse(resp);
            dsc = dsc.rows[0].value;
            string desc = dsc;
            desc = removeFigures(desc);
            return desc;
        }
        public static dynamic GetUserCollectionsJSON(string username)
        {
            HttpRequest request = new HttpRequest();
            request.Cookies = new CookieDictionary();
            request.KeepAlive = true;
            request.UserAgent = Http.ChromeUserAgent();
            string parsed = request.Get("https://wax.api.atomicassets.io/atomicassets/v1/accounts/" + username).ToString();
            dynamic json = JObject.Parse(parsed);
            dynamic temp = json.data.templates;
            return temp;
        }
        public static List<UserCollections> GetUserCollectionsList(string username)
        {
            HttpRequest request = new HttpRequest();
            request.Cookies = new CookieDictionary();
            request.KeepAlive = true;
            request.UserAgent = Http.ChromeUserAgent();
            string parsed = request.Get("https://wax.api.atomicassets.io/atomicassets/v1/accounts/" + username).ToString();
            dynamic json = JObject.Parse(parsed);
            dynamic temp = json.data.templates;
            List<UserCollections> collection = new List<UserCollections>();
            foreach (var x in temp)
            {
                string template_id = x.template_id;
                template_id = removeFigures(template_id);
                ulong templateid = ulong.Parse(template_id);
                string collection_name_ = x.collection_name;
                collection_name_ = removeFigures(collection_name_);
                string assets_ = x.assets;
                int assets = int.Parse(removeFigures(assets_));
                collection.Add(new UserCollections(collection_name_, templateid, assets));
            }
            return collection;
        }
        public static List<UserCollections> GetUserCollectionsListByName(string username, string collection_name)
        {
            HttpRequest request = new HttpRequest();
            request.Cookies = new CookieDictionary();
            request.KeepAlive = true;
            request.UserAgent = Http.ChromeUserAgent();
            string parsed = request.Get("https://wax.api.atomicassets.io/atomicassets/v1/accounts/" + username).ToString();
            dynamic json = JObject.Parse(parsed);
            dynamic temp = json.data.templates;
            List<UserCollections> collection = new List<UserCollections>();
            foreach (var x in temp)
            {
                string template_id = x.template_id;
                template_id = removeFigures(template_id);
                ulong templateid = ulong.Parse(template_id);


                string collection_name_ = x.collection_name;
                collection_name_ = removeFigures(collection_name_);
                string assets_ = x.assets;
                int assets = int.Parse(removeFigures(assets_));
                if (collection_name_ == collection_name)
                    collection.Add(new UserCollections(collection_name, templateid, assets));
            }
            return collection;
        }
        private static string removeFigures(string text = "")
        {
            text.Replace("{", string.Empty);
            text.Replace("}", string.Empty);
            return text;
        }
    }
    public struct UserCollections
    {
        public string collection_name;
        public ulong template_id;
        public int assets;
        public UserCollections(string collection_name, ulong template_id, int assets)
        {
            this.collection_name = collection_name;
            this.template_id = template_id;
            this.assets = assets;
        }
    }
}
