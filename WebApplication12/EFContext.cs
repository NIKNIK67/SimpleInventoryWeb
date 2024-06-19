using Microsoft.EntityFrameworkCore;
using WebApplication12.Models;

namespace WebApplication12
{
    public class EFContext : DbContext
    {
        public static DbSet<Item> Items { get; set; }
        public EFContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.("test1");
            optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message), LogLevel.Error);
        }
        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<Item>(entity =>
            {
                entity.HasKey(x => x.id);
                entity.Property(x => x.name);
                entity.Property(x => x.description);
                entity.Property(x => x.isDeleted);

            });
            base.OnModelCreating(model);

        }
    }
}
