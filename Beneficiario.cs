using System;
using System.IO;
using System.Net;

namespace BolsaFamiliaJF
{
    public class Beneficiario
    {
        public int Ordem { get; set; }
        public string NIS { get; set; }
        public string Nome { get; set; }
        public DateTime Nascimento { get; set; }
        public string Sexo { get; set; }
    }
}
