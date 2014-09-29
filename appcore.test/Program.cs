using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.exception.management;
using app.core.security.otp;
using app.core.util.reflection;
using appcore.test.dao;
using appcore.test.dto;

namespace appcore.test
{
    class Program
    {
        static void Main(string[] args)
        {
            var now = DateTime.Now;
            var dao = new UserDao(null);

            var users = new List<User>();
            for (int i = 0; i < 100; i++)
            {
                var user = dao.RetreiveById(1);  
                users.Add(user);
            }
            
            var allUsers = dao.RetreiveAll();
            var timeTaken = (DateTime.Now - now).TotalMilliseconds;
        }
    }
}
