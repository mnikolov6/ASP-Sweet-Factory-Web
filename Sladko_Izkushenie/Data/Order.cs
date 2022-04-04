using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sladko_Izkushenie.Data
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        [Display(Name = "Order Date")]
        [DataType(DataType.Date)]
        public DateTime Order_date { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
