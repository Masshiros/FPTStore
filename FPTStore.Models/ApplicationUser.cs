using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FPTStore.Models

{
    public class ApplicationUser : IdentityUser
    {
        
        public string? Name {get; set; }
        public string? UserStreetAddress { get; set; }
        public string? UserCity { get; set; }
        public string? UserState { get; set; }
        public string? UserPostalCode { get; set; }

        public int? CompanyId {get; set; }
        [ForeignKey("CompanyId")]
        [ValidateNever]
        public Company Company { get; set; }
    }
}
