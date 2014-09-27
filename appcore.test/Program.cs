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
            var dao = new UserDao(null);
            dao.RetreiveById(1);
        }
    }
}
