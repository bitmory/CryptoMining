using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoMiningWebApi.Models.Repository.DTO;
using CryptoMiningWebApi.Models.Repository.EFCore;
using CryptoMiningWebApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CryptoMiningWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MinerController : ControllerBase
    {
        private readonly MinerService _minerservice;

        public MinerController(MinerService minerservice)
        {
            _minerservice = minerservice;
        }

        /// <summary>
        /// 根据位置得到所有用户的所有矿池情况
        /// 
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        [Route("MiningPool")]
        [HttpGet]
        public async Task<List<Miner>> GetMiningPool(string location = null)
        {
            try
            {
                return await _minerservice.GetAllMiningPool(location);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
           


    }
}