using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http;
using Owin;
using System.Web.Http.Cors;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Host.HttpListener;
using ModuleTesting;

namespace RESTserver
{
    public class Array
    {
        public int[] data;
    }

    public class Test
    {
        public int id;
        public int[] result;
    }

    public class ID
    {
        public int id;
    }

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RESTReciverController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Subscribe()
        {
            int id = 0;

            var task = Task.Run(() => { id = Testing.GetInstance().Subscribe(); });
            task.Wait();

            Console.WriteLine("Add new client № " + id + ".");
            return Json<int>(id);
        }

        [HttpPost]
        public void Unsubscribe(ID arg)
        {
            Task.Run(()=> { if (Testing.GetInstance().Unsubscribe(arg.id))
                                Console.WriteLine("Client № " + arg.id + " unsubscribe.");
                          });
        }

        [HttpPost]
        public void CreateTests(Test arg)
        {
            Task.Run(() => { Testing.GetInstance().CreateTests(arg.id, arg.result);
                             Console.WriteLine("Client № " + arg.id + " start create tests.");
                            });
        }

        [HttpPost]
        public IHttpActionResult TestsReady(ID arg)
        {
            bool isReady = false;

            var test = Task.Run(() => { isReady = Testing.GetInstance().TestsReady(arg.id); });
            test.Wait();

            if (isReady)
                Console.WriteLine("For client № " + arg.id + " test ready.");

            return Json<bool>(isReady);
        }

        [HttpPost]
        public IHttpActionResult GetQuestions(ID arg)
        {
            string[] questions = null;

            var test = Task.Run(() => { questions = Testing.GetInstance().GetQuestions(arg.id); });
            test.Wait();

            Console.WriteLine("Client № " + arg.id + " get questions.");

            return Json<string[]>(questions);
        }

        [HttpPost]
        public void GetGradeLucky(Test arg)
        {
            Task.Run(() => { Testing.GetInstance().GetGradeLucky(arg.id, arg.result);
                             Console.WriteLine("Client № " + arg.id + " send result.");
                           });
        }

        [HttpPost]
        public IHttpActionResult ResultsReady(ID arg)
        {
            bool isReady = false;

            var test = Task.Run(() => { isReady = Testing.GetInstance().ResultsReady(arg.id); });
            test.Wait();

            if (isReady)
                Console.WriteLine("For client № " + arg.id + " results ready.");

            return Json<bool>(isReady);
        }

        [HttpPost]
        public IHttpActionResult GetFinishResult(ID arg)
        {
            string result = "";

            var test = Task.Run(() => { result = Testing.GetInstance().GetFinishResult(arg.id); });
            test.Wait();

            Console.WriteLine("Client № " + arg.id + " get finish result.");

            return Json<string>(result);
        }
    }


    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.EnableCors();
            config.Routes.MapHttpRoute(
              "Api", "api/{controller}/{action}/{id}",
                new { id = RouteParameter.Optional }
            );
            app.UseWebApi(config);
        }
    }

    public class RESTReciver
    {
        static public void RunRESTReciverControler(string url)
        {
            WebApp.Start<Startup>(url);
        }
    }
}
