using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Scripting.Hosting;

namespace ScriptingInWebAppDemo
{
    public class WebAppGlobals
    {
        public void AddEndpoint(string name, Func<HttpContext, Func<Task>, Task> endpoint)
        {
            if (!Startup.DynamicMiddleware.ContainsKey(name))
                Startup.DynamicMiddleware.Add(name, endpoint);
        }

        public void RemoveEndpoint(string name)
        {
            if (Startup.DynamicMiddleware.ContainsKey(name))
                Startup.DynamicMiddleware.Remove(name);
        }
    }
}