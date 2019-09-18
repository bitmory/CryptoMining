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
    [Authorize]
    public class MinerController : ControllerBase
    {
        private readonly MinerService _minerservice;

        public MinerController(MinerService minerservice)
        {
            _minerservice = minerservice;
        }

        /// <summary>
        /// 根据位置得到所有用户的所有矿池情况
        /// 不输入位置得到所有list
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

        /// <summary>
        /// 添加矿池矿池情况
        /// 
        /// </summary>
        /// <param name="minerview"></param>
        /// <returns></returns>
        [Route("MiningPool/Add")]
        [HttpPost]
        public async Task<bool> AddMiningPool(MinerView minerview)
        {
            try
            {
                return await _minerservice.AddMiningPool(minerview);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 编辑矿池矿池情况
        /// 
        /// </summary>
        /// <param name="poolid"></param>
        /// <param name="minerview"></param>
        /// <returns></returns>
        [Route("MiningPool/Edit")]
        [HttpPost]
        public async Task<bool> ModifyMiningPool(int poolid, MinerView minerview)
        {
            try
            {
                return await _minerservice.ModifyMiningPool(poolid, minerview);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 编辑矿池矿池情况
        /// 
        /// </summary>
        /// <param name="poolid"></param>
        /// <returns></returns>
        [Route("MiningPool/Delete")]
        [HttpPost]
        public async Task<bool> DeleteMiningPool(int poolid)
        {
            try
            {
                return await _minerservice.DeleteMiningPool(poolid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 得到矿池类型列表
        /// </summary>
        /// <returns></returns>
        [Route("PoolType")]
        [HttpGet]
        public async Task<List<string>> GetPoolTypeList()
        {
            try
            {
                return await _minerservice.GetPoolTypeList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 得到矿池地点列表
        /// </summary>
        /// <returns></returns>
        [Route("Location")]
        [HttpGet]
        public async Task<List<string>> GetLocationList()
        {
            try
            {
                return await _minerservice.GetLocationList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 暂时没有数据，input 任意id
        /// </summary>
        /// <returns></returns>
        [Route("worker")]
        [HttpGet]
        public async Task<WorkerDetail> GetWorkerDetail(int workerid)
        {
            try
            {
                return await _minerservice.GetWorkerDetail(workerid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}