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
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public async Task<IActionResult> Index(string searchString = null)
        {
            //var username = HttpContext.Session.GetString("username");
            //if (username == null)
            //{
            //    return RedirectToAction("Index", "Account");
            //}

            var miners = await _context.Miner.AsNoTracking().ToListAsync(); // from s in _context.Miner select s;
            ViewData["CurrentFilter"] = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                miners = miners.Where(s => s.Location.Equals(searchString)).ToList();
            }

            var minererror = await _context.MinerError.Where(x => x.Isresolve == false).ToListAsync();
            var errormessage = "";
            if (minererror != null)
            {
                errormessage += "错误提醒：以下矿池再爬虫程序运行中出现错误：";
                foreach (var me in minererror)
                {
                    errormessage += me.Pooltype + " ";
                }
            }

            ViewBag.Error = errormessage;

            return View(miners);
        }


        //public List<SelectListItem> Options { get; set; }
        //public void OnGet()
        //{
        //    Options = _context.Miner.Distinct().Select(a =>
        //                                  new SelectListItem
        //                                  {
        //                                      Value = a.Location,
        //                                      Text = a.Location
        //                                  }).ToList();
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
