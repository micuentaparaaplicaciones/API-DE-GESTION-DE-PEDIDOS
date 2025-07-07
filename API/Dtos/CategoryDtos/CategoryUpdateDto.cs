using System.ComponentModel.DataAnnotations;

namespace API.Dtos.CategoryDtos
{
    public class CategoryUpdateDto
    {
        [Required]
        public int Key { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public int? ModifiedBy { get; set; }

        public long RowVersion { get; set; } 
    }
}
