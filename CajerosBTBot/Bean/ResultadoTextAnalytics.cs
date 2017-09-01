using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CajerosBTBot.Bean
{
    public class ResultadoTextAnalytics
    {
        public document[] documents { get; set; }
        public error[] errors { get; set; }
    }

    public class document
    {
        public string score { get; set; }
        public string id { get; set; }
    }

    public class error
    {
        public string id { get; set; }
        public string message { get; set; }
    }
}