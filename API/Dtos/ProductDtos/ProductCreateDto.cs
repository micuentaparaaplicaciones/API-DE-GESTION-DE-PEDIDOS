using System.ComponentModel.DataAnnotations;

namespace API.Dtos.ProductDtos
{
    public class ProductCreateDto
    {
        [Required]
        public byte[] Image { get; set; } = [];

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Detail { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int AvailableQuantity { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public int SuppliedBy { get; set; }

        [Required]
        public int CategorizedBy { get; set; }
    }
}
