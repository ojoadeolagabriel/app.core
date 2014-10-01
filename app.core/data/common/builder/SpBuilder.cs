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
            schema = Cleanup(schema, prefixToRemove);
            return string.Format("[dbo].usp_select_{0}_by_id", schema);
        }

        public static string BuildRetrieveAllSp(string schema, string[] prefixToRemove = null)
        {
            schema = Cleanup(schema, prefixToRemove);
            return string.Format("[dbo].usp_select_{0}{1}", schema, schema.EndsWith("s") ? "" : "s");
        }

        public static string BuildDeleteByIdSp(string schema, string[] prefixToRemove = null)
        {
            schema = Cleanup(schema, prefixToRemove);
            return string.Format("[dbo].usp_delete_by_{0}", schema);
        }

        public static string BuildPersistSp(string schema, string[] ignoreTablePrefixes)
        {
            schema = Cleanup(schema, ignoreTablePrefixes);
            return string.Format("[dbo].usp_insert_{0}", schema);
        }

        public static string BuildUpdateSp(string schema, string[] ignoreTablePrefixes)
        {
            schema = Cleanup(schema, ignoreTablePrefixes);
            return string.Format("[dbo].usp_update_{0}_by_id", schema);
        }

        private static string Cleanup(string schema, string[] ignoreTablePrefixes)
        {
            if (ignoreTablePrefixes != null)
            {
                ignoreTablePrefixes.ToList().ForEach(c => schema = schema.Replace(c, ""));
                if (schema.StartsWith("_"))
                    schema = schema.Remove(0, 1);
            }
            return schema;
        }
    }
}
