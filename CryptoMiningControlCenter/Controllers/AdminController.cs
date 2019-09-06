using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CryptoMiningControlCenter.Models.Repository.EFCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using OfficeOpenXml;

namespace CryptoMiningControlCenter.Controllers
{
    public class AdminController : Controller
    {
        private readonly CryptoMiningContext _context;

        public AdminController(CryptoMiningContext context)
        {
            _context = context;
        }

        // GET: AdminController
        public async Task<IActionResult> Index()
        {
            var username = HttpContext.Session.GetString("username");
            if (username == null)
            {
                return RedirectToAction("Index", "Account");
            }
            else
            {
                var user = await _context.Login.Where(x => x.Username == username).FirstOrDefaultAsync();
                if (user.Role == "admin")
                {
                    return View(await _context.Miner.ToListAsync());
                }
                else
                {
                    ViewBag.error = "你没有权限";
                    return View("Authorize");
                }
            }
        }

        // GET: AdminController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var miner = await _context.Miner
                .FirstOrDefaultAsync(m => m.Id == id);
            if (miner == null)
            {
                return NotFound();
            }

            return View(miner);
        }

        // GET: AdminController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Minertype,Location,Link,Pooltype,Active,Total,Inactive,Dead,Currentcalculation,Dailycalculation,Standardcalculation,Unit,Updatedate")] Miner miner)
        {

            if (!CheckPoolType(miner.Link, miner.Pooltype))
            {
                ViewBag.error = "你输入的观察者链接和矿池类型不匹配";
                return View(miner);
            }

            if (ModelState.IsValid)
            {
                _context.Add(miner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(miner);
        }

        // GET: AdminController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var miner = await _context.Miner.FindAsync(id);
            if (miner == null)
            {
                return NotFound();
            }
            return View(miner);
        }

        // POST: AdminController/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Minertype,Location,Link,Pooltype,Active,Total,Inactive,Dead,Currentcalculation,Dailycalculation,Standardcalculation,Unit,Updatedate")] Miner miner)
        {
            if (id != miner.Id)
            {
                return NotFound();
            }

            if(!CheckPoolType(miner.Link, miner.Pooltype))
            {
                ViewBag.error = "你输入的观察者链接和矿池类型不匹配";
                return View(miner);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(miner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MinerExists(miner.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(miner);
        }

        // GET: AdminController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var miner = await _context.Miner
                .FirstOrDefaultAsync(m => m.Id == id);
            if (miner == null)
            {
                return NotFound();
            }

            return View(miner);
        }

        // POST: AdminController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var miner = await _context.Miner.FindAsync(id);
            _context.Miner.Remove(miner);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MinerExists(int id)
        {
            return _context.Miner.Any(e => e.Id == id);
        }

        public bool CheckPoolType(string url, string pooltype)
        {
            if (pooltype == "f2pool" || pooltype == "poolin" || pooltype == "antpool")
            {
                return url.Contains(pooltype);
            }
            if (pooltype == "poolbtc")
            {
                return url.Contains("pool.btc");
            }
            if (pooltype == "huobi")
            {
                return (url.Contains("huobi") || url.Contains("hpt.com"));
            }
            return false;
        }

        [HttpPost]
        public async Task<IActionResult> DownloadExcel()
        {
            string datetime = Request.Form["datepicker"];
            DateTime dateValue;
            if (DateTime.TryParse(datetime, out dateValue))
            {
                var data = _context.MinerLog.Where(x => x.UpdatedDate > dateValue && x.UpdatedDate < dateValue.AddDays(1)).ToList();
                
                ExcelPackage Ep = new ExcelPackage();
                ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
                Sheet.Cells["A1"].Value = "MinerId";
                Sheet.Cells["B1"].Value = "客户名";
                Sheet.Cells["C1"].Value = "机器型号";
                Sheet.Cells["D1"].Value = "场地";
                Sheet.Cells["E1"].Value = "观察者链接";
                Sheet.Cells["F1"].Value = "在线台数";
                Sheet.Cells["G1"].Value = "上机台数";
                Sheet.Cells["H1"].Value = "离线台数";
                Sheet.Cells["I1"].Value = "无效台数";
                Sheet.Cells["J1"].Value = "15分钟算力";
                Sheet.Cells["K1"].Value = "24小时算力";
                Sheet.Cells["L1"].Value = "理论算力";
                Sheet.Cells["M1"].Value = "算力单位";
                Sheet.Cells["N1"].Value = "更新时间";

                int row = 2;
                foreach (var item in data)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.MinerId;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.UserName;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.MinerType;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Location;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Link;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Active;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Total;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Inactive;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.Dead;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.CurrentCalculation;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.DailyCalculation;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.StandardCalculation;
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.Unit;
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.UpdatedDate.ToString();

                    row++;
                }


                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment: filename=" + "Report.xlsx");
                await Response.Body.WriteAsync(Ep.GetAsByteArray());
                //Response.StatusCode = StatusCodes.Status200OK;
                return RedirectToAction("Index","Admin");
            }          
            else
            {
                ViewBag.error = "日期格式错误";
                return View("Index", await _context.Miner.ToListAsync());
            }        
        }
    }
}
