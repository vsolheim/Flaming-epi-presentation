using AwkwardPresentation.Models.Pages;
using AwkwardPresentation.Models.Properties;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace AwkwardPresentation.Controllers
{
    public class ClickerController : ApiController
    {
        static IContentRepository contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();

        [System.Web.Http.HttpPost]
        public bool InputData([FromBody]ClickerModel model)
        {
            if (model != null && model is ClickerModel)
            {
                var clickerPage = contentRepository.GetChildren<ClickerPage>(ContentReference.StartPage).FirstOrDefault();
                if (clickerPage == null)
                {
                    clickerPage = contentRepository.GetDefault<ClickerPage>(ContentReference.StartPage);
                }
                clickerPage.DataSet.Add(model);
                return true;
            }
            return false;
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