using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoMiningWebApi.Models.Repository.DTO
{
    public class TokenUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public DateTime ExpireTime { get; set; }
    }

    public static class Role
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }

    public class TokenLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }

    }
}
