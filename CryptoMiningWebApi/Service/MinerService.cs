using CryptoMiningWebApi.Configurations;
using CryptoMiningWebApi.Models.Repository.DTO;
using CryptoMiningWebApi.Models.Repository.EFCore;
using Microsoft.EntityFrameworkCore;
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
    public class MinerService
    {
        private CryptoMiningContext _dbcontext;
        private readonly ILogger<UserService> _logger;
        private readonly AppSettings _appSettings;

        public MinerService(CryptoMiningContext dbcontext, ILogger<UserService> logger, IOptions<AppSettings> appSettings)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        public async Task<List<Miner>> GetAllMiningPool(string location = null)
        {

            var miners = await _dbcontext.Miner.AsNoTracking().ToListAsync(); // from s in _context.Miner select s;

            if (!String.IsNullOrEmpty(location))
            {
                miners = miners.Where(s => s.Location.Equals(location)).ToList();
            }

            return miners;
        }

    }
}
