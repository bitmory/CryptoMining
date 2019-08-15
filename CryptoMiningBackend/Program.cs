using CryptoMingingBackend;
using Dapper;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenScraping;
using OpenScraping.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Text.RegularExpressions;
using static CryptoMiningBackend.Model.ModelClass;

namespace CryptoMiningBackend
{
    class Program : IWorkerRepository
    {
        static IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
        static void Main(string[] args)
        {
            //WorkerSummary1(1);
            //WorkerSummary2(5);
            //WorkerSummary3(7);
            //WorkerSummary4(11);
            //WorkerSummary5(25);
            //Worker2(5);

            //ProcessPool("poolbtc");
            ProcessAll();
        }


        public static void ProcessPool(string pooltype)
        {
            var poollist = GetPoollist(pooltype);
            if(pooltype == "f2pool")
            {
                foreach (var p in poollist)
                {
                    WorkerSummary1(p);
                }
            }
            else if(pooltype == "poolin")
            {
                foreach (var p in poollist)
                {
                    WorkerSummary2(p);
                }
            }
            else if(pooltype == "poolbtc")
            {
                foreach (var p in poollist)
                {
                    WorkerSummary3(p);
                }
            }
            else if(pooltype == "huobi")
            {
                foreach (var p in poollist)
                {
                    WorkerSummary4(p);
                }
            }
            else if (pooltype == "antpool")
            {
                foreach (var p in poollist)
                {
                    WorkerSummary5(p);
                }
            }

        }

        public static void ProcessAll()
        {
            ProcessPool("f2pool");
            ProcessPool("poolin");
            ProcessPool("poolbtc");
            ProcessPool("huobi");
            ProcessPool("antpool");
        }

        public static void Worker1(int poolid)
        {
            var jsonConfig = File.ReadAllText(@"Json\\f2pool.json");
            var config = StructuredDataConfig.ParseJsonString(jsonConfig);
            string html;
            string url = GetUrl(poolid);
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.Headers.Add(HttpRequestHeader.UserAgent, "");
                html = client.DownloadString(url);

            }
            //var html = File.ReadAllText(@"f2pool.html", Encoding.UTF8);

            var openScraping = new StructuredDataExtractor(config);
            var scrapingResults = openScraping.Extract(html);

            //var json = JsonConvert.SerializeObject(scrapingResults);
            var jsonres = JsonConvert.DeserializeObject(scrapingResults.ToString());

            foreach (var item in (dynamic)jsonres)
            {
                //string checkquery = "SELECT COUNT(*) FROM worker where poolid = '" + poolid + "' and workername = '" + item.workername + "' ";
                //var count = db.Query<int>(checkquery).FirstOrDefault();
                var count = 0;

                if(count == 0)
                {
                    if (item.currenthash == "0" || item.currenthash == "0.00")
                    {
                        //
                        Worker worker = new Worker();
                        worker.poolid = poolid;
                        worker.workername = item.workername;
                        worker.currenthashrate = item.currenthash;
                        worker.dailyhashrate = item.dailyhash;
                        worker.rejected = item.rejected;
                        worker.updateat = DateTime.Now;
                        worker.isactive = false;
                        Add(worker);
                    }
                    else if (item.currenthash == null)
                    {
                        continue;
                    }
                    else
                    {
                        //DateTime datetime = Convert.ToDateTime(date);
                        // insert to 
                        Worker worker = new Worker();
                        worker.poolid = poolid;
                        worker.workername = item.workername;
                        worker.currenthashrate = item.currenthash;
                        worker.dailyhashrate = item.dailyhash;
                        worker.rejected = item.rejected;
                        worker.updateat = DateTime.Now;
                        worker.isactive = true;
                        Add(worker);
                    }
                }
                else
                {
                    bool isactive = (item.currenthash == "0" || item.currenthash == "0.00") ? false : true;
                    Worker worker = new Worker();
                    worker.poolid = poolid;
                    worker.workername = item.workername;
                    worker.currenthashrate = item.currenthash;
                    worker.dailyhashrate = item.dailyhash;
                    worker.rejected = item.rejected;
                    worker.updateat = DateTime.Now;
                    worker.isactive = isactive;
                    Update(worker);
                }
                
                //Console.WriteLine("{0} {1} {2} {3}\n", item.workername, item.currenthash,
                //    item.dailyhash, item.rejected);
            }


            //Console.ReadKey();
        }

        public static void WorkerSummary1(int poolid)
        {
            var jsonConfig = File.ReadAllText(@"Json\\f2pool.json");
            var config = StructuredDataConfig.ParseJsonString(jsonConfig);
            string url = GetUrl(poolid);

            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl(url);

            Thread.Sleep(2000);
            var source = driver.PageSource;
            driver.Quit();
            var openScraping = new StructuredDataExtractor(config);
            var scrapingResults = openScraping.Extract(source);

            JObject jObject = JObject.Parse(scrapingResults.ToString());
            JToken json = jObject["data"];

            var all = (int)json[0];
            var active = (int)json[1];
            var inactive = all - active;


            JToken json2 = jObject["currenthash"];
            JToken json3 = jObject["dailyhash"];
            var currentcalculation = (string)json2;
            var dailycalculation = (string)json3;

            UpdateSummary(currentcalculation, dailycalculation, active, inactive, poolid);
        }

        public static void WorkerSummary2(int poolid)
        {
            var jsonConfig = File.ReadAllText(@"Json\\poolin.json");
            var config = StructuredDataConfig.ParseJsonString(jsonConfig);
            string url = GetUrl(poolid);

            var driver = new ChromeDriver();
            //driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(20);
            driver.Navigate().GoToUrl(url);

            Thread.Sleep(5000);
            //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            //wait.Until(dr => dr.FindElement(By.XPath("//p[contains(@class, 'f-tac')]")));
            var source = driver.PageSource;
            driver.Quit();
            var openScraping = new StructuredDataExtractor(config);
            var scrapingResults = openScraping.Extract(source);

            //var jsonres = JsonConvert.DeserializeObject(scrapingResults.ToString());
            JObject jObject = JObject.Parse(scrapingResults.ToString());
            JToken json = jObject["data"];
            var currentcalculation = (string)json[0];
            var dailycalculation = (string)json[1];
            var active = 0;
            var inactive = 0;
            if ((string)json[2] == "-")
            {
                active = 0;
            }
            else
            {
                active = (int)json[2];
            }
            if ((string)json[3] == "-")
            {
                active = 0;
            }
            else
            {
                inactive = (int)json[3];
            }
           
            UpdateSummary(currentcalculation, dailycalculation, active, inactive, poolid);
        }

        public static void WorkerSummary3(int poolid)
        {
            var jsonConfig = File.ReadAllText(@"Json\\poolbtc.json");
            var config = StructuredDataConfig.ParseJsonString(jsonConfig);
            string url = GetUrl(poolid);

            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl(url);

            Thread.Sleep(2000);
            var source = driver.PageSource;
            driver.Quit();
            var openScraping = new StructuredDataExtractor(config);
            var scrapingResults = openScraping.Extract(source);

            JObject jObject = JObject.Parse(scrapingResults.ToString());
            JToken json = jObject["calculation"];
            var currentcalculation = (string)json[0];
            currentcalculation = currentcalculation.Replace(" ", "");
            var dailycalculation = (string)json[1];
            dailycalculation = dailycalculation.Replace(" ", "");
            JToken json2 = jObject["status"];
            var active = (int)json2[1];
            var inactive = (int)json2[2];

            UpdateSummary(currentcalculation, dailycalculation, active, inactive, poolid);
        }

        public static void WorkerSummary4(int poolid)
        {
            var jsonConfig = File.ReadAllText(@"Json\\huobi.json");
            var config = StructuredDataConfig.ParseJsonString(jsonConfig);
            string url = GetUrl(poolid);

            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl(url);
            Thread.Sleep(2000);
            var source = driver.PageSource;
            driver.Quit();
            var openScraping = new StructuredDataExtractor(config);
            var scrapingResults = openScraping.Extract(source);

            JObject jObject = JObject.Parse(scrapingResults.ToString());
            JToken json = jObject["calculation"];
            var currentcalculation = (string)json[0];
            var dailycalculation = (string)json[2];
            JToken json2 = jObject["status"];

            var temp = (string)json2;
            var numbers = Regex.Split(temp.Trim(), @"\D+");

            //bool flag = false;
            //var list = new List<int>();
            //string tmp = string.Empty;
            //for(int i = 0; i < temp.Length; i++)
            //{
            //    if (Char.IsDigit(temp[i]))
            //    {
            //        tmp += temp[i];
            //        flag = true;
            //    }
            //    else
            //    {
            //        if(flag == true)
            //        {
            //            flag = false;
            //            list.Add(Int32.Parse(tmp));
            //            tmp = string.Empty;
            //        }
            //    }
            //}

            int active = Int32.Parse(numbers[1]);
            int inactive = Int32.Parse(numbers[2]);
            int dead = Int32.Parse(numbers[3]);

            // add together
            inactive = inactive + dead;

            UpdateSummary(currentcalculation, dailycalculation, active, inactive, poolid);
        }

        public static void WorkerSummary5(int poolid)
        {
            var jsonConfig = File.ReadAllText(@"Json\\antpool.json");
            var config = StructuredDataConfig.ParseJsonString(jsonConfig);
            string url = GetUrl(poolid);

            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl(url);

            Thread.Sleep(2000);
            var source = driver.PageSource;
            driver.Quit();
            var openScraping = new StructuredDataExtractor(config);
            var scrapingResults = openScraping.Extract(source);

            JObject jObject = JObject.Parse(scrapingResults.ToString());
            JToken json = jObject["data"];

            var temp = (string)json[0];
            var currentcalculation = (string)json[1];
            var dailycalculation = (string)json[3];

            var numbers = Regex.Split(temp.Trim(), @"\D+");
            var active = Int32.Parse(numbers[0]);
            var total = Int32.Parse(numbers[1]);
            var inactive = total - active;

            UpdateSummary(currentcalculation, dailycalculation, active, inactive, poolid);
        }

        public static void Worker2(int poolid)
        {

            var jsonConfig = File.ReadAllText(@"Json\\poolin.json");
            var config = StructuredDataConfig.ParseJsonString(jsonConfig);
            string html;
            string url = GetUrl(poolid);

            //using (WebClient client = new WebClient())
            //{
            //    client.Encoding = Encoding.UTF8;
            //    //client.Headers.Add(HttpRequestHeader.UserAgent, "test");
            //    //client.Credentials = CredentialCache.DefaultCredentials;
            //    html = client.DownloadString(url);
            //}

            var driver = new ChromeDriver();
            //var homeURL = "https://www.poolin.com/my/9007375/btc/miners?read_token=wowavEpSkh6wX7yePaQ4wcsfbPKPWNBlxkqppuYlJNvm4NUHUBoLCzAKhj4QTblH";
            driver.Navigate().GoToUrl(url);
            //IWebElement element = driver.FindElement(By.XPath("//table"));
            var source = driver.PageSource;

            var openScraping = new StructuredDataExtractor(config);
            var scrapingResults = openScraping.Extract(source);
            //var json = JsonConvert.SerializeObject(scrapingResults);
            var jsonres = JsonConvert.DeserializeObject(scrapingResults.ToString());

            foreach (var item in (dynamic)jsonres)
            {
                //string checkquery = "SELECT COUNT(*) FROM worker where poolid = '" + poolid + "' and workername = '" + item.workername + "' ";
                //var count = db.Query<int>(checkquery).FirstOrDefault();
                var count = 0;

                if (count == 0)
                {
                    if (item.currenthash == "0" || item.currenthash == "0.00")
                    {
                        //
                        Worker worker = new Worker();
                        worker.poolid = poolid;
                        worker.workername = item.workername;
                        worker.currenthashrate = item.currenthash;
                        worker.dailyhashrate = item.dailyhash;
                        worker.rejected = item.rejected;
                        worker.updateat = DateTime.Now;
                        worker.isactive = false;
                        Add(worker);
                    }
                    else if (item.currenthash == null)
                    {
                        continue;
                    }
                    else
                    {
                        //DateTime datetime = Convert.ToDateTime(date);
                        // insert to 
                        Worker worker = new Worker();
                        worker.poolid = poolid;
                        worker.workername = item.workername;
                        worker.currenthashrate = item.currenthash;
                        worker.dailyhashrate = item.dailyhash;
                        worker.rejected = item.rejected;
                        worker.updateat = DateTime.Now;
                        worker.isactive = true;
                        Add(worker);
                    }
                }
                else
                {
                    bool isactive = (item.currenthash == "0" || item.currenthash == "0.00") ? false : true;
                    Worker worker = new Worker();
                    worker.poolid = poolid;
                    worker.workername = item.workername;
                    worker.currenthashrate = item.currenthash;
                    worker.dailyhashrate = item.dailyhash;
                    worker.rejected = item.rejected;
                    worker.updateat = DateTime.Now;
                    worker.isactive = isactive;
                    Update(worker);
                }

                //Console.WriteLine("{0} {1} {2} {3}\n", item.workername, item.currenthash,
                //    item.dailyhash, item.rejected);
            }

            //foreach (var item in (dynamic)jsonres)
            //{
            //    Worker worker = new Worker();
            //    worker.poolid = poolid;
            //    worker.workername = item.workername;
            //    worker.currenthashrate = item.currenthash;
            //    worker.dailyhashrate = item.dailyhash;
            //    worker.rejected = item.rejected;
            //    worker.updateat = DateTime.Now;
            //    worker.isactive = true;
            //    worker.currentcalculation = item.currenthashtotal;
            //    worker.dailycalculation = item.dailyhashtotal;
            //    UpdateSummary(worker);
            //    break;
            //}

            //Console.ReadKey();
        }


        public static void Add(Worker worker)
        {
            string query = @"Insert into worker values (@poolid , @workername, @currenthashrate , @dailyhashrate , @isactive, @rejected, @updateat);
            select Cast(Scope_Identity() as int)";
            int id = db.Query<int>(query, worker).Single();
            //worker.Id = id;
            //return worker;
        }

        public static void Update(Worker worker)
        {
            string query = "update worker set currenthashrate = '" + worker.currenthashrate + "' , dailyhashrate = '" + worker.dailyhashrate + "' where poolid = '" + worker.poolid + "' and workername = '" + worker.workername + "' ";
            db.Execute(query);
            //contact.Id = i;
            //return contact;
        }

        public static void UpdateSummary(string currentcalculation, string dailycalculation, int active, int inactive, int poolid)
        {
            string query = "update miner set currentcalculation = '" + currentcalculation + "' , dailycalculation = '" + dailycalculation + "', active = '" + active + "', inactive = '" + inactive + "' where id = '" + poolid + "' ";
            db.Execute(query);
            //string query = "update miner set currentcalculation = '" + worker.currentcalculation + "' , dailycalculation = '" + worker.dailycalculation + "' where id = '" + worker.poolid + "' ";
            //db.Execute(query);

            //var sql1 = "SELECT COUNT(*) FROM worker where isactive = 'false' and poolid = '" + worker.poolid + "' ";
            //var res1 = db.Query<int>(sql1).FirstOrDefault();
            //var sql2 = "SELECT COUNT(*) FROM worker where isactive = 'true' and poolid = '" + worker.poolid + "' ";
            //var res2 = db.Query<int>(sql2).FirstOrDefault();
            //string query2 = "update miner set inactive = '" + res1 + "' , active = '" + res2 + "' where id = '" + worker.poolid + "' ";
            //db.Execute(query2);
        }

        public static string GetUrl(int poolid)
        {
            string query = "SELECT link FROM miner where id = '" + poolid + "' ";
            var res = db.Query<string>(query).FirstOrDefault();
            return res;
        }

        public static List<int> GetPoollist(string pooltype)
        {
            string query = "select id from miner where pooltype = '" + pooltype + "' ";
            return (List<int>)db.Query<int>(query);
        }

    }

}
