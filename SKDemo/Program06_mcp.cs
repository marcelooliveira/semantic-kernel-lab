//// ============================================================
////  Program06 — MCP (Model Context Protocol)
//// ============================================================
//// Este exemplo demonstra o uso de MCP com Semantic Kernel:
//// 
//// MCP (Model Context Protocol) é um protocolo aberto que 
//// permite conectar LLMs a ferramentas externas, fontes de dados
//// e contexto de forma padronizada.
////
//// Neste exemplo, criamos um MCP Server simulado que fornece:
//// 1. Acesso a um sistema de arquivos virtual
//// 2. Busca em base de conhecimento
//// 3. Execução de comandos do sistema
////
//// O Semantic Kernel se conecta a esse servidor MCP e pode
//// invocar as ferramentas disponibilizadas.
////
//// Uso:
////   dotnet run -- 06 "sua pergunta aqui"
////
//// Exemplo:
////   dotnet run -- 06 "Liste os arquivos no diretório de documentos"
//// ============================================================

//using Microsoft.SemanticKernel;
//using Microsoft.SemanticKernel.ChatCompletion;
//using Microsoft.SemanticKernel.Connectors.OpenAI;
//using System.ComponentModel;
//using System.Text.Json;

//// ── Main Program ──────────────────────────────────────────
//var endpoint = "https://models.github.ai/inference";
//var credential = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
//var model = "openai/gpt-4o-mini";

//if (string.IsNullOrWhiteSpace(credential))
//{
//    Console.Error.WriteLine("Erro: variável de ambiente GITHUB_TOKEN não configurada.");
//    Environment.Exit(1);
//}

//// Construir o kernel com GitHub Models
//var httpClient = new HttpClient();
//httpClient.BaseAddress = new Uri(endpoint);

//var kernel = Kernel.CreateBuilder()
//    .AddOpenAIChatCompletion(
//        modelId: model,
//        apiKey: credential,
//        httpClient: httpClient)
//    .Build();

//// ── MCP Server Plugin ──────────────────────────────────────
//// Registrar o plugin MCP Server que simula um servidor MCP
//// com várias ferramentas disponíveis
//kernel.ImportPluginFromType<MCPServerPlugin>("MCPServer");

//// Obter pergunta do usuário (ou usar padrão)
//string userQuery = args.Length > 1
//    ? string.Join(" ", args.Skip(1))
//    : "Liste os arquivos no diretório de documentos";

//Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
//Console.WriteLine("║          MCP - Model Context Protocol Demo               ║");
//Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
//Console.WriteLine();
//Console.WriteLine($"🤖 Pergunta: {userQuery}");
//Console.WriteLine();

//// ── Configurar Auto Function Calling ───────────────────────
//// O Semantic Kernel pode automaticamente chamar funções
//// quando o modelo decide que precisa delas
//var executionSettings = new OpenAIPromptExecutionSettings
//{
//    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
//    Temperature = 0.7,
//    MaxTokens = 1000
//};

//Console.WriteLine("🔧 MCP Server: Ferramentas disponíveis carregadas");
//Console.WriteLine("   - ListFiles: Lista arquivos em um diretório");
//Console.WriteLine("   - ReadFile: Lê conteúdo de um arquivo");
//Console.WriteLine("   - SearchKnowledge: Busca na base de conhecimento");
//Console.WriteLine("   - ExecuteCommand: Executa comando do sistema");
//Console.WriteLine();

//// ── Executar com Auto Function Calling ─────────────────────
//Console.WriteLine("💡 Processando solicitação com MCP...");
//Console.WriteLine();

//var chatHistory = new ChatHistory();
//chatHistory.AddSystemMessage(
//    "Você é um assistente útil que pode acessar ferramentas via MCP " +
//    "(Model Context Protocol). Use as ferramentas disponíveis quando necessário " +
//    "para responder às perguntas do usuário. Sempre explique quais ferramentas " +
//    "você está usando e por quê."
//);
//chatHistory.AddUserMessage(userQuery);

//var chat = kernel.GetRequiredService<IChatCompletionService>();
//var response = await chat.GetChatMessageContentAsync(
//    chatHistory,
//    executionSettings: executionSettings,
//    kernel: kernel
//);

//Console.WriteLine("📝 Resposta do Assistente:");
//Console.WriteLine("─────────────────────────────────────────────────────────");
//Console.WriteLine(response.Content);
//Console.WriteLine("─────────────────────────────────────────────────────────");
//Console.WriteLine();

//// ── Exemplo de chat continuado ─────────────────────────────
//Console.WriteLine("🔄 Continuando a conversa...");
//chatHistory.Add(response);
//chatHistory.AddUserMessage("Agora leia o conteúdo do arquivo README.md");

//response = await chat.GetChatMessageContentAsync(
//    chatHistory,
//    executionSettings: executionSettings,
//    kernel: kernel
//);

//Console.WriteLine();
//Console.WriteLine("📝 Resposta do Assistente:");
//Console.WriteLine("─────────────────────────────────────────────────────────");
//Console.WriteLine(response.Content);
//Console.WriteLine("─────────────────────────────────────────────────────────");
//Console.WriteLine();
//Console.WriteLine("✨ Demo MCP concluída com sucesso!");

//// ══════════════════════════════════════════════════════════
////  MCP Server Plugin
//// ══════════════════════════════════════════════════════════
//// Este plugin simula um servidor MCP que fornece ferramentas
//// para o modelo de IA usar.
//// 
//// Em um cenário real, isso seria um servidor MCP separado
//// que o Semantic Kernel se conectaria via protocolo MCP.
//// ══════════════════════════════════════════════════════════

//class MCPServerPlugin
//{
//    // Sistema de arquivos virtual simulado
//    private static readonly Dictionary<string, List<string>> _fileSystem = new()
//    {
//        ["documentos"] = new List<string> { "README.md", "manual.pdf", "notas.txt" },
//        ["imagens"] = new List<string> { "foto1.jpg", "foto2.png", "logo.svg" },
//        ["codigo"] = new List<string> { "app.cs", "config.json", "teste.cs" }
//    };

//    private static readonly Dictionary<string, string> _fileContents = new()
//    {
//        ["README.md"] = "# Projeto Demo MCP\n\nEste é um projeto de demonstração do Model Context Protocol.\n\n## Recursos\n- Integração com LLMs\n- Ferramentas customizadas\n- Base de conhecimento",
//        ["manual.pdf"] = "[Conteúdo do PDF] Manual do Usuário - Instruções completas sobre o uso do sistema...",
//        ["notas.txt"] = "Notas importantes:\n1. Configurar variável de ambiente GITHUB_TOKEN\n2. Instalar dependências\n3. Executar testes"
//    };

//    private static readonly Dictionary<string, string> _knowledgeBase = new()
//    {
//        ["mcp"] = "Model Context Protocol (MCP) é um protocolo aberto que permite conectar LLMs a ferramentas externas e fontes de dados de forma padronizada.",
//        ["semantic-kernel"] = "Semantic Kernel é um SDK que integra LLMs com código convencional, permitindo criar aplicações de IA avançadas.",
//        ["rag"] = "RAG (Retrieval-Augmented Generation) é uma técnica que combina recuperação de informações com geração de texto por LLMs."
//    };

//    /// <summary>
//    /// Lista arquivos em um diretório especificado
//    /// </summary>
//    [KernelFunction, Description("Lista todos os arquivos em um diretório especificado do sistema de arquivos")]
//    public static string ListFiles(
//        [Description("O nome do diretório (ex: documentos, imagens, codigo)")]
//        string directory)
//    {
//        Console.WriteLine($"   🔧 MCP Tool Called: ListFiles(directory: '{directory}')");

//        if (_fileSystem.TryGetValue(directory.ToLower(), out var files))
//        {
//            var result = new
//            {
//                directory = directory,
//                files = files,
//                count = files.Count
//            };
//            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
//        }

//        return JsonSerializer.Serialize(new
//        {
//            error = $"Diretório '{directory}' não encontrado",
//            availableDirectories = _fileSystem.Keys.ToList()
//        });
//    }

//    /// <summary>
//    /// Lê o conteúdo de um arquivo especificado
//    /// </summary>
//    [KernelFunction, Description("Lê e retorna o conteúdo completo de um arquivo especificado")]
//    public static string ReadFile(
//        [Description("O nome do arquivo incluindo extensão (ex: README.md)")]
//        string fileName)
//    {
//        Console.WriteLine($"   🔧 MCP Tool Called: ReadFile(fileName: '{fileName}')");

//        if (_fileContents.TryGetValue(fileName, out var content))
//        {
//            var result = new
//            {
//                fileName = fileName,
//                content = content,
//                size = content.Length
//            };
//            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
//        }

//        return JsonSerializer.Serialize(new
//        {
//            error = $"Arquivo '{fileName}' não encontrado ou sem conteúdo disponível",
//            availableFiles = _fileContents.Keys.ToList()
//        });
//    }

//    /// <summary>
//    /// Busca informações na base de conhecimento
//    /// </summary>
//    [KernelFunction, Description("Busca informações sobre um tópico específico na base de conhecimento")]
//    public static string SearchKnowledge(
//        [Description("O termo de busca ou tópico (ex: mcp, semantic-kernel, rag)")]
//        string topic)
//    {
//        Console.WriteLine($"   🔧 MCP Tool Called: SearchKnowledge(topic: '{topic}')");

//        var topicLower = topic.ToLower();

//        // Busca exata
//        if (_knowledgeBase.TryGetValue(topicLower, out var info))
//        {
//            var result = new
//            {
//                topic = topic,
//                information = info,
//                source = "knowledge_base"
//            };
//            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
//        }

//        // Busca parcial
//        var partialMatches = _knowledgeBase
//            .Where(kv => kv.Key.Contains(topicLower) || kv.Value.ToLower().Contains(topicLower))
//            .ToList();

//        if (partialMatches.Any())
//        {
//            var result = new
//            {
//                topic = topic,
//                matches = partialMatches.Select(kv => new { term = kv.Key, info = kv.Value }),
//                source = "knowledge_base"
//            };
//            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
//        }

//        return JsonSerializer.Serialize(new
//        {
//            error = $"Nenhuma informação encontrada sobre '{topic}'",
//            availableTopics = _knowledgeBase.Keys.ToList()
//        });
//    }

//    /// <summary>
//    /// Executa um comando do sistema (simulado)
//    /// </summary>
//    [KernelFunction, Description("Executa um comando do sistema e retorna o resultado")]
//    public static string ExecuteCommand(
//        [Description("O comando a ser executado (ex: date, ls, dir)")]
//        string command)
//    {
//        Console.WriteLine($"   🔧 MCP Tool Called: ExecuteCommand(command: '{command}')");

//        // Simulação de comandos comuns
//        var commandResults = new Dictionary<string, string>
//        {
//            ["date"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
//            ["time"] = DateTime.Now.ToString("HH:mm:ss"),
//            ["ls"] = "README.md\nProgram.cs\nConfig.json",
//            ["dir"] = "README.md\nProgram.cs\nConfig.json",
//            ["whoami"] = "mcp_user",
//            ["pwd"] = "/home/mcp/demo"
//        };

//        if (commandResults.TryGetValue(command.ToLower(), out var output))
//        {
//            var result = new
//            {
//                command = command,
//                output = output,
//                exitCode = 0
//            };
//            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
//        }

//        return JsonSerializer.Serialize(new
//        {
//            error = $"Comando '{command}' não reconhecido ou não permitido",
//            allowedCommands = commandResults.Keys.ToList()
//        });
//    }
//}
