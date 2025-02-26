using Avicol_ISI_Farm.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Avicol_ISI_Farm.Models;
namespace Avicol_ISI_Farm.Controllers
{
    public class ProductController : Controller
    {

        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> AllAsync()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }
    }
}
