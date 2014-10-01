using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.data.common.core;

namespace app.core.data.dto
{
    public class TokenRequestor : Entity<long, TokenRequestor>
    {
        public string Name { get; set; }
        public byte[] ParentKey { get; set; }
        public DateTime CreatedOn { get; set; }

        public TokenRequestor()
        {
            OverrideTablename("tbl_token_requestor");

            Map(c => c.Name).ColumnDescription("name");
            Map(c => c.ParentKey).ColumnDescription("parent_key");
            Map(c => c.CreatedOn).ColumnDescription("created_on");
        }
    }
}
