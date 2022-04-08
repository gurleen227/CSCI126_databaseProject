using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Parser.Models;

public class ItemDb : DbContext
{
    public DbSet<ItemList> ItemList { get; set; } = null!;
    public DbSet<ItemIcon> ItemIcons { get; set; } = null!;
    public DbSet<ItemData> ItemHistory { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql("server=localhost;database=itemdb;user=root;password=scape", new MariaDbServerVersion(new Version(10, 7, 3)));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<ItemList>()
        //     .HasOne(b => b.Icon)
        //     .WithOne(i => i.Item)
        //     .HasForeignKey<ItemIcon>(b => b.ImageIconForeignKey);

        // modelBuilder.Entity<ItemList>()
        //     .HasMany(p => p.ItemData)
        //     .WithOne(o => o.ItemList)
        //     .HasForeignKey(s => s.ItemDataFK);
        //.HasPrincipalKey("ItemId");
        //.UsingEntity(j => j.ToTable("PostTags"));
    }
}