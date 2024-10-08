﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TinaKingSystem.Entities;

namespace TinaKingSystem.DAL;

public partial class WFS_2590Context : DbContext
{
    public WFS_2590Context(DbContextOptions<WFS_2590Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<EventItem> Events { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }
    public virtual DbSet<Drawing> Drawings { get; set; }

    public virtual DbSet<DrawingRequest> DrawingRequests { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }


    public virtual DbSet<Package> Packages { get; set; }

    public virtual DbSet<WP> WPs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientID).HasName("PK__Client__E67E1A0489A114ED");

            entity.Property(e => e.Role).HasDefaultValueSql("('Client')");
        });

        modelBuilder.Entity<DrawingRequest>(entity =>
        {
            entity.HasKey(e => e.DrawingRequestID).HasName("PK__DrawingR__D882DC00B0E8F233");

            entity.Property(e => e.DrawingRequestID).ValueGeneratedNever();

            entity.HasOne(d => d.Employee).WithMany(p => p.DrawingRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DrawingRequest_Employee");

            entity.HasOne(d => d.Package).WithMany(p => p.DrawingRequests).HasConstraintName("FK__DrawingRe__Packa__5CD6CB2B");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeID).HasName("PK__Employee__7AD04FF14ECD3F75");
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.PackageID).HasName("PK__Package__322035ECC80DD3B9");

            entity.HasOne(d => d.Client).WithMany(p => p.Packages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Package_Client");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}