using System;

namespace app.core.data.common.builder.contract
{
    public interface IDatabaseSourceTypeHandler
    {
        string DatabaseUnit { get; set; }
        String ConnectionString { get; set; }
        string[] IgnoreTablePrefixes { get; set; }
    }
}
