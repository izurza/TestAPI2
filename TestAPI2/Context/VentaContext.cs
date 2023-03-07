using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TestAPI2.Models;

namespace TestAPI2.Context;

public partial class VentaContext : DbContext
{
    public VentaContext()
    {
    }

    public VentaContext(DbContextOptions<VentaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Detalle> Detalles { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente);

            entity.Property(e => e.IdCliente).HasColumnName("Id_Cliente");
            entity.Property(e => e.ApellidoCliente)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("Apellido_Cliente");
            entity.Property(e => e.DireccionCliente)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("Direccion_Cliente");
            entity.Property(e => e.EmailCliente)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("Email_Cliente");
            entity.Property(e => e.NombreCliente)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("Nombre_Cliente");
        });

        modelBuilder.Entity<Detalle>(entity =>
        {
            entity.HasKey(e => e.IdDetalle);

            entity.Property(e => e.IdDetalle).HasColumnName("Id_Detalle");
            entity.Property(e => e.FkFactura).HasColumnName("FK_Factura");
            entity.Property(e => e.FkProducto).HasColumnName("FK_Producto");

            entity.HasOne(d => d.FkFacturaNavigation).WithMany(p => p.Detalles)
                .HasForeignKey(d => d.FkFactura)
                .HasConstraintName("FK_Detalles_Facturas1");

            entity.HasOne(d => d.FkProductoNavigation).WithMany(p => p.Detalles)
                .HasForeignKey(d => d.FkProducto)
                .HasConstraintName("FK_Detalles_Productos1");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.IdFactura);

            entity.Property(e => e.IdFactura).HasColumnName("Id_Factura");
            entity.Property(e => e.Fecha).HasColumnType("date");
            entity.Property(e => e.FkCliente).HasColumnName("FK_Cliente");

            entity.HasOne(d => d.FkClienteNavigation).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.FkCliente)
                .HasConstraintName("FK_Facturas_Clientes");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto);

            entity.Property(e => e.IdProducto)
                .ValueGeneratedNever()
                .HasColumnName("Id_Producto");
            entity.Property(e => e.DescripcionProducto)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("Descripcion_Producto");
            entity.Property(e => e.NombreProducto)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("Nombre_Producto");
            entity.Property(e => e.PrecioProducto).HasColumnName("Precio_Producto");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
