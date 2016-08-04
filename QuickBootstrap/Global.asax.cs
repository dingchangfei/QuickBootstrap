using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using log4net;
using QuickBootstrap.Cache;
using QuickBootstrap.Entities;
using StackExchange.Redis;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace QuickBootstrap
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private readonly static string RedisConnection = ConfigurationManager.AppSettings["RedisConnection"];
        private readonly ILog _logger = LogManager.GetLogger(typeof(MvcApplication));

        protected void Application_Start()
        {
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomainOnFirstChanceException;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Task.Factory.StartNew(() =>
            {
                Database.SetInitializer(new CreateDatabaseIfNotExists<DefaultDbContext>());
                Database.SetInitializer(new InitData());
                var count = new DefaultDbContext().User.Count();

                if (RedisContext.RedisDatabase == null)
                {
                    RedisContext.RedisDatabase = ConnectionMultiplexer.Connect(RedisConnection).GetDatabase();
                    //Stopwatch sw = new Stopwatch();
                    //sw.Start();
                    //new DefaultDbContext().User.All(SetUserToCache);
                    //sw.Stop();
                    //_logger.InfoFormat("DB-->Cache耗时：{0},条数：{1}", sw.Elapsed, count);
                    
                    RedisContext cache = new RedisContext();
                    _logger.InfoFormat("user", cache.Get<User>("1@1.com"));
                }
            });
        }

        public bool SetUserToCache(User user)
        {
            RedisContext cache = new RedisContext();
            return cache.Set(user.UserName, user);
        }

        private void CurrentDomainOnFirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            _logger.Error(e.Exception);
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            var lastError = Server.GetLastError().GetBaseException();
            {
                _logger.Error(lastError);
            }
        }
    }
}
