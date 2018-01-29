using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CajerosBTBot.Enums
{
    public enum Intensiones
    {
        None,
        SolicitarEstatusCajero,
        SolicitarEstatusCajerosEmpresa,
        SolicitarEstatusCajeroGrupo,
        SolicitarHistoricoFallasCajeros,
        SolicitarHistoricoFallasCajerosEmpresa,
        SolicitarHistoricoFallasCajerosGrupo,
        SolicitarResponsableCajero,
        SolicitarResponsableCajeroEmpresa,
        SolicitarResponsableCajeroGrupo,
        solicitarFechaSolucion,
        SolicitarCerrarDialogo,
        Saludo,
        Ayuda
    }
}