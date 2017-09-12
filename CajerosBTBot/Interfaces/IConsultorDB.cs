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

        Boolean obtenerEstatusCajerosEmpresa(string empresa);

        List<Empresa> ObtenerFallasEmpresa(string empresa);

        List<Empresa> obtenerHistoricoCajeroEmpresa(string empresa, string periodo);

        List<Cajero> obtenerHistoricoCajero(string cajero, string periodo);

        List<Empresa> ObtenerEmpresas(string empresa);

        List<Tiempo> obtenerPeriodoSolucion(string cajero);
    }
}