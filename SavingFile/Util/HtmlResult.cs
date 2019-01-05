using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SavingFile.Util
{
    public class HtmlResult : IActionResult
    {
        string htmlCode;
        public HtmlResult(string html)
        {
            htmlCode = html;
        }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            await context.HttpContext.Response.WriteAsync(htmlCode);
        }
    }
}
