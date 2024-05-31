using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaKingSystem.ViewModels
{
    public class DrawingView
    {
        public int ID { get; set; }
        public int PackageID { get; set; }
        public DateTime RegistDate { get; set; } 
        public string Detail { get; set; }
        public DrawingItem DetailInfo { get; set; } = new DrawingItem();
        public List<string> GetSummery()
        {
            List<string> lRet = new List<string>();
            if (DetailInfo.isForContruction == 0) {
                lRet.Add("Not issued for construction");
            }
            if (DetailInfo.isWeldComplete == 0) {
                lRet.Add("Welmap is incompleted");
            }
            if (DetailInfo.ThickQualifyAcceptable == 0) {
                lRet.Add("Thickness range is not acceptable");
            }
            if (DetailInfo.CorrectWPS == 0) {
                lRet.Add("WPS is incorrect");
            }
            return lRet;
        }
        public string GetSummeryString()
        {
            List<string> l = GetSummery();
            string ret = "";
            foreach (string s in l) {
                if (ret != "") ret += "\n";
                ret += s;
            }

            return ret;
        }
    }

    public class DrawingItem
    {
        [JsonProperty("a")] public int isForContruction { get; set; } = -1;
        [JsonProperty("b")] public int isWeldComplete { get; set; } = -1;
        [JsonProperty("c")] public int ThickQualifyAcceptable { get; set; } = -1;
        [JsonProperty("d")] public int CorrectWPS { get; set; } = -1;

        public static DrawingItem FromString(string s)
        {
            try {
                DrawingItem ret = JsonConvert.DeserializeObject<DrawingItem>(s);
                return ret;
            } catch (Exception e) {
                return null;
            }
        }
        public string ToString()
        {
            string ret = JsonConvert.SerializeObject(this);
            return ret;
        }
    }
}
