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
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
 


    //namespace CajerosBTBot.Dialogs
    //{
    [Serializable]
    public class RootDialog : IDialog<object>
    {

        List<string> lista;
        private const string CajeroOption = "cajero";
        private const string EmpresaOption = "empresa";
        private const string GrupoOption = "grupo";

        private const string SiOption = "si";
        private const string NoOption = "no";

        public static class Program {
            public static string cajero;
            public static string empresa;
            public static string grupo;
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
            //PromptDialog.Choice(context, this.OnOptionSelected, new List<String> { CajeroOption, EmpresaOption, GrupoOption }, "Bienvenido. Quieres consultar por?", "Opcion no valida", 4, PromptStyle.Auto);
            var mensaje = await result as Activity;
               var textoDelUsuario = mensaje.Text;

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
                    //await ManejarSaludo(context);
                    if (!Program.grupo.Equals(""))
                    {
                        
                        await SolicitarEstatusCajerosEmpresa(context, Program.grupo);
                    }
                    else {
                        await SolicitarEstatusCajerosGrupo(context, Program.empresa);
                    }
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
                           Program.empresa= objetoLuis.Entidades[1].entity;
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
                       tiempo= objetoLuis.Entidades[0].entity;
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

        private async Task ManejarSaludo(IDialogContext context)
        {
           
            ManejarAyuda(context);
            //PromptDialog.Text(context, RecibirEstadoUsuario, "¿Como se encuentra el día de hoy?");
        }


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
        private async Task MostrarAyuda(IDialogContext context)
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
            PromptDialog.Choice(context, this.OnOptionSelected, new List<String> { CajeroOption, EmpresaOption, GrupoOption }, "Quieres consultar fallas por?", "Opcion no valida", 4,PromptStyle.Auto);

        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result) {
            try {
                string optionSelected = await result;
                switch (optionSelected) {
                    case CajeroOption:
                        await opcionesAcciones(context, CajeroOption);                    
                        //await context.PostAsync($"Que deseas consultar del {CajeroOption}?..");
                        break;
                    case EmpresaOption:
                        await opcionesAcciones(context, EmpresaOption);
                        //await context.PostAsync($"Que deseas consultar de la {EmpresaOption}?..");
                        break;
                    case GrupoOption:
                        await opcionesAcciones(context, GrupoOption);
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
            MostrarAyuda(context); 
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
                                                              Text = "Qué opción deseas consultar?.."
                                                          },
                                                          new TextBlock(){
                                                              Text ="-Estatus "
                                                          },
                                                       //   new TextBlock(){
                                                       //       Text ="Responsable "
                                                       //   },
                                                          new TextBlock(){
                                                              Text = "-Historico "
                                                          },
                                                          new TextBlock(){
                                                              Text = $"Escribe la opcion + '{opcion}' + nombre {opcion}"
                                                          }
                                                      }
                                                  }

                                              }
                                          }
                                      }
                                }
                            },

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

        private IEnumerable<MenuCajeros> GetOpciones(List<object> opcion)
        {
            var opciones = new List<MenuCajeros>();
            foreach (string element in opcion) {
                MenuCajeros menu = new MenuCajeros() {opcion=element};
                opciones.Add(menu);
            }            
            return opciones;
        }

        private Column AsHotelItem(MenuCajeros menu)
        {
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
                    List<object> opt = new List<object>();
                    var titulo = Program.cajero + " tiene: " + caj.Count + " falla(s)";
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
                        var menu = tipofalla2 + ", " + element.fecha+", "+element.folio;
                        opt.Add(menu);
                    }

                    await opcionesAcciones2(context, opt, titulo);
                    //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
                    //var activity = context.MakeMessage();
                    activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    var menuHeroCard = new ThumbnailCard
                    {
                        Text = "Algo más en que le podamos ayudar?",
                        Subtitle = "Espero que la información haya sido de su utilidad"

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
                        Subtitle = "El cajero " + cajero.ToUpper()+" no tiene fallas",
                        //Subtitle = "Verifique el número de cajero",
                        //Title = "No existen fallas en el cajero " + cajero.ToUpper() + "o no pertenece a banca transaccional",
                        //Images = new List<CardImage> {
                        //new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/error.jpg" }
                        //}
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
                    Text = "Algo más en que le podamos ayudar?",
                    //Subtitle = "Verifique el número de cajero",
                    //Title = "No existen fallas en el cajero "+ cajero.ToUpper()+" o no pertenece a banca transaccional",
                    Subtitle = "El cajero " + cajero.ToUpper()+" no tiene fallas" 
                    //Images = new List<CardImage> {
                    //    new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/error.jpg" }
                    //}
                }.ToAttachment();
                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard);

                //activity.Text = "No se encontraron fallas en el cajero " + cajero.ToUpper();

                await context.PostAsync(activity);

                //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
            }
            context.Wait(MessageReceivedAsync);
        }


        private async Task SolicitarEstatusCajerosEmpresa(IDialogContext context, string empresa)
        {
            IConsultorDB bd = new CajeroDaoImpl();
            var obtienemepresa = bd.ObtenerEmpresas(empresa);
            //string cadena = String.Empty;
            StringBuilder sb = new StringBuilder();
            if (obtienemepresa.Count > 1)
            {
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
                var titulo = "Hay mas de una empresa con "+empresa.ToUpper();
                foreach (Empresa element in obtienemepresa)
                {
                    Int32 tam = empresa.Length;
                    string nombre = null;
                    if (tam >= 40) { nombre = element.empresa.Substring(0, tam - 15); }
                    else if (tam >= 30 && tam < 40) { nombre = element.empresa.Substring(0, tam - 8); } else { nombre = element.empresa; }
                    //var menu = element.empresa.Substring(tam);
                    var menu = nombre;
                    opt.Add(menu);
                }
                await opcionesAcciones2(context, opt, titulo);

                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard = new ThumbnailCard
                {
                    Text = "Escriba  la opcion  + 'empresa' + nombre de la empresa que desea consultar ",
                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard);
                await context.PostAsync(activity);
            }//fin del if cuando se encuentra mas de una empresa
            else
            {
                Program.empresa = obtienemepresa[0].empresa;              
                var conteo = bd.obtenerConteoCajerosEmpresa(Program.empresa.ToUpper());
                if (conteo >= 1) { 
                        var estatus = bd.obtenerEstatusCajerosEmpresa(Program.empresa.ToUpper());
                        if (estatus.Equals(true))
                        {
                            var empresas = bd.ObtenerFallasEmpresa(Program.empresa.ToUpper());
                            if (empresas != null && empresas.Count > 0)
                            {                               
                                var activity = context.MakeMessage();
                            /* activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                             var menuHeroCard = new ThumbnailCard
                             {
                                 //Subtitle = cajeroBean.conteo + " falla(s)",
                                 Title = "La empresa " + empresa.ToUpper() + " tiene las siguientes fallas: ",
                                 //Images = new List<CardImage> {
                                 //new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/cajeroerror.jpg" }
                                 //}
                             }.ToAttachment();

                             activity.Attachments = new List<Attachment>();
                             activity.Attachments.Add(menuHeroCard);

                             await context.PostAsync(activity);

                             for (int i = 0; i < empresas.Count; i++)
                             {
                                 Empresa cajeroBean = empresas[i];
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

                                 await context.PostAsync("Cajero: " + cajeroBean.cajero + ",  Falla: " + tipofalla + ",  Folio: " + folio);
                             }*/

                            //Program.empresa = empresas[0].empresa;
                            List<object> opt = new List<object>();
                            Int32 tam = Program.empresa.Length;
                            string nombre = null;
                            if (tam > 40) { nombre = Program.empresa.Substring(0, tam - 15); }
                            else if (tam >= 30 && tam < 40) { nombre = Program.empresa.Substring(0, tam - 8); } else { nombre = Program.empresa; }
                            

                                var titulo = nombre +" tiene fallas: ";
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
                else {
                    var activity = context.MakeMessage();
                    activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    var menuHeroCard = new ThumbnailCard
                    {
                        Text = "Si desea que la busque dentro de grupo escriba si",
                        Subtitle = "La empresa tal como la ingreso no se encontro",
                    }.ToAttachment();

                    activity.Attachments = new List<Attachment>();
                    activity.Attachments.Add(menuHeroCard);                    
                    await context.PostAsync(activity);
                    await context.PostAsync(Program.empresa);

                    //await SolicitarEstatusCajerosGrupo(context, Program.empresa);

                    //PromptDialog.Choice(context, this.OnOptionSelected2, new List<String> { SiOption, NoOption}, "Quieres buscarlo por grupo?", "Opcion no valida", 3, PromptStyle.Auto);
                    //var dialog = new PromptDialog.PromptString("Quieres buscar "+Program.empresa+"por grupo", "Por favor confirme",2);
                    //context.Call(dialog, OnOptionSelected2);
                   
                }
            }
            context.Wait(MessageReceivedAsync);
        }


        private async Task SolicitarEstatusCajerosGrupo(IDialogContext context, string grupo) {
            IConsultorDB bd = new CajeroDaoImpl();
            var obtienemepresa = bd.ObtenerGrupos(grupo);
            string cadena = String.Empty;
            StringBuilder sb = new StringBuilder();
            if (obtienemepresa.Count > 1)
            {
                //List<Empresa> choices = new List<Empresa>();
                /*for (int i = 0; i < obtienemepresa.Count; i++)
                {

                    choices.Add(new Empresa(obtienemepresa[i].grupo, obtienemepresa[i].id_grupo));
                }
                var result = ShowOptions(choices, grupo);
                var activity = context.MakeMessage();
                activity.Text = "Se encontro mas de un grupo. Escriba cual desea consultar ";
                activity.Attachments.Add(result);
                await context.PostAsync(activity);*/
                List<object> opt = new List<object>();
                var titulo = "Hay mas de un grupo con  "+grupo.ToUpper();
                foreach (Grupo element in obtienemepresa)
                {
                    Int32 tam = grupo.Length;
                    string nombre = null;
                    if (tam >= 40) { nombre = element.grupo.Substring(0, tam - 15); }
                    else if (tam >= 30 && tam < 40) { nombre = element.grupo.Substring(0, tam - 8); } else { nombre = element.grupo; }
                    var menu = nombre;
                    //var menu = element.grupo;
                    opt.Add(menu);
                }
                await opcionesAcciones2(context, opt, titulo);

                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard = new ThumbnailCard
                {
                    Text = "Escriba la opcion + 'grupo' + nombre del grupo que desea consultar ",
                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard);
                await context.PostAsync(activity);

            }
            else
            {
                if (obtienemepresa.Count == 1) {
                    Program.grupo = obtienemepresa[0].grupo;
                var estatus = bd.obtenerEstatusCajerosGrupo(Program.grupo.ToUpper());
                    if (estatus.Equals(true))
                    {
                        var empresas = bd.ObtenerFallasGrupo(Program.grupo.ToUpper());
                        if (empresas != null && empresas.Count > 0)
                        {

                            var activity = context.MakeMessage();
                            /*  activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                              var menuHeroCard = new ThumbnailCard
                              {
                                  //Subtitle = cajeroBean.conteo + " falla(s)",
                                  Subtitle = "El grupo " + grupo.ToUpper() + " tiene las siguientes fallas: ",
                                  //Images = new List<CardImage> {
                                  //new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/cajeroerror.jpg" }
                                  //}
                              }.ToAttachment();

                              activity.Attachments = new List<Attachment>();
                              activity.Attachments.Add(menuHeroCard);

                              await context.PostAsync(activity);

                              for (int i = 0; i < empresas.Count; i++)
                              {
                                  Grupo cajeroBean = empresas[i];
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

                                  await context.PostAsync("Cajero: " + cajeroBean.cajero + ",  empresa: " + cajeroBean.empresa + ",  Falla: " + tipofalla + ",  Folio: " + folio);


                              }

                              await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");*/
                           // Program.grupo = empresas[0].grupo;
                            List<object> opt = new List<object>();

                            Int32 tam = Program.grupo.Length;
                            string nombre = null;
                            if (tam >= 40) { nombre = Program.grupo.Substring(0, tam - 15); }
                            else if (tam >= 30 && tam < 40) { nombre = Program.grupo.Substring(0, tam - 8); } else { nombre = Program.grupo; }

                            var titulo = "Fallas en el grupo "+nombre;
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
                                var menu = nombre2.ToLower() + "," + element.cajero + "," + tipofalla2;
                                opt.Add(menu);
                            }

                            await opcionesAcciones2(context, opt, titulo);

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
                                //Images = new List<CardImage> {
                                //new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/error.jpg" }
                                //}
                            }.ToAttachment();

                            activity.Attachments = new List<Attachment>();
                            activity.Attachments.Add(menuHeroCard);
                            await context.PostAsync(activity);
                        }
                    }
                    else
                    {

                        var activity = context.MakeMessage();

                        /*activity.Text = "No se encontraron fallas en los cajeros del grupo " + grupo.ToUpper();
                        await context.PostAsync(activity);
                        await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");*/
                        activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                        var menuHeroCard = new ThumbnailCard
                        {
                            Text = "Algo más en que le podamos ayudar?",
                            Subtitle = "No se encontraron fallas en los cajeros del grupo " + grupo.ToUpper(),
                        }.ToAttachment();

                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard);
                        await context.PostAsync(activity);
                    }
                }else{
                    var activity = context.MakeMessage();
                     activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                     var menuHeroCard = new ThumbnailCard
                     {
                         Text = "Si desea buscarlo dentro de empresa escriba si",
                         Subtitle = "El grupo tal como lo ingreso no se encontro",
                     }.ToAttachment();

                     activity.Attachments = new List<Attachment>();
                     activity.Attachments.Add(menuHeroCard);
                    // await SolicitarEstatusCajerosEmpresa(context, Program.grupo);
                    
                    await context.PostAsync(activity);

                    //PromptDialog.Confirm(context, this.OnOptionSelected3, "Quieres buscarlo por empresa?");                    
                    //PromptDialog.Choice(context, this.OnOptionSelected3, new List<String> { SiOption, NoOption }, "Quieres buscarlo por empresa?", "Opcion no valida", 3, PromptStyle.Auto);
                    //var dialog = new PromptDialog.PromptString("Quieres buscar " + Program.empresa + "por grupo", "Por favor confirme", 2);
                    //context.Call(dialog, OnOptionSelected3);

                }
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
                var titulo = "Historico fallas cajero " + Program.cajero.ToUpper();

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

                    var menu = tipofalla + "   " + folio+"  "+cajeroBean.fecha;
                    opt.Add(menu);
                    //await context.PostAsync("Falla: " + tipofalla + ",  Folio: " + folio+", Fecha: "+cajeroBean.fecha);

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
                    Text = "Algo más en que le podamos ayudar?",
                    //Subtitle = "Verifique e",
                    Subtitle = "No es posible identificar ese cajero",
                  //  Images = new List<CardImage> {
                  //      new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/error.jpg" }
                  //  }
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
                var titulo = "Historico fallas cajeros de " + Program.empresa.ToUpper();

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
                var titulo = "Historico fallas cajeros de " + Program.empresa.ToUpper();

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