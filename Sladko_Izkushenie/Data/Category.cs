using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sladko_Izkushenie.Data
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "This field is required!")]
        [Display(Name = "Category Type")]
        public string Category_Type { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
