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
        public string? Placa { get; set; }
        public string? Modelo { get; set; }
        public string? Marca { get; set; }
        public string? Proprietario { get; set; } // Corrigido nome da variável
        public string? Cor { get; set; }
        public bool Preferencial { get; set; } //Tipo Booleano se é preferencial ou não

        public DateTime HoraDeEntrada { get; set; } // NA INSERÇÃO DO VEICULO SERA DEFININO A SUA HORA DE ENTRADA.


        public Veiculo()     //Construtor Padrão
        {
            Placa = "";   
            Modelo = "";
            Marca = "";
            Proprietario = "";
            Cor = "";
            Preferencial = false;
        }
    }


    public class Estacionamento
    {
        public List<Veiculo> vagas = new List<Veiculo>(); // vagas

        public string Endereco; // Exemplo: “Monte Carmo”
        public string Nome; // Nome do estacionamento

        public int VagasLivres; // Sempre alterado conforme ocupação
        public int VagasLivresPreferencias; // Sempre alterado conforme ocupação de vagas preferenciais
        public double PrecoFixo; // Exemplo: 14reais


        public Estacionamento()
        {
            Nome = "Estacionamento Central";
            Endereco = "Monte Carmo";
            PrecoFixo = 14.00;
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

                VagasLivres = 80 - vagas.Count;

                VagasLivresPreferencias = 20 - vagas.Count(v => v.Preferencial) > 0 ? vagas.Count(v => v.Preferencial) : 0;
            }
            else
            {
                vagas = new List<Veiculo>();
                VagasLivres = 80;
                VagasLivresPreferencias = 20;
            }
        }

       public void OrdenaVagas()
        {
            // Ordena as vagas por hora de entrada
            vagas = vagas.OrderBy(v => v.Placa).ToList();
        }

    }

    // Classe para gerar comprovante de entrada/saída
    public class Comprovante
    {
        public string Placa;

        public DateTime HoraDeEntrada;
        public DateTime HoraDeSaida;

        public double ValorPago;
        
        public bool VagaPreferencial; // Indica se a vaga é preferencial ou não

        // Comprovantes.xml out 
        public void SalvarComprovante()
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
            new XElement("placa", Placa),
            new XElement("HoraDeEntrada", HoraDeEntrada),
            new XElement("HoraDeSaida", HoraDeSaida),
            new XElement("ValorPago", ValorPago),
            new XElement("VagaPreferencial", VagaPreferencial ? "Sim" : "Não")
            );

            doc.Root.Add(novo);
            doc.Save("Comprovantes.xml");

            Console.Clear();
            Console.WriteLine("===== COMPROVANTE DE SAÍDA =====");
            Console.WriteLine($"Placa: {Placa}");
            Console.WriteLine($"Hora de Entrada: {HoraDeEntrada}");
            Console.WriteLine($"Hora de Saída: {HoraDeSaida}");

            TimeSpan tempoEstacionado = HoraDeSaida - HoraDeEntrada;
            Console.WriteLine($"Tempo Estacionado: {tempoEstacionado.Hours}h {tempoEstacionado.Minutes}m");

            Console.WriteLine($"Valor Pago: R$ {ValorPago:F2}");
            string preferencialTexto = VagaPreferencial ? "sim" : "não";
            Console.WriteLine($"Preferencial: {preferencialTexto}");

            Console.WriteLine("================================");
            Console.ReadKey();
        }
    }

    class Program
    {

        public static void Main(string[] args)
        {
            //CRUD
            int op = 1;
            Estacionamento SistemaDeEstacionamento = new Estacionamento();

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

                //Realiza releitura do XML antes de executar qualquer uma das funções
                SistemaDeEstacionamento.LerXML();

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
                        InserirVeiculo(new Veiculo(), SistemaDeEstacionamento);

                        // TODO DADO DEVE SER ORDENADO ANTES DE SALVAR NO XML;

                        break;
                    case 2:
                        MostrarVeiculos(SistemaDeEstacionamento);
                        break;
                    case 3:
                        RemoverVeiculo(SistemaDeEstacionamento);
                        //REMOVER VEÍCULO

                        //TODO COMPROVANTE DEVE SER SALVO NO XML AO FINAL DA REMOÇÃO DO VEICULO

                        // AINDA TEM QUE CRIAR O METODO DE SALVAR NO ARQUIVO XML OS COMPROVANTES  

                        break;
                    case 4:
                        FiltrarVeiculo(SistemaDeEstacionamento);
                        //FILTRAR VEÍCULO

                        //SUBFUNÇÕES OU DIFERENTES IFS DE FILTRAGEM PARA PODER PESQUISAR VEICULO 

                        break;
                    case 5:
                        //EDITAR INFORMAÇÕES DO VEÍCULO Exemplo: modelo, fiat, proprietario, 

                        EditaVeículo(new Veiculo(), SistemaDeEstacionamento); // Passa um novo objeto Veiculo para editar
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



        static void InserirVeiculo(Veiculo x, Estacionamento Sistema)
        {
            Console.Clear();


            // INSERIR TUDO NO OBJETO X 
            Console.WriteLine("===== ENTRADA DE VEÍCULO =====");
            Console.WriteLine("Placa: ");
            x.Placa = Console.ReadLine().ToUpper();
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
            if (x.Placa.Length > 7 || x.Placa.Length < 7)// conferir se a placa tem o tamanho certo
            {
                Console.WriteLine("Placa invalida....");
                Console.ReadKey();
            }
            else
            {

                if (Sistema.VagasLivres > 0)
                {
                    Sistema.vagas.Add(x);
                    Sistema.VagasLivres--;
                }
                else if ((Sistema.VagasLivresPreferencias > 0 || Sistema.VagasLivres > 0) && x.Preferencial)
                {
                    Sistema.vagas.Add(x);
                    if (Sistema.VagasLivresPreferencias > 0)
                        Sistema.VagasLivresPreferencias--;
                    else
                        Sistema.VagasLivres--;
                }
                else
                {
                    Console.WriteLine("Não há vagas disponíveis para o veículo.");
                    Console.ReadKey();
                    return;
                }
                {
                    Console.WriteLine("O Estacionamento está cheio!\n");
                }

                // LOCAL ONDE SE DEVE ORDENAR O OBJETO  Sistema.vagas



                // EXPORTANDO XML 
                Sistema.SalvarXML();
                Console.ReadKey();

            }
        }

        static void MostrarVeiculos(Estacionamento Sistema)
        {
            Console.Clear();

            // Ordena os veículos por placa antes de mostrar
            Sistema.vagas = Sistema.vagas.OrderBy(v => v.Placa).ToList();

            if (Sistema.VagasLivres != 100)
            {
                foreach (var v in Sistema.vagas)
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
                Console.WriteLine("Não há veiculos neste estacionamento!");
                Console.ReadKey();
            }
            Console.ReadKey();
        }

        static void RemoverVeiculo(Estacionamento Sistema) // Remover veículo
        {
            Console.Clear();

            Console.WriteLine($"Digite a placa do veiculo desejado para ser removido:");
            string placa = Console.ReadLine().ToUpper();

            Veiculo veiculo = Sistema.vagas.FirstOrDefault(v => v.Placa.ToUpper() == placa.ToUpper());


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
                ValorPago = Sistema.PrecoFixo,
                VagaPreferencial = veiculo.Preferencial
            };

            Sistema.vagas.Remove(veiculo);
            Sistema.VagasLivres++;

            Sistema.SalvarXML();

            c.SalvarComprovante();

            Console.WriteLine($"veiculo removido com sucesso!");
            Console.WriteLine($"valor pago {Sistema.PrecoFixo}");
            Console.ReadKey();


        }
        static void FiltrarVeiculo(Estacionamento Sistema)
        {

            Console.WriteLine("Escolha um paramentro para filtrar um veiculo: ");
            Console.WriteLine("A - Placa");
            Console.WriteLine("B - Modelo");
            Console.WriteLine("C - Marca");
            Console.WriteLine("D - Cor");
            Console.WriteLine("E - Proprietário");
            Console.WriteLine("F - Tipo da vaga (Preferencial ou normal)");

            string opc = Console.ReadLine().ToUpper();
            Console.Clear();

            switch (opc)
            {
                case "A":
                    Console.WriteLine("Digite a placa que deseja buscar: ");
                    string placa = Console.ReadLine();

                    var filtroA = from v in Sistema.vagas
                                  where placa.ToUpper() == v.Placa.ToUpper()
                                  select v;

                    Console.WriteLine();
                    Console.WriteLine(filtroA.Count() > 0 ? $"Segue o(s) veiculo(s) que possuem a placa {placa}:" : "Não encontramos nem um dado de referente ao filtro solicitado.");
                    Console.WriteLine();

                    int a = 1;
                    foreach (var veiculo in filtroA)
                    {
                        Console.WriteLine("============================================");
                        Console.WriteLine($"{a}° veiculo encontrado:");
                        Console.WriteLine($"Placa: {veiculo.Placa}");
                        Console.WriteLine($"Modelo: {veiculo.Modelo}");
                        Console.WriteLine($"Marca: {veiculo.Marca}");
                        Console.WriteLine($"Proprietário: {veiculo.Proprietario}");
                        Console.WriteLine($"Cor: {veiculo.Cor}");
                        Console.WriteLine($"Prioritaria: {(veiculo.Preferencial ? "Sim" : "Não")}");

                        a++;
                    }
                    break;

                case "B":
                    Console.WriteLine("Digite o modelo que deseja buscar: ");
                    string modelo = Console.ReadLine();

                    var filtroB = from v in Sistema.vagas
                                  where modelo.ToUpper() == v.Modelo.ToUpper()
                                  select v;

                    Console.WriteLine();
                    Console.WriteLine(filtroB.Count() > 0 ? $"Segue o(s) veiculo(s) do modelo {modelo}" : "Não encontramos nem um dado de referente ao filtro solicitado.");
                    Console.WriteLine();

                    int b = 1;
                    foreach (var veiculo in filtroB)
                    {
                        Console.WriteLine("============================================");
                        Console.WriteLine($"{b}° veiculo encontrado:");
                        Console.WriteLine($"Placa: {veiculo.Placa}");
                        Console.WriteLine($"Modelo: {veiculo.Modelo}");
                        Console.WriteLine($"Marca: {veiculo.Marca}");
                        Console.WriteLine($"Proprietário: {veiculo.Proprietario}");
                        Console.WriteLine($"Cor: {veiculo.Cor}");
                        Console.WriteLine($"Prioritaria: {(veiculo.Preferencial ? "Sim" : "Não")}");

                        b++;
                    }
                    break;

                case "C":
                    Console.WriteLine("Digite a marca que deseja buscar: ");
                    string marca = Console.ReadLine();

                    var filtroC = from v in Sistema.vagas
                                  where marca.ToUpper() == v.Marca.ToUpper()
                                  select v;

                    Console.WriteLine();
                    Console.WriteLine(filtroC.Count() > 0 ? $"Segue o(s) veiculo(s) da marca {marca}" : "Não encontramos nem um dado de referente ao filtro solicitado.");
                    Console.WriteLine();

                    int c = 1;
                    foreach (var veiculo in filtroC)
                    {
                        Console.WriteLine("============================================");
                        Console.WriteLine($"{c}° veiculo encontrado:");
                        Console.WriteLine($"Placa: {veiculo.Placa}");
                        Console.WriteLine($"Modelo: {veiculo.Modelo}");
                        Console.WriteLine($"Marca: {veiculo.Marca}");
                        Console.WriteLine($"Proprietário: {veiculo.Proprietario}");
                        Console.WriteLine($"Cor: {veiculo.Cor}");
                        Console.WriteLine($"Prioritaria: {(veiculo.Preferencial ? "Sim" : "Não")}");

                        c++;
                    }
                    break;

                case "D":
                    Console.WriteLine("Digite a cor que deseja buscar: ");
                    string cor = Console.ReadLine();

                    var filtroD = from v in Sistema.vagas
                                  where cor.ToUpper() == v.Cor.ToUpper()
                                  select v;

                    Console.WriteLine();
                    Console.WriteLine(filtroD.Count() > 0 ? $"Segue o(s) veiculo(s) da cor {cor}" : "Não encontramos nem um dado de referente ao filtro solicitado.");
                    Console.WriteLine();

                    int d = 1;
                    foreach (var veiculo in filtroD)
                    {
                        Console.WriteLine("============================================");
                        Console.WriteLine($"{d}° veiculo encontrado:");
                        Console.WriteLine($"Placa: {veiculo.Placa}");
                        Console.WriteLine($"Modelo: {veiculo.Modelo}");
                        Console.WriteLine($"Marca: {veiculo.Marca}");
                        Console.WriteLine($"Proprietário: {veiculo.Proprietario}");
                        Console.WriteLine($"Cor: {veiculo.Cor}");
                        Console.WriteLine($"Prioritaria: {(veiculo.Preferencial ? "Sim" : "Não")}");

                        d++;
                    }
                    break;

                case "E":
                    Console.WriteLine("Digite o nome do proprietário que deseja buscar: ");
                    string proprietario = Console.ReadLine();

                    var filtroE = from v in Sistema.vagas
                                  where proprietario.ToUpper() == v.Proprietario.ToUpper()
                                  select v;

                    Console.WriteLine();
                    Console.WriteLine(filtroE.Count() > 0 ? $"Segue o(s) veiculo(s) do proprietário {proprietario}:" : "Não encontramos nem um dado de referente ao filtro solicitado.");
                    Console.WriteLine();

                    int e = 1;
                    foreach (var veiculo in filtroE)
                    {
                        Console.WriteLine("============================================");
                        Console.WriteLine($"{e}° veiculo encontrado:");
                        Console.WriteLine($"Placa: {veiculo.Placa}");
                        Console.WriteLine($"Modelo: {veiculo.Modelo}");
                        Console.WriteLine($"Marca: {veiculo.Marca}");
                        Console.WriteLine($"Proprietário: {veiculo.Proprietario}");
                        Console.WriteLine($"Cor: {veiculo.Cor}");
                        Console.WriteLine($"Prioritaria: {(veiculo.Preferencial ? "Sim" : "Não")}");

                        e++;
                    }
                    break;

                case "F":
                    var filtroF = from v in Sistema.vagas
                                  where v.Preferencial == true
                                  select v;

                    Console.WriteLine();
                    Console.WriteLine(filtroF.Count() > 0 ? $"Segue o(s) veiculo(s) que estão em vagas prioritárias:" : "Não encontramos nem um dado de referente ao filtro solicitado.");
                    Console.WriteLine();

                    int f = 1;
                    foreach (var veiculo in filtroF)
                    {
                        Console.WriteLine("============================================");
                        Console.WriteLine($"{f}° veiculo encontrado:");
                        Console.WriteLine($"Placa: {veiculo.Placa}");
                        Console.WriteLine($"Modelo: {veiculo.Modelo}");
                        Console.WriteLine($"Marca: {veiculo.Marca}");
                        Console.WriteLine($"Proprietário: {veiculo.Proprietario}");
                        Console.WriteLine($"Cor: {veiculo.Cor}");
                        Console.WriteLine($"Prioritaria: {(veiculo.Preferencial ? "Sim" : "Não")}");

                        f++;
                    }
                    break;
            }

            Console.ReadKey();
        }

        public static void EditaVeículo(Veiculo editarVeiculo, Estacionamento Sistema) // Edita informações do veículo
        {
            bool achou = false;


            Console.WriteLine("\nPara editar as informações do seu veículo, por favor, informe a placa: ");
            editarVeiculo.Placa = Console.ReadLine();

            foreach (var v in Sistema.vagas)
            {


                if (v.Placa == editarVeiculo.Placa) // Verifica se a placa existe na lista de veículos
                {

                    // Se a placa existir, solicita as novas informações
                    Console.Clear();
                    Console.WriteLine("Placa encontrada! Por favor, insira as novas informações do veículo:");

                    Console.Write("Modelo: ");
                    v.Modelo = Console.ReadLine();
                    Console.Write("Placa: ");
                    v.Placa = Console.ReadLine();
                    Console.Write("Marca: ");
                    v.Marca = Console.ReadLine();
                    Console.Write("Proprietário: ");
                    v.Proprietario = Console.ReadLine();
                    Console.WriteLine("Cor: ");
                    v.Cor = Console.ReadLine();
                    Console.WriteLine("É preferencial? (s/n): ");
                    if (Console.ReadLine().ToLower() == "s")
                        v.Preferencial = true;


                    // Salva as alterações no XML e Atualiza o veículo na lista

                    Sistema.SalvarXML();
                    achou = true;

                }


            }

            if (!achou) // Se somente não encontrar
            {
                Console.WriteLine("Placa não encontrada! Por favor , insira uma placa válida.");
                Console.ReadKey();
            }
        }
    }
}
