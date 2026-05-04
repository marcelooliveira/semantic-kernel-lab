//using OpenAI;
//using OpenAI.Chat;
//using System.ClientModel;
//using System.Text.Json;

//var endpoint = "https://models.github.ai/inference";
//var credential = System.Environment.GetEnvironmentVariable("GITHUB_TOKEN");
//var model = "openai/gpt-4o-mini";

//var functionDefinition = ChatTool.CreateFunctionTool(
//    functionName: "getFlightInfo",
//    functionDescription: "Returns information about the next flight between two cities." +
//                  "This includes the name of the airline, flight number and the date and time" +
//                  "of the next flight",
//    functionParameters: BinaryData.FromObjectAsJson(new
//    {
//        type = "object",
//        properties = new
//        {
//            originCity = new
//            {
//                type = "string",
//                description = "The name of the city where the flight originates",
//            },
//            destinationCity = new
//            {
//                type = "string",
//                description = "The flight destination city",
//            }
//        }
//    },
//    new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
//);

//var openAIOptions = new OpenAIClientOptions()
//{
//    Endpoint = new Uri(endpoint)

//};
//var client = new ChatClient(model, new ApiKeyCredential(credential), openAIOptions);
//var requestOptions = new ChatCompletionOptions()
//{
//    Tools = { functionDefinition },
//};

//List<ChatMessage> messages = new List<ChatMessage>()
//{
//    new SystemChatMessage("You an assistant that helps users find flight information."),
//    new UserChatMessage("I'm interested in going to Miami. What is the next flight there from Seattle?"),
//};

//ChatCompletion response = client.CompleteChat(messages, requestOptions);

//// We expect the model to ask for a tool call
//if (response.FinishReason == ChatFinishReason.ToolCalls)
//{
//    // Append the model response to the chat history
//    messages.Add(new AssistantChatMessage(response));

//    // We expect a single tool call
//    if (response.ToolCalls.Count == 1)
//    {
//        ChatToolCall toolCall = response.ToolCalls[0];
//        // We expect the tool to be a function call
//        if (toolCall.Kind == ChatToolCallKind.Function)
//        {
//            var functionArgs = JsonSerializer.Deserialize<Dictionary<string, string>>(toolCall.FunctionArguments);
//            System.Console.WriteLine(String.Format("Calling function {0}", toolCall.FunctionArguments));
//            var callableFunc = Type.GetType("AllowedFunctions").GetMethod(toolCall.FunctionName);
//            // We have to parse the parameters to ensure, they follow in the order,
//            // in which function accepts them.
//            var requiredParams = callableFunc.GetParameters();
//            object[] parsedArgs = new object[requiredParams.Length];
//            for (int i = 0; i < requiredParams.Length; i++)
//            {
//                parsedArgs[i] = functionArgs.GetValueOrDefault(requiredParams[i].Name, "");
//            }
//            string functionReturn = (string)callableFunc.Invoke(null, parsedArgs);
//            System.Console.WriteLine(String.Format("Function returned = {0}", functionReturn));
//            messages.Add(new ToolChatMessage(toolCall.Id, functionReturn));
//            response = client.CompleteChat(messages, requestOptions);
//            System.Console.WriteLine(response.Content[0].Text);
//        }
//    }
//}


//class AllowedFunctions
//{
//    public static string getFlightInfo(string originCity, string destinationCity)
//    {
//        if (originCity == "Seattle" && destinationCity == "Miami")
//        {
//            return JsonSerializer.Serialize(
//                new Dictionary<string, string>
//                {
//                    { "airline", "Delta" },
//                    { "flight_number", "DL123" },
//                    { "flight_date", "May 7th, 2024" },
//                    { "flight_time", "10:00AM" }
//                }
//            );
//        }
//        return JsonSerializer.Serialize(
//            new Dictionary<string, string>
//            {
//                { "error", "No flights found between the cities" }
//            }
//        );
//    }
//}