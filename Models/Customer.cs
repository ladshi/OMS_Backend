using System.ComponentModel.DataAnnotations;

namespace OMS_Backend.Models
{
    public class Customer : BaseEntity
    {
        [Required]
        public string CustomerCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }
    }
}
