﻿using System;
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
        public Institution Institution { get; set; }

        public User()
        {
            OverrideTablename("tbl_user");

            PrimaryKey(c => c.Id);
            Map(c => c.Username).ColumnDescription("user_name").MaxLength(10);
            Map(c => c.EmailAddress).ColumnDescription("email").MaxLength(10);
            Map(c => c.Password).ColumnDescription("password").MaxLength(10);
            Map(c => c.Address).ColumnDescription("address").MaxLength(10);
            Foreign(c => c.Institution).ColumnDescription("institution_id");
        }
    }
}
