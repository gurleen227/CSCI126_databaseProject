using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Parser.Models
{
    [Index(nameof(ItemId), nameof(TrendValue), nameof(CurrentPrice), nameof(Date))]
    public class ItemData
    {
        [Key]
        public int Id { get; set; }
        public int ItemId { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(80)]
        public string Name { get; set; } = null!;
        public double CurrentPrice { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(10)]
        public string Trend { get; set; } = null!;
        public double TrendValue { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(90)]
        public string ImageIcon { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}