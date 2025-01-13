# llm-dotnet

A tiny REPL to try prompting with different AI models using [llm](llm.datasette.io) and [CSnakes](tonybaloney.github.io/CSnakes).

## Setting API Keys

Check the plugin documentation if you want to use API keys in environment variables.

## Adding extra models

This app is bundled with openrouter. There are plugins for hundreds of different models, including ones which run on your machine.

Edit `python/requirements.txt` and add any of the llm plugins, a good place to start is here:
https://pypi.org/user/simonw/
