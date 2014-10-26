using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.data.common.core;

namespace app.core.data.dto.custom
{
    public class UserInstituion : Entity<long, UserInstituion>
    {
        public string Username { get; set; }
        public long InstitutionId { get; set; }
        public string InstitutionName { get; set; }

        public UserInstituion()
        {
            Map(c => c.Username);
            Map(c => c.InstitutionName);
            Map(c => c.InstitutionId);
        }
    }
}
