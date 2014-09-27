using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace app.core.data.common.builder
{
    public class SpBuilder
    {
        public static string RetreiveUniqueSp(string schema, string[] prefixToRemove = null)
        {
            if (prefixToRemove != null)
            {
                prefixToRemove.ToList().ForEach(c => schema = schema.Replace(c, ""));
            }
            return string.Format("usp_select_{0}_by_id", schema);
        }
    }
}
