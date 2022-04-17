using Sladko_Izkushenie.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sladko_Izkushenie.Models
{
    public class ProductVM
    {
        public int Id { get; set; } 
        [Required(ErrorMessage = "Въведете името на продукта!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Въведете грамаж!")]
        public float Weight { get; set; }
        [Required(ErrorMessage ="Въведете кратко описание!")]
        public string Description { get; set; }
        public string ImgURL { get; set; }
        [Column(TypeName= "decimal(10,2)")]
        public float Price { get; set; }
        public DateTime Time_of_register { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string UserId { get; set; }
        public int Quantity { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
