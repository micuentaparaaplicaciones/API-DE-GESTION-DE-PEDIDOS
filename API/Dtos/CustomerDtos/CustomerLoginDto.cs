using System.ComponentModel.DataAnnotations;

namespace API.Dtos.CustomerDtos
{
    public class CustomerLoginDto
    {
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        public string Password { get; set; } = string.Empty;
    }
}
