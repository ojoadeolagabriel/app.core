using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using app.core.data.common;
using app.core.data.common.core;

namespace appcore.test.dto
{
    /// <summary>
    /// User type
    /// </summary>
    public class User : Entity<long, User>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public List<Institution> Institutions { get; set; }


        public User()
        {
            PrimaryKey(c => c.Id);
            Map(c => c.Username).ColumnDescription("user_name").MaxLength(10);
            Map(c => c.EmailAddress).ColumnDescription("email").MaxLength(10);
            Map(c => c.Password).ColumnDescription("password").MaxLength(10);

            HasMany(c => c.Institutions).ColumnDescription("institution_id");
        }
    }
}
