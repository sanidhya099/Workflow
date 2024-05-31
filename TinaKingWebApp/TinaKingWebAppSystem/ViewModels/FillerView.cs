using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TinaKingSystem.ViewModels
{
    public class FillerView
    {
        public int WPSFormID { get; set; } = 0;
        public int FillerID { get; set; } = 0;
        public int SFA { get; set; } = -1;
        public int FNo { get; set; } = -1;
		public int ANo { get; set; } = -1;
		public int SizeMatchesPQR { get; set; } = -1;
		public int ProductFormMatchesPQR { get; set; } = -1;
        public string TypeOfproductForm { get; set; } = "";
        public string AdditionalCommentFormMatchPQR { get; set; } = "";
        public int SupplementUsed { get; set; } = -1;
        public string AdditionalCommentSupplementUsed { get; set; } = "";
        public int DepositThickness { get; set; } = -1;
        public int ConsumableInsert { get; set; } = -1;
        class T
        {
            [JsonProperty("a1")] public int SFA { get; set; } = -1;
            [JsonProperty("a2")] public int FNo { get; set; } = -1;
            [JsonProperty("a3")] public int ANo { get; set; } = -1;
            [JsonProperty("a4")] public int SMR { get; set; } = -1;
            [JsonProperty("a5")] public int PMR { get; set; } = -1;
            [JsonProperty("a6")] public string TOF { get; set; } = "";
            [JsonProperty("a7")] public string ACR { get; set; } = "";
            [JsonProperty("a8")] public int SUD { get; set; } = -1;
            [JsonProperty("a9")] public string ACS { get; set; } = "";
            [JsonProperty("a10")] public int DTS { get; set; } = -1;
            [JsonProperty("a11")] public int CAI { get; set; } = -1;
        }

        public string ToString()
        {
            T obj = new T()
            {
                SFA =  SFA,
                FNo =  FNo,
                ANo =  ANo,
                SMR =  SizeMatchesPQR,
                PMR =  ProductFormMatchesPQR,
                TOF =  TypeOfproductForm,
                ACR =  AdditionalCommentFormMatchPQR,
                SUD =  SupplementUsed,
                ACS =  AdditionalCommentSupplementUsed,
                DTS =  DepositThickness,
                CAI = ConsumableInsert,
            };
            return JsonConvert.SerializeObject(obj);
        }

        public bool FromString(string v)
        {
            try
            {
                T obj = JsonConvert.DeserializeObject<T>(v);
                if (obj == null) return false;

                SFA = obj.SFA;
                FNo = obj.FNo;
                ANo = obj.ANo;
                SizeMatchesPQR = obj.SMR;
                ProductFormMatchesPQR = obj.PMR;
                TypeOfproductForm = obj.TOF;
                AdditionalCommentFormMatchPQR = obj.ACR;
                SupplementUsed = obj.SUD;
                AdditionalCommentSupplementUsed = obj.ACS;
                DepositThickness = obj.DTS;
                ConsumableInsert = obj.CAI;

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
