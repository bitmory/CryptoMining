using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoMiningWebApi.Models.Repository.DTO
{
    public class WorkerDetail
    {
        public int id { get; set; }
        public string ip { get; set; }
        public string name { get; set; }
        public string Minertype { get; set; }
        public string Location { get; set; }
        public string Pooltype { get; set; }
        public double? temperature { get; set; }
        public double? fanspeed { get; set; }

    }

}
