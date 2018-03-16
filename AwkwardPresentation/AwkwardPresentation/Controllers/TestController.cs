using AwkwardPresentation.Business.Services;
using AwkwardPresentation.Models.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace AwkwardPresentation.Controllers
{
    public class TestController : ApiController
    {
        [System.Web.Http.HttpPost]
        public async Task<ActionResult> Test()
        {
            string a = null;

            if (a != null)
                return new JsonResult()
                {
                    Data = new { Result = "Yay!" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            else
                return new JsonResult()
                {
                    Data = new { Result = "Aaw!" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
        }

        [System.Web.Http.HttpGet]
        public async Task<ActionResult> RunTest()
        {

            var a = "http://text2slides.westeurope.cloudapp.azure.com/text2slides";
            var b = "Test of concept with a few words";
            var data = await ImageProvider.GetImage(b, "", "", a) as ImageList;
            return new JsonResult()
            {
                Data = new { Result = data },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        public async Task<ActionResult> ClickerTest()
        {
            var dummy = new ClickerModel
            {
                Name = "test",
                Data = "DummyData" + new Random(10),
                Published_at = DateTime.Now
            };

            var payload = JsonConvert.SerializeObject(dummy);

            var result = await ImageProvider.GetImage(payload, "http://placeholder.no/api/clicker/inputdata");

            if (result != null && result is bool && (bool)result)
                return new JsonResult()
                {
                    Data = new { Result = "Success" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            else
                return new JsonResult()
                {
                    Data = new { Result = "Failure" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
        }

        [System.Web.Http.HttpGet]
        public async Task<ActionResult> summaryTest()
        {
            
            return new JsonResult()
            {
                Data = new { Result = await DataSummerizer.TextSummary(null) },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}