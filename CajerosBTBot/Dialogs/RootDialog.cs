using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using CajerosBTBot.implementaciones;
using CajerosBTBot.Enums;
using CajerosBTBot.Interfaces;
using CajerosBTBot.Bean;
using System.Collections.Generic;

namespace CajerosBTBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        

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
                Text = "Banca Transaccional",
                Title = "Banorte Cajeros",
                //Subtitle = "Opción",
                Images = new List<CardImage> {
                    new CardImage { Url = "http://www.steelsigns.com.mx/wp-content/uploads/2015/08/Banorte.png" }
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
                    //var cajero = objetoLuis.Entidades[0].entity;
                    cajero = objetoLuis.Entidades[0].entity;
                    await SolicitarEstatusCajero(context, cajero);
                    break;
                case Intensiones.SolicitarEstatusCajerosEmpresa:
                    empresa = objetoLuis.Entidades[0].entity;
                    await SolicitarEstatusCajerosEmpresa(context, empresa);
                    break;
                case Intensiones.SolicitarFallasCajerosEmpresa:
                    //var cajero = objetoLuis.Entidades[0].entity;
                    empresa = objetoLuis.Entidades[0].entity;
                    //await MostrarEjecutivo(context, cajero);
                    break;
                case Intensiones.SolicitarTipoFalla:
                    //var cajero = objetoLuis.Entidades[0].entity;
                    //await MostrarEjecutivo(context, cajero);
                    break;
                case Intensiones.SolicitarHistoricoFallasCajero:
                    cajero = objetoLuis.Entidades[0].entity;
                    //var cajero = objetoLuis.Entidades[0].entity;
                    //await MostrarEjecutivo(context, cajero);
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
                        ContentUrl = "http://cedecpymes.org/wp-content/uploads/2016/04/contento.jpg"
                    }
                    );
                    respuestaParaUsuario = "¡Excelente me da gusto saber eso!";
                    break;
                case Enums.EstadoDeAnimo.Regular:
                    activity.Attachments.Add(new Attachment
                    {
                        ContentType = "image/jpg",
                        ContentUrl = "https://www.reasonwhy.es/sites/default/files/styles/noticia_principal/public/informe-optimismo-empresarial-espanol-reasonwhy.es_.jpg?itok=nNzRVzDr"
                    }
                    );
                    respuestaParaUsuario = "No estoy seguro como se siente, pero confio que estará bien";
                    break;
                default:
                case Enums.EstadoDeAnimo.Malo:
                    activity.Attachments.Add(new Attachment
                    {
                        ContentType = "image/jpg",
                        ContentUrl = "http://piweek.es/wp-content/uploads/2016/04/ejecutivo-triste.jpg"
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
                        Subtitle = cajeroBean.cajero,
                        Title = "El cajero " + cajero.ToUpper() + " tiene: ",
                        Images = new List<CardImage> {
                        new CardImage { Url = "http://nuevotiempo.org/radio/files/personas-con-signo-de-interrogacion-en-la-cara.jpg" }
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
                        Subtitle = "Verifique el número de cajero",
                        Title = "No se identifico el cajero como parte de banca transaccional",
                        Images = new List<CardImage> {
                        new CardImage { Url = "https://vignette2.wikia.nocookie.net/ageofempires/images/4/41/Signo_de_exclamaci%C3%B3n.png/revision/latest?cb=20120104172733&path-prefix=es" }
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

                activity.Text = "El cajero no tiene fallas";

                await context.PostAsync(activity);

                await context.PostAsync("Espero que la información haya sido de utilidad. Algo más en que le podamos ayudar");
            }
            context.Wait(MessageReceivedAsync);
        }


        private async Task SolicitarEstatusCajerosEmpresa(IDialogContext context, string empresa)
        {
            IConsultorDB bd = new CajeroDaoImpl();


            context.Wait(MessageReceivedAsync);
        }
    }
}