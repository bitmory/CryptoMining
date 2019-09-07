using System;
using System.Collections.Generic;

namespace CryptoMiningWebApi.Models.Repository.EFCore
{
    public partial class Worker
    {
        public int Id { get; set; }
        public int Poolid { get; set; }
        public string Workername { get; set; }
        public string Currenthashrate { get; set; }
        public string Dailyhashrate { get; set; }
        public bool Isactive { get; set; }
        public string Rejected { get; set; }
        public DateTime? Updateat { get; set; }
    }
}
