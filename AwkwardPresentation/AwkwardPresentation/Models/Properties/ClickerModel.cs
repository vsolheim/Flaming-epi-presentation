using EPiServer.Core;
using EPiServer.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AwkwardPresentation.Models.Properties
{
    [ContentType(DisplayName = "StupidClickerModel", GUID = "6bd7cc12-a0fb-2c48-82d9-4c3dfa0a50a4", Description = "")]
    public class StupidClickerModel : PageData
    {
        public void UpdateStupid(ClickerModel stupid)
        {
            Name = stupid.Name;
            Data = stupid.Data;
            Published_at = stupid.Published_at;
        }
        public override string Name { get; set; }
        public virtual string Data { get; set; }

        public virtual DateTime Published_at { get; set; }
    }

    public class ClickerModel
    {
        public ClickerModel() { }
        public ClickerModel(StupidClickerModel stupid)
        {
            Name = stupid.Name;
            Data = stupid.Data;
            Published_at = stupid.Published_at;
        }
        public string Name { get; set; }
        public string Data { get; set; }

        public DateTime Published_at { get; set; }
    }
}