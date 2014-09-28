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
            var user = dao.RetreiveById(2);

            var allUsers = dao.RetreiveAll();
            var timeTaken = (DateTime.Now - now).TotalMilliseconds;
        }
    }
}
