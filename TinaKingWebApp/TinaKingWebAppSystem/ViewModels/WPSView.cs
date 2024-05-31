using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TinaKingSystem.ViewModels
{
    public class WPSView
    {
		public int WPSID { get; set; } = 0;
		public int WPSFormID { get; set; } = 0;
        public int PackageID { get; set; } = 0;
        public string WPSNo { get; set; } = "";
        public string PQRNumber { get; set; } = "";
        public int EmployeeID { get; set; } = 0;
        public string CompanyName { get; set; } = "";
		public DateTime? Date { get; set; } = DateTime.Now;
        public WeldingSpecificationView? WeldingSpecification { get; set; } = new WeldingSpecificationView();
        public WeldingProcessView? WeldingProcess { get; set; } = new WeldingProcessView();
        public PreheatAndPostWeldView? PreHeatAndPostWeld { get; set; } = new PreheatAndPostWeldView();
        public PositionsView? Position { get; set; } = new PositionsView();
        public FillerView? Filler { get; set; } = new FillerView();
        public FillerMetalView? FillerMetal { get; set; } = new FillerMetalView();
        public AdditionalDetailView? AdditionalDetail { get; set; } = new AdditionalDetailView();

		public class T
		{
			[JsonProperty("a1")] public string A1 { get; set; }
            [JsonProperty("a2")] public string a2 { get; set; }
            [JsonProperty("a3")] public string a3 { get; set; }
            [JsonProperty("a4")] public string a4 { get; set; }
            [JsonProperty("a5")] public string a5 { get; set; }
            [JsonProperty("a6")] public string a6 { get; set; }
            [JsonProperty("a7")] public string a7 { get; set; }
        }

        public string toString()
        {
			List<string> str_list = new List<string>();
			T tmp = new T()
			{
				A1 = WeldingSpecification.ToString(),
				a2 = WeldingProcess.ToString(),
				a3 = PreHeatAndPostWeld.ToString(),
				a4 = Position.ToString(),
				a5 = Filler.ToString(),
				a6 = FillerMetal.ToString(),
				a7 = AdditionalDetail.ToString(),
			};

            return JsonConvert.SerializeObject(tmp);
		}

        public bool fromString(string strJson)
        {
			try
			{
				T tmp = JsonConvert.DeserializeObject<T>(strJson);

				this.WeldingSpecification.FromString(tmp.A1);
                WeldingProcess.FromString(tmp.a2);
                PreHeatAndPostWeld.FromString(tmp.a3);
                Position.FromString(tmp.a4);
                Filler.FromString(tmp.a5);
                FillerMetal.FromString(tmp.a6);
                AdditionalDetail.FromString(tmp.a7);


                return true;
            }catch(Exception e)
			{
				return false;
			}
        }

		public string getSummery()
        {
			List<string> l = getSummeryList();
			string ret = "";
			foreach (string s in l)
			{
				if (ret != "") ret += "\n";
				ret += s;
			}

			return ret;
        }

        public List<string> getSummeryList()
        {
			List<string> r = new List<string>();

			if (this.WeldingProcess.PostWeldRequired == 1) {
				if (this.PreHeatAndPostWeld.PWHTempRangeCorrect == 1)
					r.Add("Temp range correct");
				else
					r.Add("Temp range incorrect");
				if (this.PreHeatAndPostWeld.PWHSoaktimeCorrect == 1)
					r.Add("Sock time correct");
				else
					r.Add("Sock time incorrect");
			}
			if (this.FillerMetal.ElectrodeFluxClassififcationCorrect == 1)
			{
				if (this.FillerMetal.FluxTradeNameStatedCorrect == 0)
				{
					r.Add("Flux trade name incorrectly stated.");
				}
				if (this.FillerMetal.FluxTradeNameNotStated == 0)
				{
					r.Add("Flux trade name not stated.");
				}
				if (this.FillerMetal.FluxTradeNameStatedAsManufacturer == 1)
				{
					r.Add("Flux trade name stated as the manufacturer’s address.");
				}
			}

			
			 if (this.Filler.SFA == 0) {
				r.Add("SFA is incorrect");
			 }

		    if (this.Filler.FNo == 0) {
				r.Add("F-No is incorrect");
			}

		    if (this.Filler.ANo == 0) {
				r.Add("A-No is incorrect");
			}

			if (this.AdditionalDetail.SheildingGasCompositionCorrect == 0)
			{
				r.Add("Compostion of shielding gas with PQR is incorrect");
			}

			if (this.AdditionalDetail.TrailingGasUsed == 0)
			{
				r.Add("Trailing gas not used");
			}

			if (this.AdditionalDetail.BackingGasUsed == 0)
			{
				r.Add("Backing gas not used");
			}

			if (this.AdditionalDetail.PeeningAllowed == 0)
			{
				r.Add("Peening is not allowed");
			}
			if (this.WeldingSpecification.ModeOfTransfer == "notMatch")
			{
				r.Add("Mode of transfer does not match.");
			}
			if (this.Position.ProgressionRestriction == 1)
			{
				r.Add("Progression restricted by the client.");
			}
			if (this.Filler.ProductFormMatchesPQR == 0)
			{
				r.Add("The product form on the WPS does not match that on the PQR.");
			}
			if (this.AdditionalDetail.FerriteContentInRange == 1)
			{
				r.Add("Ferrite content is not between 35 – 60.");
			}
			if (this.Filler.SupplementUsed == 0)
			{
				r.Add("Supplemental filler metal not used on PQR.");
			}

			if (this.WeldingProcess.ProcessType == "FCAW"
				|| this.WeldingProcess.ProcessType == "GTAW"
				|| this.WeldingProcess.ProcessType == "GMAW"){
				if (this.AdditionalDetail.PeeningAllowed == 1)
					r.Add("Peening allowed by client.");
				if (this.AdditionalDetail.ImpactTestRequired == 1)
					r.Add("Impact test require");
				if (this.AdditionalDetail.MaxHeatStated == 1)
					r.Add("Is the maximum heat stated");
			}

			if (this.AdditionalDetail.Process1 == 1) {
				if (this.AdditionalDetail.Process2 == 1) {
					if (this.AdditionalDetail.Pitting == 1)
						r.Add("Pitting is present.");
					if (this.AdditionalDetail.Wear == 1)
						r.Add("Wear detected.");
				}
				if (this.AdditionalDetail.PostWeldTreatment == 1) {
					if (this.AdditionalDetail.PostWeldHeatTempCorrect == 0)
						r.Add("Post Is Temperature incorrect.");
					if (this.AdditionalDetail.DwellTimeCorrect == 0)
						r.Add("Post Is Dwell time incorrect.");
					if (this.AdditionalDetail.QuenchedInWater == 1)
						r.Add("Post Quenched in water.");
				}
				if (this.AdditionalDetail.PittingCorrosionResistance == 1) {
					if (this.AdditionalDetail.IsNumberCorrect == 0)
						r.Add("Pitting Is Number incorrect.");
					if (this.AdditionalDetail.IsImpactCorrect == 0)
						r.Add("Pitting Is Impact test incorrect.");
				}
			}

			return r;
		}
    }
}
