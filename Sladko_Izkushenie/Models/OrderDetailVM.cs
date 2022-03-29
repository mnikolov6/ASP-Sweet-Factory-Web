using Sladko_Izkushenie.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sladko_Izkushenie.Models
{
    public class OrderDetailVM
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public float Quantity { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
