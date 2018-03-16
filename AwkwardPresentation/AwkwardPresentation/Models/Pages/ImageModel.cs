using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;

namespace AwkwardPresentation.Models.Pages
{
    [ContentType(DisplayName = "ImageModel", GUID = "44aba054-372e-4641-bd70-99a0f515b081", Description = "")]
    public class ImageModel : PageData
    {

        [Display(
            Name = "Image URL",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 100)]
        public virtual string Url { get; set; }

        [Display(
            Name = "Image title",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 110)]
        public virtual string Title { get; set; }

        [Display(
            Name = "Image text",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 120)]
        public virtual string Text { get; set; }

    }

    public class SimpleImageModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
}