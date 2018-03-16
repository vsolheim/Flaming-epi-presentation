using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AwkwardPresentation.Models.Properties
{
    public class StupidClickerModel : PageData
    {
        public void UpdateStupid(ClickerModel stupid)
        {
            Id = stupid.Id;
            Name = stupid.Name;
            Data = stupid.Data;
            Published_at = stupid.Published_at;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }

        public DateTime Published_at { get; set; }
    }

    public class ClickerModel
    {
        public ClickerModel() { }
        public ClickerModel(StupidClickerModel stupid)
        {
            Id = stupid.Id;
            Name = stupid.Name;
            Data = stupid.Data;
            Published_at = stupid.Published_at;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }

        public DateTime Published_at { get; set; }
    }
}