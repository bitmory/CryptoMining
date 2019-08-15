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
        public int? Inactive { get; set; }
        public string Currentcalculation { get; set; }
        public string Dailycalculation { get; set; }
    }
}
