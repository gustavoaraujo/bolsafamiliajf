using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace BolsaFamiliaJF
{
    public class Analise
    {
        private readonly List<Beneficiario> beneficiarios = new List<Beneficiario>();
        private Dictionary<string, string> nomeSexo;

        public void ObterGenero()
        {
            var arquivo = File.ReadAllText("nomesexo.json");
            nomeSexo = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,string>>(arquivo);
            if(nomeSexo == null) nomeSexo = new Dictionary<string, string>();

            foreach(var beneficiario in beneficiarios) 
            {
                var nome = beneficiario.Nome.Split(" ").First();
                if(nomeSexo.ContainsKey(nome)) 
                {
                    beneficiario.Sexo = nomeSexo[nome];
                }
                else 
                {
                    if(nome.EndsWith("A") || nome.EndsWith("ANE")|| nome.EndsWith("AINE") || nome.EndsWith("ELE") || nome.EndsWith("ELLE")) 
                    {
                        nomeSexo.Add(nome, "f");
                    }
                    else if(nome.EndsWith("O") || nome.EndsWith("SON")) 
                    {
                        nomeSexo.Add(nome, "m");
                    }
                    else 
                    {
                        Console.WriteLine($"Qual é o sexo de {nome}?");
                        var resposta = Console.ReadLine();
                        nomeSexo.Add(nome, resposta);
                    }

                    Task.Run(async () =>
                    {
                        await File.WriteAllTextAsync("nomesexo.json", Newtonsoft.Json.JsonConvert.SerializeObject(nomeSexo));
                    });
                }
            }         
        }

        public void AnalisarPorIdade()
        {
            var de2010praca = beneficiarios.Count(x => x.Nascimento.Year >= 2010);
            Console.WriteLine($"Depois de 2010: {de2010praca}");
            var entre2000e2009 = beneficiarios.Count(x => x.Nascimento.Year >= 2000 && x.Nascimento.Year < 2010);
            Console.WriteLine($"Entre 2000 e 2009: {entre2000e2009}");
            var entre1990e1999 = beneficiarios.Count(x => x.Nascimento.Year >= 1990 && x.Nascimento.Year < 2000);
            Console.WriteLine($"Entre 1990 e 1999: {entre1990e1999}");
            var entre1980e1989 = beneficiarios.Count(x => x.Nascimento.Year >= 1980 && x.Nascimento.Year < 1990);
            Console.WriteLine($"Entre 1980 e 1989: {entre1980e1989}");
            var entre1970e1979 = beneficiarios.Count(x => x.Nascimento.Year >= 1970 && x.Nascimento.Year < 1980);
            Console.WriteLine($"Entre 1970 e 1979: {entre1970e1979}");
            var entre1960e1969 = beneficiarios.Count(x => x.Nascimento.Year >= 1960 && x.Nascimento.Year < 1970);
            Console.WriteLine($"Entre 1960 e 1969: {entre1960e1969}");
            var entre1950e1959 = beneficiarios.Count(x => x.Nascimento.Year >= 1950 && x.Nascimento.Year < 1960);
            Console.WriteLine($"Entre 1950 e 1959: {entre1950e1959}");
            var entre1940e1949 = beneficiarios.Count(x => x.Nascimento.Year >= 1940 && x.Nascimento.Year < 1950);
            Console.WriteLine($"Entre 1940 e 1949: {entre1940e1949}");
            var entre1930e1939 = beneficiarios.Count(x => x.Nascimento.Year >= 1930 && x.Nascimento.Year < 1940);
            Console.WriteLine($"Entre 1930 e 1939: {entre1930e1939}");
            var entre1920e1929 = beneficiarios.Count(x => x.Nascimento.Year >= 1920 && x.Nascimento.Year < 1930);
            Console.WriteLine($"Entre 1920 e 1929: {entre1920e1929}");
            var entre1910e1919 = beneficiarios.Count(x => x.Nascimento.Year >= 1910 && x.Nascimento.Year < 1920);
            Console.WriteLine($"Entre 1910 e 1919: {entre1910e1919}");
            var entre1900e1909 = beneficiarios.Count(x => x.Nascimento.Year >= 1900 && x.Nascimento.Year < 1910);
            Console.WriteLine($"Entre 1900 e 1909: {entre1900e1909}");
            var antesde1900 = beneficiarios.Count(x => x.Nascimento.Year < 1900);
            Console.WriteLine($"Antes de 1900: {antesde1900}");
        }

        public void Buscar(bool usarMock = false) 
        {
            var html = "";
            if(usarMock) 
            {
                html = File.ReadAllText("mock.html");
            }
            else 
            {
                html = Request.DoRequest("https://www.pjf.mg.gov.br/secretarias/sds/bolsa_familia/index.php");
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var nodes = doc.DocumentNode.SelectNodes("//tr[@class='texto_tabela']");
            
            foreach(var node in nodes) 
            {
                var childNodes = node.ChildNodes.Where(x => x.Name == "td");

                if(childNodes.Count() > 0) 
                {
                    var beneficiario = new Beneficiario 
                    {
                        Ordem = int.Parse(childNodes.ElementAt(0).InnerText),
                        NIS = childNodes.ElementAt(1).InnerText,
                        Nome = childNodes.ElementAt(2).InnerText,
                        Nascimento = DateTime.Parse(childNodes.ElementAt(3).InnerText)
                    };

                    beneficiarios.Add(beneficiario);
                }
            }

            Console.WriteLine("Número de beneficiários:" + beneficiarios.Count());
        }
    
        public void AnalisarIdentidadeNIS() 
        {
            var beneficiariosComMesmoNIS = beneficiarios.GroupBy(x => x.NIS).Where(x => x.Count() > 1);
            
            if(beneficiariosComMesmoNIS.Count() > 0) 
            {
                Console.WriteLine($"Foram encontrados {beneficiariosComMesmoNIS.Count()} casos de pessoas com o mesmo NIS.");
                Console.WriteLine("Ordem - NIS - Nome - Nascimento");
                Console.WriteLine("===");
                Console.WriteLine("===");

                foreach(var grupoBeneficiario in beneficiariosComMesmoNIS) 
                {
                    foreach(var beneficiario in grupoBeneficiario) 
                    {
                        Console.WriteLine($"{beneficiario.Ordem} - {beneficiario.NIS} - {beneficiario.Nome} - {beneficiario.Nascimento.ToString("d", CultureInfo.CreateSpecificCulture("pt-BR"))}");
                    }

                    Console.WriteLine("===");
                }

                Console.WriteLine("===");
            }
            else 
            {
                Console.WriteLine("Não foram encontrados NIS duplicados.");
            }
        }

        public void ValidarNIS()
        {
            var total = 0;
            foreach(var beneficiario in beneficiarios) 
            {
                var numeros = beneficiario.NIS.Select(x => Convert.ToInt32(x.ToString()));
                var somaNumerosNIS = numeros.ElementAt(0) * 3 + 
                numeros.ElementAt(1) * 2 + 
                numeros.ElementAt(2) * 9 +
                numeros.ElementAt(3) * 8 +
                numeros.ElementAt(4) * 7 +
                numeros.ElementAt(5) * 6 +
                numeros.ElementAt(6) * 5 +
                numeros.ElementAt(7) * 4 +
                numeros.ElementAt(8) * 3 +
                numeros.ElementAt(9) * 2;
                
                var resto = somaNumerosNIS % 11;

                var resultado = 11 - resto;

                resultado = resultado >= 10 ? 0 : resultado;

                if(resultado != numeros.ElementAt(10)) 
                {
                    // Console.WriteLine("NIS inválido identificado");
                    // Console.WriteLine($"Ordem: {beneficiario.Ordem}");
                    // Console.WriteLine($"NIS: {beneficiario.NIS}");
                    // Console.WriteLine($"Nome: {beneficiario.Nome}");
                    // Console.WriteLine($"Nascimento: {beneficiario.Nascimento.ToString("d", CultureInfo.CreateSpecificCulture("pt-BR"))}");
                    total++;
                }
            }

            Console.WriteLine($"Nis inválidos: {total}");
        }

        public void AnalisarPorGenero() 
        {
            var beneficiariosFemininos = beneficiarios.Count(x => x.Sexo == "f");
            var masculinos = beneficiarios.Count - beneficiariosFemininos;
            Console.WriteLine($"Feminino: {beneficiariosFemininos}");
            Console.WriteLine($"Masculino: {masculinos}");
        }
    }
}
