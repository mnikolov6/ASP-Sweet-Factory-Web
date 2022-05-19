using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sladko_Izkushenie.Data;
using Sladko_Izkushenie.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace Sladko_Izkushenie.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public const string OrderSessionKey = "OrderId";

        public OrderDetailsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: OrderDetails
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var orderId = GetOrderId();
            if (orderId == null)
            {
                return RedirectToAction("Index", "Products");
            }
            var currentUser = _userManager.GetUserId(User);

            var applicationDbContext = _context.OrderDetails
                .Include(p => p.Product)
                .Include(o => o.Order)
                .Where(x => (x.OrderId == orderId) &&
                            (x.Order.Final == false) &&
                            (x.Order.UserId == currentUser)); //&& (x.Order.OrderedOn==DateTime.Now.Date)

            return View(await applicationDbContext.ToListAsync());
        }
        [NonAction]
        //метод за взимане на информацията от сесията за потребителя
        public int? GetOrderId()
        {
            return HttpContext.Session.GetInt32("OrderSessionKey");
        }
        public async Task<IActionResult> Calculate(int orderId)
        {
            var currentUser = _userManager.GetUserId(User);
            var dbOrderList = _context.OrderDetails
               .Include(p => p.Product)
               .Include(o => o.Order)
               .Where(x => (x.OrderId == orderId) &&
                           (x.Order.Final == false) &&
                           (x.Order.UserId == currentUser));
            decimal sum = 0;
            foreach (var item in dbOrderList)
            {
                sum += ((decimal)item.Product.Price * (decimal)item.Quantity);
            }
            //започва актуализиране на таблицата Orders /total=....; final=true
            Order order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            order.Final = true;
            order.Total = sum;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            //изтрива ОРДЕРид от сесията
            HttpContext.Session.Remove("OrderSessionKey");
            TempData["OrderActive"] = false;

            TempData["Message"] = "Успешно поръчахте на стойност " + sum.ToString();
            return RedirectToAction("Index", "Products");
            //return Content("SUM = " + sum.ToString());
            //return View(await applicationDbContext.ToListAsync());
        }
        // GET: OrderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM product)
        {
            TempData.Keep(); //запазване на всички ключ-стойности 

            if (!ModelState.IsValid) //при грешка в модела
            {
                ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
                return View();
            }

            if (GetOrderId() == null) //Ако потребителят няма поръчка досега от влизането си!!
            {
                Order order = new Order()
                {
                    UserId = _userManager.GetUserId(User),
                    Order_date = DateTime.Now
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetInt32("OrderSessionKey", order.Id);// Създавам запис в СЕСИЯТА за потребителя с Номер поръчка
                TempData["Message"] = "Имате поръчка, която не е завършена!";
                TempData["OrderActive"] = true;
            }

            //Ако потребителят ВЕЧЕ е направил поръчка!!
            int shoppingCardId = (int)GetOrderId();
            var orderItem = await _context.OrderDetails
                .SingleOrDefaultAsync(x => (x.ProductId == product.Id && x.OrderId == shoppingCardId));
            if (orderItem == null) //Ако поръчва друг/нов продукт се записва в OrderDetails
            {
                orderItem = new OrderDetail()
                {
                    ProductId = product.Id,
                    Quantity = product.Quantity,
                    OrderId = (int)GetOrderId()
                };
                _context.OrderDetails.Add(orderItem);
            }
            else //ако избира поръчан вече продукт се увеличава количеството му
            {
                orderItem.Quantity = orderItem.Quantity + product.Quantity;
                _context.OrderDetails.Update(orderItem);
            }
            await _context.SaveChangesAsync();
            //return Content("OK");
            return RedirectToAction("Index", "Products"); //??? къде да се върнем?
        }
        // GET: OrderDetails/Create
        //public IActionResult Create()
        //{
        //    ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id");
        //    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
        //    return View();
        //}

        //// POST: OrderDetails/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,ProductId,Quantity,OrderId")] OrderDetail orderDetail)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(orderDetail);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderDetail.OrderId);
        //    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderDetail.ProductId);
        //    return View(orderDetail);
        //}

        // GET: OrderDetails/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var orderDetail = await _context.OrderDetails.FindAsync(id);
        //    if (orderDetail == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderDetail.OrderId);
        //    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderDetail.ProductId);
        //    return View(orderDetail);
        //}

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Quantity,OrderId")] OrderDetail orderDetail)
        //{
        //    if (id != orderDetail.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(orderDetail);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!OrderDetailExists(orderDetail.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderDetail.OrderId);
        //    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderDetail.ProductId);
        //    return View(orderDetail);
        //}

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.Id == id);
        }
    }
}
