using AwkwardPresentation.Models.Pages;
using AwkwardPresentation.Models.Properties;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AwkwardPresentation.Business.Services
{
    public class DataSummerizer
    {
        static IContentRepository contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();

        public static object GetSummary(DateTime startTime)
        {
            var clickerPage = contentRepository.GetDefault<ClickerPage>(ContentReference.StartPage);

            if (clickerPage == null)
                return null;

            var data = clickerPage.DataSet
                .Select(d => contentRepository.Get<StupidClickerModel>(d))
                .Where(d => d.Published_at > startTime)
                .Select(d => new ClickerModel(d));

            return data;
        }
    }
}