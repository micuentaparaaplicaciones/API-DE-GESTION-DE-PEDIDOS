using System.ComponentModel.DataAnnotations;

namespace API.Dtos.CustomerDtos
{
    public class CustomerCreateDto
    {
        [Required]
        [StringLength(15, MinimumLength = 9)]
        public string Identification { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20, MinimumLength = 8)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int CreatedBy { get; set; }
    }
}
