//namespace BotBuilder.Samples.AdaptiveCards
namespace CajerosBTBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CajerosBTBot.Bean;
    using CajerosBTBot.implementaciones;
    using CajerosBTBot.Interfaces;
    using global::AdaptiveCards;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Connector;
    using Newtonsoft.Json.Linq;

    [Serializable]

    public class CajerosDialog : IDialog<object>
    {
        public static class Program
        {
            public static string cajero;      
        }
        public async Task StartAsync(IDialogContext context)
        {

            var message = context.Activity as IMessageActivity;
            var query = CajerosQuery.Parse(message.Value);
            Program.cajero = query.Cajero;
            await context.PostAsync($"Buscando cajero {query.Cajero} ...");
            try
            {
                await SearchCajeros(context, query);
              
            }
            catch (FormCanceledException ex)
            {
                await context.PostAsync($"Oops! Algo esta mal :( Technical Details: {ex.InnerException.Message}");
            }
        }

        private async Task SearchCajeros(IDialogContext context, CajerosQuery searchQuery)
        {
            Program.cajero = searchQuery.Cajero;
            IConsultorDB bd = new CajeroDaoImpl();
            var estatus = bd.ObtenerEstatusCajero(Program.cajero.ToUpper());
            if (estatus.Equals(true))
            {
                await context.PostAsync($"Cajero {Program.cajero} encontrado...");

                await muestraMenu(context);

              /*  var opciones = this.GetOpciones(Program.cajero);

                var title = $"¿Qué deseas consultar?";
                var intro = new List<CardElement>()
                {
                    new TextBlock(){
                        Text=title,
                        Size = TextSize.Medium,
                        Weight = TextWeight.Bolder,
                        Speak =$"<s>{title}</s>"
                    }
                };
            
                var rows = Split(opciones, 5)
                    .Select(group => new ColumnSet()
                    {
                        Columns = new List<Column>(group.Select(AsHotelItem))

                    });

                var card = new AdaptiveCard()
                {
                    Body = intro.Union(rows).ToList()
                };

                Attachment attachment = new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = card
                };

                var reply = context.MakeMessage();
                reply.Attachments.Add(attachment);

                await context.PostAsync(reply);*/
            }
            else {

                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard = new ThumbnailCard
                {                 
                    //Subtitle = "Verifique el número de cajero",
                    Title = "No encontramos fallas en el cajero " + Program.cajero.ToUpper(),
                    Images = new List<CardImage> {
                        new CardImage
                        {
                            Url = "https://storageserviciobt.blob.core.windows.net/imagebot/error.jpg",
                        }
                    }
                }.ToAttachment();
                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard);
                await context.PostAsync(activity);             
                await muestraMenuError(context);
            }

        }

        private async Task muestraMenu(IDialogContext context) {
            var opciones = this.GetOpciones(Program.cajero);

            var title = $"¿Qué deseas consultar?";
            var intro = new List<CardElement>()
                {
                    new TextBlock(){
                        Text=title,
                        Size = TextSize.Medium,
                        Weight = TextWeight.Bolder,
                        Speak =$"<s>{title}</s>"
                    }
                };

            // Hotels in rows of three
            var rows = Split(opciones, 5)
                .Select(group => new ColumnSet()
                {
                    Columns = new List<Column>(group.Select(AsHotelItem))

                });

            var card = new AdaptiveCard()
            {
                Body = intro.Union(rows).ToList()
            };

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);

            await context.PostAsync(reply);
        }

        private IEnumerable<MenuCajeros> GetOpciones(string cajero)
        {            
            var opciones = new List<MenuCajeros>();
            MenuCajeros menu = new MenuCajeros() {opcion= "Estatus actual" };
            opciones.Add(menu);
            MenuCajeros menu2 = new MenuCajeros() { opcion = "Responsable" };
            opciones.Add(menu2);
            MenuCajeros menu3 = new MenuCajeros() { opcion = "Fecha posible de solucion" };
            opciones.Add(menu3);
            MenuCajeros menu4 = new MenuCajeros() { opcion = "Historico de fallas" };
            opciones.Add(menu4);
            MenuCajeros menu5 = new MenuCajeros() { opcion = "Finalizar" };
            opciones.Add(menu5);
            return opciones;
        }

        private Column AsHotelItem(MenuCajeros menu)
        {
            var submitActionData = JObject.Parse("{ \"Type\": \"MenuCajeroSelection\" }");
            submitActionData.Merge(JObject.FromObject(menu));

            return new Column()
            {
                Size = "14",
                Items = new List<CardElement>()
                {
                    new TextBlock()
                    {
                        Text = menu.opcion,
                        Speak = $"<s>{menu.opcion}</s>",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Wrap = false,
                        //Weight = TextWeight.Bolder
                        Weight = TextWeight.Normal
                    }
          
                },
                SelectAction = new SubmitAction()
                {
                    DataJson = submitActionData.ToString()
                }
            };
        }


        private async Task muestraMenuError(IDialogContext context)
        {
            var opciones = this.GetOpciones2(Program.cajero);

            var title = $"¿Qué deseas hacer?";
            var intro = new List<CardElement>()
                {
                    new TextBlock(){
                        Text=title,
                        Size = TextSize.Medium,
                        Weight = TextWeight.Bolder,
                        Speak =$"<s>{title}</s>"
                    }
                };

            // Hotels in rows of three
            var rows = Split(opciones, 5)
                .Select(group => new ColumnSet()
                {
                    Columns = new List<Column>(group.Select(AsHotelItem2))

                });

            var card = new AdaptiveCard()
            {
                Body = intro.Union(rows).ToList()
            };

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);

            await context.PostAsync(reply);
        }

        private Column AsHotelItem2(MenuCajeros menu)
        {
            var submitActionData = JObject.Parse("{ \"Type\": \"MenuCajeroSelection2\" }");
            submitActionData.Merge(JObject.FromObject(menu));

            return new Column()
            {
                Size = "14",
                Items = new List<CardElement>()
                {
                    new TextBlock()
                    {
                        Text = menu.opcion,
                        Speak = $"<s>{menu.opcion}</s>",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Wrap = false,
                        //Weight = TextWeight.Bolder
                        Weight = TextWeight.Normal
                    }

                },
                SelectAction = new SubmitAction()
                {
                    DataJson = submitActionData.ToString()
                }
            };
        }

        private IEnumerable<MenuCajeros> GetOpciones2(string cajero)
        {
            var opciones = new List<MenuCajeros>();
            MenuCajeros menu = new MenuCajeros() { opcion = "Regresa Menu" };
            opciones.Add(menu);
            MenuCajeros menu2 = new MenuCajeros() { opcion = "Finalizar" };
            opciones.Add(menu2);
            return opciones;
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(IEnumerable<T> list, int parts)
        {
            return list.Select((item, ix) => new { ix, item })
                       .GroupBy(x => x.ix % parts)
                       .Select(x => x.Select(y => y.item));
        }


        

    }
}
