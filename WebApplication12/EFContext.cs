using Microsoft.EntityFrameworkCore;
using WebApplication12.Models;

namespace WebApplication12
{
    public class EFContext : DbContext
    {
        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public EFContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionstring = $"Server=10.1.0.2;Database=test;Uid=root;Pwd=myveryhardandveryhiddenpassword";
            optionsBuilder.UseMySql(connectionstring, ServerVersion.AutoDetect(connectionstring));
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
                entity.HasMany(x => x.Orders).WithMany(x=>x.Items);

            });
            model.Entity<Order>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasMany(x => x.Items).WithMany(x=>x.Orders);
            });
            base.OnModelCreating(model);

        }
    }
}
