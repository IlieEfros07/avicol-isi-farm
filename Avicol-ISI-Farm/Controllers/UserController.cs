using Avicol_ISI_Farm.Context;
using Avicol_ISI_Farm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Avicol_ISI_Farm.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Account()
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

            return View(user);
        }

        public async Task<IActionResult> Edit()
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

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserDetails(string name, string email, string phone, string address)
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

            bool userExists = await _context.Users.AnyAsync(u => (u.Name == name || u.Email == email) && u.Id != Convert.ToInt32(userId));

            if (userExists)
            {
                TempData["ErrorMessage"] = "Un utilizator cu acest Nume/Email există deja";
                return RedirectToAction("Edit");
            }

            bool phoneExists = await _context.Users.AnyAsync(u => u.Phone == phone);

            if (phoneExists)
            {
                TempData["ErrorMessage"] = "Un utilizator cu acest număr de telefon există deja";
                return RedirectToAction("Edit");
            }

            user.Name = name;
            user.Email = email;
            user.Phone = phone;
            user.Address = address;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Detaliile contului au fost actualizate cu succes";
            return RedirectToAction("Edit");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserPassword(string oldPassword, string newPassword, string confirmPassword)
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

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
            {
                TempData["ErrorMessage"] = "Parola veche este incorectă";
                return RedirectToAction("Edit");
            }

            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "Parolele nu se potrivesc";
                return RedirectToAction("Edit");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            user.Password = passwordHash;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Detaliile contului au fost actualizate cu succes";
            return RedirectToAction("Edit");
        }
    }
}