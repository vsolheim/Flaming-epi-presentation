using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;

namespace AwkwardPresentation.Models.Pages
{
    [ContentType(DisplayName = "PresentationModel", GUID = "6bd7cc52-a0fb-4c48-82d9-4c3dfa0a50a4", Description = "")]
    public class PresentationModel : PageData
    {

        [Display(
            Name = "ID",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 100)]
        public virtual int Id { get; set; }


        [Display(
            Name = "Main body",
            Description = "",
            GroupName = SystemTabNames.Content,
            Order = 110)]
        public virtual IList<ImageModel> Images { get; set; }
    }
}