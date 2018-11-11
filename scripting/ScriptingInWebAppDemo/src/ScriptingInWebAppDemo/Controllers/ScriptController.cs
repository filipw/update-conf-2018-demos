using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ScriptingInWebAppDemo.Controllers
{
    [Route("api/[controller]")]
    public class ScriptController : Controller
    {
        private static readonly ScriptOptions ScriptOpts = ScriptOptions.Default.AddReferences(
                    typeof(ScriptController).GetTypeInfo().Assembly,
                    typeof(HttpContext).GetTypeInfo().Assembly).
                AddImports("ScriptingInWebAppDemo", "Microsoft.AspNetCore.Http");

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody]Script script)
        {
            if (script?.Code == null) return BadRequest();

            var compiledScript = CSharpScript.Create(script.Code, ScriptOpts, typeof(WebAppGlobals));
            var diagnostics = compiledScript.GetCompilation().GetDiagnostics();

            if (diagnostics.Any())
            {
                return Ok(new
                {
                    error = string.Join(Environment.NewLine, diagnostics.Select(x => x.ToString()))
                });
            }

            var result = await compiledScript.RunAsync(new WebAppGlobals(), ex => true);

            if (result.Exception != null)
            {
                return Ok(new
                {
                    error = result.Exception.Message
                });
            }

            return Ok(result.ReturnValue);
        }
    }
}


/*
AddEndpoint("foo", async (c, next) => {
    if (c.Request.Path.ToString().Contains("foo"))
    {
        await c.Response.WriteAsync("bar!");
        return;
    }
    await next();
});
RemoveEndpoint("foo")

 */
