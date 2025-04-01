from fastapi import FastAPI
from fastapi.responses import StreamingResponse
from models import ChatRequest
import ollama

app = FastAPI()

@app.post("/chat")
async def chat(req: ChatRequest):
    def stream_response():
        messages = [
            {"role": m.role, "content": m.content}
            for m in req.messages
        ]

        for chunk in ollama.chat(
            model=req.model,
            messages=messages,
            stream=True
        ):
            yield f"data: {chunk['message']['content']}\n\n"

        yield "event: end\ndata: done\n\n"

    return StreamingResponse(stream_response(), media_type="text/event-stream")
