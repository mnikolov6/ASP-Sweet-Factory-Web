using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sladko_Izkushenie.Data
{
    public class User:IdentityUser
    {
        public string FullName { get; set; }
        
        public ICollection<Order> Orders { get; set; }
    }
}
