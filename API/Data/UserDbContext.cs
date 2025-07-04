using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    /// <summary>
    /// Represents the database context for user management. 
    /// </summary>
    public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USERS");

                entity.HasKey(u => u.Key);

                entity.Property(u => u.Key)
                    .HasColumnName("ID")
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
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(u => u.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(u => u.ModifiedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
