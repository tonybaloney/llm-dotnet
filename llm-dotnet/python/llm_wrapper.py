import llm


def prompt(model: str, prompt: str) -> str:
    """Call the LLM with the given prompt and return the response."""
    return str(llm.get_model(model).prompt(prompt))


def get_models() -> list[str]:
    return [model.model_id for model in llm.get_models()]
