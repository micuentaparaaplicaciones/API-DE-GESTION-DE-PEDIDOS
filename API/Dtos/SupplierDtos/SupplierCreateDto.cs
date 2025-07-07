using System.ComponentModel.DataAnnotations;

namespace API.Dtos.SupplierDtos
{
    public class SupplierCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public int? CreatedBy { get; set; }
    }
}
