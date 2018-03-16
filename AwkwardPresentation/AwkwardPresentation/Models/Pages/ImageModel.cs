using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;

namespace AwkwardPresentation.Models.Pages
{
    public class ImageModel
    {
        public virtual string Url { get; set; }
        public virtual string Title { get; set; }
        public virtual string Text { get; set; }

    }

    public class SimpleImageModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
}