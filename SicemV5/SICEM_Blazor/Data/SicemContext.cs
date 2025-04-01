using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SICEM_Blazor.Models
{
    public partial class SicemContext : DbContext
    {
        public SicemContext()
        {
        }

        public SicemContext(DbContextOptions<SicemContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CatMessagesTemplate> CatMessagesTemplates { get; set; }
        public virtual DbSet<CatOpcione> CatOpciones { get; set; }
        public virtual DbSet<DetModsOficina> DetModsOficinas { get; set; }
        public virtual DbSet<ModsOficina> ModsOficinas { get; set; }
        public virtual DbSet<OprOpcione> OprOpciones { get; set; }
        public virtual DbSet<OprSesione> OprSesiones { get; set; }
        public virtual DbSet<Ruta> Rutas { get; set; }
        public virtual DbSet<RutasLocation> RutasLocations { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("name=ConnectionStrings:SICEM");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CatMessagesTemplate>(entity =>
            {
                entity.ToTable("Cat_MessagesTemplate", "Notificacion");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Mensaje)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UltimaModificacion)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<CatOpcione>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Cat_Opciones");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("descripcion");

                entity.Property(e => e.IdOpcion)
                    .HasColumnType("numeric(4, 0)")
                    .HasColumnName("id_opcion");

                entity.Property(e => e.Inactivo).HasColumnName("inactivo");
            });

            modelBuilder.Entity<DetModsOficina>(entity =>
            {
                entity.ToTable("Det_ModsOficina");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("descripcion");

                entity.Property(e => e.IdModif).HasColumnName("id_modif");

                entity.Property(e => e.ValorAnt)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("valor_ant");

                entity.Property(e => e.ValorNuevo)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("valor_nuevo");

                entity.HasOne(d => d.IdModifNavigation)
                    .WithMany(p => p.DetModsOficinas)
                    .HasForeignKey(d => d.IdModif)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ModificacionesOficina_ID");
            });

            modelBuilder.Entity<ModsOficina>(entity =>
            {
                entity.ToTable("ModsOficina");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("descripcion");

                entity.Property(e => e.Fecha)
                    .HasColumnType("datetime")
                    .HasColumnName("fecha");

                entity.Property(e => e.IdOficina).HasColumnName("id_oficina");

                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

                entity.Property(e => e.Tabla)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("tabla");

                entity.HasOne(d => d.IdOficinaNavigation)
                    .WithMany(p => p.ModsOficinas)
                    .HasForeignKey(d => d.IdOficina)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Oficina_ID");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.ModsOficinas)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Usuario_ID");
            });

            modelBuilder.Entity<OprOpcione>(entity =>
            {
                entity.ToTable("opr_opciones");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdOpcion).HasColumnName("id_opcion");

                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            });

            modelBuilder.Entity<OprSesione>(entity =>
            {
                entity.ToTable("Opr_Sesiones", "Global");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Caducidad)
                    .HasColumnType("datetime")
                    .HasColumnName("caducidad");

                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

                entity.Property(e => e.Inicio)
                    .HasColumnType("datetime")
                    .HasColumnName("inicio");

                entity.Property(e => e.IpAddress)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ip_address");

                entity.Property(e => e.MacAddress)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("mac_address");
            });

            modelBuilder.Entity<Ruta>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Alias)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("alias");

                entity.Property(e => e.Alterno).HasColumnName("alterno");

                entity.Property(e => e.BaseDatos)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Base_Datos");

                entity.Property(e => e.Contraseña)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Desconectado)
                    .HasColumnName("desconectado")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IdZona).HasColumnName("id_zona");

                entity.Property(e => e.Inactivo).HasDefaultValueSql("((0))");

                entity.Property(e => e.Oficina)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("oficina");

                entity.Property(e => e.Ruta1)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("ruta");

                entity.Property(e => e.Servidor)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ServidorA)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Servidor_A");

                entity.Property(e => e.Usuario)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RutasLocation>(entity =>
            {
                entity.ToTable("RutasLocation");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdRuta).HasColumnName("id_ruta");

                entity.Property(e => e.Latitude)
                    .IsRequired()
                    .HasMaxLength(24)
                    .IsUnicode(false)
                    .HasColumnName("latitude");

                entity.Property(e => e.Longitude)
                    .IsRequired()
                    .HasMaxLength(24)
                    .IsUnicode(false)
                    .HasColumnName("longitude");

                entity.HasOne(d => d.IdRutaNavigation)
                    .WithMany(p => p.RutasLocations)
                    .HasForeignKey(d => d.IdRuta)
                    .HasConstraintName("FK_RutaLocation_Ruta");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(e => e.Usuario1, "IX_Usuarios")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Administrador)
                    .HasColumnName("administrador")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CfgOfi)
                    .HasColumnName("cfg_ofi")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CfgOpc)
                    .HasColumnName("cfg_opc")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Contraseña)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("contraseña");

                entity.Property(e => e.Inactivo)
                    .HasColumnName("inactivo")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("nombre");

                entity.Property(e => e.Oficinas)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("oficinas");

                entity.Property(e => e.Opciones)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("opciones");

                entity.Property(e => e.UltimaModif).HasColumnType("datetime");

                entity.Property(e => e.Usuario1)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("usuario");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
