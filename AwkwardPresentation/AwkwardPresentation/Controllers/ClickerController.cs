using AwkwardPresentation.Models.Properties;
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
        [System.Web.Http.HttpPost]
        public bool InputData([FromBody]ClickerModel model)
        {
            //if (model != null && model is ClickerModel)
            //{
            //    using (var db = new DatabaseContext())
            //    {
            //        db.ClickerData.Add(model);
            //        try
            //        {
            //            db.SaveChanges();
            //        }
            //        catch (Exception e)
            //        {
            //            return false;
            //        }
            //        return true;
            //    }
            //}
            return false;
        }
    }
}