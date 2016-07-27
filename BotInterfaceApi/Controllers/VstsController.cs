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
using System.Web.Http;

namespace BotInterfaceApi.Controllers
{
    public static class ApiHelper
    {
        public static string ContentType = "application/json";
        public static string AuthorizationToken = "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Im9PdmN6NU1fN3AtSGpJS2xGWHo5M3VfVjBabyJ9.eyJuYW1laWQiOiI5MmExYjE5NC05MjljLTRlZDQtODkyNi1mNTVmNzJmY2U3NDUiLCJzY3AiOiJ2c28uYWdlbnRwb29scyB2c28uYnVpbGQgdnNvLmNvZGUgdnNvLmNvZGVfc3RhdHVzIHZzby5jb25uZWN0ZWRfc2VydmVyIHZzby5kYXNoYm9hcmRzIHZzby5leHRlbnNpb24gdnNvLmlkZW50aXR5IHZzby5sb2FkdGVzdCB2c28ucHJvamVjdCB2c28ucmVsZWFzZSB2c28udGVzdCB2c28ud29ya193cml0ZSIsImlzcyI6ImFwcC52c3Nwcy52aXN1YWxzdHVkaW8uY29tIiwiYXVkIjoiYXBwLnZzc3BzLnZpc3VhbHN0dWRpby5jb20iLCJuYmYiOjE0Njk2Mzk2MDAsImV4cCI6MTQ2OTY0MzIwMH0.kkx11rXQy6V0XAUzJqWsAQ_c1-vMGgLmI5tnonVT5IJyPV2-lAfjoXvz4mpcF88ZracJX8_hiQ8yDPEQA53622lWK1dei5EJjJXAXkmvhRuxrkqbZglNd6mwzb165_H0vJbrM1e6OSuVD6Il3WFKLOMTOftF_1EqhtTvbresb1OfoMkf8wf5aewlukfpknwbFZA-0e51fxD0o6DZDLZ4BFH7F5T_UPB2FVS9orYENGr4pu33sNffYd2-hjKmBIzYTuFU17KD1GqtGR5-PyqNWEdCLKPWmoWn5-YM8ZMa0JxQQiuhrHLAX5Gok1H5EmHbGARKpM1jGNBZJ78IebMEdA";

    }
    public class ItemsController : ApiController
    {

        [HttpGet]
        public string Get(string id)
        {
            var error = String.Empty;
            var strResponseData = String.Empty;
            string postData = "{\"query\":\"Select [State], [Title] From WorkItems  Where [Work Item Type] = '" + id + "' and [System.AssignedTo] = @Me Order By [State] Asc, [Changed Date] Desc\"}";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(
                "https://microsoftit.visualstudio.com/DefaultCollection/OneITVSO/_apis/wit/wiql?api-version=1.0");

            webRequest.Method = "POST";
            webRequest.ContentLength = postData.Length;
            webRequest.ContentType = ApiHelper.ContentType;
            webRequest.Headers.Add("Authorization", ApiHelper.AuthorizationToken);

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
                        return GetItemList(rootObject.workItems, id);
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
        private JObject GetItem(string id)
        {
            var error = String.Empty;
            var strResponseData = String.Empty;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(
                "https://microsoftit.visualstudio.com/DefaultCollection/_apis/wit/workItems/" + id);
            webRequest.Method = "GET";
            webRequest.ContentType = ApiHelper.ContentType;
            webRequest.Headers.Add("Authorization", ApiHelper.AuthorizationToken);

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
        private string GetItemList(Workitem[] items, string type)
        {
            StringBuilder sb = new StringBuilder();
            var helpers = new List<Data>();
            foreach (var item in items)
            {
                var helper = new Data();
                helper.Id = item.id.ToString();
                helper.type = type;
                var jobject = GetItem(item.id.ToString());
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