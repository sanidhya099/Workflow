﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TinaKingSystem.BLL;
using static TinaKingSystem.ViewModels.WPSView;

namespace TinaKingSystem.Entities;

[Table("Drawing")]
public partial class Drawing
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int ID { get; set; }
    [Required] public int PackageID { get; set; }
    [Required][Column(TypeName = "date")] public DateTime RegistDate { get; set; } // 'date' SQL type is mapped to 'DateTime' in .NET
    [Required][StringLength(512)][Unicode(false)] public string Detail { get; set; }
}
