using System.ComponentModel.DataAnnotations;

namespace AuthServer.Models
{
    public record UserValidationRequestModel
    {
        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
