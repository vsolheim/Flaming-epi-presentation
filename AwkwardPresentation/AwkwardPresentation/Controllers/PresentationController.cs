using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using AwkwardPresentation.Business.Services;
using AwkwardPresentation.Models.Pages;
using AwkwardPresentation.Models.Properties;
using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Internal;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Web.Mvc;
using VisitOslo.Infrastructure.Helpers;

namespace AwkwardPresentation.Controllers
{
    public class PresentationController : ApiController
    {
        [System.Web.Http.HttpGet]
        public int StartSession()
        {
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();

            var newPage = contentRepository.GetDefault<PresentationModel>(ContentReference.StartPage);
            newPage.PageName = "New page number " + (new Random().Next());

            // Need AccessLevel.NoAccess to get permission to create and save this new page.
            contentRepository.Save(newPage, SaveAction.Publish, AccessLevel.NoAccess);

            return newPage.ContentLink.ID;
        }


        [System.Web.Http.HttpGet]
        public ActionResult GetSlideData(int id)
        {
            var contentReference = new ContentReference(id);
            if (contentReference == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var contentLoader = ServiceLocator.Current.GetInstance<ContentLoader>();
            var images = contentLoader.GetChildren<ImageModel>(contentReference);


            SimpleImageModel image = null;
            if (images != null && images.Any())
            {
                var firstImage = images.First();

                image = new SimpleImageModel()
                {
                    Url = firstImage.Url,
                    Text = firstImage.PageName
                };
            }


            return new JsonDataResult()
            {
                ContentType = "application/json",
                Data = image,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        [System.Web.Http.HttpPost]
        public async Task<ActionResult> UploadSlideData([FromBody] SimpleImageModel model)
        {
            if (model == null || model.Id == 0)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var contentLoaderWrapper = ServiceLocator.Current.GetInstance<ContentLoaderWrapper>();
            var presentation = contentLoaderWrapper.GetPageFromReference<PresentationModel>(new ContentReference(model.Id));

            if (presentation != null)
            {
                var iamgeList = await ImageProvider.GetImage(model.Text) as ImageList;
                var imageModel = contentRepository.GetDefault<ImageModel>(presentation.ContentLink);
                imageModel.PageName = "New imagemodel number" + new Random().Next();
                imageModel.Text = model.Text;
                imageModel.Url = iamgeList?.Slides?.FirstOrDefault().Url;

                // Need AccessLevel.NoAccess to get permission to create and save this new page.
                contentRepository.Save(imageModel, SaveAction.Publish, AccessLevel.NoAccess);

                throw new HttpResponseException(HttpStatusCode.Accepted);
            }

            throw new HttpResponseException(HttpStatusCode.BadRequest);
        }

        public async Task<ActionResult> SummaryPage(int id)
        {
            var contentReference = new ContentReference(id);
            if (contentReference == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var contentLoader = ServiceLocator.Current.GetInstance<ContentLoader>();
            var presentation = contentLoader.Get<PresentationModel>(contentReference);

            var data = DataSummerizer.GetSummary(presentation.Created);

            return null;
        }

    }
}