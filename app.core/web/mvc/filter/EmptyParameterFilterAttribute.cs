using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace app.core.web.mvc.filter
{
    public class EmptyParameterFilterAttribute : ActionFilterAttribute
    {
        public string ParamenterName { get; set; }

        public EmptyParameterFilterAttribute(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new Exception(string.Format("{0}, cannot be null", parameterName));

            ParamenterName = parameterName;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ActionArguments.ContainsKey(ParamenterName))
                throw new Exception(string.Format("action argument {0}, not found", ParamenterName));

            object paramValue = null;
            if (actionContext.ActionArguments.TryGetValue(ParamenterName, out paramValue))
            {
                if (paramValue == null)
                {
                    actionContext.ModelState.AddModelError(ParamenterName, string.Format("{0}, cannot be null", ParamenterName));
                    actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
                }
            }
        }
    }
}
