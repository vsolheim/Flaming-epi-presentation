using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AwkwardPresentation.Business.Services
{
    public class ImageProvider
    {
        static HttpClient client = new HttpClient();

        public static async Task<object> RunAsync(string payload = null, string url = "http://placeholder.no/test/test")
        {
            if (payload == null)
                payload = "Yay!";

            var request = new HttpRequestMessage(HttpMethod.Post, url);

            using (var content = new StringContent(payload, Encoding.UTF8, "application/json"))
            {
                request.Content = content;
                var response = await client.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }

            //var response = await client.PostAsJsonAsync(url, payload);
            //string result = "";
            //if (response.IsSuccessStatusCode)
            //{
            //    var a = await response.Content.ReadAsStringAsync();
            //    dynamic b = JsonConvert.DeserializeObject(a);
            //    if (b is bool)
            //        return b;
            //    if (b != null && b.Result != null)
            //        result = b.Result;
            //}
            //return result;
        }
    }
}