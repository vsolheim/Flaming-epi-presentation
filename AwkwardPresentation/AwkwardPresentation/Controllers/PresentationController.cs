using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AwkwardPresentation.Models.Pages;
using EPiServer;
using EPiServer.Core;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using VisitOslo.Infrastructure.Helpers;

namespace AwkwardPresentation.Controllers
{
    public class PresentationController : ApiController
    {
        [HttpGet]
        public int StartSession()
        {
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();

            var newPage = contentRepository.GetDefault<PresentationModel>(ContentReference.StartPage);
            newPage.PageName = "New page number " + (new Random().Next());
            contentRepository.Save(newPage, AccessLevel.Publish);

            return newPage.ContentLink.ID;
        }


        [HttpPost]
        public void UploadSlideData([FromBody] SimpleImageModel model)
        {
            var contentLoaderWrapper = ServiceLocator.Current.GetInstance<ContentLoaderWrapper>();

            var imageModel = new ImageModel()
            {
                Text = model.Text
            };

            var presentation = contentLoaderWrapper.GetPageFromReference<PresentationModel>(new ContentReference(model.Id));
            presentation.Images.Add(imageModel.ContentLink);
        }
    }
}