using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.data.common.core;
using app.core.data.dto;

namespace app.core.data.dao
{
    public class TokenRequestorDao: CoreDao<long, TokenRequestor>
    {
        public TokenRequestorDao(string databaseUnit) : base(databaseUnit)
        {
        }
    }
}
