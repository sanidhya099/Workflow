using Newtonsoft.Json;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaKingSystem.ViewModels
{
    public class WeldingSpecificationView
    {
        public int WPSFormID { get; set; } = 0;
        public int SpecID { get; set; } = 0;
        public int GMAWorFCAW { get; set; } = -1;
        public string ModeOfTransfer { get; set; } = "";
        public int MOT { get; set; } = -1;
        public int MOTmatchPQR { get; set; } = -1;

        class T{
            [JsonProperty("a1")] public int gma { get; set; } = -1;
            [JsonProperty("a2")] public string mot { get; set; } = "";
            [JsonProperty("a3")] public int BMT { get; set; } = -1;
            [JsonProperty("a4")] public int MR { get; set; } = -1;
        }   
            
        public string ToString() {
            T obj = new T()
            {
                gma = GMAWorFCAW,
                mot = ModeOfTransfer,
                BMT = MOT,
                MR = MOTmatchPQR
            };
            return JsonConvert.SerializeObject(obj); 
        }
        public bool FromString(string v)
        {
            try
            {
                T obj = JsonConvert.DeserializeObject<T>(v);
                if (obj == null) return false;
                GMAWorFCAW = obj. gma;
                ModeOfTransfer = obj.mot;
                MOT = obj.BMT;
                MOTmatchPQR = obj.MR;
                return true;
            }catch(Exception e)
            {
                return false;
            }
        }
    }
}
