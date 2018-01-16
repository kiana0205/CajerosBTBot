//namespace BotBuilder.Samples.AdaptiveCards
namespace CajerosBTBot
{

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


    public class CajerosQuery
    {
        [Required]
        public string Cajero { get; set; }

        public static CajerosQuery Parse(dynamic o) {
            try
            {
                return new CajerosQuery
                {
                    Cajero = o.Cajero.ToString(),             
                };
            }
            catch
            {
                throw new InvalidCastException("CajerosQuery no puede ser leido");
            }
        }

    }
}