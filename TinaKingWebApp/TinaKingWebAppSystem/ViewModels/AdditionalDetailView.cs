using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace TinaKingSystem.ViewModels
{
    public class AdditionalDetailView
    {
        public int WPSFormID { get; set; } = 0;
        public int DetailID { get; set; } = 0;
        public string FGG { get; set; } = "";
        public int SheildingGasCompositionCorrect { get; set; } = -1;
        public int TrailingGasUsed { get; set; } = -1;
        public int BackingGasUsed { get; set; } = -1;
        public string NumberOfElectrode { get; set; } = "";
        public string NumberOfPassMatchesPQR { get; set; } = "";
        public int PeeningAllowed { get; set; } = -1;
        public int ImpactTestRequired { get; set; } = -1;
        public int MaxHeatStated { get; set; } = -1;
		public string CommentSection { get; set; } = "";
        public string impactTestTempRequirement { get; set; } = "";
		public string impactTestEnergyRequirement { get; set; } = "";
		public double ImpactTestTemperature { get; set; } = 0;
        public int ImpactTestTempMatch { get; set; } = -1;
        public double ImpactTestEnergy { get; set; } = 0;
        public int PercentageShearStated { get; set; } = -1;
        public int CorrosionTestingDone { get; set; } = -1;
        public int FullSizeSpecimen { get; set; } = -1;
        public int Process1 { get; set; } = -1;
		public int Process2 { get; set; } = -1;
		public int Pitting { get; set; } = -1;
        public int Wear { get; set; } = -1;
        public int FerriteContentTest { get; set; } = -1;
        public int IsNumberCorrect { get; set; } = -1;
		public int IsImpactCorrect { get; set; } = -1;
		public int FerriteContentInRange { get; set; } = -1;
		public int PostWeldTreatment { get; set; } = -1;
		public int PostWeldHeatTempCorrect { get; set; } = -1;
		public int DwellTimeCorrect { get; set; } = -1;
        public int QuenchedInWater { get; set; } = -1;
		public int PittingCorrosionResistance { get; set; } = -1;
		public int PittingCorrosionResistanceNumberCorrect { get; set; } = -1;
		public int PittingCorrosionResistanceImpacttestCorrect { get; set; } = -1;
		public int RadiographAndLPIPassed { get; set; } = -1;
        public int ProcessSelected { get; set; } = -1;


        class T
        {
            [JsonProperty("a1")] public string FGG { get; set; } = "";
            [JsonProperty("a2")] public int SGC { get; set; } = -1;
            [JsonProperty("a3")] public int TGU { get; set; } = -1;
            [JsonProperty("a4")] public int BGU { get; set; } = -1;
            [JsonProperty("a5")] public string NOE { get; set; } = "";
            [JsonProperty("a6")] public string NOP { get; set; } = "";
            [JsonProperty("a7")] public int PAD { get; set; } = -1;
            [JsonProperty("a8")] public int ITR { get; set; } = -1;
            [JsonProperty("a9")] public int MHS { get; set; } = -1;
            [JsonProperty("a10")] public string CST { get; set; } = "";
            [JsonProperty("a11")] public string TTR { get; set; } = "";
            [JsonProperty("a12")] public string TER { get; set; } = "";
            [JsonProperty("a13")] public double ITT { get; set; } = 0;
            [JsonProperty("a14")] public int ITM { get; set; } = -1;
            [JsonProperty("a15")] public double ITE { get; set; } = 0;
            [JsonProperty("a16")] public int PSS { get; set; } = -1;
            [JsonProperty("a17")] public int CTD { get; set; } = -1;
            [JsonProperty("a18")] public int FSM { get; set; } = -1;
            [JsonProperty("a19")] public int PS1 { get; set; } = -1;
            [JsonProperty("a20")] public int PS2 { get; set; } = -1;
            [JsonProperty("a21")] public int PIT { get; set; } = -1;
            [JsonProperty("a22")] public int WER { get; set; } = -1;
            [JsonProperty("a23")] public int FCT { get; set; } = -1;
            [JsonProperty("a24")] public int INC { get; set; } = -1;
            [JsonProperty("a25")] public int IIC { get; set; } = -1;
            [JsonProperty("a26")] public int FCI { get; set; } = -1;
            [JsonProperty("a27")] public int PWT { get; set; } = -1;
            [JsonProperty("a28")] public int PWH { get; set; } = -1;
            [JsonProperty("a29")] public int DTC { get; set; } = -1;
            [JsonProperty("a30")] public int CDI { get; set; } = -1;
            [JsonProperty("a31")] public int PCR { get; set; } = -1;
            [JsonProperty("a32")] public int PCK { get; set; } = -1;
            [JsonProperty("a33")] public int PCI { get; set; } = -1;
            [JsonProperty("a34")] public int RAI { get; set; } = -1;
            [JsonProperty("a35")] public int PSD { get; set; } = -1;
        }

        public string ToString()
        {
            T obj = new T()
            {
                FGG = FGG,
                SGC = SheildingGasCompositionCorrect,
                TGU = TrailingGasUsed,
                BGU = BackingGasUsed,
                NOE = NumberOfElectrode,
                NOP = NumberOfPassMatchesPQR,
                PAD = PeeningAllowed,
                ITR = ImpactTestRequired,
                MHS = MaxHeatStated,
                CST = CommentSection,
                TTR = impactTestTempRequirement,
                TER = impactTestEnergyRequirement,
                ITT = ImpactTestTemperature,
                ITM = ImpactTestTempMatch,
                ITE = ImpactTestEnergy,
                PSS = PercentageShearStated,
                CTD = CorrosionTestingDone,
                FSM = FullSizeSpecimen,
                PS1 = Process1,
                PS2 = Process2,
                PIT = Pitting,
                WER = Wear,
                FCT = FerriteContentTest,
                INC = IsNumberCorrect,
                IIC = IsImpactCorrect,
                FCI = FerriteContentInRange,
                PWT = PostWeldTreatment,
                PWH = PostWeldHeatTempCorrect,
                DTC = DwellTimeCorrect,
                CDI = QuenchedInWater,
                PCR = PittingCorrosionResistance,
                PCK = PittingCorrosionResistanceNumberCorrect,
                PCI = PittingCorrosionResistanceImpacttestCorrect,
                RAI = RadiographAndLPIPassed,
                PSD = ProcessSelected,
            };
            return JsonConvert.SerializeObject(obj);
        }

        public bool FromString(string v)
        {
            try
            {
                T obj = JsonConvert.DeserializeObject<T>(v);
                if (obj == null) return false;

                FGG = obj.FGG;
                SheildingGasCompositionCorrect = obj.SGC;
                TrailingGasUsed = obj.TGU;
                BackingGasUsed = obj.BGU;
                NumberOfElectrode = obj.NOE;
                NumberOfPassMatchesPQR = obj.NOP;
                PeeningAllowed = obj.PAD;
                ImpactTestRequired = obj.ITR;
                MaxHeatStated = obj.MHS;
                CommentSection = obj.CST;
                impactTestTempRequirement = obj.TTR;
                impactTestEnergyRequirement = obj.TER;
                ImpactTestTemperature = obj.ITT;
                ImpactTestTempMatch = obj.ITM;
                ImpactTestEnergy = obj.ITE;
                PercentageShearStated = obj.PSS;
                CorrosionTestingDone = obj.CTD;
                FullSizeSpecimen = obj.FSM;
                Process1 = obj.PS1;
                Process2 = obj.PS2;
                Pitting = obj.PIT;
                Wear = obj.WER;
                FerriteContentTest = obj.FCT;
                IsNumberCorrect = obj.INC;
                IsImpactCorrect = obj.IIC;
                FerriteContentInRange = obj.FCI;
                PostWeldTreatment = obj.PWT;
                PostWeldHeatTempCorrect = obj.PWH;
                DwellTimeCorrect = obj.DTC;
                QuenchedInWater = obj.CDI;
                PittingCorrosionResistance = obj.PCR;
                PittingCorrosionResistanceNumberCorrect = obj.PCK;
                PittingCorrosionResistanceImpacttestCorrect = obj.PCI;
                RadiographAndLPIPassed = obj.RAI;
                ProcessSelected = obj.PSD;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}
