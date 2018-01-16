﻿//namespace BotBuilder.Samples.AdaptiveCards
namespace CajerosBTBot.Dialogs
{ 
using System;
using System.Threading;
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
        private const string CajeroOption = "Cajero";
        private const string EmpresaOption = "Empresa";
        private const string GrupoOption = "Grupo";

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

                        await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");


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
                        await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
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

                        await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");


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
                           //Subtitle = "Debes especificar un cajero o nombre de la empresa",
                           //Title = "No entendi la solicitud ",
                           Subtitle = "No entendi la solicitud",
                           Text = "Utiliza la palabra 'cajero' o 'empresa' o 'grupo' dentro de la solicitud"
                          // Images = new List<CardImage> {
                          // new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/confusion.jpg" }
                          // }
                       }.ToAttachment();

                       activity2.Attachments = new List<Attachment>();
                       activity2.Attachments.Add(menuHeroCard6);
                       await context.PostAsync(activity2);
                       break;
                   case Intensiones.SolicitarEstatusCajero:
                       if (objetoLuis.Entidades[0].entity.Equals("desconocido"))
                       {                           
                           var activity = context.MakeMessage();
                           activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                           var menuHeroCard = new ThumbnailCard
                           {
                               //Subtitle = "Debes especificar un cajero o nombre de la empresa",
                               //Title = "No entendi la solicitud ",
                               Subtitle = "No entendi la solicitud",
                               Text = "Utiliza la palabra 'cajero' o 'empresa' o 'grupo' dentro de la solicitud"
                               //Images = new List<CardImage> {
                               //new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/confusion.jpg" }
                               //}   
                           }.ToAttachment();

                           activity.Attachments = new List<Attachment>();
                           activity.Attachments.Add(menuHeroCard);
                           await context.PostAsync(activity);
                       }
                       else
                       {
                           Program.cajero = objetoLuis.Entidades[0].entity;
                           await SolicitarEstatusCajero(context, Program.cajero);
                       }
                       break;
                   case Intensiones.SolicitarEstatusCajerosEmpresa:
                       if (objetoLuis.Entidades[0].entity.Equals("desconocido"))
                       {
                           //await context.PostAsync("No entendí la solicitud. Utiliza la palabra cajero o empresa dentro de la solicitud");                       
                           var activity = context.MakeMessage();
                           activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                           var menuHeroCard = new ThumbnailCard
                           {
                               //Subtitle = "Debes especificar un cajero o nombre de la empresa",
                               Subtitle = "No entendi la solicitud ",
                               Text = "Utiliza la palabra cajero o empresa dentro de la solicitud"
                            //   Images = new List<CardImage> {
                           //new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/confusion.jpg" }
                           //}
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
                case Intensiones.SolicitarEstatusCajerosGrupo:         
                        Program.grupo = objetoLuis.Entidades[0].entity;
                        await SolicitarEstatusCajerosGrupo(context, Program.grupo);                    
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
                   case Intensiones.SolicitarHistoricoFallasCajeros:
                       tiempo = objetoLuis.Entidades[0].entity;
                       if (objetoLuis.Entidades[0].type.Equals("cajero")) {
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
                   case Intensiones.solicitarFechaSolucion:
                       tiempo= objetoLuis.Entidades[0].entity;
                       if (objetoLuis.Entidades.Count > 1) {
                           Program.cajero = objetoLuis.Entidades[1].entity;
                       }                
                       await SolicitarFechaSolucion(context, Program.cajero, tiempo);
                       break;
                   case Intensiones.SolicitarResponsableCajero:
                       Program.cajero = objetoLuis.Entidades[0].entity;                    
                       await SolicitarResponsable(context, Program.cajero);
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

            PromptDialog.Choice(context, this.OnOptionSelected, new List<String> { CajeroOption, EmpresaOption, GrupoOption }, "Bienvenido. Quieres consultar por?", "Opcion no valida", 4);

        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result) {
            try {
                string optionSelected = await result;
                switch (optionSelected) {
                    case CajeroOption:                      
                        await context.PostAsync($"Que deseas consultar del {CajeroOption}?..");
                        await context.PostAsync($"Escribe 'estatus' o 'responsable' o 'historico' + 'cajero' + el id el {CajeroOption}");
                        context.Wait(MessageReceivedAsync);
                        break;
                    case EmpresaOption:
                        await context.PostAsync($"Escribe 'estatus' o 'responsable' o 'historico' + 'empresa' + el nombre de la {EmpresaOption} ...");
                        context.Wait(MessageReceivedAsync);
                        break;
                    case GrupoOption:
                        await context.PostAsync($"Escribe 'estatus' o 'responsable' o 'historico' + 'grupo' + el nombre del  {GrupoOption} ...");
                        context.Wait(MessageReceivedAsync);
                        break;

                }

            }
            catch (TooManyAttemptsException) {
                await context.PostAsync($"Muchas peticiones ");
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ManejarAyuda(IDialogContext context)
        {

            //await context.PostAsync("Has solicitado Ayuda ");
            MostrarAyuda(context); 
            //PromptDialog.Text(context, RecibirEstadoUsuario, "¿Como se encuentra el día de hoy?");
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

                var cajeros = bd.ObtenerFallaCajero(cajero.ToUpper());
                if (cajeros != null && cajeros.Count > 0)
                {
                    Cajero cajeroBean = cajeros[0];
                    var activity = context.MakeMessage();
                    activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
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
                    await context.PostAsync(activity);

                    var tipofalla = String.Empty;
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
                    }

                    await context.PostAsync("Falla: " + tipofalla + ",    Fecha: " + cajeroBean.fecha);

                    await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
                }
                else
                {
                    var activity = context.MakeMessage();
                    activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    var menuHeroCard = new ThumbnailCard
                    {
                        Text = "Algo más en que le podamos ayudar?",
                        Subtitle = "No existen fallas en el cajero " + cajero.ToUpper(),
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
                    Subtitle = "No existen fallas en el cajero " + cajero.ToUpper() 
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
            //string[] valores1;
            string cadena = String.Empty;
            StringBuilder sb = new StringBuilder();
            if (obtienemepresa.Count > 1)
            {
                List<Empresa> choices = new List<Empresa>();
                // valores1 = new string[obtienemepresa.Count];
                for (int i = 0; i < obtienemepresa.Count; i++)
                {
                //  valores1[i] = obtienemepresa[i].empresa;
                    choices.Add(new Empresa(obtienemepresa[i].empresa, obtienemepresa[i].id_empresa));
                 }


                //List<string> lista = new List<string>();
                //for (int i = 0; i < obtienemepresa.Count; i++)
                //{
                //lista.Add(valores1[i]);
                //}

                var result = ShowOptions(choices, empresa);
                var activity = context.MakeMessage();
                activity.Text = "Se encontro mas de una empresa. Escriba cual desea consultar ";
                activity.Attachments.Add(result);
                await context.PostAsync(activity);
                //context.Wait(ConnectOption);

                //activity.Attachments = new List<Attachment>();              
                /*List<CardAction> messageOptions = new List<CardAction>();
          

                for (int i =0; i< obtienemepresa.Count; i++ ) {
                    messageOptions.Add(new CardAction
                    {
                        Title= obtienemepresa[i].empresa,
                        Text= obtienemepresa[i].id_empresa
                    });
                }

                activity.Attachments.Add(
                new HeroCard
                {
                    Title = "¿A cual empresa te refieres?",
                    Buttons = messageOptions
                }.ToAttachment()
                    );*/

                //foreach (Empresa choice in choices) {
                //}
                //activity.Attachments.Add(messageOptions);
               
                //await context.PostAsync(activity);

            }
            else
            {

                var estatus = bd.obtenerEstatusCajerosEmpresa(empresa.ToUpper());
                if (estatus.Equals(true))
                {
                    var empresas = bd.ObtenerFallasEmpresa(empresa.ToUpper());
                    if (empresas != null && empresas.Count > 0)
                    {
                        //Empresa cajeroBean = empresas[0];

                        var activity = context.MakeMessage();
                        activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
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


                        }

                        await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
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
                            Images = new List<CardImage> {
                        new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/error.jpg" }
                    }
                        }.ToAttachment();

                        activity.Attachments = new List<Attachment>();
                        activity.Attachments.Add(menuHeroCard);
                        await context.PostAsync(activity);
                    }
                }
                else
                {

                    var activity = context.MakeMessage();

                    activity.Text = "No se encontraron fallas en los cajeros de la empresa " + empresa.ToUpper();

                    await context.PostAsync(activity);

                    await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
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
                List<Empresa> choices = new List<Empresa>();
                for (int i = 0; i < obtienemepresa.Count; i++)
                {

                    choices.Add(new Empresa(obtienemepresa[i].grupo, obtienemepresa[i].id_grupo));
                }
                var result = ShowOptions(choices, grupo);
                var activity = context.MakeMessage();
                activity.Text = "Se encontro mas de un grupo. Escriba cual desea consultar ";
                activity.Attachments.Add(result);
                await context.PostAsync(activity);
            }
            else {
                var estatus = bd.obtenerEstatusCajerosGrupo(grupo.ToUpper());
                if (estatus.Equals(true))
                {
                    var empresas = bd.ObtenerFallasGrupo(grupo.ToUpper());
                    if (empresas != null && empresas.Count > 0)
                    {
    
                        var activity = context.MakeMessage();
                        activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
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

                        await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
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

                    activity.Text = "No se encontraron fallas en los cajeros del grupo " + grupo.ToUpper();

                    await context.PostAsync(activity);

                    await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
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
                //Cajero cajeroBean = historico[0];

                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard = new ThumbnailCard
                {
                    //Subtitle = cajeroBean.conteo + " falla(s)",
                    Title = "El cajero " + cajero.ToUpper() + " tiene las siguientes fallas: "
                    //Images = new List<CardImage> {
                    //    new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/cajeroerror.jpg" }
                    //}
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

                    await context.PostAsync("Falla: " + tipofalla + ",  Folio: " + folio+", Fecha: "+cajeroBean.fecha);

                }



                await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?"); 
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
    


        private async Task SolicitarHistoricoCajeroEmpresa(IDialogContext context, string tiempo, string empresa)
        {
            IConsultorDB bd = new CajeroDaoImpl();
            string periodo = string.Empty;

            var historico = bd.obtenerHistoricoCajeroEmpresa(empresa.ToUpper(), periodo);
            if (historico != null && historico.Count > 0)
            {
                //Cajero cajeroBean = historico[0];

                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
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


                await context.PostAsync(activity);

                for (int i = 0; i < historico.Count; i++)
                {
                    Empresa cajeroBean = historico[i];

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



                await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");


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

                await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar");

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
                    Title = "El responsable del cajero " + cajero.ToUpper() + " es: ",
                    Subtitle = resp
                    //Images = new List<CardImage> {
                    //    new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/solucion.jpg" }
                    //}
                }.ToAttachment();

                activity.Attachments = new List<Attachment>();
                activity.Attachments.Add(menuHeroCard);
                await context.PostAsync(activity);

                await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");


            }
            else {

                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard = new ThumbnailCard
                {
                    Text = "Algo más en que le podamos ayudar?",
                    //Subtitle = "Verifique e",
                    Title = "No es posible identificar ese cajero"
                    //Images = new List<CardImage> {
                     //   new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/error.jpg" }
                    //}
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