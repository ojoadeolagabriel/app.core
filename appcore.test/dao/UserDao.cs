using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.data.common;
using appcore.test.dto;

namespace appcore.test.dao
{
    /// <summary>
    /// User dao
    /// </summary>
    public class UserDao : CoreDao<long, User>
    {
        public UserDao(string databaseUnit)
            : base(databaseUnit)
        {
        }
    }
}
