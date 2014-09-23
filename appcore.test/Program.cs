using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.exception.management;
using app.core.util.reflection;

namespace appcore.test
{
    class Program
    {
        static void Main(string[] args)
        {
            var propertyInfo = ReflectionHelper<Test>.GetProperty((c) => c.FirstName);
            ReflectionHelper<Test>.GetMethod((c) => c.Fullname(""));
        }
    }
}
