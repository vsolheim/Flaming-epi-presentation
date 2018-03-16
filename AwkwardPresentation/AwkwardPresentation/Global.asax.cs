using System;
using System.Web.Http;
using System.Web.Mvc;
using AwkwardPresentation.Infrastructure;

namespace AwkwardPresentation
{
    public class EPiServerApplication : EPiServer.Global
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //Tip: Want to call the EPiServer API on startup? Add an initialization module instead (Add -> New Item.. -> EPiServer -> Initialization Module)
            // Manually installed WebAPI 2.2 after making an MVC project.
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}