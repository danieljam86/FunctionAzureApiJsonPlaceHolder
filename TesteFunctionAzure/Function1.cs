using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace TesteFunctionAzure
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
    ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            HttpClient client = new HttpClient();
            string url = "https://jsonplaceholder.typicode.com/posts";
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                dynamic posts = JsonConvert.DeserializeObject(responseContent);

                StringBuilder htmlBuilder = new StringBuilder();
                htmlBuilder.Append("<!DOCTYPE html>");
                htmlBuilder.Append("<html>");
                htmlBuilder.Append("<head>");
                htmlBuilder.Append("<title>Posts</title>");
                htmlBuilder.Append("<style>");
                htmlBuilder.Append("table {");
                htmlBuilder.Append("    border-collapse: collapse;");
                htmlBuilder.Append("    margin: 20px 0;");
                htmlBuilder.Append("    width: 100%;");
                htmlBuilder.Append("}");
                htmlBuilder.Append("th, td {");
                htmlBuilder.Append("    border: 1px solid #ddd;");
                htmlBuilder.Append("    padding: 8px;");
                htmlBuilder.Append("    text-align: left;");
                htmlBuilder.Append("}");
                htmlBuilder.Append("th {");
                htmlBuilder.Append("    background-color: #4CAF50;");
                htmlBuilder.Append("    color: white;");
                htmlBuilder.Append("}");
                htmlBuilder.Append("</style>");
                htmlBuilder.Append("</head>");
                htmlBuilder.Append("<body>");
                htmlBuilder.Append("<h1>Posts</h1>");
                htmlBuilder.Append("<table>");
                htmlBuilder.Append("<thead>");
                htmlBuilder.Append("<tr><th>ID</th><th>Title</th><th>Details</th></tr>");
                htmlBuilder.Append("</thead>");
                htmlBuilder.Append("<tbody>");
                foreach (var post in posts)
                {
                    htmlBuilder.Append("<tr>");
                    htmlBuilder.Append($"<td>{post.id}</td>");
                    htmlBuilder.Append($"<td><a href=\"/api/Function2?id={post.id}\">{post.title}</a></td>");
                    htmlBuilder.Append($"<td>{post.body}</td>");
                    htmlBuilder.Append("</tr>");
                }
                htmlBuilder.Append("</tbody>");
                htmlBuilder.Append("</table>");
                htmlBuilder.Append("</body>");
                htmlBuilder.Append("</html>");

                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = 200,
                    Content = htmlBuilder.ToString()
                };
            }
            else
            {
                return new BadRequestObjectResult("Could not retrieve posts data.");
            }
        }

        [FunctionName("Function2")]
        public static async Task<IActionResult> RunDetails(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Function2")] HttpRequest req,
    ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string idStr = req.Query["id"];
            int id = int.Parse(idStr);
            HttpClient client = new HttpClient();
            string url = $"https://jsonplaceholder.typicode.com/posts/{id}";
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                dynamic post = JsonConvert.DeserializeObject(responseContent);

                StringBuilder htmlBuilder = new StringBuilder();
                htmlBuilder.Append("<!DOCTYPE html>");
                htmlBuilder.Append("<html>");
                htmlBuilder.Append("<head>");
                htmlBuilder.Append("<title>Post Details</title>");
                htmlBuilder.Append("<style>");
                htmlBuilder.Append("table {");
                htmlBuilder.Append("    border-collapse: collapse;");
                htmlBuilder.Append("    margin: 20px 0;");
                htmlBuilder.Append("    width: 100%;");
                htmlBuilder.Append("}");
                htmlBuilder.Append("th, td {");
                htmlBuilder.Append("    border: 1px solid #ddd;");
                htmlBuilder.Append("    padding: 8px;");
                htmlBuilder.Append("    text-align: left;");
                htmlBuilder.Append("}");
                htmlBuilder.Append("th {");
                htmlBuilder.Append("    background-color: #4CAF50;");
                htmlBuilder.Append("    color: white;");
                htmlBuilder.Append("}");
                htmlBuilder.Append("</style>");
                htmlBuilder.Append("</head>");
                htmlBuilder.Append("<body>");
                htmlBuilder.Append("<h1>Post Details</h1>");
                htmlBuilder.Append("<table>");
                htmlBuilder.Append("<tr><th>ID</th><td>" + post.id + "</td></tr>");
                htmlBuilder.Append("<tr><th>Title</th><td>" + post.title + "</td></tr>");
                htmlBuilder.Append("<tr><th>Body</th><td>" + post.body + "</td></tr>");
                htmlBuilder.Append("</table>");
                htmlBuilder.Append("<button onclick='window.history.back()'>Back</button>");
                htmlBuilder.Append("</body>");
                htmlBuilder.Append("</html>");

                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = 200,
                    Content = htmlBuilder.ToString()
                };
            }
            else
            {
                return new BadRequestObjectResult("Could not retrieve post data.");
            }
        }
    }

    
}