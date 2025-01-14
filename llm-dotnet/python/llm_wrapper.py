import llm


def prompt(model: str, prompt: str) -> str:
    """Call the LLM with the given prompt and return the response."""
    model = llm.get_model(model)
    can_stream = model.can_stream  # Some models can't stream

    return model.prompt(prompt, stream=can_stream).text()


def get_models() -> list[str]:
    return [model.model_id for model in llm.get_models()]


def set_api_key(model: str, api_key: str) -> None:
    llm.get_model(model).key = api_key
