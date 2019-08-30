using CryptoMiningControlCenter.Models.Repository.EFCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoMiningControlCenter.Models
{
    public class HelperViewModel
    {
        private readonly CryptoMiningContext _context;

        public HelperViewModel(CryptoMiningContext context)
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
