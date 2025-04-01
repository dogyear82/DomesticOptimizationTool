from langchain.agents import Tool

def silly_tool(sillyThing1: str, sillyThing2: str = "") -> str:
    return f"Here's something silly with {sillyThing1} and {sillyThing2}"

tools = [
    Tool.from_function(
        name="silly_tool",
        func=silly_tool,
        description="Generates a silly or random sentence using two words provided by the user."
    )
]
