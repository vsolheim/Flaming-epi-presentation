using AwkwardPresentation.Models.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AwkwardPresentation.Models.Properties
{
    public class ImageList
    {
        public string InputMessage { get; set; }
        public string SearchTerm { get; set; }
        public List<Slide> Slides { get; set; }
        public string Status { get; set; }
    }

    public class Slide
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }
}