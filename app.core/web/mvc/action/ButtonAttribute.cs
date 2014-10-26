using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace app.core.web.mvc.action
{
    public class ButtonAttribute : ActionMethodSelectorAttribute
    {
        private readonly string _buttonName;
        public ButtonAttribute(string buttonName)
        {
            _buttonName = buttonName;
        }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            var containsKey = controllerContext.HttpContext.Request[_buttonName] != String.Empty;
            if (containsKey)
                return true;
            return false;
        }
    }
}
