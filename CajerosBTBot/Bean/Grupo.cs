using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CajerosBTBot.Bean
{
    public class Grupo
    {


        public Grupo() { }
        public Grupo(string grupos, string mensaje)
        {
            Grupos = grupos;
            Mensaje = mensaje;
        }

   
        public string cajero { get; set; }
        public string grupo { get; set; }

        public string Grupos { get; set; }

        public string empresa { get; set; }

        public string tipoFalla { get; set; }

        public string folio { get; set; }

        public string fecha { get; set; }

        public string id_empresa { get; set; }

        public string id_grupo { get; set; }

        public string conteo { get; set; }

        public string Empresas { get; set; }

        public string Mensaje { get; set; }
    }
}