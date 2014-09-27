using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace app.core.data.common.core.relation
{
    public class EntityColumnSummary
    {
        public Dictionary<string, ColumnInfo> MapColumns { get; set; }

        public PrimaryKeyInfo PrimaryKeyInfo { get; set; }
    }
}
