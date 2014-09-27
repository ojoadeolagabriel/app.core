using app.core.data.common.builder.contract;
using app.core.data.common.contract;

namespace app.core.data.common.handler
{
    public class MsSqlHandler : IDatabaseSourceTypeHandler
    {
        public string DatabaseUnit { get; set; }
        public string ConnectionString { get; set; }
        public string[] IgnoreTablePrefixes { get; set; }
    }
}
