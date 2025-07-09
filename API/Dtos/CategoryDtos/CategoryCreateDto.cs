using System.ComponentModel.DataAnnotations;

namespace API.Dtos.CategoryDtos
{
    public class CategoryCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int CreatedBy { get; set; }
    }
}
