using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace QuickBootstrap.Entities
{
    /// <summary>
    /// 数据库初始化种子
    /// </summary>
    public sealed class InitData : CreateDatabaseIfNotExists<DefaultDbContext>
    {
        protected override void Seed(DefaultDbContext context)
        {
            //默认用户
            new List<User>
            {
                new User{
                UserName="1@1.com", 
                UserPwd= "96e79218965eb72c92a549dd5a330112", 
                CreateTime = DateTime.Now, 
                IsEnable = true,
                Nick = "Ding"}
            }.ForEach(m => context.User.Add(m));
        }
    }
}
