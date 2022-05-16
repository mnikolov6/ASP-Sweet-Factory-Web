using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sladko_Izkushenie.Data
{
    public class Product
    {
        public int Id { get; set; }
        [Display(Name = "Име на продукт")]
        public string Name { get; set; }
        [Display(Name = "Грамаж")]
        public float Weight { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Път на снимката")]
        public string ImgURL { get; set; }
        [Display(Name = "Цена")]
        public float Price { get; set; }
        [Display(Name = "Добавено на")]
        public DateTime Time_of_register { get; set; }
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }
        [Display(Name = "Категория")]
        public Category Category { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
