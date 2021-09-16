using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityOfTulsaData {

   public class DatabaseContext : DbContext {

      public DatabaseContext(DbContextOptions options) : base(options) {
         //todo
      }

      public DbSet<FireEvent> FireEvents { get; set; }

      public DbSet<FireEventVehicle> FireEventVehicles { get; set; }

      public DbSet<FireVehicle> FireVehicles { get; set; }

      protected override void OnModelCreating(ModelBuilder modelBuilder) {

         //modelBuilder.Entity<Book>()
         //       .HasMany(x => x.Tags)
         //        .WithMany(x => x.Books)
         //        .UsingEntity<BookTag>(
         //            x => x.HasOne(x => x.Tag)
         //            .WithMany().HasForeignKey(x => x.TagId),
         //            x => x.HasOne(x => x.Book)
         //           .WithMany().HasForeignKey(x => x.BookId));

         modelBuilder.Entity<FireEvent>()
            .HasMany(e => e.FireVehicles)
            .WithMany(v => v.FireEvents)
            .UsingEntity<FireEventVehicle>(
               x => x.HasOne(x => x.FireVehicle).WithMany().HasForeignKey(x => x.FireVehicleID),
               x => x.HasOne(x => x.FireEvent).WithMany().HasForeignKey(x => x.FireEventID)
            );

         //modelBuilder.Entity<FireEvent>()
         //   .HasMany<FireVehicle>(e => e.Vehicles)
         //   .WithMany(v => v.Events)
         //   .Map(ve =>
         //      {
         //         ve.MapLeftKey("FireEventID");
         //         ve.MapRightKey("FireVehicleID");
         //         ve.ToTable("FireEventVehicles");
         //      }
         //   );

         //modelBuilder.Entity<FireEvent>()
         //   .HasMany(e => e.Vehicles)
         //   .WithOne(v => v.FireEventParent)
         //   ;

         //modelBuilder.Entity<FireVehicle>()
         //   .HasOne(v => v.FireEventParent)
         //   .WithMany(e => e.Vehicles)
         //   ;
      }
   }
}
