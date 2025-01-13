using CommandLine;
using CSnakes.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

class Options
{
    [Option('m', "model", Required = true, HelpText = "The default model to use")]
    public string Model { get; set; }
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
                        .WithUvInstaller()
                        .FromRedistributable(); // Download Python 3.12 and store it locally
                    });
                var app = builder.Build();

                var env = app.Services.GetRequiredService<IPythonEnvironment>();
                var mod = env.LlmWrapper();


                // Display available models
                AnsiConsole.MarkupLine("[bold]Available models:[/]");
                foreach (string model in mod.GetModels())
                {
                    AnsiConsole.MarkupLine($"- {model}");
                }
                AnsiConsole.MarkupLine("Use [blue]\"model <name>\"[/] to change the model");

                // Enter REPL loop
                while (true)
                {
                    // Read user input using Spectre.Console
                    var userPrompt = AnsiConsole.Ask<string>("[green]Prompt:[/]");

                    if (userPrompt == "exit")
                    {
                        break;
                    }
                    else if (userPrompt.StartsWith("model "))
                    {
                        opts.Model = userPrompt.Substring(6);
                        AnsiConsole.MarkupLine($"[bold]Model changed to {opts.Model}[/]");
                        continue;
                    }

                    // Display the response from the model
                    try
                    {
                        var response = mod.Prompt(opts.Model, userPrompt);
                        AnsiConsole.MarkupLine($"{opts.Model} : [blue]{response}[/]");
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
                    }
                }
            });
        }
    }
}
