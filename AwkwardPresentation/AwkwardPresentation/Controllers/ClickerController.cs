using AwkwardPresentation.Business.Services;
using AwkwardPresentation.Models.Pages;
using AwkwardPresentation.Models.Properties;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Web.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace AwkwardPresentation.Controllers
{
    public class ClickerController : ApiController
    {
        static IContentRepository contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();

        [System.Web.Http.HttpPost]
        public ActionResult InputData([FromBody]ClickerModel model)
        {
            if (model != null && model is ClickerModel)
            {
                var clickerPage = contentRepository.GetChildren<ClickerPage>(ContentReference.StartPage).FirstOrDefault();
                if (clickerPage == null)
                {
                    clickerPage = contentRepository.GetDefault<ClickerPage>(ContentReference.StartPage);
                    clickerPage.DataSet = new List<ContentReference>();
                    clickerPage.Name = "clicker num" + (new Random().Next());
                    contentRepository.Save(clickerPage, SaveAction.Publish, AccessLevel.NoAccess);
                }

                var modelPage = contentRepository.GetDefault<StupidClickerModel>(clickerPage.ContentLink);
                modelPage.UpdateStupid(model);

                modelPage.Name = "model num" + (new Random().Next());
                contentRepository.Save(modelPage, SaveAction.Publish, AccessLevel.NoAccess);             
              

                return new JsonDataResult()
                {
                    ContentType = "application/json",
                    Data = true,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            return new JsonDataResult()
            {
                ContentType = "application/json",
                Data = false,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [System.Web.Http.HttpGet]
        public async Task<string> MakeBeep()
        {
            var url = "https://api.particle.io/v1/devices/29003b000f47353136383631/biip?access_token=a75169066cfd3b1cb880840469f4474fbbb923dc";

            var list = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("duration", "1000")
            };
            var formInput = new FormUrlEncodedContent(list);

            await ImageProvider.SendFormRequest(formInput, url);

            return "done";
        }


        public ActionResult GetAllData()
        {
            var clickerPage = contentRepository.GetChildren<ClickerPage>(ContentReference.StartPage).FirstOrDefault();
            if (clickerPage == null)
            {
                return null;
            }
            var data = clickerPage.DataSet;
            return new JsonResult()
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}