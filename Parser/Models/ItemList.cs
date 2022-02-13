using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Parser.Models
{
    [Index(nameof(ItemId))]
    public class ItemList
    {
        [Key]
        public int Id { get; set; }
        public int ItemId { get; set; }
    }
}