using CajerosBTBot.Bean;
using CajerosBTBot.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CajerosBTBot.Interfaces
{
    public interface IConsultorCognitive
    {
        Task<ObjetoLuis> ConsultarLuis(string mensaje);
        Task<EstadoDeAnimo> ConsultarTextAnalytics(string textoEvaluar);
    }
}