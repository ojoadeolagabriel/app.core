using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace app.core.exception.management
{
    public class Test
    {
        public string FirstName { get; set; }

        public string Fullname(string mData)
        {
            Console.WriteLine("Dennis now");
            return string.Format("{0}", FirstName);
        }
    }
}
