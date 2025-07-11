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
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureUser(modelBuilder);
            ConfigureCustomer(modelBuilder);
            ConfigureCategory(modelBuilder);
            ConfigureSupplier(modelBuilder);
            ConfigureProduct(modelBuilder);
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
                    .HasColumnName("CREATIONDATE")
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

                entity.Property(c => c.CreationDate)
                    .HasColumnName("CREATIONDATE")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(c => c.ModificationDate)
                    .HasColumnName("MODIFICATIONDATE")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(s => s.CreatedBy)
                    .HasColumnName("CREATEDBY")
                    .IsRequired();

                entity.Property(s => s.ModifiedBy)
                    .HasColumnName("MODIFIEDBY")
                    .IsRequired(false);

                entity.Property(c => c.RowVersion)
                    .HasColumnName("ROWVERSION")
                    .HasDefaultValue(0)
                    .IsConcurrencyToken();

                entity.HasOne(c => c.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(c => c.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_CUSTOMERS_CREATEDBY");

                entity.HasOne(c => c.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(c => c.ModifiedBy)
                    .OnDelete(DeleteBehavior.Restrict)
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

                entity.Property(c => c.CreationDate)
                    .HasColumnName("CREATIONDATE")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(c => c.ModificationDate)
                    .HasColumnName("MODIFICATIONDATE")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(s => s.CreatedBy)
                    .HasColumnName("CREATEDBY")
                    .IsRequired();

                entity.Property(s => s.ModifiedBy)
                    .HasColumnName("MODIFIEDBY")
                    .IsRequired(false);

                entity.Property(c => c.RowVersion)
                    .HasColumnName("ROWVERSION")
                    .HasDefaultValue(0)
                    .IsConcurrencyToken();

                entity.HasOne(c => c.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(c => c.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_CATEGORIES_CREATEDBY");

                entity.HasOne(c => c.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(c => c.ModifiedBy)
                    .OnDelete(DeleteBehavior.Restrict)
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

                entity.Property(s => s.CreationDate)
                    .HasColumnName("CREATIONDATE")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(s => s.ModificationDate)
                    .HasColumnName("MODIFICATIONDATE")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(s => s.CreatedBy)
                    .HasColumnName("CREATEDBY")
                    .IsRequired();

                entity.Property(s => s.ModifiedBy)
                    .HasColumnName("MODIFIEDBY")
                    .IsRequired(false);

                entity.Property(s => s.RowVersion)
                    .HasColumnName("ROWVERSION")
                    .HasDefaultValue(0)
                    .IsConcurrencyToken();

                entity.HasOne(s => s.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(c => c.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_SUPPLIERS_CREATEDBY");

                entity.HasOne(s => s.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(c => c.ModifiedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_SUPPLIERS_MODIFIEDBY");
            });
        }

        private void ConfigureProduct(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("PRODUCTS");

                entity.HasKey(p => p.Key);

                entity.Property(p => p.Key)
                    .HasColumnName("KEY")
                    .ValueGeneratedOnAdd();

                entity.Property(p => p.Image)
                    .HasColumnName("IMAGE")
                    .HasColumnType("BLOB")
                    .IsRequired();

                entity.Property(p => p.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(p => p.Detail)
                    .HasColumnName("DETAIL")
                    .HasColumnType("CLOB")
                    .IsRequired();

                entity.Property(p => p.Price)
                    .HasColumnName("PRICE")
                    .HasColumnType("NUMBER(10,2)")
                    .IsRequired();

                entity.Property(p => p.AvailableQuantity)
                    .HasColumnName("AVAILABLEQUANTITY")
                    .IsRequired();

                entity.Property(p => p.CreationDate)
                    .HasColumnName("CREATIONDATE")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(p => p.ModificationDate)
                    .HasColumnName("MODIFICATIONDATE")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(p => p.CreatedBy)
                    .HasColumnName("CREATEDBY")
                    .IsRequired();

                entity.Property(p => p.ModifiedBy)
                    .HasColumnName("MODIFIEDBY")
                    .IsRequired(false);

                entity.Property(p => p.SuppliedBy)
                    .HasColumnName("SUPPLIEDBY")
                    .IsRequired();

                entity.Property(p => p.CategorizedBy)
                    .HasColumnName("CATEGORIZEDBY")
                    .IsRequired();

                entity.Property(p => p.RowVersion)
                    .HasColumnName("ROWVERSION")
                    .HasDefaultValue(0)
                    .IsConcurrencyToken();

                entity.HasOne(p => p.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(p => p.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_PRODUCTS_CREATEDBY");

                entity.HasOne(p => p.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(p => p.ModifiedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_PRODUCTS_MODIFIEDBY");

                entity.HasOne(p => p.SuppliedBySupplier)
                    .WithMany()
                    .HasForeignKey(p => p.SuppliedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_PRODUCTS_SUPPLIEDBY");

                entity.HasOne(p => p.CategorizedByCategory)
                    .WithMany()
                    .HasForeignKey(p => p.CategorizedBy)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_PRODUCTS_CATEGORIZEDBY");
            });
        }
    }
}
