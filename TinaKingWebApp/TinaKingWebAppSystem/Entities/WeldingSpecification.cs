﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TinaKingSystem.Entities;

public partial class WeldingSpecification
{
    [Key]
    public int SpecID { get; set; }

    public int? WPSFormID { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string ModeOfTransfer { get; set; }

    public bool? BoolMOT { get; set; }

    public bool? MOTmatchPQR { get; set; }

    [ForeignKey("WPSFormID")]
    [InverseProperty("WeldingSpecifications")]
    public virtual WP WPSForm { get; set; }
}