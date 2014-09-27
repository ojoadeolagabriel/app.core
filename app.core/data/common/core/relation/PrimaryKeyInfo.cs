using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace app.core.data.common.core.relation
{
    public class PrimaryKeyInfo
    {
        private string _columnDescription;
        public PrimaryKeyInfo ColumnDescription(string desc)
        {
            _columnDescription = desc;
            return this;
        }
    }
}
