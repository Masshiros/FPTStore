using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FPTStore.Models
{
    public class ProductImage
    {
        [Key]
        public int ImageId { get; set; }
        public string ImageUrl {get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        [JsonIgnore]
        public Product Product { get; set; }
    }
}
