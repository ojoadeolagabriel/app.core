using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace app.core.util.xml
{
    /// <summary>
    /// Xml Helper Class
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public static class XmlHelper<TData>
    {
        /// <summary>
        /// Read Field
        /// </summary>
        /// <param name="element"></param>
        /// <param name="descendant"></param>
        /// <param name="attribute"></param>
        /// <param name="defData"></param>
        /// <param name="considerDefaultFirst"></param>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        public static TData ReadField(XElement element, string descendant, string attribute, TData defData = default(TData), bool considerDefaultFirst = false, bool throwOnError = false)
        {
            try
            {
                var data = "";
                if (considerDefaultFirst)
                    return defData;

                data = element.Descendants(descendant).First().Attribute(attribute).Value;
                return (TData)Convert.ChangeType(data, typeof(TData));
            }
            catch
            {
                if (throwOnError)
                    throw new Exception("Error reading xml");
                return default(TData);
            }
        }
    }
}
