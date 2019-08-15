using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CryptoMiningControlCenter.Models;
using CryptoMiningControlCenter.Models.Repository.EFCore;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Index()
        {
            return View(await _context.Miner.ToListAsync());
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
