using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace app.core.data.common.core.relation
{
    public class PrimaryKeyInfo
    {
        public string columnDescription;
        public Type ColumnType { get; set; }

        public PrimaryKeyInfo ColumnDescription(string desc)
        {
            columnDescription = desc;
            return this;
        }
    }
}
