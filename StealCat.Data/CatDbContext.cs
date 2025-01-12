using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using StealCat.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealCat.Data
{
    public class CatDbContext : DbContext
    {
            public DbSet<Cat> Cats { get; set; }
            public DbSet<Tag> Tags { get; set; }
            public DbSet<CatTag> CatTags { get; set; }  // Join table for many-to-many relationship

            public CatDbContext(DbContextOptions<CatDbContext> options) : base(options) { }

            // Configure the model
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // Configure Cat entity
                modelBuilder.Entity<Cat>()
                    .HasKey(c => c.Id);  // Cat entity has primary key on 'Id'

                modelBuilder.Entity<Cat>()
                    .Property(c => c.CatId)
                    .IsRequired();  // CatId is required

                modelBuilder.Entity<Cat>()
                    .Property(c => c.Width)
                    .IsRequired();  // Width is required

                modelBuilder.Entity<Cat>()
                    .Property(c => c.Height)
                    .IsRequired();  // Height is required

                modelBuilder.Entity<Cat>()
                    .Property(c => c.Created)
                    .IsRequired();  // Created timestamp is required

                modelBuilder.Entity<Cat>()
                    .HasMany(c => c.CatTags)
                    .WithOne(ct => ct.Cat)
                    .HasForeignKey(ct => ct.CatId);

                // Configure Tag entity
                modelBuilder.Entity<Tag>()
                    .HasKey(t => t.Id);  // Tag entity has primary key on 'Id'

                modelBuilder.Entity<Tag>()
                    .Property(t => t.Name)
                    .IsRequired();  // Tag name is required

                modelBuilder.Entity<Tag>()
                    .Property(t => t.Created)
                    .IsRequired();  // Created timestamp is required

                modelBuilder.Entity<Tag>()
                    .HasMany(t => t.CatTags)
                    .WithOne(ct => ct.Tag)
                    .HasForeignKey(ct => ct.TagId);

                // Configure the many-to-many relationship through CatTag join table
                modelBuilder.Entity<CatTag>()
                    .HasKey(ct => new { ct.CatId, ct.TagId });  // Composite key

                modelBuilder.Entity<CatTag>()
                    .HasOne(ct => ct.Cat)
                    .WithMany(c => c.CatTags)
                    .HasForeignKey(ct => ct.CatId);

                modelBuilder.Entity<CatTag>()
                    .HasOne(ct => ct.Tag)
                    .WithMany(t => t.CatTags)
                    .HasForeignKey(ct => ct.TagId);
            }
    }
}
