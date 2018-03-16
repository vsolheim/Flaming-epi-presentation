using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AwkwardPresentation.Models.Properties
{
    public class ClickerModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }

        public DateTime Published_at { get; set; }
    }
}