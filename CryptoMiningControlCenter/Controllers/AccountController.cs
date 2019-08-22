using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoMiningControlCenter.Models.Repository.EFCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CryptoMiningControlCenter.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly CryptoMiningContext _context;

        public AccountController(CryptoMiningContext context)
        {
            _context = context;
        }

        //[Route("")]
        [Route("index")]
        [Route("~/")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("login")]
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Login.Where(x => x.Username == username && x.Password == password).FirstOrDefault();
            if (user != null)
            {
                HttpContext.Session.SetString("username", username);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.error = "用户名或密码错误";
                return View("Index");
            }
        }

        [Route("logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("username");
            return RedirectToAction("Index");
        }
    }
}