﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TinaKingSystem.Entities;

public partial class AdditionalDetail
{
    [Key]
    public int DetailID { get; set; }

    public int? WPSFormID { get; set; }

    public bool? ShieldingGasCompositionCorrect { get; set; }

    public bool? TrailingGasUsed { get; set; }

    public bool? BackingGasUsed { get; set; }

    [StringLength(25)]
    [Unicode(false)]
    public string NumberOfElectrode { get; set; }

    [StringLength(25)]
    [Unicode(false)]
    public string NumberOfPassMatchesPQR { get; set; }

    public bool? PeeningAllowed { get; set; }

    public bool? ImpactTestRequired { get; set; }

    public bool? MaxHeatStated { get; set; }

    public double? ImpactTestTemperature { get; set; }

    public double? ImpactTestEnergy { get; set; }

    public bool? PercentageShearStated { get; set; }

    public bool? FullSizeSpecimen { get; set; }

    public bool? CorrosionTestingDone { get; set; }

    [StringLength(25)]
    [Unicode(false)]
    public string Pitting { get; set; }

    [StringLength(25)]
    [Unicode(false)]
    public string Wear { get; set; }

    public bool? FerriteContentTest { get; set; }

    public bool? FerriteContentInRange { get; set; }

    public bool? PostWeldTreatment { get; set; }

    public bool? PostWeldHeatTreatmentTempCorrect { get; set; }

    public bool? DwellTimeCorrect { get; set; }

    public bool? QuenchedInWater { get; set; }

    public bool? PittingCorrosionResistance { get; set; }

    public bool? PittingCorrosionResistanceNumberCorrect { get; set; }

    public bool? PittingCorrosionResistanceImpactTestCorrect { get; set; }

    public bool? RadiographAndLPIPassed { get; set; }

    public bool? IsNumberCorrect { get; set; }

    public bool? IsImpactCorrect { get; set; }

    public bool? ProcessSelected { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CommentSection { get; set; }

    [ForeignKey("WPSFormID")]
    [InverseProperty("AdditionalDetails")]
    public virtual WP WPSForm { get; set; }
}