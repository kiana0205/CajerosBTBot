using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using CajerosBTBot.Dialogs;
using System;
using System.Reflection;
using System.Web.Http;
using Microsoft.Bot.Builder.Azure;

namespace CajerosBTBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Adding DocumentDB endpoint and primary key
            var docDbServiceEndpoint = new Uri("https://storelogbot.documents.azure.com:443/");
            var docDbKey = "qAGDBpjPrvppIjipL4GKvj7Q6pOqS2Lb1z9Rkquqvs6gh1xupiHCFBsikFPBCzLPvOOdnG3kNeZ49ZhueFLzqg==";

            // Creating a data store based on DocumentDB
            var store = new DocumentDbBotDataStore(docDbServiceEndpoint, docDbKey);

            // Adding Azure dependencies to your bot (documentDB data store and Azure module)
            // var builder = new ContainerBuilder();

            //builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));

            // Key_DataStore is the key for data store register with the container
            //builder.Register(c => store)
            //    .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
            //    .AsSelf()
            //     .SingleInstance();

            // After adding new dependencies, update the container
            //builder.Update(Conversation.Container);


            //GlobalConfiguration.Configure(WebApiConfig.Register);


            Conversation.UpdateContainer(
                        builder =>
                        {
                            builder.Register(c => store)
                                .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                                .AsSelf()
                                .SingleInstance();

                            builder.Register(c => new CachingBotDataStore(store, CachingBotDataStoreConsistencyPolicy.ETagBasedConsistency))
                                .As<IBotDataStore<BotData>>()
                                .AsSelf()
                                .InstancePerLifetimeScope();

                        });

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
