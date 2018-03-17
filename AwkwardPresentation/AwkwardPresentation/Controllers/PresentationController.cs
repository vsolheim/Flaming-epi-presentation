using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
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
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
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

            // Just changing the page name to contain its ContentLink ID, to make it more clear in the CMS
            newPage.PageName = "Page nr. " + newPage.ContentLink.ID;
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

            if (id == 0)
            {
                return new JsonDataResult()
                {
                    ContentType = "application/json",
                    Data = new SimpleImageModel()
                    {
                        Url = "https://assets3.thrillist.com/v1/image/2546883/size/tmg-article_tall.jpg",
                        Text = "Some text that caused this image"
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
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
                    Text = firstImage.Text
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

                // Just changing the page name to contain its ContentLink ID, to make it more clear in the CMS
                imageModel.PageName = "Imagemodel nr. " + imageModel.ContentLink.ID;
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

            var sensorData = DataSummerizer.SensorySummary(presentation.Created);

            return null;
        }

    }
}