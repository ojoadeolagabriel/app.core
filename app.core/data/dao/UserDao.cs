using System.Collections.Generic;
using System.Data.SqlClient;
using app.core.data.common.builder.contract;
using app.core.data.common.core;
using app.core.data.dto;
using app.core.data.dto.custom;

namespace app.core.data.dao
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

        /// <summary>
        /// Get user_inst info. 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public UserInstituion GetUserInstituion(int i)
        {
            return ExecuteUniqueQuery<UserInstituion>("[dbo].[select_second_user_institution]", new List<SqlParameter> { new SqlParameter("@id", i) });
        }

        public User GetSecondUser(long id = 2)   
        {
            return ExecuteUniqueQuery<User>("[dbo].[select_second_user]", new List<SqlParameter> { new SqlParameter("@id", id) });
        }
    }
}
