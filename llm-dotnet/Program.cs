using CommandLine;
using CSnakes.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

class Options
{
    [Option('m', "model", Required = true, HelpText = "The default model to use")]
    public string Model { get; set; }

    [Option("api-key", Required = false, HelpText = "The API key to use for the model")]
    public string? ApiKey { get; set; }
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

                // Display a fancy welcome message
                AnsiConsole.MarkupLine("[bold]Welcome to the LLM REPL![/]");

                if (!string.IsNullOrEmpty(opts.ApiKey))
                {
                    mod.SetApiKey(opts.Model, opts.ApiKey);
                }

                AnsiConsole.MarkupLine("Type [blue]\"models\"[/] to list available models.");
                AnsiConsole.MarkupLine("Type [blue]\"model <name>\"[/] to change the model.");
                AnsiConsole.MarkupLine("Type [blue]\"key <key>\"[/] to change the API key.");
                AnsiConsole.MarkupLine("Type [blue]\"exit\"[/] to exit the REPL.");
                // Enter REPL loop
                while (true)
                {
                    // Read user input using Spectre.Console
                    var userPrompt = AnsiConsole.Ask<string>("[green]Prompt:[/]");

                    if (userPrompt == "exit")
                    {
                        break;
                    }
                    else if (userPrompt == "models")
                    {
                        AnsiConsole.MarkupLine("[bold]Available models:[/]");
                        // They are unsorted, so sort them first otherwise it's really hard to read
                        var models = mod.GetModels().ToList();
                        models.Sort();
                        foreach (string model in models)
                        {
                            AnsiConsole.MarkupLine($"- {model}");
                        }
                        continue;
                    }
                    else if (userPrompt.StartsWith("model "))
                    {
                        opts.Model = userPrompt.Substring(6);
                        if (!string.IsNullOrEmpty(opts.ApiKey))
                            mod.SetApiKey(opts.Model, opts.ApiKey);
                        AnsiConsole.MarkupLine($"[bold]Model changed to {opts.Model}[/]");
                        continue;
                    }
                    else if (userPrompt.StartsWith("key "))
                    {
                        opts.ApiKey = userPrompt.Substring(4);
                        if (!string.IsNullOrEmpty(opts.ApiKey))
                            mod.SetApiKey(opts.Model, opts.ApiKey);
                        AnsiConsole.MarkupLine($"[bold]API key updated[/]");
                        continue;
                    }

                    // Display the response from the model
                    try
                    {
                        var response = mod.Prompt(opts.Model, userPrompt);
                        AnsiConsole.MarkupLine($"{opts.Model} : [blue]{response}[/]");
                    }
                    catch (PythonInvocationException ex)
                    {
                        AnsiConsole.MarkupLine($"[red]Error: {ex.InnerException!.Message}[/]");
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
