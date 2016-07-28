using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.Web;
using System.IO;
using BotInterfaceApi.Models.TFS;
using System.Text;

namespace BotInterfaceApi
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            if (activity.Type == ActivityTypes.Message)
            {
                StateClient stateClient = activity.GetStateClient();
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

                var token = IsValidUser(activity.Text);
                string entityString;
                string name = string.Empty;
                if (token)
                {
                    name = GetValidEncodedEmail(activity.Text);
                    entityString = "[This is your first time. Please grant permission to communicate with VSTS by clicking the link.](https://botinterfaceapi.azurewebsites.net/oauth/requesttoken?userName=" + name + "). After login type your query like 'Show my task' or 'Display my task' to get result.";

                }
                else if (!token && activity.Text.ToLower().Contains("hi"))
                {
                    entityString = "Please share your work email Id to validate your data.";
                }
                else
                {
                    LUIS.Rootobject StLUIS = await GetEntityFromLUIS(activity.Text);
                    if (StLUIS.intents.Count() > 0)
                    {
                        switch (StLUIS.intents[0].intent.ToLower())
                        {
                            case "show":
                                entityString = await GetWorkitems(StLUIS.entities[0].type, activity); //Call VSTS API
                                break;
                            default:
                                entityString = " Sorry, I am not getting you ...";
                                break;
                        }
                    }

                    else
                    {
                        entityString = " Sorry, I am not getting you ...";
                    }
                }
                Activity reply = activity.CreateReply(entityString);
                if (!string.IsNullOrEmpty(name))
                {
                    name = HttpUtility.UrlDecode(name);
                    userData.SetProperty<string>("userName", name);
                    await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                }
                activity.TextFormat = "markdown";
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task<LUIS.Rootobject> GetEntityFromLUIS(string query)
        {
            query = Uri.EscapeDataString(query);
            LUIS.Rootobject Data = new LUIS.Rootobject();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://api.projectoxford.ai/luis/v1/application?id=a72b5e9d-a2be-4e89-a788-76427e41af1a&subscription-key=a5764e3cc7a442709aa8c0e4a708502c&q=" + query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);
                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<LUIS.Rootobject>(JsonDataResponse);
                }
            }
            return Data;
        }

        

        private async Task<string> GetWorkitems(string type, Activity activity)
        {
            StateClient stateClient = activity.GetStateClient();
            BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
            string RequestURI = string.Empty;
            if (!string.IsNullOrEmpty(userData.GetProperty<string>("userName")))
            {
                using (HttpClient client = new HttpClient())
                {
                    RequestURI = "https://botinterfaceapi.azurewebsites.net/api/items/" + type + "$" + userData.GetProperty<string>("userName") + "/";
                    HttpResponseMessage msg = await client.GetAsync(RequestURI);
                    if (msg.IsSuccessStatusCode)
                    {
                        var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<string>(JsonDataResponse);
                    }
                }
            }
            return "No " + type + " exist. or Some issue with parsing data ";
        }

        
        private bool IsValidUser(string query)
        {
            var helper = new Helpers.RegexUtilities();
            if (helper.IsValidEmail(query))
            {
                return true;
            }
            return false;
        }

        private string GetValidEncodedEmail(string query)
        {
            var helper = new Helpers.RegexUtilities();
            var email = helper.GetValidEmailId(query);
            return HttpUtility.UrlEncode(email);
        }
        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }

    public enum WorkItemTypes
    {
        Task,
        Bug
    }
 
}