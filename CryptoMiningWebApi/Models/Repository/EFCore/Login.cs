using System;
using System.Collections.Generic;

namespace CryptoMiningWebApi.Models.Repository.EFCore
{
    public partial class Login
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
