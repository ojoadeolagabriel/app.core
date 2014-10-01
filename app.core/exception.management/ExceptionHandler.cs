using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace app.core.exception.management
{
    public static class ExceptionHandler
    {
        public static void Handle(string msg, string errorCode, Exception innerException = null)
        {
            if (innerException != null)
                throw new Exception(string.Format("[ {0} ] {1}", errorCode, msg), innerException);
            throw new Exception(string.Format("[ {0} ] {1}", errorCode, msg));
        }
    }
}
