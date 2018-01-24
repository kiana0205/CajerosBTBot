namespace CajerosBTBot
{
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
//using Microsoft.Bot.Builder.Dialogs.Internals;
using CajerosBTBot.Dialogs;
using Autofac;
using CajerosBTBot.Services;


using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;


    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private readonly MicrosoftCognitiveSpeechService speechService = new MicrosoftCognitiveSpeechService();
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            try
            {
                            
                if (activity.Type == ActivityTypes.Message)
                {

                    await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
                    //await Conversation.SendAsync(activity, () => new RootDialog());
                    /*  var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                      string message;

                      try
                      {
                          var audioAttachment = activity.Attachments?.FirstOrDefault(a => a.ContentType.Equals("audio/wav") || a.ContentType.Equals("application/octet-stream"));
                          if (audioAttachment != null)
                          {
                              var stream = await GetAudioStream(connector, audioAttachment);
                              var text = await this.speechService.GetTextFromAudioAsync(stream);
                              message = ProcessText(text);
                          }
                          else
                          {
                              message = "Did you upload an audio file? I'm more of an audible person. Try sending me a wav file";
                          }
                      }
                      catch (Exception e)
                      {
                          message = "Oops! Something went wrong. Try again later";
                          if (e is HttpException)
                          {
                              var httpCode = (e as HttpException).GetHttpCode();
                              if (httpCode == 401 || httpCode == 403)
                              {
                                  message += $" [{e.Message} - hint: check your API KEY at web.config]";
                              }
                              else if (httpCode == 408)
                              {
                                  message += $" [{e.Message} - hint: try send an audio shorter than 15 segs]";
                              }
                          }

                          Trace.TraceError(e.ToString());
                      }

                      Activity reply = activity.CreateReply(message);
                      await connector.Conversations.ReplyToActivityAsync(reply);*/

                }
                else
                {
                     HandleSystemMessage(activity);
                    
                }
            }
            catch (Exception ex)
            {
                await EnviarMensajeUsuario(ex.Message, activity);
            }
            finally
            {

            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }


        private static async Task EnviarMensajeUsuario(string mensaje, Activity activity)
        {
          /*  using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
            {
                var client = scope.Resolve<IConnectorClient>();
                var reply = activity.CreateReply();
                reply.Text = mensaje;

                await client.Conversations.ReplyToActivityAsync(reply);
            }*/
        }



        private static string ProcessText(string text)
        {
            string message = "You said : " + text + ".";

            if (!string.IsNullOrEmpty(text))
            {
                var wordCount = text.Split(' ').Count(x => !string.IsNullOrEmpty(x));
                message += "\n\nWord Count: " + wordCount;

                var characterCount = text.Count(c => c != ' ');
                message += "\n\nCharacter Count: " + characterCount;

                var spaceCount = text.Count(c => c == ' ');
                message += "\n\nSpace Count: " + spaceCount;

                var vowelCount = text.ToUpper().Count("AEIOU".Contains);
                message += "\n\nVowel Count: " + vowelCount;
            }

            return message;
        }

        private static async Task<Stream> GetAudioStream(ConnectorClient connector, Attachment audioAttachment)
        {
            using (var httpClient = new HttpClient())
            {
                // The Skype attachment URLs are secured by JwtToken,
                // you should set the JwtToken of your bot as the authorization header for the GET request your bot initiates to fetch the image.
                // https://github.com/Microsoft/BotBuilder/issues/662
                var uri = new Uri(audioAttachment.ContentUrl);
                if (uri.Host.EndsWith("skype.com") && uri.Scheme == "https")
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync(connector));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
                }

                return await httpClient.GetStreamAsync(uri);
            }
        }

        /// <summary>
        /// Gets the JwT token of the bot. 
        /// </summary>
        /// <param name="connector"></param>
        /// <returns>JwT token of the bot</returns>
        private static async Task<string> GetTokenAsync(ConnectorClient connector)
        {
            var credentials = connector.Credentials as MicrosoftAppCredentials;
            if (credentials != null)
            {
                return await credentials.GetTokenAsync();
            }

            return null;
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
                IConversationUpdateActivity update = message;
                var Client = new ConnectorClient(new Uri(message.ServiceUrl), new MicrosoftAppCredentials());
                if (update.MembersAdded != null && update.MembersAdded.Any()) {
                    foreach (var newMember in update.MembersAdded) {
                        if (newMember.Id != message.Recipient.Id) {
                            var reply = message.CreateReply();
                            reply.Text = $"Hola! Bienvenido. En este lugar podrás conocer información sobre fallas en cajeros";
                            Client.Conversations.ReplyToActivityAsync(reply);
                        }
                    }
                }
                //ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
                //Activity reply = message.CreateReply("Bienvenido al chat de fallas en cajeros. Hola!");
                //connector.Conversations.ReplyToActivityAsync(reply);
           
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
}