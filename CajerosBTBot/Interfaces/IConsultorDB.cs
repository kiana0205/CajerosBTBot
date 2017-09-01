using CajerosBTBot.Bean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CajerosBTBot.Interfaces
{
    interface IConsultorDB
    {

        Boolean ObtenerEstatusCajero(string cajero);

        List<Cajero> ObtenerFallaCajero(string cajerp);

        List<Cajero> obtenerEstatusCajerosEmpresa(string empresa);


       // List<Cajero> obtenerHistoricoCajeroEmpresa(string fecha);
    }
}