using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CryptoMiningControlCenter.Models.Repository.EFCore;

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
            return View(await _context.Miner.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("Id,Username,Minertype,Location,Link,Pooltype,Active,Inactive,Dead,Currentcalculation,Dailycalculation,Standardcalculation,Unit,Updatedate")] Miner miner)
        {
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Minertype,Location,Link,Pooltype,Active,Inactive,Dead,Currentcalculation,Dailycalculation,Standardcalculation,Unit,Updatedate")] Miner miner)
        {
            if (id != miner.Id)
            {
                return NotFound();
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
    }
}
