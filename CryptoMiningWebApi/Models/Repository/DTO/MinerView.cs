using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoMiningWebApi.Models.Repository.DTO
{
    public class MinerView
    {
        public string Username { get; set; }
        public string Minertype { get; set; }
        public string Location { get; set; }
        public string Link { get; set; }
        public string Pooltype { get; set; }
        public int? Total { get; set; }
        public double? Standardcalculation { get; set; }
        public string Unit { get; set; }
        public DateTime? Updatedate { get; set; }
    }

}
