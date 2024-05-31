using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaKingSystem.ViewModels
{
    public class PreheatAndPostWeldView
    {
        public int WPSFormID { get; set; } = 0;
        public int HeatID { get; set; } = 0;
//         public int? PreHeatMinCorrect { get; set; } = -1;
//         public int? InterPassMaxCorrect { get; set; } = -1;
// 		public int? PreHeatMaintainenceRequired { get; set; } = -1;
		public int PWHTempRangeCorrect { get; set; } = -1;
        public int PWHSoaktimeCorrect { get; set; } = -1;
		public int SolutionAnnealRequired { get; set; } = -1;


        class T
        {
            [JsonProperty("a1")] public int PRC { get; set; } = -1;
            [JsonProperty("a2")] public int PSC { get; set; } = -1;
            [JsonProperty("a3")] public int SA { get; set; } = -1;

        }

        public string ToString()
        {
            T obj = new T()
            {
                PRC = PWHTempRangeCorrect,
                PSC = PWHSoaktimeCorrect,
                SA = SolutionAnnealRequired,
            };
            return JsonConvert.SerializeObject(obj);
        }

        public bool FromString(string v)
        {
            try
            {
                T obj = JsonConvert.DeserializeObject<T>(v);
                if (obj == null) return false;

                PWHTempRangeCorrect = obj.PRC;
                PWHSoaktimeCorrect = obj.PSC;
                SolutionAnnealRequired = obj.SA;

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}
