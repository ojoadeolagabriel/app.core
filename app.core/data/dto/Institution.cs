using app.core.data.common.core;

namespace app.core.data.dto
{
    public class Institution : Entity<long, Institution>
    {
        public string Name { get; set; }

        public Institution()
        {
            SetTablename("tbl_institution");
            Map(c => c.Name).ColumnDescription("name");
        }
    }
}
