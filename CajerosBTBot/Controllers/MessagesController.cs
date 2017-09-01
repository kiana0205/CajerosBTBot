using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;

namespace CajerosBTBot
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
            try
            {
                if (activity.Type == ActivityTypes.Message)
                {
                    //await EnviarMensajeUsuario("Testingggggg", activity);
                    await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
                    //await Conversation.SendAsync(activity, () => new Dialogs.DialogoLuis());
                }
                else
                {
                    //HandleSystemMessage(activity);
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
            using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
            {
                var client = scope.Resolve<IConnectorClient>();
                var reply = activity.CreateReply();
                reply.Text = mensaje;

                await client.Conversations.ReplyToActivityAsync(reply);
            }
        }
        
    }
}