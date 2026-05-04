// ============================================================
//  MathPlugin — Plugin nativo de exemplo
// ============================================================
// Um plugin nativo é uma classe C# cujos métodos são anotados
// com [KernelFunction] e [Description].
//
// O Semantic Kernel expõe esses métodos ao modelo de IA, que
// pode decidir invocá-los automaticamente quando necessário
// (via "function calling" / Auto Function Invocation).
// ============================================================

using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SKDemo.Plugins;

public class MathPlugin
{
    // [KernelFunction]  → marca o método como invocável pelo Kernel/LLM
    // [Description]     → descreve o método para o modelo de IA entender quando usar
    [KernelFunction]
    [Description("Soma dois números inteiros e retorna o resultado.")]
    public int Somar(
        [Description("Primeiro número")] int a,
        [Description("Segundo número")]  int b)
    {
        return a + b;
    }

    [KernelFunction]
    [Description("Multiplica dois números inteiros e retorna o resultado.")]
    public int Multiplicar(
        [Description("Primeiro número")] int a,
        [Description("Segundo número")]  int b)
    {
        return a * b;
    }
}
