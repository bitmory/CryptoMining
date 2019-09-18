using CryptoMiningWebApi.Configurations;
using CryptoMiningWebApi.Models.Repository.DTO;
using CryptoMiningWebApi.Models.Repository.EFCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CryptoMiningWebApi.Service
{
    public class UserService
    {
        private CryptoMiningContext _dbcontext;
        private readonly ILogger<UserService> _logger;
        private readonly AppSettings _appSettings;

        public UserService(CryptoMiningContext dbcontext, ILogger<UserService> logger, IOptions<AppSettings> appSettings)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        private List<TokenUser> _users = new List<TokenUser>
        {
            new TokenUser { Id = 1, Username  = "mineradmin",  Password = "miner123456", Role = Role.Admin }
            //new TokenUser { Id = 2, Username  = "user123", Password = "user123", Role = Role.Admin  }
        };

        public TokenUser Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            user.ExpireTime = DateTime.Now.AddDays(7);
            user.Password = null;

            return user;
        }

        public UserResult Login(string username, string password)
        {
            var res = new UserResult();
            var user = _dbcontext.Login.Where(x => x.Username == username && x.Password == password).FirstOrDefault();
            if (user != null)
            {
                res.isValid = true;
                if(user.Role == "admin")
                {
                    res.role = "admin";
                }
                else
                {
                    res.role = "user"; 
                }
            }
            else
            {
                res.isValid = false;
            }

            return res;
        }
    }
}
