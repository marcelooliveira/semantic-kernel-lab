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

//var requestOptions = new ChatCompletionOptions()
//{
//    Temperature = 1,
//    MaxOutputTokenCount = 1000,
//};

//List<ChatMessage> userContent = new List<ChatMessage>
//{
//    new UserChatMessage(
//        ChatMessageContentPart.CreateTextPart("What's in this image?"),
//        ChatMessageContentPart.CreateImagePart(
//            BinaryData.FromBytes(File.ReadAllBytes("sample.jpg")), "image/jpeg", ChatImageDetailLevel.Low
//        )
//    )
//];

//var response = client.CompleteChat(userContent, requestOptions);
//System.Console.WriteLine(response.Value.Content[0].Text);