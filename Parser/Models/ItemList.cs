using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Parser.Models
{
    [Index(nameof(ItemId), nameof(Name))]
    public class ItemList
    {
        public ItemList()
        {
            this.ItemData = new HashSet<ItemData>();
        }
        [Key]
        public int Id { get; set; }
        public int ItemId { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(80)]
        public string Name { get; set; } = null!;
        public virtual ItemIcon? Icon { get; set; }
        public virtual ICollection<ItemData> ItemData { get; set; }
    }
}