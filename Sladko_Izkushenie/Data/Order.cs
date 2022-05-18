using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sladko_Izkushenie.Data
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        [Display(Name = "Дата на поръчка")]
        [DataType(DataType.Date)]
        public DateTime Order_date { get; set; }
        public bool Final { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
