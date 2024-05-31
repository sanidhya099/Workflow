using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaKingSystem.ViewModels
{
    public class PositionsView
    {
        public int WPSFormID { get; set; } = 0;
        public string PositionID { get; set; } = "";
		public string PositionType { get; set; } = "";
        public int OverheadVertical { get; set; } = -1;
        public string TypeOfProgression { get; set; } = "";
        public int ProgressionRestriction { get; set; } = -1;
        public string AdditionalComment { get; set; } = "";

        class T
        {
           [JsonProperty("a1")] public string PT { get; set; } = "";
           [JsonProperty("a2")]  public int OV { get; set; } = -1;
           [JsonProperty("a3")]  public string TOP { get; set; } = "";
           [JsonProperty("a4")]  public int PR { get; set; } = -1;
           [JsonProperty("a5")]  public string AC { get; set; } = "";
          
        } 

        public string ToString()
        {
            T obj = new T()
            {
                PT  = PositionType,
                OV  = OverheadVertical,
                TOP = TypeOfProgression,
                PR  = ProgressionRestriction,
                AC  = AdditionalComment,
            };
            return JsonConvert.SerializeObject(obj);
        }

        public bool FromString(string v)
        {
            try
            {
                T obj = JsonConvert.DeserializeObject<T>(v);
                if (obj == null) return false;

                PositionType = obj.PT;
                OverheadVertical = obj.OV;
                TypeOfProgression = obj.TOP;
                ProgressionRestriction = obj.PR;
                AdditionalComment = obj.AC;

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
