using Sladko_Izkushenie.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sladko_Izkushenie.Models
{
    public class CategoryVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Това поле е задължително!")]
        [Display(Name = "Тип категория")]
        public string Category_Type { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
