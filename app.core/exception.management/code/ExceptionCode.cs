using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace app.core.exception.management.code
{
    public class ExceptionCode
    {
        public const string UnKnownError = "10001";
        public const string CouldNotConnectToDatSource = "10002";
        public const string DataPersistenceIssue = "10003";
        public const string DataUpdateIssue = "10004";
        
        public const string RequestorAllReadyExists = "20001";
    }
}
