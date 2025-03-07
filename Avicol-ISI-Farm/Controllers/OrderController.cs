using Avicol_ISI_Farm.Context;
using Avicol_ISI_Farm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Avicol_ISI_Farm.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Add()
        {
            ViewBag.Products = await _context.Products.ToListAsync();
            return View();
        }

        public async Task<IActionResult> All()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");

            }
            int idUser = Int32.Parse(userId);


            var orders = await _context.Orders
                .Where(o => o.UserId == idUser)
                .Include(o => o.User)
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .ToListAsync();

            if (orders == null)
            {
                return NotFound();
            }
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrder(int productId, int quantity, string customer, string status)
        {
            string userId = HttpContext.Session.GetString("UserId");



            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
           
            }

            

            User user = await _context.Users.FindAsync(Convert.ToInt32(userId));


            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                ViewBag.Message = "Product does not exist";
                ViewBag.Products = await _context.Products.ToListAsync();
                return View("Add");
            }


            if (quantity <= 0)
            {
                ViewBag.Message = "Quantity must be greater than zero";
                ViewBag.Products = await _context.Products.ToListAsync();
                return View("Add");
            }


            if (quantity > product.Quantity)
            {
                ViewBag.Message = "Not enough stock available";
                ViewBag.Products = await _context.Products.ToListAsync();
                return View("Add");
            }


            bool customerExists = await _context.Users.AnyAsync(u => u.Name == customer);
            if (!customerExists)
            {
                ViewBag.Message = "Customer does not exist";
                ViewBag.Products = await _context.Products.ToListAsync();
                return View("Add");
            }

            var customerO = await _context.Users.FirstOrDefaultAsync(u => u.Name == customer);

            int idUser = Int32.Parse(userId);

            string userAddress = await _context.Users
                .Where(u => u.Id == idUser)
                .Select(u => u.Address)
                .FirstOrDefaultAsync();

            decimal totalPrice= quantity*product.Price;

            var newOrder = new Order
            {
                CreatedAt = DateTime.Now,
                UserId = customerO.Id,
                User = customerO,
                Adress=userAddress,
                TotalPrice= totalPrice,
                Status="Pending"
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();


            var orderProduct = new OrderProduct
            {
                OrderId = newOrder.Id,
                Order = newOrder,
                ProductId = product.Id,
                Product = product,
                Quantity = quantity,
                PriceAtOrder = product.Price
            };

            _context.OrderProducts.Add(orderProduct);

            HttpContext.Session.SetString("UserId", user.Id.ToString());

            product.Quantity -= quantity;
            _context.Products.Update(product);

            await _context.SaveChangesAsync();

            return RedirectToAction("All");
        }
    }
}