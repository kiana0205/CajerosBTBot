using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using CajerosBTBot.implementaciones;
using CajerosBTBot.Enums;
using CajerosBTBot.Interfaces;
using CajerosBTBot.Bean;
using System.Collections.Generic;
using System.Text;


namespace CajerosBTBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        List<string> lista;

        public static class Program {
            public static string cajero;
        }
        //Hola Diana :)
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async void MostrarBienvenida(IDialogContext context)
        {
            var activity = context.MakeMessage();
            //Muestra el canal (facebook, skype, etc);
            //var canal = activity.ChannelId;
            activity.Text = "Bienvenido ";
            activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            var menuHeroCard = new HeroCard
            {
                Text = "Disponibilidad",
                Title = "Cajeros Banca Transaccional",
                //Subtitle = "Opción",
                Images = new List<CardImage> {
                    new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/Banorte.png" }
                }
            }.ToAttachment();

            activity.Attachments = new List<Attachment>();
            activity.Attachments.Add(menuHeroCard);

            await context.PostAsync(activity);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var mensaje = await result as Activity;
            var textoDelUsuario = mensaje.Text;

            var consultor = new ConsultorLuis();
            var objetoLuis = await consultor.ConsultarLuis(textoDelUsuario);

            var intension = consultor.ConvertirObjetoLuisAIntencion(objetoLuis);

            var empresa = String.Empty;
            var cajero = String.Empty;
            var tiempo = String.Empty;

           /* if (objetoLuis.Entidades[0].type.Equals("cajero") && objetoLuis.TopScoringIntent.intent.Equals("SolicitarEstatusCajero"))
            {
                Program.cajero = objetoLuis.Entidades[0].entity;
            }*/



            switch (intension)
            {
                case Intensiones.Saludo:
                    await ManejarSaludo(context);
                    //context.Wait(MessageReceivedAsync);
                    break;
                case Intensiones.None:
                    await context.PostAsync("No entendí la solicitud");
                    context.Wait(MessageReceivedAsync);
                    break;
                case Intensiones.SolicitarEstatusCajero:
                    Program.cajero = objetoLuis.Entidades[0].entity;
                    await SolicitarEstatusCajero(context, Program.cajero);
                    break;
                case Intensiones.SolicitarEstatusCajerosEmpresa:
                    empresa = objetoLuis.Entidades[0].entity;
                    await SolicitarEstatusCajerosEmpresa(context, empresa);
                    break;
                case Intensiones.SolicitarHistoricoFallasCajerosEmpresa:
                    tiempo = objetoLuis.Entidades[0].entity;
                    empresa = objetoLuis.Entidades[1].entity;
                    await SolicitarHistoricoCajeroEmpresa(context, tiempo, empresa);
                    break;
                case Intensiones.SolicitarHistoricoFallasCajeros:
                    tiempo = objetoLuis.Entidades[0].entity;
                    if (objetoLuis.Entidades[0].type.Equals("cajero")) {
                       // var activity = context.MakeMessage();

                       // activity.Text = "No entiendo la solicitud. Vuelva a intentar ";

                       // await context.PostAsync(activity);

                       // await context.PostAsync("En que le podamos ayudar?");
                    }
                    else
                    {
                        Program.cajero = objetoLuis.Entidades[1].entity;
                        await SolicitarHistoricoCajero(context, tiempo, Program.cajero);

                    }
                    //cajero = objetoLuis.Entidades[1].entity;
                    //await SolicitarHistoricoCajero(context, tiempo, cajero);
                    break;
                case Intensiones.solicitarFechaSolucion:
                    Program.cajero = objetoLuis.Entidades[0].entity;
                    await SolicitarFechaSolucion(context, Program.cajero);
                    break;
                default:
                    await context.PostAsync(intension.ToString());
                    context.Wait(MessageReceivedAsync);
                    break;
            }
        }

        private async Task ManejarSaludo(IDialogContext context)
        {
            await context.PostAsync("Hola");
            MostrarBienvenida(context);

            PromptDialog.Text(context, RecibirEstadoUsuario, "¿Como se encuentra el día de hoy?");
        }

        private async Task RecibirEstadoUsuario(IDialogContext context, IAwaitable<string> estadoUsuarioAwaitable)
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
        }


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
                    var menuHeroCard = new HeroCard
                    {
                        //Subtitle = cajeroBean.conteo + " falla(s)",
                        Title = "El cajero " + cajero.ToUpper() + " tiene: ",
                        Text = cajeroBean.conteo + " falla(s)",
                        Images = new List<CardImage> {
                        new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/cajeroerror.jpg" }
                    }
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
                    var menuHeroCard = new HeroCard
                    {
                        Text = "Algo más en que le podamos ayudar",
                        Subtitle = "Verifique el número de cajero",
                        Title = "No se identifico el cajero como parte de banca transaccional",
                        Images = new List<CardImage> {
                        new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/error.jpg" }
                    }
                    }.ToAttachment();

                    activity.Attachments = new List<Attachment>();
                    activity.Attachments.Add(menuHeroCard);
                    await context.PostAsync(activity);

                    //await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar");
                }
            }
            else {

                var activity = context.MakeMessage();

                activity.Text = "No se encontraron fallas en el cajero " + cajero.ToUpper();

                await context.PostAsync(activity);

                await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?");
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

                var result = ShowOptions(choices);
                var activity = context.MakeMessage();
                activity.Text = "Se encontro mas de una empresa con ese nombre ";
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
                        var menuHeroCard = new HeroCard
                        {
                            //Subtitle = cajeroBean.conteo + " falla(s)",
                            Title = "La empresa " + empresa.ToUpper() + " tiene las siguientes fallas: ",
                            Images = new List<CardImage> {
                        new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/cajeroerror.jpg" }
                    }
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
                        var menuHeroCard = new HeroCard
                        {
                            Text = "Algo más en que le podamos ayudar",
                            Subtitle = "Verifique el número de cajero",
                            Title = "No se identifico el cajero como parte de banca transaccional",
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


        private async Task SolicitarHistoricoCajero(IDialogContext context, String tiempo, string cajero) {
            IConsultorDB bd = new CajeroDaoImpl();

            string periodo = string.Empty;
            if (tiempo.Contains("ultimos"))
            {
                periodo = "TOP 5";
            }
            else if (tiempo.Contains("historico"))
            {
                periodo = "*";
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
                var menuHeroCard = new HeroCard
                {
                    //Subtitle = cajeroBean.conteo + " falla(s)",
                    Title = "El cajero " + cajero.ToUpper() + " tiene ha tenido las siguientes fallas: ",
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

                    await context.PostAsync("Falla: " + tipofalla + ",  Folio: " + folio+", Fecha: "+cajeroBean.fecha);

                }



                await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar?"); 
            }
            else
            {
                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard = new HeroCard
                {
                    Text = "Algo más en que le podamos ayudar",
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


            }
            else
            {
                var activity = context.MakeMessage();
                activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                var menuHeroCard = new HeroCard
                {
                    Text = "Algo más en que le podamos ayudar",
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

        private async Task SolicitarFechaSolucion(IDialogContext context, string cajero) {
            IConsultorDB bd = new CajeroDaoImpl();
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
                    estimada = cajeroBean.fechaestimada;
                }

                var menuHeroCard = new HeroCard
                {
                    //Subtitle = cajeroBean.conteo + " falla(s)",
                    Title = "El cajero " + cajero.ToUpper() + " tiene fecha posible de solucion: "+estimada,
                    Text = " Responsable :"+cajeroBean.responsable,                    
                    Images = new List<CardImage> {                       
                        new CardImage { Url = "https://storageserviciobt.blob.core.windows.net/imagebot/solucion.jpg" }
                    }
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
                var menuHeroCard = new HeroCard
                {
                    Text = "Algo más en que le podamos ayudar",
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


        }

        private Attachment ShowOptions(List<Empresa> choices)
        {

            List<CardAction> messageOptions = new List<CardAction>();

            for (int i = 0; i < choices.Count; i++)
            {
                messageOptions.Add(new CardAction
                {
                    Title = choices[i].Empresas,
                    Text = choices[i].Mensaje
                });
            }

            //activity.Attachments.Add(
            var card = new HeroCard
            {
                Title = "¿Escribe a cual empresa te refieres?",
                Buttons = messageOptions
            };
             
             //.ToAttachment()
             //   );
            return card.ToAttachment();
        }
        private async Task ConnectOption(IDialogContext context, IAwaitable<Object> result) {
            var temp = await result;

            context.Done(new Object());
        }

    }
}