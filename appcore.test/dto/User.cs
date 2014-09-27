using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using app.core.data.common;

namespace appcore.test.dto
{
    /// <summary>
    /// User type
    /// </summary>
    public class User : Entity<long>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
    }
}
