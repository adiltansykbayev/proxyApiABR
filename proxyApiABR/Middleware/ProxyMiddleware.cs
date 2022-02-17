using System.Net.Http.Headers;
using System.Text.Json;
using Google.Apis.Discovery.v1;
using Google.Apis.Discovery.v1.Data;
using Google.Apis.Services;
using proxyApiABR.Services;

namespace proxyApiABR.Middleware
{
    public class ProxyMiddleware
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly RequestDelegate _requestDelegate;
        private readonly MongoCrudService _mongo;

        public ProxyMiddleware(RequestDelegate requestDelegate, MongoCrudService mongoCrudService) {
            _requestDelegate = requestDelegate;
            _mongo = mongoCrudService;
        }

        public async Task Invoke(HttpContext context)
        {
            if (await FindByQuery(context.Request.QueryString.Value) != null) {
                //одинаковые запросы не должны отправляться в гугл
                context.Response.StatusCode = StatusCodes.Status200OK;
                //вытащить с монго по query, сериализовать в модель и в reponse.body
                return;
            }

            var googleUri = BuildGoogleUri(context.Request);
            var requestMessage = BuildRequestMessage(context, googleUri);

            using (var response = await _httpClient.SendAsync(requestMessage))
            {
                FillResponseHeaders(context, response);
                context.Response.StatusCode = (int)response.StatusCode;
                await response.Content.CopyToAsync(context.Response.Body);
                await SaveToMongo(context.Response.Body);
            }

            await _requestDelegate(context);
        }
        private async Task<GoogleModel> FindByQuery(string query) { 
           return await _mongo.GetByQueryAsync(query);
        }
        private async Task SaveToMongo(Stream body) {

            string bodyContent = new StreamReader(body).ReadToEnd();
            List<GoogleModel>? searchList = JsonSerializer.Deserialize<List<GoogleModel>>(bodyContent);

            foreach (var item in searchList) {
                var model = new GoogleModel()
                {
                    Link = item.Link,
                    SearchQuery = item.SearchQuery,
                    Title = item.Title
                };

                await _mongo.CreateAsync(model);
            }
        }

        private void FillResponseHeaders(HttpContext context, HttpResponseMessage responseMessage)
        {
            foreach (var header in responseMessage.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }
        }

        private HttpRequestMessage BuildRequestMessage(HttpContext context, Uri uri) { 
        
            var requestMessage =  new HttpRequestMessage() { 
                Method = GetMethod(context.Request.Method),
                RequestUri = uri,
                Content = new StreamContent(context.Request.Body),

            };

            requestMessage.Headers.Host = uri.Host;

            foreach (var header in context.Request.Headers)
            {
                requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }

            return requestMessage;
        }

        private static HttpMethod GetMethod(string method)
        {
            if (HttpMethods.IsDelete(method)) return HttpMethod.Delete;
            if (HttpMethods.IsGet(method)) return HttpMethod.Get;
            if (HttpMethods.IsHead(method)) return HttpMethod.Head;
            if (HttpMethods.IsOptions(method)) return HttpMethod.Options;
            if (HttpMethods.IsPost(method)) return HttpMethod.Post;
            if (HttpMethods.IsPut(method)) return HttpMethod.Put;
            if (HttpMethods.IsTrace(method)) return HttpMethod.Trace;
            return new HttpMethod(method);
        }
        private Uri BuildGoogleUri( HttpRequest request) {

            if (request.Path.StartsWithSegments("какой то сегмент")) {
            // строить другой uri
            } 

            return new Uri("https://google.com/search?" + request.Path.Value);
        }
    }
}
