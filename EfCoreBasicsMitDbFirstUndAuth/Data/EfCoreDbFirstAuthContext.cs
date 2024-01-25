using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace EfCoreBasicsMitDbFirstUndAuth.Data
{
    public partial class EfCoreDbFirstAuthContext : DbContext
    {
        public EfCoreDbFirstAuthContext()
        {
        }

        public EfCoreDbFirstAuthContext(DbContextOptions<EfCoreDbFirstAuthContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppRole> AppRoles { get; set; }
        public virtual DbSet<AppUser> AppUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("name=AppDb");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

            modelBuilder.Entity<AppRole>(entity =>
            {
                entity.HasIndex(e => e.RoleName, "UQ__AppRoles__8A2B616046378888")
                    .IsUnique();

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.ToTable("AppUser");

                entity.HasIndex(e => e.Username, "UQ__AppUser__536C85E446FCEE0D")
                    .IsUnique();

                entity.Property(e => e.PwHash)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsFixedLength(true);

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsFixedLength(true);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AppUsers)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AppUser__RoleId__2F10007B");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
