using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CajerosBTBot.Bean
{
    [Serializable]
    public class IntentLuis
    {
        public string intent { get; set; }
        public double score { get; set; }
    }
    [Serializable]
    public class TopScoringIntent
    {
        public string intent { get; set; }
        public double score { get; set; }
    }
    [Serializable]
    public class EntityLuis
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public double score { get; set; }
    }
    [Serializable]
    public class ObjetoLuis
    {
        public List<EntityLuis> Entidades { get; set; }
        public TopScoringIntent TopScoringIntent { get; set; }
    }
    [Serializable]
    public class ResultadoLuis
    {
        public string query { get; set; }
        public TopScoringIntent topScoringIntent { get; set; }
        public List<EntityLuis> entities { get; set; }
    }


}