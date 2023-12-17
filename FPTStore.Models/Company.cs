using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FPTStore.Models
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }
        [Required]
        public string CompanyName { get; set;}
        public string? CompanyStreetAddress { get; set; }
        public string? CompanyCity { get; set; }
        public string? CompanyState { get; set; }
        public string? CompanyPostalCode { get; set; }
        public string? CompanyPhoneNumber { get; set; }
    }
}
