using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaKingSystem.ViewModels
{
    public class WeldingProcessView
    {
        public int WPSFormID { get; set; } = 0;
        public int WeldingProcessID { get; set; } = 0;
		public string ProcessType { get; set; } = "";
		public int Backing { get; set; } = -1;
        public string TypeOfBacking { get; set; } = "";
		public string BackingOther { get; set; } = "";
		public int PostWeldRequired { get; set; } = -1;

		public int MaxThicknessRange { get; set; } = -1;
		public string MaxThicknessRangeValue { get; set; } = "";
		public int MinThicknessRange { get; set; } = -1;
		public string TypeOfBaseMetal { get; set; } = "";
		public string Metal1 { get; set; } = "";
		public string Metal1Group { get; set; } = "";
		public string Metal2 { get; set; } = "";
		public string Metal2Group { get; set; } = "";
		public string UNSNumber { get; set; } = "";
		public int MetalOtherStay { get; set; } = -1;

        class T
        {
            [JsonProperty("a1")] public string PT { get; set; } = "";
            [JsonProperty("a2")] public int B { get; set; } = -1;
            [JsonProperty("a3")] public string TOB { get; set; } = "";
            [JsonProperty("a4")] public string BO { get; set; } = "";
            [JsonProperty("a5")] public int PWR { get; set; } = -1;
            [JsonProperty("a6")] public int MTR { get; set; } = -1;
            [JsonProperty("a7")] public string MTRV { get; set; } = "";
            [JsonProperty("a8")] public int MITR { get; set; } = -1;
            [JsonProperty("a9")] public string TOBM { get; set; } = "";
            [JsonProperty("a10")] public string M1 { get; set; } = "";
            [JsonProperty("a11")] public string M1G { get; set; } = "";
            [JsonProperty("a12")] public string M2 { get; set; } = "";
            [JsonProperty("a13")] public string M2G { get; set; } = "";
            [JsonProperty("a14")] public string UNS { get; set; } = "";
            [JsonProperty("a15")] public int MOS { get; set; } = -1;
        }
        public string ToString()
        {
            T obj = new T()
            {
                PT = ProcessType,
                B = Backing,
                TOB = TypeOfBacking,
                BO = BackingOther,
                PWR = PostWeldRequired,
                MTR = MaxThicknessRange,
                MTRV = MaxThicknessRangeValue,
                MITR = MinThicknessRange,
                TOBM = TypeOfBaseMetal,
                M1 = Metal1,
                M1G = Metal1Group,
                M2 = Metal2,
                M2G = Metal2Group,
                UNS = UNSNumber,
                MOS = MetalOtherStay,
            };
            return JsonConvert.SerializeObject(obj);
        }

        public bool FromString(string v)
        {
            try
            {
                T obj = JsonConvert.DeserializeObject<T>(v);
                if (obj == null) return false;

                ProcessType = obj.PT;
                Backing = obj.B;
                TypeOfBacking = obj.TOB;
                BackingOther = obj.BO;
                PostWeldRequired = obj.PWR;
                MaxThicknessRange = obj.MTR;
                MaxThicknessRangeValue = obj.MTRV;
                MinThicknessRange = obj.MITR;
                TypeOfBaseMetal = obj.TOBM;
                Metal1 = obj.M1;
                Metal1Group = obj.M1G;
                Metal2 = obj.M2;
                Metal2Group = obj.M2G;
                UNSNumber = obj.UNS;
                MetalOtherStay = obj.MOS;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
