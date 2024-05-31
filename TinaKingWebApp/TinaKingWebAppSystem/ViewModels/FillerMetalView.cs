using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace TinaKingSystem.ViewModels
{
    public class FillerMetalView
    {
        public int WPSFormID { get; set; } = 0;
        public int ElectrodeFluxClassififcationID { get; set; } = -1;
        public int ElectrodeFluxClassififcationCorrect { get; set; } = -1;
        public int FluxTradeNameStatedCorrect { get; set; } = -1;
		public int FluxTradeNameNotStated { get; set; } = -1;
		public int FluxTradeNameStatedAsManufacturer { get; set; } = -1;

        class T
        {
            [JsonProperty("a1")] public int EFD { get; set; } = -1;
            [JsonProperty("a2")] public int ECC { get; set; } = -1;
            [JsonProperty("a3")] public int FTC { get; set; } = -1;
            [JsonProperty("a4")] public int FTS { get; set; } = -1;
            [JsonProperty("a5")] public int FTM { get; set; } = -1;
        }

        public string ToString()
        {
            T obj = new T()
            {
                EFD = ElectrodeFluxClassififcationID,
                ECC = ElectrodeFluxClassififcationCorrect,
                FTC = FluxTradeNameStatedCorrect,
                FTS = FluxTradeNameNotStated,
                FTM = FluxTradeNameStatedAsManufacturer,
            };
            return JsonConvert.SerializeObject(obj);
        }

        public bool FromString(string v)
        {
            try
            {
                T obj = JsonConvert.DeserializeObject<T>(v);
                if (obj == null) return false;

                ElectrodeFluxClassififcationID = obj.EFD;
                ElectrodeFluxClassififcationCorrect = obj.ECC;
                FluxTradeNameStatedCorrect = obj.FTC;
                FluxTradeNameNotStated = obj.FTS;
                FluxTradeNameStatedAsManufacturer = obj.FTM;

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }

}
