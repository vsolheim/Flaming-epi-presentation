using AwkwardPresentation.Models.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AwkwardPresentation.Business.Services
{
    public class ImageProvider
    {
        static HttpClient client = new HttpClient();

        public static async Task<object> GetImage(string searchText, string staticText = "", string prevImageText = "", string url = "http://text2slides.westeurope.cloudapp.azure.com/text2slides")
        {

            var textObject = JsonConvert.SerializeObject(new
            {
                text = searchText,
                excludedText = prevImageText,
                staticText = staticText
            });

            var returnObject = await SendJsonRequest(textObject, url);
            if (returnObject == null)
            {
                return null;
            }

            var jsonObj = JsonConvert.DeserializeObject<ImageList>(returnObject.ToString());

            return jsonObj;
        }

        public static async Task<Object> SendJsonRequest(object requestObject, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            using (var content = new StringContent((string)requestObject, Encoding.UTF8, "application/json"))
            {
                request.Content = content;
                var response = await client.SendAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                    return null;
                var returnObject = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return returnObject;
            }

        }

        public static async Task<Object> SendFormRequest(object requestObject, string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            using (var content = (FormUrlEncodedContent)requestObject)
            {
                request.Content = content;
                var response = await client.SendAsync(request).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    return null;
                var returnObject = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return returnObject;
            }
        }
    }
}