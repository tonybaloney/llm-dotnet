import llm


def prompt(model: str, prompt: str) -> str:
    """Call the LLM with the given prompt and return the response."""
    return str(llm.get_model(model).prompt(prompt))


def get_models() -> list[str]:
    return [model.model_id for model in llm.get_models()]


def set_api_key(model: str, api_key: str) -> None:
    llm.get_model(model).key = api_key
