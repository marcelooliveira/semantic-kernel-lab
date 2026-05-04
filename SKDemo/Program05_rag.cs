//// ============================================================
////  Program05 — RAG (Retrieval-Augmented Generation)
//// ============================================================
//// Este exemplo demonstra um caso simples de RAG usando
//// Semantic Kernel:
//// 1. Uma base de conhecimento (Knowledge Base) com documentos
//// 2. Recuperação (Retrieval) de documentos relevantes
//// 3. Geração (Generation) de respostas com contexto
////
//// RAG é útil quando você quer que o LLM responda com base em
//// informações específicas da sua organização, não apenas em
//// conhecimento geral treinado no modelo.
////
//// Uso:
////   dotnet run -- 05 "sua pergunta aqui"
////
//// Exemplo:
////   dotnet run -- 05 "Qual é a política de retorno?"
//// ============================================================

//using Microsoft.SemanticKernel;
//using Microsoft.SemanticKernel.ChatCompletion;
//using System.Collections.Generic;

//// ── Main Program ──────────────────────────────────────────
//var endpoint = "https://models.github.ai/inference";
//var credential = System.Environment.GetEnvironmentVariable("GITHUB_TOKEN");
//var model = "openai/gpt-4o-mini";

//if (string.IsNullOrWhiteSpace(credential))
//{
//    Console.Error.WriteLine("Erro: variável de ambiente GITHUB_TOKEN não configurada.");
//    Environment.Exit(1);
//}

//// Construir o kernel com GitHub Models
//// Usamos HttpClient para configurar o endpoint customizado do GitHub Models
//var httpClient = new HttpClient();
//httpClient.BaseAddress = new Uri(endpoint);

//var kernel = Kernel.CreateBuilder()
//    .AddOpenAIChatCompletion(
//        modelId: model, 
//        apiKey: credential,
//        httpClient: httpClient)
//    .Build();

//// Obter pergunta do usuário (ou usar padrão)
//string userQuery = args.Length > 1 ? string.Join(" ", args.Skip(1)) : "Qual é a política de retorno?";

//Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
//Console.WriteLine("║        RAG - Retrieval-Augmented Generation              ║");
//Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
//Console.WriteLine();
//Console.WriteLine($"📋 Pergunta: {userQuery}");
//Console.WriteLine();

//// ── Passo 1: Retrieval (Recuperação) ──────────────────────
//// Buscar documentos relevantes da base de conhecimento
//Console.WriteLine("🔍 Passo 1: Recuperando documentos relevantes...");
//var allDocuments = KnowledgeBase.GetDocuments();
//var relevantDocuments = allDocuments
//    .Select(doc => new { doc, score = doc.CalculateRelevance(userQuery) })
//    .Where(x => x.score > 0)
//    .OrderByDescending(x => x.score)
//    .Take(3)  // Top 3 documentos mais relevantes
//    .Select(x => x.doc)
//    .ToList();

//if (relevantDocuments.Count == 0)
//{
//    Console.WriteLine("⚠️  Nenhum documento relevante encontrado. Usando conhecimento geral do modelo.");
//    Console.WriteLine();
//}
//else
//{
//    Console.WriteLine($"✓ {relevantDocuments.Count} documento(s) relevante(s) encontrado(s):");
//    foreach (var doc in relevantDocuments)
//    {
//        Console.WriteLine($"  - {doc.Title}");
//    }
//    Console.WriteLine();
//}

//// ── Passo 2: Aumentar o Prompt com Contexto ───────────────
//// Construir um prompt que inclua o contexto recuperado
//string context = string.Empty;
//if (relevantDocuments.Count > 0)
//{
//    context = "\n\nContexto relevante da base de conhecimento:\n";
//    foreach (var doc in relevantDocuments)
//    {
//        context += $"\n[{doc.Title}]\n{doc.Content}\n";
//    }
//}

//string augmentedPrompt = $@"Você é um assistente de atendimento ao cliente útil e amigável.

//Responda a pergunta do usuário com base no contexto fornecido abaixo.
//Se a resposta não estiver no contexto, diga que não tem essa informação.
//Sempre seja educado e profissional.

//Pergunta do usuário: {userQuery}
//{context}

//Resposta:";

//// ── Passo 3: Generation (Geração) ──────────────────────────
//// Usar o LLM para gerar resposta baseada no contexto
//Console.WriteLine("💡 Passo 2: Gerando resposta com base no contexto...");
//Console.WriteLine();

//var chat = kernel.GetRequiredService<IChatCompletionService>();
//var chatHistory = new ChatHistory();
//chatHistory.AddSystemMessage("Você é um assistente de atendimento ao cliente útil e amigável.");
//chatHistory.AddUserMessage(augmentedPrompt);

//var response = await chat.GetChatMessageContentAsync(chatHistory, kernel: kernel);

//Console.WriteLine("📝 Resposta do Assistente:");
//Console.WriteLine("─────────────────────────────────────────────────────────");
//Console.WriteLine(response.Content);
//Console.WriteLine("─────────────────────────────────────────────────────────");
//Console.WriteLine();
//Console.WriteLine("✨ RAG concluído com sucesso!");

//// ── Knowledge Base (Base de Conhecimento) ──────────────────
//// Em um cenário real, isso viria de um banco de dados,
//// arquivo ou serviço de busca (Elasticsearch, Azure Cognitive Search, etc.)
//class KnowledgeBase
//{
//    public static List<Document> GetDocuments()
//    {
//        return new List<Document>
//        {
//            new Document
//            {
//                Id = "doc-001",
//                Title = "Política de Retorno",
//                Content = "Oferecemos 30 dias de garantia de devolução do dinheiro para todos os produtos. " +
//                         "Se você não estiver satisfeito com sua compra, basta contatar nosso suporte " +
//                         "dentro de 30 dias da compra e enviar o produto de volta. Oferecemos reembolso total " +
//                         "sem perguntas ou devoluções."
//            },
//            new Document
//            {
//                Id = "doc-002",
//                Title = "Horário de Atendimento",
//                Content = "Nosso atendimento ao cliente está disponível de segunda a sexta, das 9h às 18h " +
//                         "(horário de Brasília). Aos sábados, estamos abertos das 9h às 13h. " +
//                         "Domingos e feriados, o atendimento não funciona. Você pode enviar um e-mail " +
//                         "a qualquer hora e responderemos no próximo dia útil."
//            },
//            new Document
//            {
//                Id = "doc-003",
//                Title = "Envio e Entrega",
//                Content = "Oferecemos envio grátis para compras acima de R$ 100. Para compras menores, " +
//                         "o envio custa R$ 15. O prazo de entrega é de 3 a 7 dias úteis dependendo da sua localização. " +
//                         "Rastreamento está disponível para todos os pedidos."
//            },
//            new Document
//            {
//                Id = "doc-004",
//                Title = "Segurança de Pagamento",
//                Content = "Aceitamos cartão de crédito, débito e Pix. Todos os pagamentos são criptografados " +
//                         "com SSL de 256 bits. Nunca armazenamos detalhes completos do cartão em nossos servidores. " +
//                         "Sua segurança é nossa prioridade."
//            },
//            new Document
//            {
//                Id = "doc-005",
//                Title = "Política de Privacidade",
//                Content = "Seus dados pessoais são protegidos sob a Lei Geral de Proteção de Dados (LGPD). " +
//                         "Não compartilhamos suas informações com terceiros sem consentimento. " +
//                         "Você pode solicitar acesso, correção ou exclusão de seus dados a qualquer momento."
//            }
//        };
//    }
//}

//// ── Document Class ────────────────────────────────────────
//// Representa um documento na base de conhecimento
//class Document
//{
//    public string Id { get; set; }
//    public string Title { get; set; }
//    public string Content { get; set; }

//    // Calcula relevância simples usando contagem de palavras-chave
//    public double CalculateRelevance(string query)
//    {
//        var queryWords = query.ToLower().Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
//        var contentLower = Content.ToLower();
//        var titleLower = Title.ToLower();

//        double score = 0;
//        foreach (var word in queryWords)
//        {
//            // Palavras no título recebem peso maior
//            if (titleLower.Contains(word))
//                score += 2;
//            if (contentLower.Contains(word))
//                score += 1;
//        }
//        return score;
//    }
//}
