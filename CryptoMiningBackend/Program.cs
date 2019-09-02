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
            //WorkerSummary2(44);
            //WorkerSummary3(7);
            //WorkerSummary4(14);
            //WorkerSummary5(25);
            //WorkerSummary6(1);

            //ProcessPool("f2pool");
            //ProcessPool("poolin");
            //ProcessPool("poolbtc");
            //ProcessPool("huobi");
            //ProcessPool("antpool");
            //ProcessPool("viabtc");
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
            else if (pooltype == "viabtc")
            {
                foreach (var p in poollist)
                {
                    WorkerSummary6(p);
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
            ProcessPool("viabtc");
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
            var driver = new ChromeDriver();
            try
            {
                var jsonConfig = File.ReadAllText(@"Json\\f2pool.json");
                var config = StructuredDataConfig.ParseJsonString(jsonConfig);
                string url = GetUrl(poolid);
                driver.Navigate().GoToUrl(url);

                //Thread.Sleep(1000);
                var source = driver.PageSource;
                driver.Close();
                driver.Quit();
                var openScraping = new StructuredDataExtractor(config);
                var scrapingResults = openScraping.Extract(source);

                JObject jObject = JObject.Parse(scrapingResults.ToString());
                JToken json = jObject["data"];

                //var all = (int)json[0];
                var active = (int)json[1];
                var inactive = (int)json[2];
                var dead = 0;
                if (json.Count() == 4)
                {
                    dead = (int)json[3];
                }

                JToken json2 = jObject["currenthash"];
                JToken json3 = jObject["dailyhash"];
                var temp1 = (string)json2;
                var temp2 = (string)json3;

                var currentcalculation = GetFloat(temp1);
                var dailycalculation = GetFloat(temp2);
                var unit = GetString(temp1);

                UpdateSummary(currentcalculation, dailycalculation, unit, active, inactive, dead, poolid);
            }
            catch(Exception ex)
            {
                driver.Close();
                driver.Quit();
                throw ex;
            }
            
        }

        public static void WorkerSummary2(int poolid)
        {
            var driver = new ChromeDriver();
            try
            {
                var jsonConfig = File.ReadAllText(@"Json\\poolin.json");
                var config = StructuredDataConfig.ParseJsonString(jsonConfig);
                string url = GetUrl(poolid);

                driver.Navigate().GoToUrl(url);

                Thread.Sleep(6000);
                //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                //wait.Until(dr => dr.FindElement(By.XPath("//p[contains(@class, 'f-tac')]")));
                var source = driver.PageSource;
                driver.Close();
                driver.Quit();
                var openScraping = new StructuredDataExtractor(config);
                var scrapingResults = openScraping.Extract(source);

                //var jsonres = JsonConvert.DeserializeObject(scrapingResults.ToString());
                JObject jObject = JObject.Parse(scrapingResults.ToString());
                JToken json = jObject["data"];
                var temp1 = (string)json[0];
                var temp2 = (string)json[1];

                var currentcalculation = GetFloat(temp1);
                var dailycalculation = GetFloat(temp2);
                var unit = GetString(temp1);

                int active, inactive;
                int dead = 0;
                // no data for dead

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
                    inactive = 0;
                }
                else
                {
                    inactive = (int)json[3];
                }

                UpdateSummary(currentcalculation, dailycalculation, unit, active, inactive, dead, poolid);
            }
            catch (Exception ex)
            {
                driver.Close();
                driver.Quit();
                throw ex;
            }

        }

        public static void WorkerSummary3(int poolid)
        {
            var driver = new ChromeDriver();
            try
            {
                var jsonConfig = File.ReadAllText(@"Json\\poolbtc.json");
                var config = StructuredDataConfig.ParseJsonString(jsonConfig);
                string url = GetUrl(poolid);

                driver.Navigate().GoToUrl(url);

                //Thread.Sleep(1000);
                var source = driver.PageSource;
                driver.Close();
                driver.Quit();
                var openScraping = new StructuredDataExtractor(config);
                var scrapingResults = openScraping.Extract(source);

                JObject jObject = JObject.Parse(scrapingResults.ToString());
                JToken json = jObject["calculation"];
                var currentcalculationtemp = (string)json[0];
                var temp1 = currentcalculationtemp.Replace(" ", "");
                var dailycalculationtemp = (string)json[1];
                var temp2 = dailycalculationtemp.Replace(" ", "");

                var currentcalculation = GetFloat(temp1);
                var dailycalculation = GetFloat(temp2);
                var unit = GetString(temp1);

                JToken json2 = jObject["status"];
                var active = (int)json2[1];
                var inactive = (int)json2[2];
                int dead = 0;

                UpdateSummary(currentcalculation, dailycalculation, unit, active, inactive, dead, poolid);
            }
            catch (Exception ex)
            {
                driver.Close();
                driver.Quit();
                throw ex;
            } 
        }

        public static void WorkerSummary4(int poolid)
        {
            var driver = new ChromeDriver();
            try
            {
                var jsonConfig = File.ReadAllText(@"Json\\huobi.json");
                var config = StructuredDataConfig.ParseJsonString(jsonConfig);
                string url = GetUrl(poolid);


                driver.Navigate().GoToUrl(url);
                Thread.Sleep(3000);
                var source = driver.PageSource;
                driver.Close();
                driver.Quit();
                var openScraping = new StructuredDataExtractor(config);
                var scrapingResults = openScraping.Extract(source);

                JObject jObject = JObject.Parse(scrapingResults.ToString());
                JToken json = jObject["calculation"];
                var temp1 = (string)json[0];
                var temp2 = (string)json[2];

                var currentcalculation = GetFloat(temp1);
                var dailycalculation = GetFloat(temp2);
                var unit = GetString(temp1);

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

                UpdateSummary(currentcalculation, dailycalculation, unit, active, inactive, dead, poolid);
            }
            catch (Exception ex)
            {
                driver.Close();
                driver.Quit();
                throw ex;
            } 
        }

        public static void WorkerSummary5(int poolid)
        {

            var driver = new ChromeDriver();
            try
            {
                var jsonConfig = File.ReadAllText(@"Json\\antpool.json");
                var config = StructuredDataConfig.ParseJsonString(jsonConfig);
                string url = GetUrl(poolid);

                driver.Navigate().GoToUrl(url);

                //Thread.Sleep(1000);
                var source = driver.PageSource;
                driver.Close();
                driver.Quit();
                var openScraping = new StructuredDataExtractor(config);
                var scrapingResults = openScraping.Extract(source);

                JObject jObject = JObject.Parse(scrapingResults.ToString());
                JToken json = jObject["data"];

                var temp = (string)json[0];
                var temp1 = (string)json[1];
                var temp2 = (string)json[3];

                var currentcalculation = GetFloat(temp1);
                var dailycalculation = GetFloat(temp2);
                var unit = GetString(temp1);

                var numbers = Regex.Split(temp.Trim(), @"\D+");
                var active = Int32.Parse(numbers[0]);
                var total = Int32.Parse(numbers[1]);
                var inactive = total - active;
                int dead = 0;

                UpdateSummary(currentcalculation, dailycalculation, unit, active, inactive, dead, poolid);
            }
            catch (Exception ex)
            {
                driver.Close();
                driver.Quit();
                throw ex;
            }    
        }


        public static void WorkerSummary6(int poolid)
        {

            var driver = new ChromeDriver();
            try
            {
                var jsonConfig = File.ReadAllText(@"Json\\viabtc.json");

                var config = StructuredDataConfig.ParseJsonString(jsonConfig);
                string url = GetUrl(poolid);

                //var url = "https://pool.viabtc.com/observer/dashboard?access_key=cb735a866859b626a748c0fb4a479394";
                driver.Navigate().GoToUrl(url);

                //Thread.Sleep(1000);
                var source = driver.PageSource;
                driver.Close();
                driver.Quit();
                var openScraping = new StructuredDataExtractor(config);
                var scrapingResults = openScraping.Extract(source);

                JObject jObject = JObject.Parse(scrapingResults.ToString());
                JToken json = jObject["data"];

   
                var temp1 = (string)json[0];
                var temp2 = (string)json[2];

                var currentcalculation = GetFloat(temp1);
                var dailycalculation = GetFloat(temp2);
                var unit = GetString(temp1);

                var active = Int32.Parse((string)json[3]);
                var inactive = Int32.Parse((string)json[4]);
                int total = 0;
                int dead = 0;

                UpdateSummary(currentcalculation, dailycalculation, unit, active, inactive, dead, poolid);
            }
            catch (Exception ex)
            {
                driver.Close();
                driver.Quit();
                throw ex;
            }
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

            driver.Close();
            driver.Quit();

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
            db.Query<int>(query, worker).Single();
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

        public static void UpdateSummary(float currentcalculation, float dailycalculation, string unit, int active, int inactive,int dead, int poolid)
        {
            string date = DateTime.Now.ToString();
            string query = "update miner set currentcalculation = '" + currentcalculation + "' , dailycalculation = '" + dailycalculation + "', active = '" + active + "', inactive = '" + inactive + "', dead = '" + dead + "', updatedate = '" + date +"' where id = '" + poolid + "' ";
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

        public static void UpdateLog(float currentcalculation, float dailycalculation, string unit, int active, int inactive, int dead, int poolid)
        {
            string date = DateTime.Now.ToString();
            string query = "update miner set currentcalculation = '" + currentcalculation + "' , dailycalculation = '" + dailycalculation + "', active = '" + active + "', inactive = '" + inactive + "', dead = '" + dead + "', updatedate = '" + date + "' where id = '" + poolid + "' ";
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
            if (!res.StartsWith("http"))
            {
                res = "https://" + res;
            }
            return res;
        }

        public static List<int> GetPoollist(string pooltype)
        {
            string query = "select id from miner where pooltype = '" + pooltype + "' ";
            return (List<int>)db.Query<int>(query);
        }

        public static float GetFloat(string s)
        {
            if (s == null || s == "")
            {
                return 0;
            }
            else
            {
                var floatres = Regex.Split(s, @"[^0-9\.]+");
                return Convert.ToSingle(floatres[0]);
            }
        }

        public static string GetString(string s)
        {
            if (s == null || s == "")
            {
                return "";
            }
            else
            {
                var nonNumeric = Regex.Replace(s, "[.0-9]", "");
                return nonNumeric.Trim();
            }
        }

    }

}
