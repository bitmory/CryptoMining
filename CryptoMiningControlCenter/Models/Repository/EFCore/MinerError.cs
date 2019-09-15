using System;
using System.Collections.Generic;

namespace CryptoMiningControlCenter.Models.Repository.EFCore
{
    public partial class MinerError
    {
        public int Id { get; set; }
        public string Pooltype { get; set; }
        public string Errormessage { get; set; }
        public bool? Isresolve { get; set; }
        public DateTime? Updatedate { get; set; }
    }
}
