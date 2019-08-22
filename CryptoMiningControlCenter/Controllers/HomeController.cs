using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CryptoMiningControlCenter.Models;
using CryptoMiningControlCenter.Models.Repository.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace CryptoMiningControlCenter.Controllers
{
    public class HomeController : Controller
    {
        private readonly CryptoMiningContext _context;

        public HomeController(CryptoMiningContext context)
        {
            _context = context;
        }

        // GET: Workers
        public async Task<IActionResult> Index(string searchString)
        {
            var username = HttpContext.Session.GetString("username");
            if(username == null)
            {
                return RedirectToAction("Index", "Account");
            }

            var miners = from s in _context.Miner select s;
            ViewData["CurrentFilter"] = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                miners = miners.Where(s => s.Location.Contains(searchString));
            }

            return View(await miners.AsNoTracking().ToListAsync());
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
