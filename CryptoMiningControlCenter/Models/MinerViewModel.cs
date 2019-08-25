using CryptoMiningControlCenter.Models.Repository.EFCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoMiningControlCenter.Models
{
    public class MinerViewModel : PageModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Minertype { get; set; }
        public string Location { get; set; }
        public string Link { get; set; }
        public string Pooltype { get; set; }
        public int? Active { get; set; }
        public int? Total { get; set; }
        public int? Inactive { get; set; }
        public int? Dead { get; set; }
        public double? Currentcalculation { get; set; }
        public double? Dailycalculation { get; set; }
        public double? Standardcalculation { get; set; }
        public string Unit { get; set; }
        public DateTime? Updatedate { get; set; }


        private readonly CryptoMiningContext _context;

        public MinerViewModel(CryptoMiningContext context)
        {
            _context = context;
        }

        public List<SelectListItem> Options { get; set; }
        public void OnGet()
        {
            Options = _context.Miner.Distinct().Select(a =>
                                          new SelectListItem
                                          {
                                              Value = a.Location,
                                              Text = a.Location
                                          }).ToList();
        }

    }
}
