# llm-dotnet

A tiny REPL to try prompting with different AI models using [llm](llm.datasette.io) and [CSnakes](tonybaloney.github.io/CSnakes).

<img width="1583" alt="CleanShot 2025-01-13 at 16 52 05" src="https://github.com/user-attachments/assets/256060f8-1a0b-482c-9d50-f0ad289adfca" />

## Setting API Keys

Check the plugin documentation if you want to use API keys in environment variables.

There is a `key` command but it doesn't work for all plugins.

## Adding extra models

This app is bundled with openrouter. There are plugins for hundreds of different models, including ones which run on your machine.

Edit `python/requirements.txt` and add any of the llm plugins, a good place to start is here:
https://pypi.org/user/simonw/
