using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace app.core.data.common.builder
{
    public class SpBuilder
    {
        public static string BuildRetrieveByIdSp(string schema, string[] prefixToRemove = null)
        {
            if (prefixToRemove != null)
            {
                prefixToRemove.ToList().ForEach(c => schema = schema.Replace(c, ""));
            }
            return string.Format("[dbo].usp_select_{0}_by_id", schema);
        }

        public static string BuildRetrieveAllSp(string schema, string[] prefixToRemove = null)
        {
            if (prefixToRemove != null)
            {
                prefixToRemove.ToList().ForEach(c => schema = schema.Replace(c, ""));
            }
            return string.Format("[dbo].usp_select_{0}{1}", schema, schema.EndsWith("s") ? "" : "s");
        }

        public static string BuildDeleteByIdSp(string schema, string[] prefixToRemove = null)
        {
            if (prefixToRemove != null)
            {
                prefixToRemove.ToList().ForEach(c => schema = schema.Replace(c, ""));
            }
            return string.Format("usp_delete_by_{0}", schema);
        }

        public static string BuildPersistSp(string tableName, string[] ignoreTablePrefixes)
        {
            return "";
        }
    }
}
