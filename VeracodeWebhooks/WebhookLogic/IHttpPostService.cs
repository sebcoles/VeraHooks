using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebhookLogic
{
    public interface IHttpPostService
    {
        Task<HttpResponseMessage> SendMessage(string json, string url);
    }
    public class HttpPostService : IHttpPostService
    {
        public async Task<HttpResponseMessage> SendMessage(string json, string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
                return response;
            }
        }
    }
}
