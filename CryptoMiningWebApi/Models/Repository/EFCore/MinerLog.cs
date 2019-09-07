using System;
using System.Collections.Generic;

namespace CryptoMiningWebApi.Models.Repository.EFCore
{
    public partial class MinerLog
    {
        public int LogId { get; set; }
        public int MinerId { get; set; }
        public string UserName { get; set; }
        public string MinerType { get; set; }
        public string Location { get; set; }
        public string Link { get; set; }
        public string PoolType { get; set; }
        public int? Active { get; set; }
        public int? Total { get; set; }
        public int? Inactive { get; set; }
        public int? Dead { get; set; }
        public double? CurrentCalculation { get; set; }
        public double? DailyCalculation { get; set; }
        public double? StandardCalculation { get; set; }
        public string Unit { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string Type { get; set; }
    }
}
