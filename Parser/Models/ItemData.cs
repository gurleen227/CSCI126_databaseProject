using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Parser.Models
{
    [Index(nameof(ItemId), nameof(TrendValue), nameof(CurrentPrice), nameof(Date))]
    public class ItemData
    {
        [Key]
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string Name { get; set; } = null!;
        public double CurrentPrice { get; set; }
        public string Trend { get; set; } = null!;
        public double TrendValue { get; set; }
        public string ImageIcon { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}