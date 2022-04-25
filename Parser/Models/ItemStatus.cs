using System.ComponentModel.DataAnnotations;

namespace Parser.Models
{
    public class ItemStatus
    {
        [Key]
        public int ItemId { get; set; }
        public string? MembersOnly { get; set; }
    }
}