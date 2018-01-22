using CajerosBTBot.Bean;
using CajerosBTBot.Enums;
using CajerosBTBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Globalization;


namespace CajerosBTBot.implementaciones
{
    public class ConsultorLuis : IConsultorCognitive
    {

        public static class Program
        {
            public static string cajero;
            public static string empresa;
            public static string grupo;
        }

        private const string UrlServicioLuis = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/ea11a124-10ea-4977-bb9a-b76bdc58ddb2?subscription-key=22cbe58ef25b47e3a08e4f472d774359&timezoneOffset=0&verbose=false&q=";
        public async Task<ObjetoLuis> ConsultarLuis(string textoEvaluar)
        {
            //throw new NotImplementedException();
            textoEvaluar = HttpUtility.UrlEncode(textoEvaluar);
            var urlLuisConRequisicion = UrlServicioLuis + textoEvaluar;

            var client = new HttpClient();
            var body = new { };

            var bodySerializado = new JavaScriptSerializer().Serialize(body);
            byte[] bodyByte = Encoding.UTF8.GetBytes(bodySerializado);

            using (var content = new ByteArrayContent(bodyByte))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var respuesta = await client.GetAsync(urlLuisConRequisicion);
                var contenidoRespuesta = await respuesta.Content.ReadAsStringAsync();

                var javaScriptSerializer = new JavaScriptSerializer();
                var resultadoAnalisisTexto = javaScriptSerializer.Deserialize<ResultadoLuis>(contenidoRespuesta);


                if (resultadoAnalisisTexto.entities.Count == 0 && resultadoAnalisisTexto.topScoringIntent.intent.Equals("SolicitarEstatusCajero")  )
                {
                    EntityLuis ent = new EntityLuis();
                    ent.entity =Program.cajero;
                    ent.score = 0.9;
                    ent.type = "cajero";
                    resultadoAnalisisTexto.entities.Add(ent);
                }

                if (resultadoAnalisisTexto.entities.Count == 0 && resultadoAnalisisTexto.topScoringIntent.intent.Equals("SolicitarEstatusCajerosEmpresa"))
                {
                    EntityLuis ent = new EntityLuis();
                    ent.entity = Program.empresa;
                    ent.score = 0.9;
                    ent.type = "empresa";
                    resultadoAnalisisTexto.entities.Add(ent);
                }
                if (resultadoAnalisisTexto.entities.Count == 0 && resultadoAnalisisTexto.topScoringIntent.intent.Equals("SolicitarEstatusCajerosGrupo"))
                {
                    EntityLuis ent = new EntityLuis();
                    ent.entity = Program.grupo;
                    ent.score = 0.9;
                    ent.type = "grupo";
                    resultadoAnalisisTexto.entities.Add(ent);
                }

                if (resultadoAnalisisTexto.entities.Count == 0 && resultadoAnalisisTexto.topScoringIntent.intent.Equals("solicitarFechaSolucion")) {                                
                    EntityLuis ent = new EntityLuis();
                    ent.entity = Program.cajero;
                    ent.score = 0.9;
                    ent.type = "cajero";
                    resultadoAnalisisTexto.entities.Add(ent);             
                }

                if (resultadoAnalisisTexto.entities.Count == 0 && resultadoAnalisisTexto.topScoringIntent.intent.Equals("None"))
                {
                    EntityLuis ent = new EntityLuis();
                    if (resultadoAnalisisTexto.topScoringIntent.score <=8.0 ) {
                   
                            ent.entity = "noidentifico";
                            ent.score = 0.9;
                            ent.type = resultadoAnalisisTexto.query;

                    }
                    resultadoAnalisisTexto.entities.Add(ent);
                }

                if (resultadoAnalisisTexto.entities.Count == 1 && resultadoAnalisisTexto.topScoringIntent.intent.Equals("SolicitarHistoricoFallasCajeros") && resultadoAnalisisTexto.entities[0].type.Equals("tiempo")) {
                    EntityLuis ent = new EntityLuis();
                    ent.entity = Program.cajero;
                    ent.type = "cajero";
                    resultadoAnalisisTexto.entities.Add(ent);
                }
                if (resultadoAnalisisTexto.entities.Count == 1 && resultadoAnalisisTexto.topScoringIntent.intent.Equals("SolicitarHistoricoFallasCajerosEmpresa") && resultadoAnalisisTexto.entities[0].type.Equals("tiempo"))
                {
                    EntityLuis ent = new EntityLuis();
                    ent.entity = Program.empresa;
                    ent.type = "empresa";
                    resultadoAnalisisTexto.entities.Add(ent);
                }
                if (resultadoAnalisisTexto.entities.Count == 1 && resultadoAnalisisTexto.topScoringIntent.intent.Equals("SolicitarHistoricoFallasCajerosGrupo") && resultadoAnalisisTexto.entities[0].type.Equals("tiempo"))
                {
                    EntityLuis ent = new EntityLuis();
                    ent.entity = Program.grupo;
                    ent.type = "grupo";
                    resultadoAnalisisTexto.entities.Add(ent);
                }

                return new ObjetoLuis()
                {
                    Entidades = resultadoAnalisisTexto.entities.ToList(),
                    TopScoringIntent = resultadoAnalisisTexto.topScoringIntent
                };
            }
        }
        

        public Intensiones ConvertirObjetoLuisAIntencion(ObjetoLuis objetosLuis)
        {
            var topScoringIntent = objetosLuis.TopScoringIntent.intent;
            switch (topScoringIntent)
            {
                case "None":
                    return Intensiones.None;
                case "SolicitarEstatusCajero":
                        Program.cajero = objetosLuis.Entidades[0].entity;                
                        return Intensiones.SolicitarEstatusCajero;
                case "SolicitarEstatusCajerosEmpresa":
                        Program.empresa = objetosLuis.Entidades[0].entity;
                        return Intensiones.SolicitarEstatusCajerosEmpresa;
                case "SolicitarEstatusCajeroGrupo":
                        Program.grupo = objetosLuis.Entidades[0].entity;
                        return Intensiones.SolicitarEstatusCajeroGrupo;
                case "SolicitarHistoricoFallasCajerosEmpresa":
                    if (objetosLuis.Entidades[0].type.Equals("tiempo"))
                    {
                        Program.empresa = objetosLuis.Entidades[1].entity;
                    }
                    else
                    {
                        Program.empresa = objetosLuis.Entidades[0].entity;
                    }
                        return Intensiones.SolicitarHistoricoFallasCajerosEmpresa;
                case "SolicitarHistoricoFallasCajerosGrupo":
                    if (objetosLuis.Entidades[0].type.Equals("tiempo"))
                    {
                        Program.grupo = objetosLuis.Entidades[1].entity;
                    }
                    else
                    {
                        Program.grupo = objetosLuis.Entidades[0].entity;
                    }
                        return Intensiones.SolicitarHistoricoFallasCajerosGrupo;
                case "SolicitarHistoricoFallasCajeros":
                    if (objetosLuis.Entidades[0].type.Equals("tiempo"))
                    {
                        Program.cajero = objetosLuis.Entidades[1].entity;
                    }
                    else {
                        Program.cajero = objetosLuis.Entidades[0].entity;
                    }                  
                    return Intensiones.SolicitarHistoricoFallasCajeros;
                case "solicitarFechaSolucion":
                    return Intensiones.solicitarFechaSolucion;
                case "SolicitarResponsableCajero":
                    return Intensiones.SolicitarResponsableCajero;
                case "SolicitarResponsableCajeroEmpresa":
                    return Intensiones.SolicitarResponsableCajeroEmpresa;
                case "SolicitarResponsableCajeroGrupo":
                    return Intensiones.SolicitarResponsableCajeroGrupo;
                case "Saludo":
                    return Intensiones.Saludo;
                case "Ayuda":
                    return Intensiones.Ayuda;
                default:
                    return Intensiones.None;
            }
        }

        //Integra analysis de sentimientos a cualquier aplicación.
        public async Task<EstadoDeAnimo> ConsultarTextAnalytics(string textoEvaluar)
        {

            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "09cc0151e19d4676a3c5db84d319859d");

            var uri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";

            var body = new
            {
                documents = new[]
                {
                    new
                    {
                        language = "es",
                        id = Guid.NewGuid(),
                        text = textoEvaluar
                    }
                }
            };

            var bodySerializado = new JavaScriptSerializer().Serialize(body);
            byte[] byteData = Encoding.UTF8.GetBytes(bodySerializado);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var respuesta = await client.PostAsync(uri, content);
                var contenidoRespuesta = await respuesta.Content.ReadAsStringAsync();

                var js = new JavaScriptSerializer();
                var ResultadoTextAnalytics = js.Deserialize<ResultadoTextAnalytics>(contenidoRespuesta);

                return ConvertirTextAnalyticsAEstadoDeAnimo(ResultadoTextAnalytics);

            }
        }

        public class Class1
        {

            public static float score;

        }
        private EstadoDeAnimo ConvertirTextAnalyticsAEstadoDeAnimo(ResultadoTextAnalytics textAnalytics)
        {

      
            Class1.score = float.Parse(textAnalytics.documents[0].score, CultureInfo.InvariantCulture.NumberFormat);

            //var score = Convert.ToDouble(textAnalytics.documents[0].score);
            if (Class1.score > 0.6)
            {
                return EstadoDeAnimo.Bueno;
            }
            else if (Class1.score > 0.4)
            {
                return EstadoDeAnimo.Regular;
            }
            else
            {
                return EstadoDeAnimo.Malo;
            }

        }

    }
}