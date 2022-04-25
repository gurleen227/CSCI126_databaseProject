using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Parser.Models
{
    [Index(nameof(CurrentPrice), nameof(ItemListId))]
    [Index(nameof(Date), nameof(ItemListId))]
    public class ItemData
    {
        public int Id { get; set; }
        public double CurrentPrice { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(10)]
        public string Trend { get; set; } = null!;
        public double TrendValue { get; set; }
        public DateTime Date { get; set; }
        public int ItemListId { get; set; }
        public virtual ItemList? ItemList { get; set; }
    }
}