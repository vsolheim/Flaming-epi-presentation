using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AwkwardPresentation.Models.Pages;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using VisitOslo.Infrastructure.Helpers;

namespace AwkwardPresentation.Controllers
{
    public class PresentationController : ApiController
    {
        //[HttpGet]
        //public int StartSession()
        //{
        //    var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();

        //    var newPage = contentRepository.GetDefault<PresentationModel>(ContentReference.StartPage);

        //    newPage.PageName = "Page"

        //    return newPage.ContentLink.ID;

        //}
    }
}