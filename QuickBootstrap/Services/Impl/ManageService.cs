using System;
using System.Linq;
using QuickBootstrap.Services.Util;
using QuickBootstrap.Sessions;
using System.Diagnostics;
using log4net.Repository.Hierarchy;
using log4net;
using QuickBootstrap.Entities;
using QuickBootstrap.Cache;

namespace QuickBootstrap.Services.Impl
{
    public sealed class ManageService : ServiceContext, IManageService
    {

        private readonly ILog _logger = LogManager.GetLogger(typeof(MvcApplication));

        public void LoginOut(string username, string ipAddress)
        {
        }

        public UserSession GetUserSession(string username, string password, string ipAddress)
        {
            RedisContext cache = new RedisContext();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool isExist = DbContext.User.Any(p => p.UserName == username && p.UserPwd == password && p.IsEnable);
            sw.Stop();

            _logger.InfoFormat("login by db 耗时：{0}", sw.Elapsed);

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();
            User user = cache.Get<User>(username);
            isExist = user.UserPwd == password && user.IsEnable;
            sw2.Stop();

            _logger.InfoFormat("login by cache 耗时：{0}", sw2.Elapsed);

            if (isExist)
            {
                return new UserSession
                {
                    LoginDateTime = DateTime.Now,
                    LoginIpAddress = ipAddress,
                    UserName = username,
                };
            }

            return null;
        }
    }
}