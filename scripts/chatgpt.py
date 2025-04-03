import sys
import os
import datetime as dt
import openai as ai

API_KEY = os.environ['OPENAI_API_KEY'] 
RESPONSE_FOLDER = os.path.expanduser("~/chatgpt/")

def ask_chatgpt(prompt, filename):
    try:
        gpt = ai.OpenAI(api_key=API_KEY)
        response = gpt.chat.completions.create(
            model="gpt-4o",
            messages=[
                {"role": "user", "content": prompt}
            ],
            max_tokens=500
        )
    except Exception as e:
        print(f"Error: {e}")
    
    print_response(response)
    write_response_to_file(response, filename if filename != "" else get_filename())
 
def print_response(response):
    counter = 0
    n = len(response.choices)
    for choice in response.choices:
        counter += 1
        print(f"response {counter}/{n}:")
        print(choice.message.content)

def write_response_to_file(response, filename):
    with open(RESPONSE_FOLDER + filename, "w") as file:
        counter = 0
        n = len(response.choices)
        for choice in response.choices:
            file.write(choice.message.content)

def get_filename():
    now = dt.datetime.now()
    format_now = now.strftime("%Y-%m-%d-%H:%M:%S")
    return format_now + ".txt"

def main():
    if len(sys.argv) < 2:
        print(f"Usage: python ./chatgpt.py {prompt} {filename (optional)}")
        sys.exit(1)

    prompt = sys.argv[1]
    filename = sys.argv[2] if len(sys.argv) > 2 else ""

    ask_chatgpt(prompt, filename)

if __name__ == "__main__":
    main()
