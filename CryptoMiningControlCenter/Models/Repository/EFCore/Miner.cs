using System;
using System.Collections.Generic;

namespace CryptoMiningControlCenter.Models.Repository.EFCore
{
    public partial class Miner
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
    }
}
