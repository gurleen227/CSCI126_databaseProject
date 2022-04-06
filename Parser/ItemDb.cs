using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Parser.Models;

public class ItemDb : DbContext
{
    public DbSet<ItemList> Items { get; set; } = null!;
    public DbSet<ItemData> ItemHistory { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql("server=localhost;database=itemdb;user=root;password=scape", new MariaDbServerVersion(new Version(10, 7, 3)));
    }

}