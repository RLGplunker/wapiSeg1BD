using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wapiSeg1BD.Models;
using wapiSeg1BD.ModelDb;

namespace wapiSeg1BD.Context
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {
                
        }

        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            MakeRolAdmin(builder);
            OnModelCreatingProducts(builder);
        }
        


        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Sale> Sale { get; set; }


        #region Métodos Privados

        private void MakeRolAdmin(ModelBuilder builder)
        {
            var roleAdmin = new IdentityRole()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "admin",
                NormalizedName = "admin"
            };

            builder.Entity<IdentityRole>().HasData(roleAdmin);
            base.OnModelCreating(builder);
        }

        private void OnModelCreatingProducts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(200)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("Nombre_Producto")
                    .IsUnique();

                entity.Property(e => e.Alias).HasMaxLength(100);

                entity.Property(e => e.DeletionDate).HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.IdCategoryNavigation)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.IdCategory)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Category");
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.Property(e => e.SaleDate).HasColumnType("datetime");

                entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 2)");
            });
            
        }
        #endregion

    }
}
