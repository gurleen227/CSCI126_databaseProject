using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parser.Models
{
    public class ItemIcon
    {
        [Key]
        public int ItemId { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(255)]
        public string Description { get; set; } = null!;
        [Column(TypeName = "VARCHAR")]
        [StringLength(90)]
        public string ImageIcon { get; set; } = null!;
        public virtual ItemList? Item { get; set; }
    }
}