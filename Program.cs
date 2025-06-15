using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Estacionamento
{

    public class Veiculo
    {
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public string Marca { get; set; }
        public string Proprietario { get; set; } // Corrigido nome da variável
        public string Cor { get; set; }
        public bool Preferencial { get; set; } //Tipo Booleano se é preferencial ou não

        public DateTime HoraDeEntrada { get; set; } // NA INSERÇÃO DO VEICULO SERA DEFININO A SUA HORA DE ENTRADA.


        public Veiculo(bool x)     //Exemplo de utilização: Veiculo x = new Veiculo(true);
        {
            Placa = GerarPlaca();   //Foi mais facil utilizar uma função.
            Modelo = "";
            Marca = "";
            Proprietario = "";
            Cor = "";
            Preferencial = false;
        }

        public Veiculo()       //Exemplo de utilização: Veiculo x = new Veiculo();
        {
            Placa = "";
            Modelo = "";
            Marca = "";
            Proprietario = "";           // Veiculo instanciado sem a placa, para edição ou qualquer outro fim.
            Cor = "";
            Preferencial = false;
        }


        public static string GerarPlaca()
        {
            Random x = new Random();
            string letras = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string numeros = "0123456789";

            string placa =
                $"{letras[x.Next(letras.Length)]}" +
                $"{letras[x.Next(letras.Length)]}" +
                $"{letras[x.Next(letras.Length)]}" +
                $"{numeros[x.Next(numeros.Length)]}" +
                $"{letras[x.Next(letras.Length)]}" +
                $"{numeros[x.Next(numeros.Length)]}" +
                $"{numeros[x.Next(numeros.Length)]}";

            return placa;
        }
    }


    public class Estacionamento
    {
        public List<Veiculo> vagas; // vagas

        public string Endereco; // Exemplo: “Monte Carmo”
        public string Nome; // Nome do estacionamento

        public int VagasLivres; // Sempre alterado conforme ocupação

        public double PrecoFixo; // Exemplo: 14reais


        public Estacionamento()
        {
            LerXML();
        }

        public void SalvarXML()
        {
            var arquivo = new XDocument(
                new XElement("Veiculos",
                    vagas.Select(v => new XElement("Veiculo",
                        new XElement("Placa", v.Placa),
                        new XElement("Modelo", v.Modelo),
                        new XElement("Marca", v.Marca),
                        new XElement("Proprietario", v.Proprietario),
                        new XElement("Cor", v.Cor),
                        new XElement("Preferencial", v.Preferencial),
                        new XElement("HoraDeEntrada", v.HoraDeEntrada)
                    ))
                )
            );

            arquivo.Save("veiculos.xml");
        }

        public void LerXML()
        {
            if (File.Exists("veiculos.xml"))
            {
                XDocument arquivo = XDocument.Load("veiculos.xml");

                Nome = arquivo.Root.Element("Nome")?.Value ?? "Estacionamento Sem Nome";
                Endereco = arquivo.Root.Element("Endereco")?.Value ?? "Endereço Não Informado";
                PrecoFixo = double.Parse(arquivo.Root.Element("PrecoFixo")?.Value ?? "0");

                vagas = arquivo.Descendants("Veiculo").Select(v => new Veiculo
                {
                    Placa = v.Element("Placa")?.Value,
                    Modelo = v.Element("Modelo")?.Value,
                    Marca = v.Element("Marca")?.Value,
                    Proprietario = v.Element("Proprietario")?.Value,
                    Cor = v.Element("Cor")?.Value,
                    Preferencial = bool.Parse(v.Element("Preferencial")?.Value ?? "false"),
                    HoraDeEntrada = DateTime.Parse(v.Element("HoraDeEntrada")?.Value)
                }).ToList();

                VagasLivres = 100 - vagas.Count;
            }
            else
            {
                vagas = new List<Veiculo>();
                VagasLivres = 100;
                Nome = "Estacionamento Central";
                Endereco = "Monte Carmo";
                PrecoFixo = 14.00;
            }
        }


    }

    // Classe para gerar comprovante de entrada/saída
    public class Comprovante
    {
        public string Placa;

        public DateTime HoraDeEntrada;
        public DateTime HoraDeSaida;

        public double ValorPago;

        // Comprovantes.xml out 
    }

    class program
    {

        public static void Main(string[] args)
        {
            //CRUD
            int op = 1;

            do
            {
                Console.Clear();
                Console.WriteLine("========== MENU DO SISTEMA ==========");
                Console.WriteLine("1 - Inserir veículo");
                Console.WriteLine("2 - Mostrar veículos");
                Console.WriteLine("3 - Remover veículo");
                Console.WriteLine("4 - Filtro veículo");
                Console.WriteLine("5 - Editar informações do veículo");
                Console.WriteLine("0 - Sair");
                Console.WriteLine("=====================================");
                Console.Write("Escolha uma opção: ");


                bool conversao = int.TryParse(Console.ReadLine(), out op);

                if (!conversao)
                {
                    Console.WriteLine("Opção inválida!");
                    Console.ReadKey();
                    continue;
                }

                switch (op)
                {
                    case 1:
                        InserirVeiculo();

                        // TODO DADO DEVE SER ORDENADO ANTES DE SALVAR NO XML;

                        break;
                    case 2:
                        MostrarVeiculos();
                        break;
                    case 3:
                        RemoverVeiculo();
                        //REMOVER VEÍCULO

                        //TODO COMPROVANTE DEVE SER SALVO NO XML AO FINAL DA REMOÇÃO DO VEICULO

                        // AINDA TEM QUE CRIAR O METODO DE SALVAR NO ARQUIVO XML OS COMPROVANTES  

                        break;
                    case 4:
                        //FILTRAR VEÍCULO

                        //SUBFUNÇÕES OU DIFERENTES IFS DE FILTRAGEM PARA PODER PESQUISAR VEICULO 

                        break;
                    case 5:
                        //EDITAR INFORMAÇÕES DO VEÍCULO Exemplo: modelo, fiat, proprietario, 


                        // APÓS EDITAR SALVAR e.vagas NO arquivo
                        break;
                    case 0:
                        Console.Clear();
                        Console.WriteLine("\nEncerrou Programa!!!");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Opção Invalida...");
                        Console.ReadKey();
                        break;
                }
            } while (op != 0);
        }

        static void InserirVeiculo()
        {
            Console.Clear();


            // INSERIR TUDO NO OBJETO X 
            Veiculo x = new Veiculo(true);
            Console.Write("Modelo: ");
            x.Modelo = Console.ReadLine();
            Console.Write("Marca: ");
            x.Marca = Console.ReadLine();
            Console.Write("Proprietário: ");
            x.Proprietario = Console.ReadLine();
            Console.Write("Cor: ");
            x.Cor = Console.ReadLine();
            Console.Write("É preferencial? (s/n): ");
            if (Console.ReadLine().ToLower() == "s")
                x.Preferencial = true;

            x.HoraDeEntrada = DateTime.Now;

            // LEITURA DO XML  / da matriz 
            Estacionamento e = new Estacionamento();

            if (e.VagasLivres > 0)
            {
                e.vagas.Add(x);
                e.VagasLivres--;
            }
            else
            {
                Console.WriteLine("O Estacionamento está cheio!\n");
            }

            // LOCAL ONDE SE DEVE ORDENAR O OBJETO  e.vagas



            // EXPORTANDO XML 
            e.SalvarXML();
        }

        static void MostrarVeiculos()
        {
            Console.Clear();
            Estacionamento e = new Estacionamento();

            if (e.VagasLivres != 100)
            {
                foreach (var v in e.vagas)
                {
                    Console.WriteLine("============================================");
                    Console.WriteLine($"\nPlaca: {v.Placa}");
                    Console.WriteLine($"Proprietario: {v.Proprietario}");
                    Console.WriteLine($"Marca: {v.Marca}");
                    Console.WriteLine($"Modelo: {v.Modelo}");
                    Console.WriteLine($"Cor: {v.Cor}");
                    Console.WriteLine($"Preferencial: {(v.Preferencial ? "Sim" : "Não")}");
                    Console.WriteLine($"Hora De Entrada: {v.HoraDeEntrada}\n");
                }
            }
            else
            {
                Console.WriteLine("Não há veiculos!");
                Console.ReadKey();
            }
            Console.ReadKey();
        }
        static void RemoverVeiculo()
        {
            Console.Clear();

            Console.WriteLine($"Digite a placa do veiculo desejado para ser removido:");
            string placa = Console.ReadLine().ToUpper();

            Estacionamento x = new Estacionamento();

            Veiculo veiculo = x.vagas.FirstOrDefault(v => placa.ToUpper() == placa);


            if (veiculo == null)
            {
                Console.WriteLine("veiculo nao encontrado");
                Console.ReadKey();
                return;
            }

            Comprovante c = new Comprovante
            {
                Placa = veiculo.Placa,
                HoraDeEntrada = veiculo.HoraDeEntrada,
                HoraDeSaida = DateTime.Now,
                ValorPago = x.PrecoFixo
            };

            x.vagas.Remove(veiculo);
            x.VagasLivres++;

            x.SalvarXML();

            SalvarComprovante(c);

            Console.WriteLine($"veiculo removido com sucesso!");
            Console.WriteLine($"valor pago {x.PrecoFixo}");
            Console.ReadKey();


        }

        static void SalvarComprovante(Comprovante c)
        {

            XDocument doc;

            if (File.Exists("Comprovantes.xml"))
            {
                doc = XDocument.Load("Comprovantes.xml");
            }
            else
            {
                doc = new XDocument(new XElement("comprovantes"));
            }

            XElement novo = new XElement("comprovante",
            new XElement("placa", c.Placa),
            new XElement("HoraDeEntrada", c.HoraDeEntrada),
            new XElement("HoraDeSaida", c.HoraDeSaida),
            new XElement("ValorPago", c.ValorPago)
            );

            doc.Root.Add(novo);
            doc.Save("Comprovantes.xml");

            Console.Clear();
            Console.WriteLine("===== COMPROVANTE DE SAÍDA =====");
            Console.WriteLine($"Placa: {c.Placa}");
            Console.WriteLine($"Hora de Entrada: {c.HoraDeEntrada}");
            Console.WriteLine($"Hora de Saída: {c.HoraDeSaida}");

            TimeSpan tempoEstacionado = c.HoraDeSaida - c.HoraDeEntrada;
            Console.WriteLine($"Tempo Estacionado: {tempoEstacionado.Hours}h {tempoEstacionado.Minutes}m");

            Console.WriteLine($"Valor Pago: R$ {c.ValorPago:F2}");
            Console.WriteLine("================================");
            Console.ReadKey();
            
        }


    }
}
