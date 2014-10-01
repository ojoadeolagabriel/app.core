using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.data.common.core;

namespace app.core.data.dto
{
    public class Token : Entity<long, Token>
    {
        public string TokenKey { get; set; }
        public long Counter { get; set; }
        public DateTime ExpirationDate { get; set; }
        public byte[] ChildKey { get; set; }
        public TokenRequestor Requestor { get; set; }
        public string LastOneTimePassword { get; set; }

        public Token()
        {
            OverrideTablename("tbl_token");
            Map(c => c.ChildKey).ColumnDescription("child_key");
            Map(c => c.TokenKey).ColumnDescription("token_key");
            Map(c => c.Counter).ColumnDescription("counter");
            Map(c => c.LastOneTimePassword).ColumnDescription("last_one_time_password");
            Map(c => c.ExpirationDate).ColumnDescription("expiration_date");  

            Foreign(c => c.Requestor).ColumnDescription("requestor_id");
        }
    }
}
