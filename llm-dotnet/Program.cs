using CommandLine;
using CSnakes.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Options
{
    [Option('m', "model", Required = true, HelpText = "The model to use")]
    public string Model { get; set; }

    [Option('p', "prompt", Required = true, HelpText = "The prompt to use")]
    public string Prompt { get; set; }
}

namespace llm_dotnet
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(opts =>
            {
                var builder = Host
                    .CreateDefaultBuilder(args)
                    .ConfigureServices(services =>
                    {
                        var home = Path.Join(Environment.CurrentDirectory, "python");
                        services
                        .WithPython()
                        .WithHome(home)
                        .WithVirtualEnvironment(Path.Join(home, "env"))
                        .WithPipInstaller()
                        .FromRedistributable(); // Download Python 3.12 and store it locally
                    });
                var app = builder.Build();

                var env = app.Services.GetRequiredService<IPythonEnvironment>();
                var mod = env.LlmWrapper();

                Console.WriteLine("Available models:");
                foreach (string model in mod.GetModels())
                {
                    Console.WriteLine(model);
                }
                Console.WriteLine($"You asked model: {opts.Model} '{opts.Prompt}'");
                Console.Write(mod.Prompt(opts.Model, opts.Prompt));
            });
        }
    }
}
