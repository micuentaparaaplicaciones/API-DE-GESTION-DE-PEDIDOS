using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    /// <summary>
    /// Represents the application's database context, managing entities and their configurations. 
    /// </summary>
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureUser(modelBuilder);
            ConfigureCustomer(modelBuilder);
            ConfigureCategory(modelBuilder);
            ConfigureSupplier(modelBuilder);
        }

        private void ConfigureUser(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USERS");

                entity.HasKey(u => u.Key);

                entity.Property(u => u.Key)
                    .HasColumnName("KEY")
                    .ValueGeneratedOnAdd();

                entity.Property(u => u.Identification)
                    .HasColumnName("IDENTIFICATION")
                    .HasMaxLength(15)
                    .IsRequired();

                entity.HasIndex(u => u.Identification).IsUnique();

                entity.Property(u => u.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(u => u.Email)
                    .HasColumnName("EMAIL")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.Phone)
                    .HasColumnName("PHONE")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(u => u.Address)
                    .HasColumnName("ADDRESS")
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(u => u.Password)
                    .HasColumnName("PASSWORD")
                    .HasMaxLength(256)
                    .IsRequired();

                entity.Property(u => u.Role)
                    .HasColumnName("ROLE")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(u => u.RegistrationDate)
                    .HasColumnName("REGISTRATIONDATE")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(u => u.ModificationDate)
                    .HasColumnName("MODIFICATIONDATE")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(u => u.CreatedBy)
                    .HasColumnName("CREATEDBY");

                entity.Property(u => u.ModifiedBy)
                    .HasColumnName("MODIFIEDBY");

                entity.Property(u => u.RowVersion)
                    .HasColumnName("ROWVERSION")
                    .HasDefaultValue(0)
                    .IsConcurrencyToken();

                entity.HasOne(u => u.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(u => u.CreatedBy)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_USERS_CREATEDBY");

                entity.HasOne(u => u.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(u => u.ModifiedBy)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_USERS_MODIFIEDBY");

            });
        }

        private void ConfigureCustomer(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("CUSTOMERS");

                entity.HasKey(c => c.Key);

                entity.Property(c => c.Key)
                    .HasColumnName("KEY")
                    .ValueGeneratedOnAdd();

                entity.Property(c => c.Identification)
                    .HasColumnName("IDENTIFICATION")
                    .HasMaxLength(15)
                    .IsRequired();

                entity.HasIndex(c => c.Identification).IsUnique();

                entity.Property(c => c.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(c => c.Email)
                    .HasColumnName("EMAIL")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(c => c.Email).IsUnique();

                entity.Property(c => c.Phone)
                    .HasColumnName("PHONE")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(c => c.Address)
                    .HasColumnName("ADDRESS")
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(c => c.Password)
                    .HasColumnName("PASSWORD")
                    .HasMaxLength(256)
                    .IsRequired();

                entity.Property(c => c.RegistrationDate)
                    .HasColumnName("REGISTRATIONDATE")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(c => c.ModificationDate)
                    .HasColumnName("MODIFICATIONDATE")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(c => c.CreatedBy)
                    .HasColumnName("CREATEDBY");

                entity.Property(c => c.ModifiedBy)
                    .HasColumnName("MODIFIEDBY");

                entity.Property(c => c.RowVersion)
                    .HasColumnName("ROWVERSION")
                    .HasDefaultValue(0)
                    .IsConcurrencyToken();

                entity.HasOne(c => c.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(c => c.CreatedBy)
                    //.OnDelete(DeleteBehavior.Restrict)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_CUSTOMERS_CREATEDBY");

                entity.HasOne(c => c.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(c => c.ModifiedBy)
                    //.OnDelete(DeleteBehavior.Restrict)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_CUSTOMERS_MODIFIEDBY");

            });
        }

        private void ConfigureCategory(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("CATEGORIES");

                entity.HasKey(c => c.Key);

                entity.Property(c => c.Key)
                    .HasColumnName("KEY")
                    .ValueGeneratedOnAdd();

                entity.Property(c => c.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(c => c.Name).IsUnique();

                entity.Property(c => c.RegistrationDate)
                    .HasColumnName("REGISTRATIONDATE")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(c => c.ModificationDate)
                    .HasColumnName("MODIFICATIONDATE")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(c => c.CreatedBy)
                    .HasColumnName("CREATEDBY");

                entity.Property(c => c.ModifiedBy)
                    .HasColumnName("MODIFIEDBY");

                entity.Property(c => c.RowVersion)
                    .HasColumnName("ROWVERSION")
                    .HasDefaultValue(0)
                    .IsConcurrencyToken();

                entity.HasOne(c => c.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(c => c.CreatedBy)
                    //.OnDelete(DeleteBehavior.Restrict)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_CATEGORIES_CREATEDBY");

                entity.HasOne(c => c.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(c => c.ModifiedBy)
                    //.OnDelete(DeleteBehavior.Restrict)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_CATEGORIES_MODIFIEDBY");
            });
        }

        private void ConfigureSupplier(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("SUPPLIERS");

                entity.HasKey(s => s.Key);

                entity.Property(s => s.Key)
                    .HasColumnName("KEY")
                    .ValueGeneratedOnAdd();

                entity.Property(s => s.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(s => s.Name).IsUnique();

                entity.Property(s => s.RegistrationDate)
                    .HasColumnName("REGISTRATIONDATE")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(s => s.ModificationDate)
                    .HasColumnName("MODIFICATIONDATE")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(s => s.CreatedBy)
                    .HasColumnName("CREATEDBY");

                entity.Property(s => s.ModifiedBy)
                    .HasColumnName("MODIFIEDBY");

                entity.Property(s => s.RowVersion)
                    .HasColumnName("ROWVERSION")
                    .HasDefaultValue(0)
                    .IsConcurrencyToken();

                entity.HasOne(s => s.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(c => c.CreatedBy)
                    //.OnDelete(DeleteBehavior.Restrict)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_SUPPLIERS_CREATEDBY");

                entity.HasOne(s => s.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(c => c.ModifiedBy)
                    //.OnDelete(DeleteBehavior.Restrict)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_SUPPLIERS_MODIFIEDBY");
            });
        }

    }
}
