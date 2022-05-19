using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sladko_Izkushenie.Data
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [Display(Name ="Продукт")]
        public Product Product { get; set; }
        [Display(Name = "Количество")]
        public float Quantity { get; set; }
        [Display(Name ="Номер на поръчка")]
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
