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
                var token = IsValidUser(activity.Text);
                string entityString;
                if (token)
                {
                    entityString = "[This is your first time. Please grant permission to communicate with VSTS by clicking the link.](https://botinterfaceapi.azurewebsites.net/oauth/requesttoken?userName=" + GetValidEncodedEmail(activity.Text) + ")";
                }
                else if (!token && activity.Text.ToLower().Contains("hi"))
                {
                    entityString = "Please share your work email Id to validate your data.";// [please login first.](https://botinterfaceapi.azurewebsites.net/oauth/requesttoken)";
                }
                else
                {
                    LUIS.Rootobject StLUIS = await GetEntityFromLUIS(activity.Text);
                    if (StLUIS.intents.Count() > 0)
                    {
                        switch (StLUIS.intents[0].intent.ToLower())
                        {
                            case "show":
                                entityString = GetWorkItems(StLUIS.entities[0].type); //Call VSTS API
                                break;
                            default:
                                entityString = "Sorry, I am not getting you...";
                                break;
                        }
                    }

                    else
                    {
                        entityString = "Sorry, I am not getting you...";
                    }
                }
                Activity reply = activity.CreateReply(entityString);
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

        private string GetWorkItems(string type)
        {
            var data = string.Empty;
            if (type.Equals("task"))
            {
                return new Models.TFS.Helper().Tasks;
            }
            else if(type.Equals("bug"))
            {
                return new Models.TFS.Helper().Bugs;
            } 
            return "Something went wrong while looking into VSTS.";
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
    public class VstsController : ApiController
    {
        
    }
}