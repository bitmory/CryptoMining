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

        public async Task<bool> AddMiningPool(MinerView minerview)
        {
            try
            {
                if (minerview == null || minerview.Link == null || minerview.Pooltype == null)
                    return false;
                if (!CheckPoolType(minerview.Link, minerview.Pooltype))
                {
                    return false;
                }
                var miners = _dbcontext.Miner; // from s in _context.Miner select s;
                var miner = new Miner();
                miner.Username = minerview.Username;
                miner.Minertype = minerview.Minertype;
                miner.Location = minerview.Location;
                miner.Link = minerview.Link;
                miner.Total = minerview.Total;
                miner.Unit = minerview.Unit;
                miner.Pooltype = minerview.Pooltype;
                miner.Standardcalculation = minerview.Standardcalculation;
                miner.Updatedate = DateTime.Now;
                await miners.AddAsync(miner);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<bool> ModifyMiningPool(int poolid, MinerView minerview)
        {
            try
            {
                if (minerview == null || minerview.Link == null || minerview.Pooltype == null)
                    return false;
                var miner = await _dbcontext.Miner.Where(x => x.Id == poolid).FirstOrDefaultAsync();
                if (miner == null)
                    return false;
                if (!CheckPoolType(minerview.Link, minerview.Pooltype))
                {
                    return false;
                }
                miner.Username = minerview.Username;
                miner.Minertype = minerview.Minertype;
                miner.Location = minerview.Location;
                miner.Link = minerview.Link;
                miner.Total = minerview.Total;
                miner.Unit = minerview.Unit;
                miner.Pooltype = minerview.Pooltype;
                miner.Standardcalculation = minerview.Standardcalculation;
                miner.Updatedate = DateTime.Now;
                _dbcontext.Entry(miner).State = EntityState.Modified;
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteMiningPool(int poolid)
        {
            try
            {
                if (poolid == 0)
                    return false;
                var miners = _dbcontext.Miner;
                var miner = await _dbcontext.Miner.Where(x => x.Id == poolid).FirstOrDefaultAsync();
                if (miner == null)
                    return false;
                miners.Remove(miner);
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<string>> GetPoolTypeList()
        {
            var pooltypes = await _dbcontext.Pooltype.Select(x => x.Type).ToListAsync();
            return pooltypes;
        }

        public async Task<List<string>> GetLocationList()
        {
            var locations = await _dbcontext.Miner.Select(x => x.Location).Distinct().ToListAsync();
            return locations;
        }

        public async Task<WorkerDetail> GetWorkerDetail(int workerid)
        {
            var res = new WorkerDetail();
            return res;
        }



        private bool CheckPoolType(string url, string pooltype)
        {
            if (pooltype == "f2pool" || pooltype == "poolin" || pooltype == "antpool")
            {
                return url.Contains(pooltype);
            }
            if (pooltype == "poolbtc")
            {
                return url.Contains("pool.btc");
            }
            if (pooltype == "huobi")
            {
                return (url.Contains("huobi") || url.Contains("hpt.com"));
            }
            return false;
        }
    }
}
