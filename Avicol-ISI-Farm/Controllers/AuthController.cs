using System.Text.RegularExpressions;
using Avicol_ISI_Farm.Context;
using Avicol_ISI_Farm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Avicol_ISI_Farm.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        bool _authenticated=false;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(string name, string phone, string email, string password, string password2, string address)
        {
            if (await _context.Users.AnyAsync(i => i.Name == name))
            {
                ViewBag.Message = "Numele este deja înregistrat";
                return View("Register");
            }

            if (await _context.Users.AnyAsync(i => i.Email == email))
            {
                ViewBag.Message = "Email-ul există deja";
                return View("Register");
            }

            if (await _context.Users.AnyAsync(i => i.Phone == phone))
            {
                ViewBag.Message = "Numărul de telefon există deja";
                return View("Register");
            }

            if (phone.Length < 9 || phone.Length > 9)
            {
                ViewBag.Message = "Introdu un număr de telefon valid";
                return View("Register");
            }

            if (!Regex.IsMatch(name, @"^[A-Za-z ]{2,}$"))
            {
                ViewBag.Message = "Numele trebuie să conțină doar litere și spații, cu cel puțin 2 caractere.";
                return View("Register");
            }

            if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                ViewBag.Message = "Introdu un email valid";
                return View("Register");
            }

            if (password.Length < 6)
            {
                ViewBag.Message = "Parola trebuie să fie de cel puțin 6 caractere";
                return View("Register");
            }

            if (password != password2)
            {
                ViewBag.Message = "Parolele nu se potrivesc";
                return View("Register");
            }

            string passwrodHashed = BCrypt.Net.BCrypt.HashPassword(password);
            var newUser = new User
            {
                Name = name,
                Email = email,
                Password = passwrodHashed,
                Address = address,
                Created_At = DateTime.Now,
                Phone = phone
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> LoginUser(string name, string password)
        {
            if (name == "" || password == "")
            {
                ViewBag.Message = "Te rog completează toate câmpurile";
                return View();
            }
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Name == name);

            if (user == null)
            {
                ViewBag.Message = "Utilizatorul nu există";
                return View("Register");
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                ViewBag.Message = "Parolă incorectă";
                return View("Login");
            }

            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("_authenticated", "true");
            _authenticated = true;

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}