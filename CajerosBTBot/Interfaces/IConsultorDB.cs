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

        Boolean ObtenerCajero(string cajero);

        List<Cajero> ObtenerFallaCajero(string cajerp);

        Boolean obtenerEstatusCajerosEmpresa(string empresa);

        List<Empresa> ObtenerFallasEmpresa(string empresa);

        Boolean obtenerEstatusCajerosGrupo(string empresa);

        List<Grupo> ObtenerFallasGrupo(string empresa);

        List<Empresa> obtenerHistoricoCajeroEmpresa(string empresa, string periodo);

        List<Cajero> obtenerHistoricoCajero(string cajero, string periodo);

        List<Grupo> obtenerHistoricoCajeroGrupo(string grupo, string periodo);

        List<Empresa> ObtenerEmpresas(string empresa);

        List<Grupo> ObtenerGrupos(string empresa);

        List<Tiempo> obtenerPeriodoSolucion(string cajero);

        List<Cajero> obtenerResponsable(string cajero);

        List<Empresa> obtenerResponsableEmpresa(string empresa);

        List<Grupo> obtenerResponsableGrupo(string grupo);

        Int32 obtenerConteoCajerosGrupo(string grupo);

        Int32 obtenerConteoCajerosEmpresa(string empresa);
    }
}