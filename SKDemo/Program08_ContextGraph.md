# .NET Semantic Kernel com Context Graph - Hands On

![Cover image - Context Graph](https://images.unsplash.com/photo-1639322537228-f710d846310a?w=1200)

_Cover image: courtesy of Unsplash_

Se você programa em .NET e está explorando o universo de agentes de IA, este artigo é essencial para entender como dar memória institucional aos seus sistemas inteligentes.

Continuando nossa série sobre **Semantic Kernel** e técnicas avançadas de IA, hoje vamos mergulhar em um conceito poderoso que vem ganhando destaque: **Context Graph (Grafo de Contexto)**.

Enquanto sistemas tradicionais apenas registram **O QUE** aconteceu, um Context Graph captura **O PORQUÊ** - transformando decisões, políticas e raciocínio institucional em estrutura consultável e legível por máquina.

## Contexto

Agentes de IA modernos precisam tomar decisões consistentes baseadas em políticas empresariais, precedentes históricos e evidências documentadas. Mas como garantir que um agente saiba **por que** uma decisão foi tomada no passado? Como evitar que ele aplique uma política que já foi superada?

O **Context Graph** é uma técnica de representação de conhecimento onde:

- **Decisões**, **políticas**, **exceções**, **precedentes** e **evidências** são modelados como **nós de primeira classe** em um grafo
- Cada aresta possui **validade temporal** (`ValidFrom` e `ValidUntil`)
- Fatos superados são **invalidados**, não sobrescritos
- Cadeias causais multi-hop podem ser rastreadas para entender a procedência

Para demonstrar isso em C#, vamos criar um sistema de aprovação de descontos corporativos usando [**Microsoft Semantic Kernel**](https://learn.microsoft.com/en-us/semantic-kernel/overview/).

## Problema

Imagine um agente de IA que precisa aprovar solicitações de desconto para clientes. Sem um Context Graph, o agente enfrenta problemas graves:

### ? **Problema 1: Políticas Obsoletas**
```
?? Agente: "A política permite 10% de desconto para B2B"
????? Gerente: "Mas mudamos isso há 2 meses! Agora é 15%!"
```

O agente não sabe que a política foi atualizada. Ele pode encontrar ambas as versões em documentos e não consegue determinar qual está vigente.

### ? **Problema 2: Exceções vs. Políticas**
```
?? Agente: "Vi que aprovamos 20% para TechCorp. Logo, posso aprovar 
           20% para qualquer cliente B2B."
????? Gerente: "NÃO! Aquela foi uma EXCEÇÃO pontual devido a um 
           contrato de $500k. Não é política permanente!"
```

Sem contexto sobre **procedência**, o agente não consegue distinguir entre uma política estabelecida e uma exceção pontual.

### ? **Problema 3: Raciocínio Sem Evidências**
```
?? Agente: "Aprovei 25% de desconto."
????? Gerente: "Com base em quê?"
?? Agente: "... Não sei, parecia razoável?"
```

O agente não mantém rastro das **evidências** que justificam decisões, tornando impossível auditar ou explicar o raciocínio.

![Problem meme](https://media.giphy.com/media/3o7btPCcdNniyf0ArS/giphy.gif)

## Solução

Com um **Context Graph**, modelamos o conhecimento institucional como um grafo temporal onde cada decisão, política e evidência é rastreável:

### ? **Solução 1: Validade Temporal**

```csharp
// Política antiga (EXPIRADA)
var oldB2BPolicy = graph.AddNode(new ContextNode
{
    Id = "policy-b2b-2023",
    Type = NodeType.Policy,
    Title = "Política B2B 2023",
    Content = "Desconto máximo de 10% para clientes B2B",
    Metadata = new Dictionary<string, object>
    {
        ["maxDiscount"] = 10
    }
});

// Política atual (VIGENTE)
var currentB2BPolicy = graph.AddNode(new ContextNode
{
    Id = "policy-b2b-2024",
    Type = NodeType.Policy,
    Title = "Política B2B 2024 (Atualizada)",
    Content = "Desconto máximo de 15% para clientes B2B",
    Metadata = new Dictionary<string, object>
    {
        ["maxDiscount"] = 15
    }
});

// Aresta temporal: nova política SUBSTITUI antiga
graph.AddEdge(new ContextEdge
{
    SourceId = currentB2BPolicy.Id,
    TargetId = oldB2BPolicy.Id,
    Type = EdgeType.Supersedes,
    ValidFrom = DateTime.UtcNow.AddMonths(-2), // Há 2 meses
    ValidUntil = null // Ainda válida
});

// Aresta temporal: antiga política EXPIROU
graph.AddEdge(new ContextEdge
{
    SourceId = oldB2BPolicy.Id,
    Type = EdgeType.ExpiredBy,
    ValidFrom = DateTime.UtcNow.AddYears(-1),
    ValidUntil = DateTime.UtcNow.AddMonths(-2) // Expirou há 2 meses
});
```

? **Resultado**: O agente consulta apenas políticas **vigentes** e sabe exatamente quando cada política estava válida.

### ? **Solução 2: Exceções Documentadas**

```csharp
// Decisão excepcional
var exceptionDecision = graph.AddNode(new ContextNode
{
    Id = "decision-order-1234",
    Type = NodeType.Decision,
    Title = "Aprovação Ordem #1234 - Desconto 20%",
    Content = "Aprovado 20% para TechCorp devido a contrato anual de $500k"
});

// Evidência que justifica a exceção
var evidence = graph.AddNode(new ContextNode
{
    Id = "evidence-contract-techcorp",
    Type = NodeType.Evidence,
    Content = "Contrato de volume anual de $500k"
});

// Conectar: Decisão BASEADA EM evidência
graph.AddEdge(new ContextEdge
{
    SourceId = exceptionDecision.Id,
    TargetId = evidence.Id,
    Type = EdgeType.BasedOn
});

// Conectar: Decisão é EXCEÇÃO À política
graph.AddEdge(new ContextEdge
{
    SourceId = exceptionDecision.Id,
    TargetId = currentB2BPolicy.Id,
    Type = EdgeType.ExceptionTo,
    Metadata = new Dictionary<string, object>
    {
        ["reason"] = "Volume anual justifica exceção"
    }
});
```

? **Resultado**: O agente sabe que aquela aprovação foi uma **exceção pontual**, não uma política permanente, e entende o **porquê**.

### ? **Solução 3: Cadeias Causais Rastreáveis**

```csharp
// Ferramentas que o agente pode usar
[KernelFunction]
public string TraceDecisionChain(string nodeId)
{
    var node = _graph.GetNode(nodeId);

    // Rastrear evidências que embasaram a decisão
    var evidences = _graph.GetEdgesFrom(nodeId)
        .Where(e => e.Type == EdgeType.BasedOn)
        .Select(e => _graph.GetNode(e.TargetId));

    // Rastrear políticas relacionadas
    var policies = _graph.GetEdgesFrom(nodeId)
        .Where(e => e.Type == EdgeType.ExceptionTo || 
                    e.Type == EdgeType.EnforcedBy);

    // Retornar cadeia completa de raciocínio
    return JsonSerializer.Serialize(new
    {
        decision = node,
        evidences,
        policies
    });
}
```

? **Resultado**: O agente pode explicar **exatamente** por que uma decisão foi tomada, rastreando toda a cadeia causal.

## Teoria

### **Nós de Primeira Classe**

Um Context Graph modela diferentes tipos de conhecimento como nós:

```csharp
enum NodeType
{
    Policy,      // Política ou regra de negócio
    Decision,    // Decisão tomada
    Precedent,   // Precedente estabelecido
    Evidence,    // Evidência ou justificativa
    Exception    // Exceção documentada
}

class ContextNode
{
    public string Id { get; set; }
    public NodeType Type { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}
```

### **Arestas Temporais**

Cada conexão entre nós possui validade temporal:

```csharp
class ContextEdge
{
    public string SourceId { get; set; }
    public string TargetId { get; set; }
    public EdgeType Type { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; } // null = ainda válido
    public Dictionary<string, object> Metadata { get; set; }
}

enum EdgeType
{
    BasedOn,        // Decisão baseada em evidência
    Supersedes,     // Nova versão substitui antiga
    ExceptionTo,    // Exceção a uma política
    EnforcedBy,     // Aplicação de política
    ExpiredBy,      // Expirada por nova versão
    SupportsPolicy, // Precedente que suporta política
    ResultOf        // Resultado de uma decisão
}
```

### **Consultas Temporais**

O grafo permite consultar o estado do conhecimento em qualquer ponto no tempo:

```csharp
public IEnumerable<ContextNode> GetActiveNodes(NodeType type, DateTime? asOf = null)
{
    var timestamp = asOf ?? DateTime.UtcNow;

    // Identificar nós expirados
    var expiredNodeIds = _edges
        .Where(e => e.Type == EdgeType.ExpiredBy && 
                    e.ValidUntil <= timestamp)
        .Select(e => e.SourceId)
        .ToHashSet();

    // Retornar apenas nós ativos
    return _nodes.Values
        .Where(n => n.Type == type && 
                    !expiredNodeIds.Contains(n.Id));
}
```

### **Ferramentas para o Agente**

O Semantic Kernel permite que o agente consulte o grafo através de funções:

```csharp
class ContextGraphPlugin
{
    [KernelFunction]
    [Description("Consulta políticas VIGENTES (não expiradas)")]
    public string QueryActivePolicies(string category = "all")
    {
        var activePolicies = _graph.GetActiveNodes(NodeType.Policy);
        // ... filtrar e retornar JSON
    }

    [KernelFunction]
    [Description("Busca precedentes históricos relevantes")]
    public string QueryPrecedents(string searchTerm)
    {
        // ... buscar e retornar JSON
    }

    [KernelFunction]
    [Description("Rastreia cadeia causal de uma decisão")]
    public string TraceDecisionChain(string nodeId)
    {
        // ... rastrear e retornar JSON
    }

    [KernelFunction]
    [Description("Verifica validade temporal de políticas")]
    public string CheckPolicyValidity(string segment, string date = "now")
    {
        // ... verificar e retornar JSON
    }
}
```

### **Integração com Semantic Kernel**

```csharp
// 1. Criar e popular o grafo
var contextGraph = new ContextGraph();
SeedContextGraph(contextGraph);

// 2. Registrar plugin no kernel
kernel.ImportPluginFromObject(
    new ContextGraphPlugin(contextGraph), 
    "ContextGraph"
);

// 3. Configurar auto function calling
var executionSettings = new OpenAIPromptExecutionSettings
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};

// 4. Instruir o agente
var chatHistory = new ChatHistory();
chatHistory.AddSystemMessage(
    "Você tem acesso a um Context Graph com histórico completo " +
    "de decisões e políticas. Sempre consulte políticas VIGENTES " +
    "e rastreie precedentes relevantes antes de responder."
);

// 5. O agente automaticamente usa as ferramentas quando necessário
var response = await chat.GetChatMessageContentAsync(
    chatHistory,
    executionSettings,
    kernel
);
```

### **Context Graph vs. GraphRAG**

É importante entender a diferença:

| Característica | Context Graph | GraphRAG |
|---|---|---|
| **Objetivo** | Captura o "porquê" | Recupera o "o quê" |
| **Fonte** | Decisões e políticas vivas | Documentos estáticos |
| **Temporal** | Validade em arestas | Snapshot no tempo |
| **Evolução** | Invalidação temporal | Sobrescrição de dados |
| **Uso Principal** | Raciocínio de agentes | Busca semântica |

## Conclusão

A adoção de **Context Graph** representa um salto qualitativo na construção de agentes de IA empresariais. Enquanto sistemas de registro tradicionais capturam apenas **o que aconteceu**, um grafo de contexto preserva **o porquê**, transformando conhecimento institucional tácito em estrutura consultável.

**Vantagens consolidadas:**

- ? **Rastreabilidade**: Cada decisão possui cadeia causal completa
- ? **Temporalidade**: Políticas obsoletas são invalidadas, não sobrescritas
- ? **Transparência**: Agentes podem explicar seu raciocínio
- ? **Consistência**: Exceções não são confundidas com políticas
- ? **Auditabilidade**: Todo o histórico de evolução é preservado

Essa técnica é especialmente vital para **aplicações baseadas em agentes** que exigem:
- Memória persistente entre sessões
- Raciocínio de decisão rastreável
- Aplicação consistente de políticas corporativas
- Distinção clara entre normas e exceções

## Código Completo

```csharp
// ============================================================
//  Program08 — Context Graph (Grafo de Contexto)
// ============================================================

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel;
using System.Text.Json;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// ?? Main Program ??????????????????????????????????????????
var endpoint = "https://models.github.ai/inference";
var credential = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
var model = "openai/gpt-4o-mini";

if (string.IsNullOrWhiteSpace(credential))
{
    Console.Error.WriteLine("Erro: variável GITHUB_TOKEN não configurada.");
    Environment.Exit(1);
}

var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri(endpoint);

var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(
        modelId: model,
        apiKey: credential,
        httpClient: httpClient)
    .Build();

// Inicializar e popular o grafo
var contextGraph = new ContextGraph();
SeedContextGraph(contextGraph);

// Registrar plugin
kernel.ImportPluginFromObject(
    new ContextGraphPlugin(contextGraph), 
    "ContextGraph"
);

string userQuery = args.Length > 1
    ? string.Join(" ", args.Skip(1))
    : "Posso dar 15% de desconto para um cliente VIP?";

Console.WriteLine("????????????????????????????????????????????????????????????");
Console.WriteLine("?         Context Graph - Raciocínio com Histórico         ?");
Console.WriteLine("????????????????????????????????????????????????????????????");
Console.WriteLine();
Console.WriteLine($"?? Pergunta: {userQuery}");
Console.WriteLine();

var executionSettings = new OpenAIPromptExecutionSettings
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
    Temperature = 0.7,
    MaxTokens = 1500
};

Console.WriteLine("???  Context Graph: Sistema inicializado");
Console.WriteLine($"   - {contextGraph.GetAllNodes().Count()} nós no grafo");
Console.WriteLine($"   - {contextGraph.GetAllEdges().Count()} arestas temporais");
Console.WriteLine();

var chatHistory = new ChatHistory();
chatHistory.AddSystemMessage(
    "Você é um assistente de aprovação de descontos que tem acesso a um Context Graph " +
    "com histórico completo de decisões, políticas e precedentes da empresa. " +
    "\n\nSempre que analisar uma solicitação de desconto:" +
    "\n1. Consulte as políticas VIGENTES (não use políticas expiradas)" +
    "\n2. Busque precedentes relevantes para o caso" +
    "\n3. Rastreie as cadeias de decisão quando necessário" +
    "\n4. Explique CLARAMENTE o raciocínio, citando:" +
    "\n   - Qual política se aplica e desde quando está vigente" +
    "\n   - Se há precedentes e se foram exceções ou política permanente" +
    "\n   - A cadeia causal que justifica a decisão"
);
chatHistory.AddUserMessage(userQuery);

var chat = kernel.GetRequiredService<IChatCompletionService>();
var response = await chat.GetChatMessageContentAsync(
    chatHistory,
    executionSettings: executionSettings,
    kernel: kernel
);

Console.WriteLine("?? Resposta do Assistente:");
Console.WriteLine("?????????????????????????????????????????????????????????");
Console.WriteLine(response.Content);
Console.WriteLine("?????????????????????????????????????????????????????????");

// ??????????????????????????????????????????????????????????
//  Context Graph - Classes de Domínio
// ??????????????????????????????????????????????????????????

enum NodeType
{
    Policy,
    Decision,
    Precedent,
    Evidence,
    Exception
}

enum EdgeType
{
    BasedOn,
    Supersedes,
    ExceptionTo,
    EnforcedBy,
    ExpiredBy,
    SupportsPolicy,
    ResultOf
}

class ContextNode
{
    public string Id { get; set; } = string.Empty;
    public NodeType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

class ContextEdge
{
    public string Id { get; set; } = string.Empty;
    public string SourceId { get; set; } = string.Empty;
    public string TargetId { get; set; } = string.Empty;
    public EdgeType Type { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

class ContextGraph
{
    private readonly Dictionary<string, ContextNode> _nodes = new();
    private readonly List<ContextEdge> _edges = new();

    public ContextNode AddNode(ContextNode node)
    {
        _nodes[node.Id] = node;
        return node;
    }

    public ContextEdge AddEdge(ContextEdge edge)
    {
        _edges.Add(edge);
        return edge;
    }

    public ContextNode? GetNode(string id) => _nodes.GetValueOrDefault(id);
    public IEnumerable<ContextNode> GetAllNodes() => _nodes.Values;
    public IEnumerable<ContextEdge> GetAllEdges() => _edges;

    public IEnumerable<ContextEdge> GetEdgesFrom(string nodeId, DateTime? asOf = null)
    {
        var timestamp = asOf ?? DateTime.UtcNow;
        return _edges.Where(e =>
            e.SourceId == nodeId &&
            e.ValidFrom <= timestamp &&
            (e.ValidUntil == null || e.ValidUntil > timestamp)
        );
    }

    public IEnumerable<ContextNode> GetActiveNodes(NodeType type, DateTime? asOf = null)
    {
        var timestamp = asOf ?? DateTime.UtcNow;
        var expiredNodeIds = _edges
            .Where(e => e.Type == EdgeType.ExpiredBy && e.ValidUntil <= timestamp)
            .Select(e => e.SourceId)
            .ToHashSet();

        return _nodes.Values
            .Where(n => n.Type == type && !expiredNodeIds.Contains(n.Id));
    }

    public IEnumerable<ContextNode> TraceChain(string startNodeId, EdgeType edgeType, int maxDepth = 5)
    {
        var visited = new HashSet<string>();
        var result = new List<ContextNode>();
        var queue = new Queue<(string nodeId, int depth)>();
        queue.Enqueue((startNodeId, 0));

        while (queue.Count > 0)
        {
            var (currentId, depth) = queue.Dequeue();
            if (depth >= maxDepth || visited.Contains(currentId))
                continue;

            visited.Add(currentId);
            var node = GetNode(currentId);
            if (node != null)
                result.Add(node);

            var edges = GetEdgesFrom(currentId).Where(e => e.Type == edgeType);
            foreach (var edge in edges)
                queue.Enqueue((edge.TargetId, depth + 1));
        }

        return result;
    }
}

// ??????????????????????????????????????????????????????????
//  Context Graph Plugin
// ??????????????????????????????????????????????????????????

class ContextGraphPlugin
{
    private readonly ContextGraph _graph;

    public ContextGraphPlugin(ContextGraph graph)
    {
        _graph = graph;
    }

    [KernelFunction, Description("Consulta políticas VIGENTES")]
    public string QueryActivePolicies(
        [Description("Categoria (ex: 'discount')")] string category = "all")
    {
        var activePolicies = _graph.GetActiveNodes(NodeType.Policy);

        if (category != "all")
        {
            activePolicies = activePolicies.Where(p =>
                p.Metadata.TryGetValue("category", out var cat) &&
                cat.ToString()!.Equals(category, StringComparison.OrdinalIgnoreCase)
            );
        }

        var policies = activePolicies.Select(p => new
        {
            id = p.Id,
            title = p.Title,
            content = p.Content,
            metadata = p.Metadata
        }).ToList();

        return JsonSerializer.Serialize(new
        {
            category,
            count = policies.Count,
            policies
        }, new JsonSerializerOptions { WriteIndented = true });
    }

    [KernelFunction, Description("Busca precedentes históricos")]
    public string QueryPrecedents(
        [Description("Termo de busca")] string searchTerm)
    {
        var precedents = _graph.GetAllNodes()
            .Where(n => n.Type == NodeType.Precedent)
            .Where(n =>
                n.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                n.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            )
            .Select(p => new
            {
                id = p.Id,
                title = p.Title,
                content = p.Content,
                metadata = p.Metadata
            })
            .ToList();

        return JsonSerializer.Serialize(new
        {
            searchTerm,
            count = precedents.Count,
            precedents
        }, new JsonSerializerOptions { WriteIndented = true });
    }

    [KernelFunction, Description("Rastreia cadeia de decisão")]
    public string TraceDecisionChain(
        [Description("ID do nó")] string nodeId)
    {
        var node = _graph.GetNode(nodeId);
        if (node == null)
        {
            return JsonSerializer.Serialize(new
            {
                error = $"Nó '{nodeId}' não encontrado"
            });
        }

        var evidences = _graph.GetEdgesFrom(nodeId)
            .Where(e => e.Type == EdgeType.BasedOn)
            .Select(e => _graph.GetNode(e.TargetId))
            .Where(n => n != null)
            .Select(n => new { n!.Title, n.Content })
            .ToList();

        return JsonSerializer.Serialize(new
        {
            node = new { node.Title, node.Content },
            evidences
        }, new JsonSerializerOptions { WriteIndented = true });
    }

    [KernelFunction, Description("Verifica validade de políticas")]
    public string CheckPolicyValidity(
        [Description("Segmento (ex: 'B2B')")] string segment,
        [Description("Data YYYY-MM-DD ou 'now'")] string dateString = "now")
    {
        DateTime checkDate = dateString == "now" 
            ? DateTime.UtcNow 
            : DateTime.Parse(dateString);

        var policies = _graph.GetAllNodes()
            .Where(n => n.Type == NodeType.Policy)
            .Where(n => n.Metadata.TryGetValue("segment", out var seg) &&
                       seg.ToString()!.Equals(segment, StringComparison.OrdinalIgnoreCase))
            .Select(p => new
            {
                p.Title,
                p.Content,
                p.CreatedAt
            })
            .ToList();

        return JsonSerializer.Serialize(new
        {
            segment,
            checkDate,
            policies
        }, new JsonSerializerOptions { WriteIndented = true });
    }
}

// ??????????????????????????????????????????????????????????
//  Seed Data
// ??????????????????????????????????????????????????????????

static void SeedContextGraph(ContextGraph graph)
{
    var now = DateTime.UtcNow;

    // Política antiga (expirada)
    var oldPolicy = graph.AddNode(new ContextNode
    {
        Id = "policy-b2b-2023",
        Type = NodeType.Policy,
        Title = "Política B2B 2023",
        Content = "Desconto máximo de 10% sem aprovação",
        Metadata = new Dictionary<string, object>
        {
            ["category"] = "discount",
            ["segment"] = "B2B",
            ["maxDiscount"] = 10
        }
    });

    // Política atual (vigente)
    var currentPolicy = graph.AddNode(new ContextNode
    {
        Id = "policy-b2b-2024",
        Type = NodeType.Policy,
        Title = "Política B2B 2024",
        Content = "Desconto máximo de 15% sem aprovação",
        Metadata = new Dictionary<string, object>
        {
            ["category"] = "discount",
            ["segment"] = "B2B",
            ["maxDiscount"] = 15
        }
    });

    // Política VIP
    var vipPolicy = graph.AddNode(new ContextNode
    {
        Id = "policy-vip-current",
        Type = NodeType.Policy,
        Title = "Política VIP",
        Content = "Desconto até 20% sem aprovação",
        Metadata = new Dictionary<string, object>
        {
            ["category"] = "discount",
            ["segment"] = "VIP",
            ["maxDiscount"] = 20
        }
    });

    // Conectar evolução temporal
    graph.AddEdge(new ContextEdge
    {
        Id = "edge-supersedes",
        SourceId = currentPolicy.Id,
        TargetId = oldPolicy.Id,
        Type = EdgeType.Supersedes,
        ValidFrom = now.AddMonths(-2),
        ValidUntil = null
    });

    graph.AddEdge(new ContextEdge
    {
        Id = "edge-expired",
        SourceId = oldPolicy.Id,
        TargetId = currentPolicy.Id,
        Type = EdgeType.ExpiredBy,
        ValidFrom = now.AddYears(-1),
        ValidUntil = now.AddMonths(-2)
    });

    // Decisão excepcional com evidência
    var decision = graph.AddNode(new ContextNode
    {
        Id = "decision-1234",
        Type = NodeType.Decision,
        Title = "Aprovação Ordem #1234",
        Content = "20% para TechCorp - contrato $500k",
        Metadata = new Dictionary<string, object>
        {
            ["discount"] = 20,
            ["customer"] = "TechCorp"
        }
    });

    var evidence = graph.AddNode(new ContextNode
    {
        Id = "evidence-contract",
        Type = NodeType.Evidence,
        Title = "Contrato TechCorp",
        Content = "Contrato anual de $500k"
    });

    graph.AddEdge(new ContextEdge
    {
        SourceId = decision.Id,
        TargetId = evidence.Id,
        Type = EdgeType.BasedOn,
        ValidFrom = now.AddDays(-30),
        ValidUntil = null
    });

    graph.AddEdge(new ContextEdge
    {
        SourceId = decision.Id,
        TargetId = currentPolicy.Id,
        Type = EdgeType.ExceptionTo,
        ValidFrom = now.AddDays(-30),
        ValidUntil = null,
        Metadata = new Dictionary<string, object>
        {
            ["reason"] = "Volume anual justifica exceção"
        }
    });

    // Precedente VIP
    var precedent = graph.AddNode(new ContextNode
    {
        Id = "precedent-vip",
        Type = NodeType.Precedent,
        Title = "Precedente VIP 15-20%",
        Content = "15 aprovações consistentes em 6 meses",
        Metadata = new Dictionary<string, object>
        {
            ["count"] = 15,
            ["avgDiscount"] = 17.5
        }
    });

    graph.AddEdge(new ContextEdge
    {
        SourceId = precedent.Id,
        TargetId = vipPolicy.Id,
        Type = EdgeType.SupportsPolicy,
        ValidFrom = now.AddMonths(-6),
        ValidUntil = null
    });
}
```

---

**Fontes:**

:link: [Technology Radar ThoughtWorks | Volume 34](https://www.thoughtworks.com/pt-br/radar)

:link: [Microsoft Semantic Kernel Documentation](https://learn.microsoft.com/en-us/semantic-kernel/overview/)

:link: [Context Graph - AI Engineering Patterns](https://www.anthropic.com/research/context-protocol)

---

**Execute o exemplo:**

```bash
# Configurar token
$env:GITHUB_TOKEN = "seu-token-aqui"

# Executar
dotnet run -- 08 "Posso dar 15% de desconto para cliente VIP?"
dotnet run -- 08 "Por que aprovamos 20% na ordem #1234?"
dotnet run -- 08 "Como a política B2B mudou ao longo do tempo?"
```

? **Happy Context Graphing!**
