using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.data.common.core;

namespace app.core.data.dto
{
    public class CardBrand : Entity<long, CardBrand>
    {
        public string Name { get; set; }
        public bool ReQuireOtp { get; set; }
        public double MaxLimit { get; set; }
        public string CountryCode { get; set; }
        public bool IsEnabled { get; set; }
        public bool RequiresUserAuthorization { get; set; }
        public bool CanRedeem { get; set; }

        public CardBrand()
        {
            SetTablename("tbl_card_brand");
            Map(c => c.Name);
            Map(c => c.MaxLimit);
            Map(c => c.CountryCode);
            Map(c => c.IsEnabled);
            Map(c => c.CanRedeem);
            Map(c => c.RequiresUserAuthorization).ColumnDescription("RequiresUserAuth");
        }
    }
}
