using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ScriptingDemos
{

    class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                Console.Write("Please provide an option: ");
                var opt = Console.ReadLine();

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();

                switch (int.Parse(opt))
                {
                    case 1:
                        await Run();
                        break;
                    case 2:
                        await RunWithHost();
                        break;
                    case 3:
                        await RunRepl();
                        break;
                    case 4:
                        await RunScriptConfig();
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        private static async Task Run()
        {
            var code = File.ReadAllText(GetScriptTestFile("test1.csx"));
            await CSharpScript.RunAsync(code);
        }

        private static async Task RunWithHost()
        {
            var code = File.ReadAllText(GetScriptTestFile("test2.csx"));

            var result = await CSharpScript.EvaluateAsync<int>(code,
                globals: new ScriptHost { Number = 5 });

            //result is now 25
            Console.WriteLine(result);
        }

        private static async Task RunScriptConfig()
        {
            var scriptConfig = await new ScriptConfig(GetScriptTestFile("config.csx")).
                Create<MyAppConfig>();

            Console.WriteLine($"DataTarget: {scriptConfig.Target}");
            Console.WriteLine($"AppUrl: {scriptConfig.AppUrl}");
            Console.WriteLine($"CacheTime: {scriptConfig.CacheTime}");
        }

        private static string GetScriptTestFile(string filename)
        {
            return Path.Combine(AppContext.BaseDirectory, "Fixtures", filename);
        }

        private static async Task RunRepl()
        {
            ScriptState<object> scriptState = null;
            while (true)
            {
                Console.Write("* ");
                var input = Console.ReadLine();
                scriptState = scriptState == null ?
                    await CSharpScript.RunAsync(input, ScriptOptions.Default.AddImports("System", "System.Console")) :
                    await scriptState.ContinueWithAsync(input);
            }
        }
    }
}
