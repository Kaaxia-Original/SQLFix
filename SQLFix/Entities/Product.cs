using System.ComponentModel.DataAnnotations.Schema;

namespace SQLFix.Entities
{
    [Table("Products")]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public TimeSpan? CreatedAt { get; set; }
        public TimeSpan? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

}
