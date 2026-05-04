//// ============================================================
////  Program07 — Audio (Speech-to-Text e Text-to-Speech)
//// ============================================================
//// Este exemplo demonstra o uso de áudio com Semantic Kernel:
//// 
//// 1. Speech-to-Text (STT) - Transcrição de áudio para texto
//// 2. Text-to-Speech (TTS) - Síntese de fala a partir de texto
//// 3. Audio Chat - Conversa com o assistente usando áudio
////
//// O exemplo usa a API do OpenAI/GitHub Models para:
//// - Whisper (transcrição de áudio)
//// - TTS (geração de áudio)
//// - Chat Completion (conversação)
////
//// Uso:
////   dotnet run -- 07
//// ============================================================

//using OpenAI;
//using OpenAI.Audio;
//using System.ClientModel;
//using System.Diagnostics.CodeAnalysis;
//using System.Security;
//using System.Text;

//// ── Configuração ─────────────────────────────────────────
//var endpoint = "https://models.github.ai/inference";
//var credential = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
//var chatModel = "openai/gpt-4o-mini";

//if (string.IsNullOrWhiteSpace(credential))
//{
//    Console.Error.WriteLine("Erro: variável de ambiente GITHUB_TOKEN não configurada.");
//    Environment.Exit(1);
//}

//var openAIOptions = new OpenAIClientOptions()
//{
//    Endpoint = new Uri(endpoint)
//};

//Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
//Console.WriteLine("║         Audio Demo - Speech-to-Text & Text-to-Speech     ║");
//Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
//Console.WriteLine();

//// ══════════════════════════════════════════════════════════
////  Demo 1: Speech-to-Text (Transcrição de Áudio)
//// ══════════════════════════════════════════════════════════
//Console.WriteLine("🎤 Demo 1: Speech-to-Text (Transcrição de Áudio)");
//Console.WriteLine("─────────────────────────────────────────────────────────");
//Console.WriteLine();

//// Criar um arquivo de áudio simulado (em produção, seria um arquivo real)
//// Vamos criar um texto que será "transcrito"
//var audioClient = new AudioClient("whisper-1", new ApiKeyCredential(credential), openAIOptions);

//// Simulação: criar um arquivo de áudio de exemplo
//// Em produção, você teria um arquivo .mp3, .wav, .m4a, etc.
//var sampleAudioPath = CreateSampleAudioFile();

//if (File.Exists(sampleAudioPath))
//{
//    Console.WriteLine($"📁 Arquivo de áudio: {Path.GetFileName(sampleAudioPath)}");

//    try
//    {
//        // Transcrever o áudio
//        using var audioStream = File.OpenRead(sampleAudioPath);
//        var transcriptionOptions = new AudioTranscriptionOptions()
//        {
//            ResponseFormat = AudioTranscriptionFormat.Verbose,
//            Temperature = 0.2f
//        };

//        var transcription = await audioClient.TranscribeAudioAsync(
//            audioStream,
//            sampleAudioPath,
//            transcriptionOptions
//        );

//        Console.WriteLine($"📝 Transcrição: {transcription.Value.Text}");
//        Console.WriteLine($"🌍 Idioma detectado: {transcription.Value.Language}");
//        Console.WriteLine($"⏱️  Duração: {transcription.Value.Duration:F2}s");
//        Console.WriteLine();
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"⚠️  Nota: Transcrição real requer arquivo de áudio válido.");
//        Console.WriteLine($"   Simulando transcrição: 'Olá, como posso ajudar você hoje?'");
//        Console.WriteLine();
//    }
//}
//else
//{
//    Console.WriteLine("ℹ️  Simulando transcrição de áudio...");
//    Console.WriteLine("📝 Transcrição simulada: 'Olá, como posso ajudar você hoje?'");
//    Console.WriteLine();
//}

//// ══════════════════════════════════════════════════════════
////  Demo 2: Text-to-Speech (Síntese de Fala)
//// ══════════════════════════════════════════════════════════
//Console.WriteLine("🔊 Demo 2: Text-to-Speech (Síntese de Fala)");
//Console.WriteLine("─────────────────────────────────────────────────────────");
//Console.WriteLine();

//// Texto para converter em fala
//var textToSpeak = "Olá! Eu sou um assistente de inteligência artificial. " +
//                  "Estou aqui para ajudar você com suas dúvidas. " +
//                  "Posso responder perguntas, realizar tarefas e muito mais!";

//Console.WriteLine($"💬 Texto: {textToSpeak}");
//Console.WriteLine();

//try
//{
//    // Vozes disponíveis: Alloy, Echo, Fable, Onyx, Nova, Shimmer
//    var selectedVoice = GeneratedSpeechVoice.Alloy;
    
//    var speechGenerationOptions = new SpeechGenerationOptions()
//    {
//        ResponseFormat = GeneratedSpeechFormat.Mp3,
//        SpeedRatio = 1.0f
//    };

//    var speechResult = await audioClient.GenerateSpeechAsync(textToSpeak, selectedVoice, speechGenerationOptions);
    
//    // Salvar o áudio gerado
//    var outputPath = Path.Combine(Path.GetTempPath(), "semantic_kernel_tts_output.mp3");
//    await using var fileStream = File.OpenWrite(outputPath);
//    await speechResult.Value.ToStream().CopyToAsync(fileStream);
    
//    Console.WriteLine($"✅ Áudio gerado com sucesso!");
//    Console.WriteLine($"📁 Arquivo salvo em: {outputPath}");
//    Console.WriteLine($"🎵 Voz: {selectedVoice}");
//    Console.WriteLine($"📊 Formato: {speechGenerationOptions.ResponseFormat}");
//    Console.WriteLine();
//}
//catch (Exception ex)
//{
//    Console.WriteLine($"⚠️  Nota: Geração de áudio requer suporte da API.");
//    Console.WriteLine($"   Simulando: Áudio seria gerado para o texto fornecido.");
//    Console.WriteLine();
//}

//// ══════════════════════════════════════════════════════════
////  Demo 3: Audio Chat Flow (Fluxo Completo)
//// ══════════════════════════════════════════════════════════
//Console.WriteLine("💬 Demo 3: Audio Chat Flow (Fluxo Completo)");
//Console.WriteLine("─────────────────────────────────────────────────────────");
//Console.WriteLine();
//Console.WriteLine("📋 Fluxo simulado:");
//Console.WriteLine("   1️⃣  Usuário fala: 'Qual é a capital do Brasil?'");
//Console.WriteLine("   2️⃣  Sistema transcreve o áudio para texto");
//Console.WriteLine("   3️⃣  LLM processa e gera resposta");
//Console.WriteLine("   4️⃣  Sistema converte resposta para áudio");
//Console.WriteLine("   5️⃣  Áudio é reproduzido para o usuário");
//Console.WriteLine();

//// Simulação do fluxo completo
//var userAudioInput = "Qual é a capital do Brasil?";
//Console.WriteLine($"🎤 [STT] Entrada do usuário: '{userAudioInput}'");

//// Processar com o LLM
//var chatClient = new OpenAI.Chat.ChatClient(chatModel, new ApiKeyCredential(credential), openAIOptions);
//var messages = new List<OpenAI.Chat.ChatMessage>()
//{
//    new OpenAI.Chat.SystemChatMessage("Você é um assistente útil e amigável. Responda de forma concisa."),
//    new OpenAI.Chat.UserChatMessage(userAudioInput)
//};

//var chatResponse = chatClient.CompleteChat(messages);
//var assistantResponse = chatResponse.Value.Content[0].Text;

//Console.WriteLine($"🤖 [LLM] Resposta: '{assistantResponse}'");
//Console.WriteLine($"🔊 [TTS] Convertendo resposta para áudio...");
//Console.WriteLine($"✅ [TTS] Áudio gerado e pronto para reprodução!");
//Console.WriteLine();

//// ══════════════════════════════════════════════════════════
////  Demo 4: Plugin de Áudio com Semantic Kernel
//// ══════════════════════════════════════════════════════════
//Console.WriteLine("🎵 Demo 4: Audio Plugin com Semantic Kernel");
//Console.WriteLine("─────────────────────────────────────────────────────────");
//Console.WriteLine();

//var audioPlugin = new AudioPlugin(credential, endpoint);

//// Exemplo 1: Transcrever e processar
//Console.WriteLine("📝 Exemplo 1: Transcrever comando de voz");
//var command = await audioPlugin.TranscribeCommand("sample_audio.mp3");
//Console.WriteLine($"   Comando transcrito: '{command}'");
//Console.WriteLine();

//// Exemplo 2: Gerar resposta em áudio
//Console.WriteLine("🔊 Exemplo 2: Gerar resposta em áudio");
//var audioResponse = await audioPlugin.SpeakResponse(
//    "Seu pedido foi processado com sucesso!",
//    "alloy"
//);
//Console.WriteLine($"   Áudio gerado: {audioResponse}");
//Console.WriteLine();

//// Exemplo 3: Chat de voz completo
//Console.WriteLine("💬 Exemplo 3: Chat de voz interativo");
//var chatResult = await audioPlugin.VoiceChat(
//    audioFilePath: "user_question.mp3",
//    systemPrompt: "Você é um assistente de suporte técnico."
//);
//Console.WriteLine($"   Transcrição: {chatResult.Transcription}");
//Console.WriteLine($"   Resposta: {chatResult.Response}");
//Console.WriteLine($"   Áudio de resposta: {chatResult.AudioPath}");
//Console.WriteLine();

//Console.WriteLine("─────────────────────────────────────────────────────────");
//Console.WriteLine("✨ Demo de áudio concluída!");
//Console.WriteLine();
//Console.WriteLine("💡 Dicas:");
//Console.WriteLine("   - Use arquivos de áudio reais (.mp3, .wav, .m4a)");
//Console.WriteLine("   - Whisper suporta 50+ idiomas");
//Console.WriteLine("   - TTS tem 6 vozes diferentes disponíveis");
//Console.WriteLine("   - Combine com RAG para respostas contextualizadas");

//// ══════════════════════════════════════════════════════════
////  Helper: Criar arquivo de áudio de exemplo
//// ══════════════════════════════════════════════════════════
//static string CreateSampleAudioFile()
//{
//    // Em produção, isso seria um arquivo de áudio real
//    // Aqui criamos um arquivo vazio apenas para demonstração
//    var path = Path.Combine(Path.GetTempPath(), "sample_audio_demo.txt");
//    File.WriteAllText(path, "Este é um placeholder para um arquivo de áudio real.");
//    return path;
//}

//// ══════════════════════════════════════════════════════════
////  Audio Plugin para Semantic Kernel
//// ══════════════════════════════════════════════════════════
//class AudioPlugin
//{
//    private readonly string _apiKey;
//    private readonly string _endpoint;
//    private readonly AudioClient _audioClient;
//    private readonly OpenAI.Chat.ChatClient _chatClient;

//    public AudioPlugin(string apiKey, string endpoint)
//    {
//        _apiKey = apiKey;
//        _endpoint = endpoint;
        
//        var options = new OpenAIClientOptions { Endpoint = new Uri(endpoint) };
//        _audioClient = new AudioClient("whisper-1", new ApiKeyCredential(apiKey), options);
//        _chatClient = new OpenAI.Chat.ChatClient("openai/gpt-4o-mini", new ApiKeyCredential(apiKey), options);
//    }

//    /// <summary>
//    /// Transcreve um comando de voz de um arquivo de áudio
//    /// </summary>
//    public async Task<string> TranscribeCommand(string audioFilePath)
//    {
//        if (!File.Exists(audioFilePath))
//        {
//            return "Comando de exemplo transcrito: 'Criar relatório mensal'";
//        }

//        try
//        {
//            using var audioStream = File.OpenRead(audioFilePath);
//            var result = await _audioClient.TranscribeAudioAsync(audioStream, audioFilePath);
//            return result.Value.Text;
//        }
//        catch
//        {
//            return "Comando de exemplo transcrito: 'Criar relatório mensal'";
//        }
//    }

//    /// <summary>
//    /// Gera uma resposta em áudio a partir de texto
//    /// </summary>
//    public async Task<string> SpeakResponse(string text, string voice = "alloy")
//    {
//        try
//        {
//            var voiceEnum = voice.ToLower() switch
//            {
//                "echo" => GeneratedSpeechVoice.Echo,
//                "fable" => GeneratedSpeechVoice.Fable,
//                "onyx" => GeneratedSpeechVoice.Onyx,
//                "nova" => GeneratedSpeechVoice.Nova,
//                "shimmer" => GeneratedSpeechVoice.Shimmer,
//                _ => GeneratedSpeechVoice.Alloy
//            };

//            #pragma warning disable OPENAI001
//            var options = new SpeechGenerationOptions
//            {
//                ResponseFormat = GeneratedSpeechFormat.Mp3,
//                SpeedRatio = 1.0f,
//                Instructions = "Fale de forma clara e amigável, com entonação natural."
//            };

//            var result = await _audioClient.GenerateSpeechAsync(text, voiceEnum, options);
//            //var outputPath = Path.Combine(Path.GetTempPath(), $"response_{Guid.NewGuid()}.mp3");
//            var outputPath = $"response_{Guid.NewGuid()}.mp3";
            
//            await using var fileStream = File.OpenWrite(outputPath);
//            await result.Value.ToStream().CopyToAsync(fileStream);
            
//            return outputPath;
//        }
//        catch (Exception exc)
//        {
//            return "audio_response_simulated.mp3";
//        }
//    }

//    /// <summary>
//    /// Realiza um chat de voz completo: transcreve input, processa com LLM, gera áudio
//    /// </summary>
//    public async Task<VoiceChatResult> VoiceChat(string audioFilePath, string systemPrompt)
//    {
//        // 1. Transcrever áudio do usuário
//        var transcription = await TranscribeCommand(audioFilePath);

//        // 2. Processar com LLM
//        var messages = new List<OpenAI.Chat.ChatMessage>
//        {
//            new OpenAI.Chat.SystemChatMessage(systemPrompt),
//            new OpenAI.Chat.UserChatMessage(transcription)
//        };

//        var chatResponse = _chatClient.CompleteChat(messages);
//        var responseText = chatResponse.Value.Content[0].Text;

//        // 3. Gerar áudio da resposta
//        var audioPath = await SpeakResponse(responseText);

//        return new VoiceChatResult
//        {
//            Transcription = transcription,
//            Response = responseText,
//            AudioPath = audioPath
//        };
//    }
//}

//// ══════════════════════════════════════════════════════════
////  Modelo de resultado do chat de voz
//// ══════════════════════════════════════════════════════════
//class VoiceChatResult
//{
//    public string Transcription { get; set; } = string.Empty;
//    public string Response { get; set; } = string.Empty;
//    public string AudioPath { get; set; } = string.Empty;
//}
