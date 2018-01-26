//namespace BotBuilder.Samples.AdaptiveCards
namespace CajerosBTBot.Dialogs
{ 
using System;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using CajerosBTBot.implementaciones;
using CajerosBTBot.Enums;
using CajerosBTBot.Interfaces;
using CajerosBTBot.Bean;
using System.Collections.Generic;
using System.Text;
using global::AdaptiveCards;
    using Newtonsoft.Json.Linq;
    using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;



    //namespace CajerosBTBot.Dialogs
    //{
    [Serializable]
    public class RootDialog : IDialog<object>
    {

        List<string> lista;
        private const string CajeroOption = "cajeros";
        private const string EmpresaOption = "empresas";
        private const string GrupoOption = "grupos";

        private const string SiOption = "si";
        private const string NoOption = "no";
        public static Int32 cnt = 0;
        public static class Program {
            public static string cajero;
            public static string empresa;
            public static string grupo;

            public static string estatus;
            public static string historico;
        }

        //public  Task StartAsync(IDialogContext context)    
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
            //return Task.CompletedTask;
        }

        /* public async Task StartAsync(IDialogContext context)
         {
             context.Wait(this.MessageReceivedAsync);
         }*/

        /* public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
         //public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
         {

            var message = await result;
             if (message.Value != null)
             {               
                 // Got an Action Submit
                 dynamic value = message.Value;
                 string submitType = value.Type.ToString();
                 switch (submitType)
                 {
                     case "CajeroSearch":
                         CajerosQuery query;
                         try
                         {
                             query = CajerosQuery.Parse(value);
                             Program.cajero = query.Cajero;

                             // Trigger validation using Data Annotations attributes from the HotelsQuery model
                             List<ValidationResult> results = new List<ValidationResult>();
                             bool valid = Validator.TryValidateObject(query, new ValidationContext(query, null, null), results, true);
                             if (!valid)
                             {

                                 await context.PostAsync("Hubo un error . al introducir el cajero:\n" );
                                 return;
                             }
                         }
                         catch (InvalidCastException)
                         {
                             // Hotel Query could not be parsed
                             await context.PostAsync("Por favor, introduzca el cajero");
                             return;
                         }

                         // Proceed with cajero search
                         await context.Forward(new CajerosDialog(), this.ResumeAfterOptionDialog, message, CancellationToken.None);
                         return;
                     case "MenuCajeroSelection":
                         await SendMenuSelectionAsync(context, (MenuCajeros)JsonConvert.DeserializeObject<MenuCajeros>(value.ToString()));
                         context.Wait(MessageReceivedAsync);
                         //await context.PostAsync("Selecciono la opcion");
                         return;
                     case "MenuCajeroSelection2":
                         //await SendHotelSelectionAsync(context, (Hotel)JsonConvert.DeserializeObject<Hotel>(value.ToString()));
                         //context.Wait(MessageReceivedAsync);
                         await context.PostAsync("Selecciono la opcion2");
                         return;
                 }


             }            
             if (message.Text != null )
             {
                 await ShowOptionsAsync(context);
                 //await context.Forward( message, System.Threading.CancellationToken.None);
             }
             else
             {
                 //await MostrarAyuda(context);
                 await ShowOptionsAsync(context);
             }

         }*/
        private async Task ShowOptionsAsync(IDialogContext context)
        {
            AdaptiveCard card = new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {
                    new Container()
                    {
                        Speak = "<s>Consulta fallas en cajeros.</s><s>Te sabes el cajero o la empresa?</s>",
                        Items = new List<CardElement>()
                        {
                            new ColumnSet()
                            {
                                Columns = new List<Column>()
                                {
                                    new Column()
                                    {
                                        Size = ColumnSize.Auto,
                                        Items = new List<CardElement>()
                                        {
                                            new Image()
                                            {
                                                Url = "https://storageserviciobt.blob.core.windows.net/imagebot/banorte2.jpg",
                                                Size = ImageSize.Medium,
                                                Style = ImageStyle.Person
                                            }
                                        }
                                    },
                                    new Column()
                                    {
                                        Size = ColumnSize.Stretch,
                                        Items = new List<CardElement>()
                                        {
                                            new TextBlock()
                                            {
                                                Text =  "Consulta Fallas en Cajeros",
                                                Weight = TextWeight.Bolder,
                                                 Size = TextSize.Medium,
                                                IsSubtle = true
                                            },
                                            new TextBlock()
                                            {
                                                Text = "¿Te sabes el cajero o la empresa?",
                                                Wrap = true
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                // Buttons
                Actions = new List<ActionBase>() {
                    new ShowCardAction()
                    {
                        Title = "Cajero",
                        Speak = "<s>Cajero</s>",
                        Card = GetCajeroSearchCard()
                    },
                    new ShowCardAction()
                    {
                        Title = "Empresa",
                        Speak = "<s>Empresa</s>",
                        Card = new AdaptiveCard()
                        {
                            Body = new List<CardElement>()
                            {
                                new TextBlock()
                                {
                                    Text = "Empresa no esta implementada =(",
                                    Speak = "<s>Empresa no esta implementada</s>",
                                    Weight = TextWeight.Bolder
                                }
                            }
                        }
                    }
                }
            };

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);

            await context.PostAsync(reply);

            context.Wait(MessageReceivedAsync);
        }


        private static AdaptiveCard GetCajeroSearchCard()
        {
            return new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {
                        // Hotels Search form
                       new TextBlock()
                        {
                            Text = "Bienvenido a la opcion de cajeros",
                            Speak = "<s>Bienvenido a la opcion de cajeros</s>",
                            Weight = TextWeight.Bolder,
                            Size = TextSize.Medium
                        },
                        new TextBlock() { Text = "Por favor introduzca el cajero:" },
                        new TextInput()
                        {
                            //Id = "Destination",
                            Id = "Cajero",
                            Speak = "<s>Por favor introduzca el cajero</s>",
                            Placeholder = "NM10081",
                            Style = TextInputStyle.Text
                        }
                     /*   ,
                        new TextBlock() { Text = "When do you want to check in?" },
                        new DateInput()
                        {
                            Id = "Checkin",
                            Speak = "<s>When do you want to check in?</s>"
                        },
                        new TextBlock() { Text = "How many nights do you want to stay?" },
                        new NumberInput()
                        {
                            Id = "Nights",
                            Min = 1,
                            Max = 60,
                            Speak = "<s>How many nights do you want to stay?</s>"
                        }*/
                },
                Actions = new List<ActionBase>()
                {
                    new SubmitAction()
                    {
                        Title = "Buscar",
                        Speak = "<s>Buscar</s>",
                        DataJson = "{ \"Type\": \"CajeroSearch\" }"
                    }
                }
            };
        }

        private static async Task SendMenuSelectionAsync(IDialogContext context, MenuCajeros selecciona)
        {
            //var description = $"{hotel.Rating} start with {hotel.NumberOfReviews}. From ${hotel.PriceStarting} per night.";
            var description = selecciona.opcion;
            IConsultorDB bd = new CajeroDaoImpl();
            switch (description)
            {
                case "Estatus actual":
                    var cajeros = bd.ObtenerFallaCajero(Program.cajero.ToUpper());
                    if (cajeros != null && cajeros.Count > 0)
                    {
                        Cajero cajeroBean = cajeros[0];
                        var activity = context.MakeMessage();
                        activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        var menuHeroCard = new ThumbnailCard
                        {
                            //Subtitle = cajeroBean.conteo + " falla(s)",
                            Title = "El cajero " + Program.cajero.ToUpper() + " tiene: ",
                            Subtitle = cajeroBean.conteo + " falla(s)",
                            Images = new List<CardImage> {
                        new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/cajeroerror.jpg" }
                    }
                        }.ToAttachment();

                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard);
                        await context.PostAsync(activity);

                        var tipofalla = String.Empty;
                        switch (cajeroBean.tipoFalla)
                        {
                            case "ComunicacionEnergia":
                                tipofalla = "Cajero sin energía";
                                break;
                            case "ErrorSinEfectivo":
                                tipofalla = "Cajero sin efectivo";
                                break;
                            case "ModoSupervisor":
                                tipofalla = "Modo Supervisor";
                                break;
                            case "FallaHardware":
                                tipofalla = "Falla en el hardware";
                                break;
                            case "ProblemaLocal":
                                tipofalla = "Cajero con problema local";
                                break;
                            case "TrxsNoMonetarias":
                                tipofalla = "Transacciones no monetarias";
                                break;
                            default:
                                tipofalla = "Sin identificar";
                                break;
                        }

                        await context.PostAsync("Falla: " + tipofalla + ",    Fecha: " + cajeroBean.fecha);

                        // await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");

                    }
                    else
                    {
                        var activity = context.MakeMessage();
                        activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        var menuHeroCard = new ThumbnailCard
                        {
                            Text = "Algo más en que le podamos ayudar?",
                            Subtitle = "Verifique el número de cajero",
                            Title = "No existen fallas en el cajero " + Program.cajero.ToUpper() + "o no pertenece a banca transaccional",
                            Images = new List<CardImage> {
                        new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/error.jpg" }
                    }
                        }.ToAttachment();

                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard);
                        await context.PostAsync(activity);
                    }

                    return;
                case "Responsable":
                    // IConsultorDB bd = new CajeroDaoImpl();
                    bd = new CajeroDaoImpl();
                    var responsable = bd.obtenerResponsable(Program.cajero.ToUpper());
                    if (responsable != null && responsable.Count > 0)
                    {
                        Cajero cajeroBean = responsable[0];
                        var activity = context.MakeMessage();
                        activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        string resp = string.Empty;
                        if (cajeroBean.responsable.Equals(""))
                        {
                            resp = "No hay datos del responsable";

                        }
                        else
                        {
                            resp = cajeroBean.responsable;
                        }

                        var menuHeroCard = new ThumbnailCard
                        {
                            //Subtitle = cajeroBean.conteo + " falla(s)",
                            Title = "El responsable del cajero " + Program.cajero.ToUpper() + " es: ",
                            Subtitle = resp,
                            Images = new List<CardImage> {
                        new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/solucion.jpg" }
                    }
                        }.ToAttachment();

                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard);
                        await context.PostAsync(activity);

                        //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
                        //var activity = context.MakeMessage();
                        activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        var menuHeroCard2 = new ThumbnailCard
                        {
                            Text = "Algo más en que le podamos ayudar?",
                            Subtitle = "Espero que la información sea de su utilidad"

                        }.ToAttachment();

                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard2);
                        await context.PostAsync(activity);


                    }
                    else
                    {

                        var activity = context.MakeMessage();
                        activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        var menuHeroCard = new ThumbnailCard
                        {
                            Text = "Algo más en que le podamos ayudar?",
                            //Subtitle = "Verifique e",
                            Title = "No es posible identificar ese cajero",
                            Images = new List<CardImage> {
                        new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/error.jpg" }
                    }
                        }.ToAttachment();

                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard);
                        await context.PostAsync(activity);

                    }
                    return;
                case "Historico de fallas":
                    bd = new CajeroDaoImpl();
                    string periodo = string.Empty;
                    periodo = "MONTH";
                    var historico = bd.obtenerHistoricoCajero(Program.cajero.ToUpper(), periodo);
                    if (historico != null && historico.Count > 0)
                    {
                        //Cajero cajeroBean = historico[0];

                        var activity = context.MakeMessage();
                        activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        var menuHeroCard = new ThumbnailCard
                        {
                            //Subtitle = cajeroBean.conteo + " falla(s)",
                            Title = "El cajero " + Program.cajero.ToUpper() + " tiene ha tenido las siguientes fallas: ",
                            Images = new List<CardImage> {
                        new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/cajeroerror.jpg" }
                    }
                        }.ToAttachment();

                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard);


                        await context.PostAsync(activity);

                        for (int i = 0; i < historico.Count; i++)
                        {
                            Cajero cajeroBean = historico[i];

                            var tipofalla = String.Empty;
                            var folio = String.Empty;
                            switch (cajeroBean.tipoFalla)
                            {
                                case "ComunicacionEnergia":
                                    tipofalla = "Cajero sin energía";
                                    break;
                                case "ErrorSinEfectivo":
                                    tipofalla = "Cajero sin efectivo";
                                    break;
                                case "ModoSupervisor":
                                    tipofalla = "Modo Supervisor";
                                    break;
                                case "FallaHardware":
                                    tipofalla = "Falla en el hardware";
                                    break;
                                case "ProblemaLocal":
                                    tipofalla = "Cajero con problema local";
                                    break;
                                case "TrxsNoMonetarias":
                                    tipofalla = "transacciones no monetarias";
                                    break;
                                default:
                                    tipofalla = "Sin identificar";
                                    break;
                            }

                            if (cajeroBean.folio == "")
                            {
                                folio = "Sin folio";
                            }
                            else
                            {
                                folio = cajeroBean.folio;
                            }

                            await context.PostAsync("Falla: " + tipofalla + ",  Folio: " + folio + ", Fecha: " + cajeroBean.fecha);

                        }
                        //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
                        //var activity = context.MakeMessage();
                        activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        var menuHeroCard2 = new ThumbnailCard
                        {
                            Text = "Algo más en que le podamos ayudar?",
                            Subtitle = "Espero que la información sea de su utilidad"

                        }.ToAttachment();

                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard2);
                        await context.PostAsync(activity);
                    }
                    else
                    {
                        var activity = context.MakeMessage();
                        activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        var menuHeroCard = new ThumbnailCard
                        {
                            Text = "Algo más en que le podamos ayudar?",
                            //Subtitle = "Verifique e",
                            Title = "No es posible identificar ese cajero",
                            Images = new List<CardImage> {
                        new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/error.jpg" }
                    }
                        }.ToAttachment();

                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard);
                        await context.PostAsync(activity);

                        //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar");
                    }

                    return;
                case "Fecha posible de solucion":
                    bd = new CajeroDaoImpl();
                    responsable = bd.obtenerResponsable(Program.cajero.ToUpper());
                    if (responsable != null && responsable.Count > 0)
                    {
                        Cajero cajeroBean = responsable[0];
                        var activity = context.MakeMessage();
                        activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        string resp = string.Empty;
                        if (cajeroBean.responsable.Equals(""))
                        {
                            resp = "No hay datos del responsable";

                        }
                        else
                        {
                            resp = cajeroBean.responsable;
                        }

                        var menuHeroCard = new ThumbnailCard
                        {
                            //Subtitle = cajeroBean.conteo + " falla(s)",
                            Title = "El responsable del cajero " + Program.cajero.ToUpper() + " es: ",
                            Subtitle = resp,
                            Images = new List<CardImage> {
                        new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/solucion.jpg" }
                    }
                        }.ToAttachment();

                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard);
                        await context.PostAsync(activity);

                        // await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
                        //var activity = context.MakeMessage();
                        activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        var menuHeroCard2 = new ThumbnailCard
                        {
                            Text = "Algo más en que le podamos ayudar?",
                            Subtitle = "Espero que la información sea de su utilidad"

                        }.ToAttachment();

                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard2);
                        await context.PostAsync(activity);


                    }
                    else
                    {

                        var activity = context.MakeMessage();
                        activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        var menuHeroCard = new ThumbnailCard
                        {
                            Text = "Algo más en que le podamos ayudar?",
                            //Subtitle = "Verifique e",
                            Title = "No es posible identificar ese cajero",
                            Images = new List<CardImage> {
                        new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/error.jpg" }
                    }
                        }.ToAttachment();

                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard);
                        await context.PostAsync(activity);

                    }

                    return;
                case "Finalizar":


                    return;

            }
            /*  var card = new AdaptiveCard()
              {
                  Body = new List<CardElement>()
                  {
                      new Container()
                      {
                          Items = new List<CardElement>()
                          {
                              new TextBlock()
                              {
                                  Text = $"{selecciona.opcion}",
                                  Weight = TextWeight.Bolder,
                                  Speak = $"<s>{hotel.Name}</s>"
                              },
                              new TextBlock()
                              {
                                  Text = description,
                                  Speak = $"<s>{description}</s>"
                              },
                              new Image()
                              {
                                  Size = ImageSize.Large,
                                  Url = hotel.Image
                              },
                              new ImageSet()
                              {
                                  ImageSize = ImageSize.Medium,
                                  Separation = SeparationStyle.Strong,
                                  Images = hotel.MoreImages.Select(img => new Image()
                                  {
                                      Url = img
                                  }).ToList()
                              }
                          },
                          SelectAction = new OpenUrlAction()
                          {
                               Url = "https://dev.botframework.com/"
                          }
                      }
                  }
              };

              Attachment attachment = new Attachment()
              {
                  ContentType = AdaptiveCard.ContentType,
                  Content = card
              };

              var reply = context.MakeMessage();
              reply.Attachments.Add(attachment);

              await context.PostAsync(reply, CancellationToken.None);
              */
        }

        /* private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
         {
             context.Wait(this.MessageReceivedAsync);
         }*/

        //private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {

            var mensaje = await result as Activity;
            var textoDelUsuario = mensaje.Text;


            if (mensaje.Value != null)
            {
                dynamic value = mensaje.Value;
                string submitType = value.Type.ToString();
                switch (submitType)
                {
                    case "EstatusSearch":
                        var activity = context.MakeMessage();
                        activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        var menuHeroCard = new ThumbnailCard
                        {
                            Text = $"Escriba el nombre de {Program.estatus} que desea consultar"
                        }.ToAttachment();

                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard);
                        await context.PostAsync(activity);
                        return;
                    case "HistoricoSearch":
                        var activity2 = context.MakeMessage();
                        activity2.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        var menuHeroCard2 = new ThumbnailCard
                        {
                            Text = $"Escriba 'historico' + nombre de {Program.historico} que desea consultar"
                        }.ToAttachment();

                        activity2.Attachments = new List<Attachment>();
                        activity2.Attachments.Add(menuHeroCard2);
                        await context.PostAsync(activity2);
                        //await opcionesAcciones_2(context, CajeroOption);
                        return;
                    case "Menu":
                        await ManejarSaludo(context);
                        return;
                    case "MenuCajeroSelection":
                        await SendMenuSelectionAsync2(context, (MenuCajeros)JsonConvert.DeserializeObject<MenuCajeros>(value.ToString()));
                        //await opcionesAcciones(context, EmpresaOption);
                        //context.Wait(MessageReceivedAsync);
                        //await context.PostAsync("hola");
                        return;
                }
            }


            var consultor = new ConsultorLuis();
            var objetoLuis = await consultor.ConsultarLuis(textoDelUsuario);
            var intension = consultor.ConvertirObjetoLuisAIntencion(objetoLuis);

            var empresa = String.Empty;
            var cajero = String.Empty;
            var tiempo = String.Empty;


            switch (intension)
            {
                case Intensiones.Saludo:
                    await ManejarSaludo(context);
                    //context.Wait(MessageReceivedAsync);
                    break;
                case Intensiones.Ayuda:
                    await ManejarSaludo(context);
                    break;
                case Intensiones.None:
                    var activity2 = context.MakeMessage();
                    activity2.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    var menuHeroCard6 = new ThumbnailCard
                    {
                        Subtitle = "No entendi la solicitud",
                        Text = "Utiliza la opcion + 'cajero', 'empresa' o 'grupo' + nombre del elemento",
                    }.ToAttachment();

                    activity2.Attachments = new List<Attachment>();
                    activity2.Attachments.Add(menuHeroCard6);
                    await context.PostAsync(activity2);
                    break;
                case Intensiones.SolicitarEstatusCajero:
                    /*if (objetoLuis.Entidades[0].entity.Equals("desconocido"))
                    {                           
                        var activity = context.MakeMessage();
                        activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        var menuHeroCard = new ThumbnailCard
                        {
                            Subtitle = "No entendi la solicitud",
                            Text = "Utiliza la opcion + 'cajero', 'empresa' o 'grupo' + nombre del elemento"
                        }.ToAttachment();

                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard);
                        await context.PostAsync(activity);
                    }
                    else
                    {*/
                    Program.cajero = objetoLuis.Entidades[0].entity;
                    await SolicitarEstatusCajero(context, Program.cajero);
                    // }
                    break;
                case Intensiones.SolicitarEstatusCajerosEmpresa:
                    if (objetoLuis.Entidades[0].entity.Equals("desconocido"))
                    {
                        var activity = context.MakeMessage();
                        activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        var menuHeroCard = new ThumbnailCard
                        {
                            Subtitle = "No entendi la solicitud ",
                            Text = "Utiliza la palabra 'cajero' o 'empresa' o 'grupo' dentro de la solicitud + nombre del elemento"
                        }.ToAttachment();

                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard);
                        await context.PostAsync(activity);
                    }
                    else
                    {
                        Program.empresa = objetoLuis.Entidades[0].entity;
                        await SolicitarEstatusCajerosEmpresa(context, Program.empresa);
                    }
                    break;
                case Intensiones.SolicitarEstatusCajeroGrupo:
                    Program.grupo = objetoLuis.Entidades[0].entity;
                    await SolicitarEstatusCajerosGrupo(context, Program.grupo);
                    break;
                case Intensiones.SolicitarHistoricoFallasCajeros:
                    tiempo = objetoLuis.Entidades[0].entity;
                    if (objetoLuis.Entidades[0].type.Equals("cajero"))
                    {
                        Program.cajero = objetoLuis.Entidades[0].entity;
                        tiempo = "historico";
                        await SolicitarHistoricoCajero(context, tiempo, Program.cajero);
                    }
                    else
                    {
                        Program.cajero = objetoLuis.Entidades[1].entity;
                        await SolicitarHistoricoCajero(context, tiempo, Program.cajero);

                    }
                    break;
                case Intensiones.SolicitarHistoricoFallasCajerosEmpresa:
                    tiempo = objetoLuis.Entidades[0].entity;
                    if (objetoLuis.Entidades[0].type.Equals("empresa"))
                    {
                        Program.empresa = objetoLuis.Entidades[1].entity;
                        tiempo = "historico";
                        await SolicitarHistoricoCajeroEmpresa(context, tiempo, Program.empresa);
                    }
                    else {
                        Program.empresa = objetoLuis.Entidades[1].entity;
                        await SolicitarHistoricoCajeroEmpresa(context, tiempo, Program.empresa);
                    }
                    break;
                case Intensiones.SolicitarHistoricoFallasCajerosGrupo:
                    tiempo = objetoLuis.Entidades[0].entity;
                    if (objetoLuis.Entidades[0].type.Equals("grupo"))
                    {
                        Program.grupo = objetoLuis.Entidades[1].entity;
                        tiempo = "historico";
                        //await SolicitarHistoricoCajero(context, tiempo, Program.empresa);
                    }
                    else
                    {
                        Program.empresa = objetoLuis.Entidades[1].entity;
                        //await SolicitarHistoricoCajeroEmpresa(context, tiempo, Program.empresa);
                    }
                    break;

                case Intensiones.solicitarFechaSolucion:
                    tiempo = objetoLuis.Entidades[0].entity;
                    if (objetoLuis.Entidades.Count > 1) {
                        Program.cajero = objetoLuis.Entidades[1].entity;
                    }
                    await SolicitarFechaSolucion(context, Program.cajero, tiempo);
                    break;
                case Intensiones.SolicitarResponsableCajero:
                    if (objetoLuis.Entidades[0].GetType().Equals("responsable")) {
                        Program.cajero = objetoLuis.Entidades[1].entity;
                    }
                    else
                    {
                        Program.cajero = objetoLuis.Entidades[0].entity;
                    }
                    //Program.cajero = objetoLuis.Entidades[1].entity;                    
                    await SolicitarResponsable(context, Program.cajero);
                    break;
                case Intensiones.SolicitarResponsableCajeroEmpresa:
                    if (objetoLuis.Entidades[0].GetType().Equals("responsable"))
                    {
                        Program.empresa = objetoLuis.Entidades[1].entity;
                    }
                    else
                    {
                        Program.empresa = objetoLuis.Entidades[0].entity;
                    }
                    //Program.empresa = objetoLuis.Entidades[1].entity;
                    await SolicitarResponsableEmpresa(context, Program.empresa);
                    break;
                case Intensiones.SolicitarResponsableCajeroGrupo:
                    if (objetoLuis.Entidades[0].GetType().Equals("responsable"))
                    {
                        Program.grupo = objetoLuis.Entidades[1].entity;
                    }
                    else
                    {
                        Program.grupo = objetoLuis.Entidades[0].entity;
                    }
                    //Program.grupo = objetoLuis.Entidades[1].entity;
                    await SolicitarResponsableGrupo(context, Program.grupo);
                    break;
                default:
                    await context.PostAsync(intension.ToString());
                    context.Wait(MessageReceivedAsync);
                    break;
            }
        }

        //private async Task ManejarSaludo(IDialogContext context)
        //{

        //ManejarAyuda(context);
        //PromptDialog.Text(context, RecibirEstadoUsuario, "¿Como se encuentra el día de hoy?");
        //}


        private async void MostrarBienvenida(IDialogContext context)
        {
            var activity = context.MakeMessage();
            activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            //var menuHeroCard = new HeroCard
            var menuHeroCard = new ThumbnailCard
            {
                //Text = "Disponibilidad",
                Title = "Bienvenido ChatBot Banca Transaccional",
                //Subtitle = "Cajeros Disponibilidad",
                Images = new List<CardImage> {
                    new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/banorte2.jpg" }
                }
            }.ToAttachment();

            activity.Attachments = new List<Attachment>();
            activity.Attachments.Add(menuHeroCard);

            await context.PostAsync(activity);
        }


        //private async void MostrarAyuda(IDialogContext context)
        private async Task ManejarSaludo(IDialogContext context)
        {
            /* var activity = context.MakeMessage();
             var menuHeroCard = new ThumbnailCard
             {

                 Text = "Bienvenido ChatBot Banca Transaccional",
                 Images = new List<CardImage> {
                     new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/banorte2.jpg" }
                 }
             }.ToAttachment();

             activity.Attachments = new List<Attachment>();
             activity.Attachments.Add(menuHeroCard);


             // activity.Text = "En este chat puedes preguntar sobre informacion de cajeros.";
             List<string> choices = new List<string>();
             choices.Add("Estatus del cajero  XXXXX");
             choices.Add("Fecha probable solucion del cajero XXXXX");
             choices.Add("Estatus cajeros de la empresa XXXXX");


             var result = ShowOptions2(choices);
             var reply = context.MakeMessage();
             reply.Attachments.Add(result);
             await context.PostAsync(reply);
             context.Wait(MessageReceivedAsync);*/

            //PromptDialog.Choice(context, this.OnOptionSelected, new List<String> { CajeroOption, EmpresaOption, GrupoOption }, "¿Qué deseas consultar?. Te puedo mostrar información sobre ", "Opcion no valida", 4,PromptStyle.PerLine);
            PromptDialog.Text(context, this.OnOptionSelected, $"Te puedo mostrar información referente a  {CajeroOption}, {EmpresaOption}, {GrupoOption}. ¿Sobre que deseas consultar?");



        }
        private static async Task SendMenuSelectionAsync2(IDialogContext context, MenuCajeros selecciona)
        {
            //var description = $"{hotel.Rating} start with {hotel.NumberOfReviews}. From ${hotel.PriceStarting} per night.";
            Program.empresa = selecciona.opcion;

        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result) {
            try {
                string optionSelected = await result;
                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                switch (optionSelected) {
                    case CajeroOption:
                        Program.estatus = CajeroOption;
                        Program.historico = CajeroOption;
                        await opcionesAcciones(context, CajeroOption);
                        var menuHeroCard = new ThumbnailCard
                        {
                            Text = "Para consultar por favor proporciona la opción deseada + el id del cajero"
                        }.ToAttachment();
                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard);
                        await context.PostAsync(activity);
                        //await context.PostAsync($"Que deseas consultar del {CajeroOption}?..");
                        break;
                    case EmpresaOption:
                        Program.estatus = EmpresaOption;
                        Program.historico = EmpresaOption;
                        await opcionesAcciones(context, EmpresaOption);
                        var menuHeroCard2 = new ThumbnailCard
                        {
                            Text = "Para consultar por favor proporciona la opción deseada + el nombre de la empresa"
                        }.ToAttachment();
                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard2);
                        await context.PostAsync(activity);
                        //await context.PostAsync($"Que deseas consultar de la {EmpresaOption}?..");
                        break;
                    case GrupoOption:
                        Program.estatus = GrupoOption;
                        Program.historico = GrupoOption;
                        await opcionesAcciones(context, GrupoOption);
                        var menuHeroCard3 = new ThumbnailCard
                        {
                            Text = "Para consultar por favor proporciona la opción deseada + el nombre del grupo"
                        }.ToAttachment();
                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard3);
                        await context.PostAsync(activity);
                        //await context.PostAsync($"Que deseas consultar del {GrupoOption}?..");
                        break;

                }

            }
            catch (TooManyAttemptsException) {
                await context.PostAsync($"Muchas peticiones ");
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task OnOptionSelected2(IDialogContext context, IAwaitable<string> result)
        {
            var confirm = await result;
            context.Done(confirm);

        }

        private async Task OnOptionSelected3(IDialogContext context, IAwaitable<string> result)
        {
            var confirm = await result;
            context.Done(confirm);

        }

        private async Task ManejarAyuda(IDialogContext context)
        {

            //await context.PostAsync("Has solicitado Ayuda ");
            //MostrarAyuda(context); 
            //PromptDialog.Text(context, RecibirEstadoUsuario, "¿Como se encuentra el día de hoy?");
        }
        private async Task opcionesAcciones(IDialogContext context, string opcion)
        {
            AdaptiveCard card = new AdaptiveCard()
            {
                Body = new List<CardElement>()
                            {
                                new Container()
                                {
                                      Items = new List<CardElement>(){
                                          //columna 1
                                          new ColumnSet()
                                          {
                                              Columns= new List<Column>{
                                                  new Column()
                                                  {
                                                      Size=ColumnSize.Auto,
                                                      Items = new List<CardElement>(){
                                                           new TextBlock(){
                                                              Text =$"De {opcion} te puedo mostar información como"
                                                          }
                                                          ,
                                                          new TextBlock(){
                                                              Text =$"Estatus actual de un(a) {opcion}"
                                                          },
                                                          new TextBlock(){
                                                              Text = $"Historico de un(a) {opcion}"
                                                          }
                                                          /*,
                                                          new TextBlock(){
                                                              Text = $"Selecciona la opcion + id de {opcion}"
                                                          }*/
                                                      }
                                                  }

                                              }
                                          }
                                      }
                                }
                            },
                /*Actions = new List<ActionBase>() {
                    new SubmitAction()
                    {                       
                        Title = $"Estatus {opcion}",
                        DataJson = "{ \"Type\": \"EstatusSearch\" }"
                        //Card = GetCajeroSearchCard()
                    }*/
                /*,
                new SubmitAction()
                {
                    Title = $"Historico {opcion}",
                    //Card = new AdaptiveCard()  
                    DataJson = "{ \"Type\": \"HistoricoSearch\" }"
                }*/
                //}

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



        private async Task opcionesAcciones2(IDialogContext context, List<object> lista, string titulo)
        {
            var opciones = this.GetOpciones(lista);
            //var title = $"Estas son las opciones";
            var title = titulo;
            var intro = new List<CardElement>()
                {
                    new TextBlock(){
                        Text=title,
                        Size = TextSize.Normal,
                        Weight = TextWeight.Normal,
                        Speak =$"<s>{title}</s>"
                    }
                };
            var rows = Split(opciones, 15)
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

        private async Task opcionesAcciones3(IDialogContext context, string lista, string titulo)
        {
            //var opciones = this.GetOpciones(lista);
            string[] plot = lista.Split(',');
            List<object> lis = new List<object>();
            foreach (string plots in plot) {
                lis.Add(plots);
            }
            var opciones = this.GetOpciones(lis);
            var title = titulo;
            var intro = new List<CardElement>()
                {
                    new TextBlock(){
                        Text=title,
                        Size = TextSize.Normal,
                        Weight = TextWeight.Normal,
                        Speak =$"<s>{title}</s>"
                    }
                };
            var rows = Split(opciones, 15)
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

        private IEnumerable<MenuCajeros> GetOpciones(List<object> opcion)
        {
            var opciones = new List<MenuCajeros>();
            foreach (string element in opcion) {
                MenuCajeros menu = new MenuCajeros() { opcion = element };
                opciones.Add(menu);
            }
            return opciones;
        }

        private Column AsHotelItem(MenuCajeros menu)
        {
            var submitActionData = JObject.Parse("{ \"Type\": \"MenuCajeroSelection\" }");
            submitActionData.Merge(JObject.FromObject(menu));
            return new Column()
            {
                //Size = "8",
                Size = ColumnSize.Auto,
                Items = new List<CardElement>()
                {
                    new TextBlock()
                    {
                        Text = menu.opcion,
                        Speak = $"<s>{menu.opcion}</s>",
                        HorizontalAlignment = HorizontalAlignment.Left,
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
        public static IEnumerable<IEnumerable<T>> Split<T>(IEnumerable<T> list, int parts)
        {
            return list.Select((item, ix) => new { ix, item })
                       .GroupBy(x => x.ix % parts)
                       .Select(x => x.Select(y => y.item));
        }


        /* private async Task RecibirEstadoUsuario(IDialogContext context, IAwaitable<string> estadoUsuarioAwaitable)
         {
             var estadoUsuario = await estadoUsuarioAwaitable;

             var consultor = new ConsultorLuis();
             var estadoDeAnimo = await consultor.ConsultarTextAnalytics(estadoUsuario);
             string respuestaParaUsuario = String.Empty;

             var activity = context.MakeMessage();
             activity.Attachments = new List<Attachment>();

             activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;

             switch (estadoDeAnimo)
             {
                 case Enums.EstadoDeAnimo.Bueno:
                     activity.Attachments.Add(new Attachment
                     {
                         ContentType = "image/jpg",
                         ContentUrl = "https://storageserviciobt.blob.core.windows.net/imagebot/feliz.jpg"
                     }
                     );
                     respuestaParaUsuario = "¡Excelente me da gusto saber eso!";
                     break;
                 case Enums.EstadoDeAnimo.Regular:
                     activity.Attachments.Add(new Attachment
                     {
                         ContentType = "image/jpg",
                         ContentUrl = "https://storageserviciobt.blob.core.windows.net/imagebot/confundido.jpg"
                     }
                     );
                     respuestaParaUsuario = "No estoy seguro como se siente, pero confio que estará bien";
                     break;
                 default:
                 case Enums.EstadoDeAnimo.Malo:
                     activity.Attachments.Add(new Attachment
                     {
                         ContentType = "image/jpg",
                         ContentUrl = "https://storageserviciobt.blob.core.windows.net/imagebot/triste.jpg"
                     }
                     );
                     respuestaParaUsuario = "Lamento saber eso, espero mejore su día.";
                     break;
             }

             activity.Text = respuestaParaUsuario;
             await context.PostAsync(activity);

             await context.PostAsync("¿En que le podemos ayudar?");
         }*/


        private async Task SolicitarEstatusCajero(IDialogContext context, string cajero) {
            IConsultorDB bd = new CajeroDaoImpl();
            var estatus = bd.ObtenerEstatusCajero(cajero.ToUpper());
            if (estatus.Equals(true))
            {
                List<Cajero> caj = bd.ObtenerFallaCajero(cajero.ToUpper());
                //var cajeros = bd.ObtenerFallaCajero(cajero.ToUpper());
                //if (cajeros != null && cajeros.Count > 0)
                if (caj != null && caj.Count > 0)
                {
                    // Cajero cajeroBean = cajeros[0];
                    var activity = context.MakeMessage();
                    /* activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                     var menuHeroCard = new ThumbnailCard
                     {
                         Text = "El cajero " + cajero.ToUpper() + " tiene: "+ cajeroBean.conteo + " falla(s)"
                         //Title = "El cajero " + cajero.ToUpper() + " tiene: ",
                         //Subtitle = cajeroBean.conteo + " falla(s)",
                        // Images = new List<CardImage> {
                        // new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/cajeroerror.jpg" }
                        // }
                     }.ToAttachment();

                     activity.Attachments = new List<Attachment>();
                     activity.Attachments.Add(menuHeroCard);
                     await context.PostAsync(activity);*/

                    /*  var tipofalla = String.Empty;
                      switch (cajeroBean.tipoFalla) {
                          case "ComunicacionEnergia":
                              tipofalla = "Cajero sin energía";
                              break;
                          case "ErrorSinEfectivo":
                              tipofalla = "Cajero sin efectivo";
                              break;
                          case "ModoSupervisor":
                              tipofalla = "Modo Supervisor";
                              break;
                          case "FallaHardware":
                              tipofalla = "Falla en el hardware";
                              break;
                          case "ProblemaLocal":
                              tipofalla = "Cajero con problema local";
                              break;
                          case "TrxsNoMonetarias":
                              tipofalla = "Transacciones no monetarias";
                              break;
                          default:
                              tipofalla = "Sin identificar";
                              break;
                      }*/

                    //await context.PostAsync("Falla: " + tipofalla + ",    Fecha: " + cajeroBean.fecha);
                    // await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");

                    // List<Cajero> caj = bd.ObtenerFallaCajero(cajero.ToUpper());
                    Program.cajero = cajero.ToUpper();
                    string menu = null;
                    List<object> opt = new List<object>();
                    var titulo = "Se encontraron " + caj.Count + " falla(s) en el cajero " + Program.cajero;
                    foreach (Cajero element in caj)
                    {
                        var tipofalla2 = String.Empty;
                        switch (element.tipoFalla)
                        {
                            case "ComunicacionEnergia":
                                tipofalla2 = "Sin energía";
                                break;
                            case "ErrorSinEfectivo":
                                tipofalla2 = "Sin efectivo";
                                break;
                            case "ModoSupervisor":
                                tipofalla2 = "Modo Supervisor";
                                break;
                            case "FallaHardware":
                                tipofalla2 = "Falla hardware";
                                break;
                            case "ProblemaLocal":
                                tipofalla2 = "Problema local";
                                break;
                            case "TrxsNoMonetarias":
                                tipofalla2 = "Trans. no monetarias";
                                break;
                            default:
                                tipofalla2 = "Sin identificar";
                                break;
                        }
                        //var menu = tipofalla2 + ", " + element.fecha+", "+element.folio;
                        //opt.Add(menu);
                        menu = "Falla de tipo " + tipofalla2 + ", con fecha del " + element.fecha + ",no. de folio: " + element.folio + ", El responsable es " + element.responsable + ",fecha posible de solucion: " + element.fechasolucion;
                        await opcionesAcciones3(context, menu, titulo);
                    }

                    ///await opcionesAcciones_2(context, CajeroOption);

                    //await opcionesAcciones3(context, menu, titulo);
                    //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
                    //var activity = context.MakeMessage();
                    activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    var menuHeroCard = new ThumbnailCard
                    {
                        Text = "Para este cajero puede consultar su 'historico' o si desea puede consultar el 'estatus' de  otro cajero",
                        Subtitle = "Espero que la información haya sido de utilidad"

                    }.ToAttachment();

                    activity.Attachments = new List<Attachment>();
                    activity.Attachments.Add(menuHeroCard);
                    await context.PostAsync(activity);

                }
                else
                {
                    var activity = context.MakeMessage();
                    activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    var menuHeroCard = new ThumbnailCard
                    {
                        Subtitle = "El cajero " + cajero.ToUpper() + " no tiene fallas",
                        Text = "Escribe menu para regresar a las opciones de fallas"
                    }.ToAttachment();

                    activity.Attachments = new List<Attachment>();
                    activity.Attachments.Add(menuHeroCard);
                    await context.PostAsync(activity);

                    //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar");
                }
            }
            else {

                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard = new ThumbnailCard
                {
                    //Subtitle = "El cajero " + cajero.ToUpper() + " no se encontro",
                    Text = "No se encontraron fallas en el cajero " + cajero.ToUpper()
                }.ToAttachment();
                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard);
                await context.PostAsync(activity);
                //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
            }
            context.Wait(MessageReceivedAsync);
        }


        private async Task SolicitarEstatusCajerosEmpresa(IDialogContext context, string empresa)
        {
            IConsultorDB bd = new CajeroDaoImpl();
            var obtienemepresa = bd.ObtenerEmpresas(empresa);
            StringBuilder sb = new StringBuilder();
            if (obtienemepresa.Count > 1)
            {
                if (obtienemepresa.Count > 15 && cnt == 0)
                {
                    cnt++;

                    var activity2 = context.MakeMessage();
                    activity2.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    var menuHeroCard2 = new ThumbnailCard
                    {
                        Text = "Se encontraron mas de 15 empresas con ese criterio, intente agregando dos o mas palabras para reducir la busqueda o vuelva a escribir la opcion, 'empresa' + nombre de la empresa",
                    }.ToAttachment();

                    activity2.Attachments = new List<Attachment>();
                    activity2.Attachments.Add(menuHeroCard2);
                    await context.PostAsync(activity2);


                }
                else
                {
                    cnt = 0;
                    /* List<Empresa> choices = new List<Empresa>();
                     for (int i = 0; i < obtienemepresa.Count; i++)
                     {            
                         choices.Add(new Empresa(obtienemepresa[i].empresa, obtienemepresa[i].id_empresa));
                      }
                     var result = ShowOptions(choices, empresa);
                     var activity = context.MakeMessage();
                     activity.Text = "Se encontro mas de una empresa. Escriba cual desea consultar ";
                     activity.Attachments.Add(result);
                     await context.PostAsync(activity);
                     //context.Wait(ConnectOption);*/
                    List<object> opt = new List<object>();
                    var titulo = "Las coincidencias encontradas son " + empresa.ToUpper();
                    foreach (Empresa element in obtienemepresa)
                    {
                        Int32 tam = empresa.Length;
                        string nombre = null;
                        if (tam >= 50) { nombre = element.empresa.Substring(0, tam - 18); }
                        else if (tam >= 40 && tam < 50) { nombre = element.empresa.Substring(0, tam - 10
                            ); } else { nombre = element.empresa; }
                        //var menu = element.empresa.Substring(tam);
                        var menu = nombre;
                        opt.Add(menu);

                    }
                    await opcionesAcciones2(context, opt, titulo);

                    var activity = context.MakeMessage();
                    activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    var menuHeroCard = new ThumbnailCard
                    {
                        Text = "Para consultar alguna proporciona 'empresa' + nombre de la empresa que desea",
                    }.ToAttachment();

                    activity.Attachments = new List<Attachment>();
                    activity.Attachments.Add(menuHeroCard);
                    await context.PostAsync(activity);
                }
            }//fin del if cuando se encuentra mas de una empresa
            else if (obtienemepresa.Count == 1)
            {
                Program.empresa = obtienemepresa[0].empresa;
                var conteo = bd.obtenerConteoCajerosEmpresa(Program.empresa.ToUpper());
                if (conteo == 1)
                {
                    var estatus = bd.obtenerEstatusCajerosEmpresa(Program.empresa.ToUpper());
                    if (estatus.Equals(true))
                    {
                        var empresas = bd.ObtenerFallasEmpresa(Program.empresa.ToUpper());
                        if (empresas != null && empresas.Count > 0)
                        {
                            var activity = context.MakeMessage();
                            List<object> opt = new List<object>();
                            Int32 tam = Program.empresa.Length;
                            string nombre = null;
                            if (tam > 40) { nombre = Program.empresa.Substring(0, tam - 15); }
                            else if (tam >= 30 && tam < 40) { nombre = Program.empresa.Substring(0, tam - 8); } else { nombre = Program.empresa; }


                            var titulo = nombre + " tiene " + empresas.Count + " cajeros con fallas: ";
                            foreach (Empresa element in empresas)
                            {
                                var tipofalla2 = String.Empty;
                                switch (element.tipoFalla)
                                {
                                    case "ComunicacionEnergia":
                                        tipofalla2 = "Sin energía";
                                        break;
                                    case "ErrorSinEfectivo":
                                        tipofalla2 = "Sin efectivo";
                                        break;
                                    case "ModoSupervisor":
                                        tipofalla2 = "Modo Supervisor";
                                        break;
                                    case "FallaHardware":
                                        tipofalla2 = "Falla hardware";
                                        break;
                                    case "ProblemaLocal":
                                        tipofalla2 = "Problema local";
                                        break;
                                    case "TrxsNoMonetarias":
                                        tipofalla2 = "Trans. no monetarias";
                                        break;
                                    default:
                                        tipofalla2 = "Sin identificar";
                                        break;
                                }
                                var menu = element.cajero + "   " + tipofalla2 + "  " + element.folio;
                                opt.Add(menu);
                            }

                            await opcionesAcciones2(context, opt, titulo);
                            //var activity = context.MakeMessage();
                            activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                            var menuHeroCard = new ThumbnailCard
                            {
                                Text = "Algo más en que le podamos ayudar?",
                                Subtitle = "Espero que la información sea de su utilidad"

                            }.ToAttachment();

                            activity.Attachments = new List<Attachment>();
                            activity.Attachments.Add(menuHeroCard);
                            await context.PostAsync(activity);

                            //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
                        }
                        else
                        {
                            var activity = context.MakeMessage();
                            activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                            var menuHeroCard = new ThumbnailCard
                            {
                                Text = "Algo más en que le podamos ayudar?",
                                Subtitle = "Verifique el nombre de la empresa",
                                //Title = "No se identifico el cajero como parte de banca transaccional",
                                //Images = new List<CardImage> {
                                //new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/error.jpg" }
                                //}
                            }.ToAttachment();

                            activity.Attachments = new List<Attachment>();
                            activity.Attachments.Add(menuHeroCard);
                            await context.PostAsync(activity);
                        }
                    }//fin del if cuando hay fallas
                    else
                    {
                        var activity = context.MakeMessage();
                        /*   activity.Text = "No se encontraron fallas en los cajeros de la empresa " + empresa.ToUpper();
                           await context.PostAsync(activity);
                           await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");*/


                        activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        var menuHeroCard = new ThumbnailCard
                        {
                            Text = "Algo más en que le podamos ayudar?",
                            Subtitle = "No se encontraron fallas en los cajeros de la empresa " + empresa.ToUpper(),
                        }.ToAttachment();

                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard);
                        await context.PostAsync(activity);
                    }
                }//fin del id cuando se encutra una empresa
                else
                {
                    var activity = context.MakeMessage();
                    activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    var menuHeroCard = new ThumbnailCard
                    {
                        Text = $"La empresa {Program.empresa} no tiene fallas en los cajeros"
                    }.ToAttachment();
                    activity.Attachments = new List<Attachment>();
                    activity.Attachments.Add(menuHeroCard);

                    await context.PostAsync(activity);
                    //await context.PostAsync(Program.empresa);

                    //await SolicitarEstatusCajerosGrupo(context, Program.empresa);

                    //PromptDialog.Choice(context, this.OnOptionSelected2, new List<String> { SiOption, NoOption}, "Quieres buscarlo por grupo?", "Opcion no valida", 3, PromptStyle.Auto);
                    //var dialog = new PromptDialog.PromptString("Quieres buscar "+Program.empresa+"por grupo", "Por favor confirme",2);
                    //context.Call(dialog, OnOptionSelected2);

                }
            }
            else {

                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var buscaengrupo = bd.ObtenerGrupos(Program.empresa);
                if (buscaengrupo.Count == 1)
                {
                    string obj = null;
                    String titulo = "La empresa como la ingreso no se encontro";
                    obj = "Coincide con el grupo," + buscaengrupo[0].grupo;
                    await opcionesAcciones3(context, obj, titulo);
                    var menuHeroCard = new ThumbnailCard
                    {
                        Text = "Para consultar el estatus del grupo escriba grupo + nombre del grupo"
                    }.ToAttachment();
                    activity.Attachments = new List<Attachment>();
                    activity.Attachments.Add(menuHeroCard);
                }
                else if (buscaengrupo.Count > 1)
                {
                    List<object> obj = new List<object>();
                    String titulo = "La empresa no se encontro, pero si coincide con varios grupos";
                    //obj = "Coincide con el grupo," + buscaengrupo[0].grupo;
                    foreach (Grupo grup in buscaengrupo)
                    {
                        obj.Add(grup.grupo);
                    }
                    await opcionesAcciones2(context, obj, titulo);
                    var menuHeroCard = new ThumbnailCard
                    {
                        Text = "Para buscarla por grupo escriba grupo + el nombre del grupo"
                    }.ToAttachment();
                    activity.Attachments = new List<Attachment>();
                    activity.Attachments.Add(menuHeroCard);
                }
                else
                {
                    var menuHeroCard = new ThumbnailCard
                    {
                        Text = "La empresa como la ingreso no se encontro o no pertenece a banca transaccional. vuelva a intentarlo"
                    }.ToAttachment();
                    activity.Attachments = new List<Attachment>();
                    activity.Attachments.Add(menuHeroCard);
                }

                await context.PostAsync(activity);


            }
            context.Wait(MessageReceivedAsync);
        }


        private async Task SolicitarEstatusCajerosGrupo(IDialogContext context, string grupo)
        {
            IConsultorDB bd = new CajeroDaoImpl();
            var obtienemepresa = bd.ObtenerGrupos(grupo);
            string cadena = String.Empty;
            StringBuilder sb = new StringBuilder();
            if (obtienemepresa.Count > 1)
            {
                if (obtienemepresa.Count > 15 && cnt == 0)
                {
                    cnt++;

                    var activity2 = context.MakeMessage();
                    activity2.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    var menuHeroCard2 = new ThumbnailCard
                    {
                        Text = "Se encontraron mas de 15 grupo con ese criterio, intente agregando dos o mas palabras para reducir la busqueda o vuelva a escribir la opcion, 'grupo' + nombre del grupo",
                    }.ToAttachment();

                    activity2.Attachments = new List<Attachment>();
                    activity2.Attachments.Add(menuHeroCard2);
                    await context.PostAsync(activity2);
                }
                else
                {
                    cnt = 0;
                    List<object> opt = new List<object>();
                    var titulo = "Las coincidencias encontrads con " + grupo.ToUpper() + " son";
                    foreach (Grupo element in obtienemepresa)
                    {
                        Int32 tam = grupo.Length;
                        string nombre = null;
                        if (tam >= 50) { nombre = element.grupo.Substring(0, tam - 18); }
                        else if (tam >= 40 && tam < 50) { nombre = element.grupo.Substring(0, tam - 10); } else { nombre = element.grupo; }
                        var menu = nombre;
                        //var menu = element.grupo;
                        opt.Add(menu);
                    }
                    await opcionesAcciones2(context, opt, titulo);

                    var activity = context.MakeMessage();
                    activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    var menuHeroCard = new ThumbnailCard
                    {
                        Text = "Escriba 'grupo' + el nombre del grupo que desea consultar ",
                    }.ToAttachment();

                    activity.Attachments = new List<Attachment>();
                    activity.Attachments.Add(menuHeroCard);
                    await context.PostAsync(activity);
                }
            }//fin del if cuando se encuentra mas de una empresa
            else 
            if (obtienemepresa.Count == 1)
            {
                Program.grupo = obtienemepresa[0].grupo;
                var conteo = bd.obtenerConteoCajerosGrupo(Program.grupo.ToUpper());
                if (conteo >= 1)
                {
                    var estatus = bd.obtenerEstatusCajerosGrupo(Program.grupo.ToUpper());
                    if (estatus.Equals(true))
                    {
                        var empresas = bd.ObtenerFallasGrupo(Program.grupo.ToUpper());
                        if (empresas != null && empresas.Count > 0)
                        {
                            var activity = context.MakeMessage();
                            //List<object> opt = new List<object>();
                            string opt;

                            Int32 tam = Program.grupo.Length;
                            string nombre = null;
                            if (tam >= 40) { nombre = Program.grupo.Substring(0, tam - 15); }
                            else if (tam >= 30 && tam < 40) { nombre = Program.grupo.Substring(0, tam - 8); } else { nombre = Program.grupo; }

                            var titulo =  nombre+" tiene fallas en ";
                            foreach (Grupo element in empresas)
                            {
                                var tipofalla2 = String.Empty;
                                switch (element.tipoFalla)
                                {
                                    case "ComunicacionEnergia":
                                        tipofalla2 = "Sin energía";
                                        break;
                                    case "ErrorSinEfectivo":
                                        tipofalla2 = "Sin efectivo";
                                        break;
                                    case "ModoSupervisor":
                                        tipofalla2 = "Modo Supervisor";
                                        break;
                                    case "FallaHardware":
                                        tipofalla2 = "Falla hardware";
                                        break;
                                    case "ProblemaLocal":
                                        tipofalla2 = "Problema local";
                                        break;
                                    case "TrxsNoMonetarias":
                                        tipofalla2 = "Trans. no monetarias";
                                        break;
                                    default:
                                        tipofalla2 = "Sin identificar";
                                        break;
                                }

                                Int32 tam2 = element.empresa.Length;
                                string nombre2 = null;
                                if (tam >= 30) { nombre2 = element.empresa.Substring(0, tam - 15); }
                                else if (tam >= 20 && tam < 30) { nombre2 = element.empresa.Substring(0, tam - 8); } else { nombre2 = element.empresa; }


                                //var menu = nombre2.ToLower()+","+element.cajero + "," + tipofalla2 + "," + element.folio;
                                //var menu = "La empresa"+nombre2.ToLower() + "," + element.cajero + ", de tipo " + tipofalla2;
                                opt = "La empresa " + nombre2.ToLower() + ",en el cajero " + element.cajero + ", de tipo " + tipofalla2;
                                //opt.Add(menu);
                                await opcionesAcciones3(context, opt, titulo);
                            }

                            //await opcionesAcciones2(context, opt, titulo);

                            //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
                            //var activity = context.MakeMessage();
                            activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                            var menuHeroCard = new ThumbnailCard
                            {
                                Text = "Algo más en que le podamos ayudar?",
                                Subtitle = "Espero que la información sea de su utilidad"

                            }.ToAttachment();

                            activity.Attachments = new List<Attachment>();
                            activity.Attachments.Add(menuHeroCard);
                            await context.PostAsync(activity);
                        }
                        else
                        {
                            var activity = context.MakeMessage();
                            activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                            var menuHeroCard = new ThumbnailCard
                            {
                                Text = "Algo más en que le podamos ayudar?",
                                Subtitle = "Verifique el nombre del grupo",
                            }.ToAttachment();

                            activity.Attachments = new List<Attachment>();
                            activity.Attachments.Add(menuHeroCard);
                            await context.PostAsync(activity);
                        }
                    }
                    else
                    {
                        var activity = context.MakeMessage();
                        activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        var menuHeroCard = new ThumbnailCard
                        {
                            Text = $"El grupo {Program.grupo} no tiene fallas en los cajeros"
                        }.ToAttachment();

                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard);
                        await context.PostAsync(activity);
                    }
                }
                else
                {
                    var activity = context.MakeMessage();
                    activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    var menuHeroCard = new ThumbnailCard
                    {
                        Text = $"El grupo {Program.grupo} no tiene fallas en los cajeros"
                    }.ToAttachment();
                    activity.Attachments = new List<Attachment>();
                    activity.Attachments.Add(menuHeroCard);

                    await context.PostAsync(activity);

                }
            }//fin de cuando se encontro el grupo
            else
            {
                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var buscaenempresa = bd.ObtenerEmpresas(Program.grupo);
                if (buscaenempresa.Count == 1)
                {
                    string obj = null;
                    String titulo = "El grupo como lo ingreso no se encontro";
                    obj = "Coincide con la empresa," + buscaenempresa[0].empresa;
                    await opcionesAcciones3(context, obj, titulo);
                    var menuHeroCard = new ThumbnailCard
                    {
                        Text = "Para consultar el estatus de la empresa escriba empresa + nombre de la empresa"
                    }.ToAttachment();
                    activity.Attachments = new List<Attachment>();
                    activity.Attachments.Add(menuHeroCard);
                }
                else if (buscaenempresa.Count > 1)
                {
                    List<object> obj = new List<object>();
                    String titulo = "El grupo no se encontro, pero si coincide con varias empresas";
                    //obj = "Coincide con el grupo," + buscaengrupo[0].grupo;
                    foreach (Empresa grup in buscaenempresa)
                    {
                        obj.Add(grup.empresa);
                    }
                    await opcionesAcciones2(context, obj, titulo);
                    var menuHeroCard = new ThumbnailCard
                    {
                        Text = "si desea buscarla por empresa escriba empresa + el nombre de la empresa"
                    }.ToAttachment();
                    activity.Attachments = new List<Attachment>();
                    activity.Attachments.Add(menuHeroCard);
                }
                else
                {
                    var menuHeroCard = new ThumbnailCard
                    {
                        Text = "El grupo como la ingreso no se encontro no pertenece a banca transaccional. vuelva a intentarlo"
                    }.ToAttachment();
                    activity.Attachments = new List<Attachment>();
                    activity.Attachments.Add(menuHeroCard);
                }
                await context.PostAsync(activity);
            }
             context.Wait(MessageReceivedAsync);            
        }

        private async Task SolicitarHistoricoCajero(IDialogContext context, String tiempo, string cajero) {
            IConsultorDB bd = new CajeroDaoImpl();

            string periodo = string.Empty;
            if (tiempo.Contains("ultimos"))
            {
                periodo = "TOP 5";
            }
            else if (tiempo.Contains("historico"))
            {
                periodo = "MONTH";
            }
            else if (tiempo.Contains("mes"))
            {
                periodo = "MONTH";
            }
            else if (tiempo.Contains("dia"))
            {
                periodo = "DAY";
            }
            else {
                periodo = tiempo;
            }


            var historico = bd.obtenerHistoricoCajero(cajero.ToUpper(), periodo);
            if (historico != null && historico.Count > 0)
            {
                var activity = context.MakeMessage();
                /*activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard = new ThumbnailCard
                {
                    //Subtitle = cajeroBean.conteo + " falla(s)",
                    Subtitle = "Historico fallas cajero "+Program.cajero.ToUpper() 
                    //Images = new List<CardImage> {
                    //    new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/cajeroerror.jpg" }
                    //}
                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard);


                await context.PostAsync(activity);*/
                List<object> opt = new List<object>();
                string obj = null;
                var titulo = "El cajero " + Program.cajero.ToUpper()+" ha tenido las siguientes fallas";

                for (int i = 0; i < historico.Count; i++)
                {
                    Cajero cajeroBean = historico[i];

                    var tipofalla = String.Empty;
                    var folio = String.Empty;
                    switch (cajeroBean.tipoFalla)
                    {
                        case "ComunicacionEnergia":
                            tipofalla = "Sin energía";
                            break;
                        case "ErrorSinEfectivo":
                            tipofalla = "Sin efectivo";
                            break;
                        case "ModoSupervisor":
                            tipofalla = "Modo Supervisor";
                            break;
                        case "FallaHardware":
                            tipofalla = "Falla hardware";
                            break;
                        case "ProblemaLocal":
                            tipofalla = "Problema local";
                            break;
                        case "TrxsNoMonetarias":
                            tipofalla = "trans. no monetarias";
                            break;
                        default:
                            tipofalla = "Sin identificar";
                            break;
                    }

                    if (cajeroBean.folio == "")
                    {
                        folio = "Sin folio";
                    }
                    else
                    {
                        folio = cajeroBean.folio;
                    }

                    //var menu = tipofalla + "   " + folio+"  "+cajeroBean.fecha;
                    //opt.Add(menu);
                    //await context.PostAsync("Falla: " + tipofalla + ",  Folio: " + folio+", Fecha: "+cajeroBean.fecha);
                    obj= "Falla de tipo " + tipofalla + ",con fecha " + cajeroBean.fecha + ", y folio " + cajeroBean.folio;
                    await opcionesAcciones3(context, obj, titulo);
                }
                //await opcionesAcciones3(context, obj, titulo);
                //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?"); 
                //var activity = context.MakeMessage();

               /* activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard2 = new ThumbnailCard
                {
                    Text = "Escribe menu para regresar a las opciones de fallas",
                    Subtitle = "Escribe estatus para consultar fallas en el cajero"

                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard2);
                await context.PostAsync(activity);*/
            }
            else
            {
                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard = new ThumbnailCard
                {
                    //Text = "Escribe menu para regresar a las opciones de fallas",
                    Text = "El cajero " + Program.cajero +" no ha tenido fallas en los ultimos meses"

                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard);
                await context.PostAsync(activity);

                //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar");
            }


            context.Wait(MessageReceivedAsync);
        }
    


        private async Task SolicitarHistoricoCajeroEmpresa(IDialogContext context, string tiempo, string empresa)
        {
            IConsultorDB bd = new CajeroDaoImpl();
            string periodo = string.Empty;

            var historico = bd.obtenerHistoricoCajeroEmpresa(empresa.ToUpper(), periodo);
            if (historico != null && historico.Count > 0)
            {
                //Cajero cajeroBean = historico[0];

                var activity = context.MakeMessage();
                /* activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                 var menuHeroCard = new ThumbnailCard
                 {
                     //Subtitle = cajeroBean.conteo + " falla(s)",
                     Subtitle = "La empresa " + empresa.ToUpper() + " tiene ha tenido las siguientes fallas: "
                     //Images = new List<CardImage> {
                     //    new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/cajeroerror.jpg" }
                     //}
                 }.ToAttachment();

                 activity.Attachments = new List<Attachment>();
                 activity.Attachments.Add(menuHeroCard);
                 await context.PostAsync(activity);*/

                List<object> opt = new List<object>();
                var titulo = "La empresa " + Program.empresa.ToUpper()+" ha tenido las siguientes fallas ";

                for (int i = 0; i < historico.Count; i++)
                {
                    Empresa cajeroBean = historico[i];

                    var tipofalla = String.Empty;
                    var folio = String.Empty;
                    switch (cajeroBean.tipoFalla)
                    {
                        case "ComunicacionEnergia":
                            tipofalla = "Sin energía";
                            break;
                        case "ErrorSinEfectivo":
                            tipofalla = "Sin efectivo";
                            break;
                        case "ModoSupervisor":
                            tipofalla = "Modo Supervisor";
                            break;
                        case "FallaHardware":
                            tipofalla = "Falla hardware";
                            break;
                        case "ProblemaLocal":
                            tipofalla = "Problema local";
                            break;
                        case "TrxsNoMonetarias":
                            tipofalla = "trans. no monetarias";
                            break;
                        default:
                            tipofalla = "Sin identificar";
                            break;
                    }

                    if (cajeroBean.folio == "")
                    {
                        folio = "Sin folio";
                    }
                    else
                    {
                        folio = cajeroBean.folio;
                    }

                    //await context.PostAsync("Falla: " + tipofalla + ",  Folio: " + folio + ", Fecha: " + cajeroBean.fecha);
                    var menu = cajeroBean.cajero + "   " + tipofalla + "   " + folio + "  " + cajeroBean.fecha;
                    opt.Add(menu);

                }

                await opcionesAcciones2(context, opt, titulo);

                //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
                //var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard2 = new ThumbnailCard
                {
                    Text = "Algo más en que le podamos ayudar?",
                    Subtitle = "Espero que la información sea de su utilidad"

                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard2);
                await context.PostAsync(activity);


            }
            else
            {
                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard = new ThumbnailCard
                {
                    Text = "No se encontraron fallas en cajeros de la empresa  "+Program.empresa
                    //Title = "No es posible identificar ese cajero"
                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard);
                await context.PostAsync(activity);

                //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar");
            }


            context.Wait(MessageReceivedAsync);
        }

        private async Task SolicitarHistoricoCajeroGrupo(IDialogContext context, string tiempo, string grupo)
        {
            IConsultorDB bd = new CajeroDaoImpl();
            string periodo = string.Empty;

            var historico = bd.obtenerHistoricoCajeroGrupo(grupo.ToUpper(), periodo);
            if (historico != null && historico.Count > 0)
            {
                //Cajero cajeroBean = historico[0];

                var activity = context.MakeMessage();
  
                List<object> opt = new List<object>();
                var titulo = "El grupo " + Program.grupo.ToUpper()+" ha tenido las siguientes fallas";

                for (int i = 0; i < historico.Count; i++)
                {
                    Grupo cajeroBean = historico[i];

                    var tipofalla = String.Empty;
                    var folio = String.Empty;
                    switch (cajeroBean.tipoFalla)
                    {
                        case "ComunicacionEnergia":
                            tipofalla = "Sin energía";
                            break;
                        case "ErrorSinEfectivo":
                            tipofalla = "Sin efectivo";
                            break;
                        case "ModoSupervisor":
                            tipofalla = "Modo Supervisor";
                            break;
                        case "FallaHardware":
                            tipofalla = "Falla hardware";
                            break;
                        case "ProblemaLocal":
                            tipofalla = "Problema local";
                            break;
                        case "TrxsNoMonetarias":
                            tipofalla = "trans. no monetarias";
                            break;
                        default:
                            tipofalla = "Sin identificar";
                            break;
                    }

                    if (cajeroBean.folio == "")
                    {
                        folio = "Sin folio";
                    }
                    else
                    {
                        folio = cajeroBean.folio;
                    }

                    //await context.PostAsync("Falla: " + tipofalla + ",  Folio: " + folio + ", Fecha: " + cajeroBean.fecha);
                    var menu = cajeroBean.empresa+"  "+cajeroBean.cajero + "   " + tipofalla + "   " + folio + "  " + cajeroBean.fecha;
                    opt.Add(menu);

                }

                await opcionesAcciones2(context, opt, titulo);

                //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
                //var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard2 = new ThumbnailCard
                {
                    //Text = "No se encontraron fallas en los cajeros del grupo "+Program.grupo
                    Text = "Espero que la información sea de su utilidad"

                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard2);
                await context.PostAsync(activity);


            }
            else
            {
                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard = new ThumbnailCard
                {
                    Text = "No se encontraron fallas en los cajeros del grupo " + Program.grupo
                    //Title = "No es posible identificar ese cajero",
                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard);
                await context.PostAsync(activity);

                //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar");
            }


            context.Wait(MessageReceivedAsync);
        }

        private async Task SolicitarFechaSolucion(IDialogContext context, string cajero, string fecha) {
            IConsultorDB bd = new CajeroDaoImpl();
            string texto = string.Empty;
            var tiempo = bd.obtenerPeriodoSolucion(cajero);

            if (tiempo != null && tiempo.Count > 0)
            {
                Tiempo cajeroBean = tiempo[0];
                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                string estimada = string.Empty;
                if (cajeroBean.fechaestimada.Equals(""))
                {
                    estimada = "Sin especificar";
                    
                }
                else {
                                       
                    switch (fecha)
                    {
                        case "hora":
                            texto = "hora";
                            string sean = cajeroBean.fechaestimada;
                            estimada = sean.Substring(sean.Length-8);
                            break;
                        case "fecha":
                            texto = "fecha";
                            estimada = cajeroBean.fechaestimada;
                            break;             
                        default:
                            texto = "fecha";
                            estimada = cajeroBean.fechaestimada;
                            break;
                    }
                    //estimada = cajeroBean.fechaestimada;
                }

                var menuHeroCard = new ThumbnailCard
                {
                    //Subtitle = cajeroBean.conteo + " falla(s)",
                    Title = "El cajero " + cajero.ToUpper() + " tiene "+texto+" posible de solucion: "+estimada,
                    Subtitle = " Responsable :"+cajeroBean.responsable                   
                    //Images = new List<CardImage> {                       
                     //   new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/solucion.jpg" }
                    //}
                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard);
                await context.PostAsync(activity);

                //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar");
                //var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard2 = new ThumbnailCard
                {
                    Text = "Algo más en que le podamos ayudar?",
                    Subtitle = "Espero que la información sea de su utilidad"

                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard2);
                await context.PostAsync(activity);

            }
            else
            {
                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard = new ThumbnailCard
                {
                    Text = "Algo más en que le podamos ayudar?",
                    //Subtitle = "Verifique e",
                    Title = "No es posible identificar ese cajero"
                    //Images = new List<CardImage> {                        
                    //    new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/error.jpg" }
                    //}
                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard);
                await context.PostAsync(activity);

                //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar");
            }


        }

        private async Task SolicitarResponsable(IDialogContext context, string cajero)
        {
            IConsultorDB bd = new CajeroDaoImpl();
            var responsable = bd.obtenerResponsable(cajero.ToUpper());
            if (responsable != null && responsable.Count > 0)
            {
                Cajero cajeroBean = responsable[0];
                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                string resp = string.Empty;
                if (cajeroBean.responsable.Equals(""))
                {
                    resp = "No hay datos del responsable";

                }
                else
                {
                    resp = cajeroBean.responsable;
                }

                var menuHeroCard = new ThumbnailCard
                {
                    //Subtitle = cajeroBean.conteo + " falla(s)",
                    //Title = "El responsable del cajero " + cajero.ToUpper() + " es: ",
                    Text = "El responsable del cajero " + cajero.ToUpper() + " es: "+resp
                    //Images = new List<CardImage> {
                    //    new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/solucion.jpg" }
                    //}
                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard);
                await context.PostAsync(activity);

               // await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
             
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard2 = new ThumbnailCard
                {
                    Text = "Algo más en que le podamos ayudar?",            
                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard2);
                await context.PostAsync(activity);


            }
            else {

                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard = new ThumbnailCard
                {
                    Text = "Algo más en que le podamos ayudar?",
                    //Subtitle = "Verifique e",
                    Subtitle = "No es posible identificar ese cajero"
                    //Images = new List<CardImage> {
                     //   new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/error.jpg" }
                    //}
                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard);
                await context.PostAsync(activity);

            }

        }

        private async Task SolicitarResponsableEmpresa(IDialogContext context, string empresa)
        {
            IConsultorDB bd = new CajeroDaoImpl();
            var responsable = bd.obtenerResponsableEmpresa(empresa.ToUpper());
            if (responsable != null && responsable.Count > 0)
            {
                Empresa cajeroBean = responsable[0];
                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                string resp = string.Empty;
                if (cajeroBean.responsable.Equals(""))
                {
                    resp = "No hay datos del responsable";

                }
                else
                {
                    resp = cajeroBean.responsable;
                }

                var menuHeroCard = new ThumbnailCard
                {
                    Text = "El responsable del cajero " + empresa.ToUpper() + " es: " + resp
                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard);
                await context.PostAsync(activity);

                //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard2 = new ThumbnailCard
                {
                    Text = "Algo más en que le podamos ayudar?",
                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard2);
                await context.PostAsync(activity);


            }
            else
            {

                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard = new ThumbnailCard
                {
                    Text = "Algo más en que le podamos ayudar?",
                    Subtitle = "No es posible identificar la empresa"
                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard);
                await context.PostAsync(activity);

            }

        }


        private async Task SolicitarResponsableGrupo(IDialogContext context, string grupo)
        {
            IConsultorDB bd = new CajeroDaoImpl();
            var responsable = bd.obtenerResponsableGrupo(grupo.ToUpper());
            if (responsable != null && responsable.Count > 0)
            {
                Grupo cajeroBean = responsable[0];
                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                string resp = string.Empty;
                if (cajeroBean.responsable.Equals(""))
                {
                    resp = "No hay datos del responsable";

                }
                else
                {
                    resp = cajeroBean.responsable;
                }

                var menuHeroCard = new ThumbnailCard
                {
                    Text = "El responsable del cajero " + grupo.ToUpper() + " es: " + resp
                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard);
                await context.PostAsync(activity);
                //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");         
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard2 = new ThumbnailCard
                {
                    Text = "Algo más en que le podamos ayudar?",
                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard2);
                await context.PostAsync(activity);
            }
            else
            {

                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard = new ThumbnailCard
                {
                    Text = "Algo más en que le podamos ayudar?",
                    Subtitle = "No es posible identificar el grupo"
                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard);
                await context.PostAsync(activity);

            }

        }

        private Attachment ShowOptions(List<Empresa> choices, string empresa)
        {
            int tam = empresa.Length;

            List<CardAction> messageOptions = new List<CardAction>();

            for (int i = 0; i < choices.Count; i++)
            {
                int tam2=choices[i].Empresas.Length;
                int tam3 = tam2 - tam;
                string emp=choices[i].Empresas.Substring(tam, tam3);
                messageOptions.Add(new CardAction
                {
                    Title = emp,
                    //Title = choices[i].Empresas,
                    //Text = choices[i].Mensaje
                });
            }

            //activity.Attachments.Add(
            var card = new HeroCard
            {
                Title = "Escribe el nombre empezando por ",
                Subtitle= empresa.ToUpper()+" ...",
                Buttons = messageOptions
            };
             
             //.ToAttachment()
             //   );
            return card.ToAttachment();
        }


        private Attachment ShowOptions2(List<string> choices)
        {

            List<CardAction> messageOptions = new List<CardAction>();

            for (int i = 0; i < choices.Count; i++)
            {
                /* messageOptions.Add(new CardAction
                 {
                     Title = choices[i],
                     Value = i

                 });*/
                CardAction plButton = new CardAction() {
                    Title = choices[i],
                    Value = choices[i],
                    Type = "postBack",
                };
                messageOptions.Add(plButton);
            }

            var card = new HeroCard
            {          
                Subtitle = "En este chat puedes preguntar sobre informacion de cajeros. ",
                Text ="Ejemplo de preguntas :",
                Buttons = messageOptions,
            };

            return card.ToAttachment();
        }
        private async Task ConnectOption(IDialogContext context, IAwaitable<Object> result) {
            var temp = await result;

            context.Done(new Object());
        }

    }
}