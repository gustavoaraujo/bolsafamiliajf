using System;

namespace BolsaFamiliaJF
{
    public class Program
    {
        static void Main(string[] args)
        {
            var analise = new Analise();
            analise.Buscar();
            analise.AnalisarIdentidadeNIS();
            analise.ValidarNIS();
            analise.ObterGenero();
            analise.AnalisarPorGenero();
            analise.AnalisarPorIdade();
        }
    }
}
