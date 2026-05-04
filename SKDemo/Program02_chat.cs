//using OpenAI;
//using OpenAI.Chat;
//using System.ClientModel;

//var endpoint = "https://models.github.ai/inference";
//var credential = System.Environment.GetEnvironmentVariable("GITHUB_TOKEN");
//var model = "openai/gpt-4o-mini";

//var openAIOptions = new OpenAIClientOptions()
//{
//    Endpoint = new Uri(endpoint)

//};

//var client = new ChatClient(model, new ApiKeyCredential(credential), openAIOptions);

//List<ChatMessage> messages = new List<ChatMessage>()
//{
//    new SystemChatMessage("You are a helpful assistant."),
//    new UserChatMessage("What is the capital of France?"),
//};

//var response = client.CompleteChat(messages);
//System.Console.WriteLine(response.Value.Content[0].Text);
//// Append the model response to the chat history.
//messages.Add(new AssistantChatMessage(response.Value.Content[0].Text));
//// Append new user question.
//messages.Add(new UserChatMessage("What about Spain?"));

//response = client.CompleteChat(messages);
//System.Console.WriteLine(response.Value.Content[0].Text);