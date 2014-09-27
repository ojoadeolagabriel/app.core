using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.exception.management;
using app.core.security.otp;
using app.core.util.reflection;
using appcore.test.dao;

namespace appcore.test
{
    class Program
    {
        static void Main(string[] args)
        {
            var otp = new OTP(2);

            var data = otp.GetCurrentOtp();
            var data2 = otp.GetCurrentOtp();

            var dao = new UserDao("");
            dao.RetreiveById(0);

            var propertyInfo = ReflectionHelper<Test>.GetProperty((c) => c.FirstName);
            ReflectionHelper<Test>.GetMethod((c) => c.Fullname(""));
        }
    }
}
