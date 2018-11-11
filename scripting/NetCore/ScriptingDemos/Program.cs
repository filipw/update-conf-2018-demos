using System;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ScriptingDemos
{

    class Program
    {
        static void Main(string[] args)
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
                        Run();
                        break;
                    case 2:
                        RunWithHost();
                        break;
                    case 3:
                        RunRepl();
                        break;
                    case 4:
                        RunScriptConfig();
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

        private static void Run()
        {
            var code = File.ReadAllText(GetScriptTestFile("test1.csx"));
            CSharpScript.RunAsync(code, ScriptOptions.Default).GetAwaiter().GetResult();
        }

        private static void RunWithHost()
        {
            var code = File.ReadAllText(GetScriptTestFile("test2.csx"));

            var result = CSharpScript.EvaluateAsync<int>(code, 
                globals: new ScriptHost { Number = 5 }).GetAwaiter().GetResult();

            //result is now 25
            Console.WriteLine(result);
        }

        private static void RunScriptConfig()
        {
            var scriptConfig = new ScriptConfig(GetScriptTestFile("config.csx")).
                Create<MyAppConfig>().Result;

            Console.WriteLine($"DataTarget: {scriptConfig.Target}");
            Console.WriteLine($"AppUrl: {scriptConfig.AppUrl}");
            Console.WriteLine($"CacheTime: {scriptConfig.CacheTime}");
        }

        private static string GetScriptTestFile(string filename)
        {
            return Path.Combine(AppContext.BaseDirectory, "Fixtures", filename);
        }

        private static void RunRepl()
        {
            ScriptState<object> scriptState = null;
            while (true)
            {
                Console.Write("* ");
                var input = Console.ReadLine();
                scriptState = scriptState == null ?
                    CSharpScript.RunAsync(input, ScriptOptions.Default.AddImports("System", "System.Console")).Result :
                    scriptState.ContinueWithAsync(input).Result;
            }
        }
    }
}
