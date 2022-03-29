using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sladko_Izkushenie.Data
{
    public class Category
    {
        public int Id { get; set; }
        public string Category_Type { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
