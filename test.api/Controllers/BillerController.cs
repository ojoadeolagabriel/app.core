using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using app.core.data.dao;
using app.core.data.dto;
using app.core.web.mvc.action;

namespace test.api.Controllers
{
    public class BillerController : Controller
    {
        //
        // GET: /Biller/

        public ActionResult Index()
        {
            var isEnabled = FormsAuthentication.IsEnabled;
            
            var dao = new CardBrandDao("");
            var cardBrands = dao.RetreiveAll();

            var list = BuildList(cardBrands);
            ViewBag.CardBrands = list.AsEnumerable();

            return View();
        }

        private SelectList BuildList(IEnumerable<CardBrand> cardBrands)
        {
            var dictData = new Dictionary<string, string>();
            var list = new SelectList(dictData, "Value", "Key");

            foreach (var cardBrand in cardBrands)
            {
                var data = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}",
                    cardBrand.Name,
                    cardBrand.ReQuireOtp ? 1 : 0,
                    cardBrand.MaxLimit,
                    cardBrand.CountryCode.Trim(),
                    cardBrand.IsEnabled ? 1 : 0,
                    cardBrand.RequiresUserAuthorization ? 1 : 0,
                    cardBrand.CanRedeem ? 1 : 0);

                var name = HttpUtility.HtmlEncode(cardBrand.Name);
                dictData.Add(name, data);
            }
            return list;
        }

        public JsonResult GetAllUsers()
        {
            var userDao = new UserDao("");
            var users = userDao.RetreiveAll();
            var jsonUsers = users.Select(c => new
            {
                Name = c.Username, c.Password,
                InstitutionName = c.Institution != null ? c.Institution.Name : "", c.EmailAddress
            });

            return Json(jsonUsers, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpPost]
        [Button("btn_submit")]
        public ActionResult Index(FormCollection formData)
        {
            return View();
        }

        public ActionResult PayWth(string id)
        {
            return View();
        }

        public ActionResult SelectOption(string id)
        {
            var routeData = this.ControllerContext.RouteData;
            var routeId = RouteData.Values["Id"];

            ViewData["BillerTitle"] = id;
            ViewBag.BillerId = id;
            return View();
        }

    }

    public class MissingRouteDataFilterAttribute : ActionFilterAttribute
    {
        public string ParamDesc { get; set; }
        public MissingRouteDataFilterAttribute(string paramName)
        {
            ParamDesc = paramName;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ActionParameters.ContainsKey(ParamDesc))
                throw new Exception("Error ooo");

            object newData = "";
            var resData = filterContext.ActionParameters.TryGetValue(ParamDesc, out newData);

            var data = filterContext.RouteData.Values[ParamDesc].ToString();
            if (string.IsNullOrEmpty(data))
            {

            }

            filterContext.ActionParameters[ParamDesc] = "wale";
        }
    }

}
