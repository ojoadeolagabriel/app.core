using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using app.core.data.dao;
using app.core.data.dto;
using app.core.data.helper;
using app.core.web.mvc.filter;

namespace test.api.Controllers
{
    public class HomeController : ApiController
    {
        [EmptyParameterFilter("data")]
        public IEnumerable<string> Get(string data)
        {
            var client = new HttpClient();
            
            var rs = client.PostAsJsonAsync("http://localhost:777/synapse.api/values", new User { Username = "Person" }).Result;

            var dao = new UserDao("");
            var allUsers = dao.RetreiveAll();
            var datas = PaginatedDto<User>.Transform(allUsers);
            datas.MoveNext();
            var nextPageData = datas.CurrentPagedData;

            var resp = Request.CreateResponse(HttpStatusCode.Conflict); 

            Task.Factory.StartNew(ProcessNow);
            
            var users = allUsers.Select(c => c.Username);
            return users;
        }

        private void ProcessNow()
        {
            var dao = new UserDao("");
            var data = dao.RetreiveById(1);

            var httpClient = new HttpClient();
            httpClient.PostAsync(
                "http://localhost:777/synapse.api/value",
                data,
                new JsonMediaTypeFormatter()
                ).ContinueWith(t =>
                {
                    if (t.Result.StatusCode == HttpStatusCode.Accepted)
                    {

                    }
                    else if (t.Result.StatusCode == HttpStatusCode.Created)
                    {
                        
                    }
                });
        }
    }
}
