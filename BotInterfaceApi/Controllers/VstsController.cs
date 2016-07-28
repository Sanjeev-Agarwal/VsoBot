using BotInterfaceApi.Models.TFS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace BotInterfaceApi.Controllers
{
    public static class ApiHelper
    {
        public static string ContentType = "application/json";
        public static string AuthorizationToken = "Bearer ";

    }
    public class ItemsController : ApiController
    {

        [HttpGet]
        public string Get(string id)
        {
            var userName = id.Trim().Split('$')[1];
            id = id.Trim().Split('$')[0];
            var error = String.Empty;
            var strResponseData = String.Empty;
            string postData = "{\"query\":\"Select [State], [Title] From WorkItems  Where [Work Item Type] = '" + id + "' and [System.AssignedTo] = @Me Order By [State] Asc, [Changed Date] Desc\"}";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(
                "https://microsoftit.visualstudio.com/DefaultCollection/OneITVSO/_apis/wit/wiql?api-version=1.0");
            webRequest.Method = "POST";
            webRequest.ContentLength = postData.Length;
            webRequest.ContentType = ApiHelper.ContentType;
            var storage = new VsoBotStorage.Token();
            var token = storage.GetToken(userName);
            if (token == null) return string.Empty;
            webRequest.Headers.Add("Authorization", ApiHelper.AuthorizationToken + token.accessToken);

            using (StreamWriter swRequestWriter = new StreamWriter(webRequest.GetRequestStream()))
            {
                swRequestWriter.Write(postData);
            }

            try
            {
                HttpWebResponse hwrWebResponse = (HttpWebResponse)webRequest.GetResponse();

                if (hwrWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader srResponseReader = new StreamReader(hwrWebResponse.GetResponseStream()))
                    {
                        strResponseData = srResponseReader.ReadToEnd();
                    }

                    var rootObject = JsonConvert.DeserializeObject<Rootobject>(strResponseData);
                    if(rootObject != null && rootObject.workItems.Any())
                    {
                        return GetItemList(rootObject.workItems, id, token.accessToken);
                    }    
                }
            }
            catch (WebException wex)
            {
                error = "Request Issue: " + wex.Message;
            }
            catch (Exception ex)
            {
                error = "Issue: " + ex.Message;
            }

            throw new Exception(error);
        }
        private JObject GetItem(string id, string token)
        {
            var error = String.Empty;
            var strResponseData = String.Empty;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(
                "https://microsoftit.visualstudio.com/DefaultCollection/_apis/wit/workItems/" + id);
            webRequest.Method = "GET";
            webRequest.ContentType = ApiHelper.ContentType;
            webRequest.Headers.Add("Authorization", ApiHelper.AuthorizationToken + token);

            try
            {
                HttpWebResponse hwrWebResponse = (HttpWebResponse)webRequest.GetResponse();

                if (hwrWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader srResponseReader = new StreamReader(hwrWebResponse.GetResponseStream()))
                    {
                        strResponseData = srResponseReader.ReadToEnd();
                    }

                    var jObject = JObject.Parse(strResponseData);
                    return jObject["fields"] as JObject;
                }
            }
            catch (WebException wex)
            {
                error = "Request Issue: " + wex.Message;
            }
            catch (Exception ex)
            {
                error = "Issue: " + ex.Message;
            }

            throw new Exception(error);
        }
        private string GetItemList(Workitem[] items, string type, string userName)
        {
            StringBuilder sb = new StringBuilder();
            var helpers = new List<Data>();
            foreach (var item in items)
            {
                var helper = new Data();
                helper.Id = item.id.ToString();
                helper.type = type;
                var jobject = GetItem(item.id.ToString(), userName);
                helper.state = jobject["System.State"].ToString();
                helper.title = jobject["System.Title"].ToString();
                var url = "https://microsoftit.visualstudio.com/OneITVSO/Mktg-MKS-BP-Marketing%20Budget%20Planning/_workItems?_a=edit&id=" + helper.Id;
                sb.Append(string.Format(" *{3}-{0}, id:{1} *{2}*, [url:]({4})", helper.type, helper.Id, helper.title, helper.state, url));
                helpers.Add(helper);
            }
            return sb.ToString();
            //return helpers.ToArray();
        }

    }

}