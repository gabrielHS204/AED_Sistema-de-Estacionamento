using System;
using System.Xml.Linq;

namespace Estacionamento // Organizar os blocos de códigos
{

    //Classe de veículos do estacionamento
    public class Veiculo
    {
        public string? Placa { get; set; }
        public string? Modelo { get; set; }
        public string? Marca { get; set; }
        public string? Proprietario { get; set; }
        public string? Cor { get; set; }
        public bool Preferencial { get; set; }

        public DateTime HoraDeEntrada { get; set; }


        public Veiculo()     //Construtor Padrão da classe Veículo
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
        public List<Veiculo> vagas = new List<Veiculo>();  // Tipo Genérico List
        public string Endereco;
        public string Nome;

        public int VagasLivres; // Sempre alterado conforme ocupação
        public int VagasLivresPreferencias; // Sempre alterado conforme ocupação de vagas preferenciais
        public double PrecoFixo; // Exemplo: 14reais


        public Estacionamento() // Construtor do estacionamneto
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

            double precoPorHoras = 10;

            TimeSpan tempoEstacionado = HoraDeSaida - HoraDeEntrada;
            int horas = (int)Math.Ceiling(tempoEstacionado.TotalHours + (tempoEstacionado.Days) * 24);
            ValorPago = horas * precoPorHoras;

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
                new XElement("ValorPago", ValorPago.ToString("F2")),
                new XElement("VagaPreferencial", VagaPreferencial ? "Sim" : "Não")
            );

            doc.Root.Add(novo);
            doc.Save("Comprovantes.xml");

            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("===== COMPROVANTE DE SAÍDA =====");
            Console.WriteLine($"Placa: {Placa}");
            Console.WriteLine($"Hora de Entrada: {HoraDeEntrada}");
            Console.WriteLine($"Hora de Saída: {HoraDeSaida}");
            Console.WriteLine($"Tempo Estacionado: {tempoEstacionado.Days}Dias {tempoEstacionado.Hours}h {tempoEstacionado.Minutes}m");
            Console.WriteLine($"Valor Pago: R$ {ValorPago:F2}");
            Console.WriteLine($"Preferencial: {(VagaPreferencial ? "Sim" : "Não")}");
            Console.WriteLine("================================\n");


        }
    }

    class Program
    {

        //Programa Principal
        public static void Main(string[] args)
        {
            //CRUD
            int op = 1;
            Estacionamento SistemaDeEstacionamento = new Estacionamento();

            do
            {
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine("========== MENU DO SISTEMA ==========");
                Console.WriteLine("1 - Inserir veículo");
                Console.WriteLine("2 - Mostrar veículos");
                Console.WriteLine("3 - Remover veículo");
                Console.WriteLine("4 - Filtrar veículos");
                Console.WriteLine("5 - Editar informações do veículo");
                Console.WriteLine("0 - Sair");
                Console.WriteLine("=====================================\n");
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

                //Casa para chamada de funcões de acordo com a opção escolhida
                switch (op)
                {
                    case 1:

                        // Chama a função para inserção de veículos no xml , todo dado deve ser ordenado antes de ser salvo no XML;
                        InserirVeiculo(new Veiculo(), SistemaDeEstacionamento);

                        break;
                    case 2:

                        //Chama a função para mostrar veíulos presentes no estacionamento
                        MostrarVeiculos(SistemaDeEstacionamento);

                        break;
                    case 3:

                        //Chama a função remove veículos e salva comprovantes em Comprovantes.xml
                        RemoverVeiculo(SistemaDeEstacionamento);

                        break;
                    case 4:

                        //Chama a função  para filtrar veículos , contém subfunções de filtragem para pesquisar pelo veículo
                        FiltrarVeiculo(SistemaDeEstacionamento);

                        break;
                    case 5:

                        //Chama a função para editar veículos e após edição os dados são alterados em veiculos.xml
                        EditaVeículo(new Veiculo(), SistemaDeEstacionamento);

                        break;
                    case 0:
                        Console.Clear();
                        Console.WriteLine("\nEncerrou Programa!!!");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Opção Inválida..."); // Tratamento de erro caso opção inválida do Menu
                        Console.ReadKey();
                        break;
                }
            } while (op != 0);
        }



        static void InserirVeiculo(Veiculo x, Estacionamento Sistema)
        {
            Console.Clear();


            // INSERIR TUDO NO OBJETO X 
            Console.WriteLine("");
            Console.WriteLine("===== ENTRADA DE VEÍCULO =====\n");
            Console.Write("Por favor , informe a Placa:  "); // Placa é a chave primária
            x.Placa = Console.ReadLine().ToUpper();

            XDocument doc = XDocument.Load("veiculos.xml"); //Carrega o arquivo XML 

            bool PlacaExiste = doc.Descendants("Veiculo").Any(p => (string)p.Element("Placa") == x.Placa); // Expressão Lamnda do LINQ para verificar se a placa já existe no XML

            if (x.Placa.Length != 7 || PlacaExiste)   // Tratamento de erro para assegurar que o proprietário coloque 7 caracteres
            {
                Console.WriteLine("\nPlaca inválida....Insira uma placa válida no formato PIV.");
                Console.ReadKey();
            }
            else
            {
                Console.Write("\nModelo: ");
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

                Console.Write("\nSeu veículo foi adicionado com sucesso. Seja bem-vindo(a) ao Estacionamento!");

                // O veículo entra nas Vagas Livres ou nas Vagas Livres Preferenciais
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
                    Console.WriteLine("\nNão há vagas disponíveis para o veículo. Por favor, procure outra vaga ou espere uma vaga ser liberada.");
                    Console.ReadKey();
                    return;
                }


                // LOCAL ONDE SE DEVE ORDENAR O OBJETO  Sistema.vagas


                // Exporta para o XML 
                Sistema.SalvarXML();
                Console.ReadKey();

            }
        }

        static void MostrarVeiculos(Estacionamento Sistema) // Função mostra veículos
        {
            Console.Clear();

            // Ordena os veículos por placa antes de mostrar , atarvés da expressão Lambda do LINQ
            Sistema.vagas = Sistema.vagas.OrderBy(v => v.Placa).ToList();

            if ((Sistema.VagasLivres + Sistema.VagasLivresPreferencias) != 100) // Verifica se há veículos no estacionamento
            {
                foreach (var v in Sistema.vagas)
                {
                    Console.WriteLine("");
                    Console.WriteLine("============================================\n");
                    Console.WriteLine($"Placa: {v.Placa}");
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
                Console.WriteLine("\nNão há veículos neste estacionamento!");
                Console.ReadKey();
            }
            Console.ReadKey();
        }

        static void RemoverVeiculo(Estacionamento Sistema) // Remover veículo
        {
            Console.Clear();
            Console.WriteLine();
            Console.Write("Digite a placa do veículo desejado para ser removido: "); // Busca pela chave primária Placa 
            string placa = Console.ReadLine().ToUpper();

            Veiculo veiculo = Sistema.vagas.FirstOrDefault(v => v.Placa.ToUpper() == placa.ToUpper()); // Utiliza a expressão Lambda do LINQ para buscar o veículo pela placa

            XDocument doc = XDocument.Load("veiculos.xml");


            if (veiculo == null)
            {
                Console.WriteLine("\nVeículo não encontrado. Por favor, insira uma placa válida!");
                Console.ReadKey();
                return;
            }

            Comprovante c = new Comprovante // Cria um novo comprovante para o veículo que está sendo removido
            {
                Placa = veiculo.Placa,
                HoraDeEntrada = veiculo.HoraDeEntrada,
                HoraDeSaida = DateTime.Now,
                ValorPago = Sistema.PrecoFixo,
                VagaPreferencial = veiculo.Preferencial
            };

            Sistema.vagas.Remove(veiculo);

            if (veiculo.Preferencial == true)
            {
                Sistema.VagasLivresPreferencias++;
            }
            else
            {
                Sistema.VagasLivres++;
            }

            Sistema.SalvarXML();

            c.SalvarComprovante();

            Console.WriteLine("Veículo removido com sucesso.Volte sempre!");
            Console.ReadKey();


        }

        // Filtra véiculos de acordo com  a escolha do proprietário;
        // A busca pode ser feita por : Placa , Modelo , Marca , cor, Proprietário e Tipo de vaga (Preferencial ou não preferencial);
        static void FiltrarVeiculo(Estacionamento Sistema)
        {
            // Menu de filtragem
            Console.WriteLine();
            Console.WriteLine("Escolha um paramentro para filtrar um veiculo: ");
            Console.WriteLine("A - Placa");
            Console.WriteLine("B - Modelo");
            Console.WriteLine("C - Marca");
            Console.WriteLine("D - Cor");
            Console.WriteLine("E - Proprietário");
            Console.WriteLine("F - Tipo da vaga (Preferencial ou normal)\n");


            string opc = Console.ReadLine().ToUpper();
            Console.Clear();

            switch (opc)
            {
                case "A":
                    Console.Write("Digite a placa que deseja buscar: ");
                    string placa = Console.ReadLine();


                    var filtroA = from v in Sistema.vagas //Filtra os veículos na lista Sistemas.vagas onde a placa corresponde a entrada
                                  where placa.ToUpper() == v.Placa.ToUpper()
                                  select v;

                    Console.WriteLine();
                    // Verifica se o filtro obteve algum resultado, se sim retorna os veiculos filtra, se não retorna uma mensagem de que nenhum dado foi encontrado.
                    Console.WriteLine(filtroA.Count() > 0 ? $"Segue o(s) veiculo(s) que possuem a placa {placa}:" : "Não encontramos nenhum dado de referente ao filtro solicitado.");
                    Console.WriteLine();


                    int a = 1;
                    foreach (var veiculo in filtroA)
                    {
                        Console.WriteLine("==================================================");
                        Console.WriteLine($"{a}° veiculo encontrado:");
                        Console.WriteLine($"Placa: {veiculo.Placa}");
                        Console.WriteLine($"Modelo: {veiculo.Modelo}");
                        Console.WriteLine($"Marca: {veiculo.Marca}");
                        Console.WriteLine($"Proprietário: {veiculo.Proprietario}");
                        Console.WriteLine($"Cor: {veiculo.Cor}");
                        Console.WriteLine($"Prioritária: {(veiculo.Preferencial ? "Sim" : "Não")}");

                        a++;
                    }
                    break;

                case "B":
                    Console.Write("Digite o modelo que deseja buscar: ");
                    string modelo = Console.ReadLine();

                    var filtroB = from v in Sistema.vagas// Filtra os veículos na lista Sistema.vagas onde o modelo corresponde a entrada do usuário.
                                  where modelo.ToUpper() == v.Modelo.ToUpper()
                                  select v;

                    Console.WriteLine();
                    // Verifica se o filtro obteve algum resultado, se sim retorna os veiculos filtra, se não retorna uma mensagem de que nenhum dado foi encontrado.
                    Console.WriteLine(filtroB.Count() > 0 ? $"Segue o(s) veículo(s) do modelo {modelo}" : "Não encontramos nenhum dado de referente ao filtro solicitado.");
                    Console.WriteLine();

                    int b = 1;
                    foreach (var veiculo in filtroB)
                    {
                        Console.WriteLine("==================================================");
                        Console.WriteLine($"{b}° veículo encontrado:");
                        Console.WriteLine($"Placa: {veiculo.Placa}");
                        Console.WriteLine($"Modelo: {veiculo.Modelo}");
                        Console.WriteLine($"Marca: {veiculo.Marca}");
                        Console.WriteLine($"Proprietário: {veiculo.Proprietario}");
                        Console.WriteLine($"Cor: {veiculo.Cor}");
                        Console.WriteLine($"Prioritária: {(veiculo.Preferencial ? "Sim" : "Não")}");

                        b++;
                    }
                    break;

                case "C":
                    Console.Write("Digite a marca que deseja buscar: ");
                    string marca = Console.ReadLine();

                    var filtroC = from v in Sistema.vagas// Filtra os veículos na lista Sistema.vagas onde a marca corresponde a entrada do usuário.
                                  where marca.ToUpper() == v.Marca.ToUpper()
                                  select v;

                    Console.WriteLine();
                    // Verifica se o filtro obteve algum resultado, se sim retorna os veiculos filtra, se não retorna uma mensagem de que nenhum dado foi encontrado.
                    Console.WriteLine(filtroC.Count() > 0 ? $"Segue o(s) veículo(s) da marca {marca}" : "Não encontramos nenhum dado de referente ao filtro solicitado.");
                    Console.WriteLine();

                    int c = 1;
                    foreach (var veiculo in filtroC)
                    {
                        Console.WriteLine("==================================================");
                        Console.WriteLine($"{c}° veículo encontrado:");
                        Console.WriteLine($"Placa: {veiculo.Placa}");
                        Console.WriteLine($"Modelo: {veiculo.Modelo}");
                        Console.WriteLine($"Marca: {veiculo.Marca}");
                        Console.WriteLine($"Proprietário: {veiculo.Proprietario}");
                        Console.WriteLine($"Cor: {veiculo.Cor}");
                        Console.WriteLine($"Prioritária: {(veiculo.Preferencial ? "Sim" : "Não")}");

                        c++;
                    }
                    break;

                case "D":
                    Console.Write("Digite a cor que deseja buscar: ");
                    string cor = Console.ReadLine();

                    var filtroD = from v in Sistema.vagas// Filtra os veículos na lista Sistema.vagas onde a cor corresponde a entrada do usuário.
                                  where cor.ToUpper() == v.Cor.ToUpper()
                                  select v;

                    Console.WriteLine();
                    // Verifica se o filtro obteve algum resultado, se sim retorna os veiculos filtra, se não retorna uma mensagem de que nenhum dado foi encontrado.
                    Console.WriteLine(filtroD.Count() > 0 ? $"Segue o(s) veículo(s) da cor {cor}" : "Não encontramos nenhum dado de referente ao filtro solicitado.");
                    Console.WriteLine();

                    int d = 1;
                    foreach (var veiculo in filtroD)
                    {
                        Console.WriteLine("==================================================");
                        Console.WriteLine($"{d}° veículo encontrado:");
                        Console.WriteLine($"Placa: {veiculo.Placa}");
                        Console.WriteLine($"Modelo: {veiculo.Modelo}");
                        Console.WriteLine($"Marca: {veiculo.Marca}");
                        Console.WriteLine($"Proprietário: {veiculo.Proprietario}");
                        Console.WriteLine($"Cor: {veiculo.Cor}");
                        Console.WriteLine($"Prioritária: {(veiculo.Preferencial ? "Sim" : "Não")}");

                        d++;
                    }
                    break;

                case "E":
                    Console.Write("Digite o nome do proprietário que deseja buscar: ");
                    string proprietario = Console.ReadLine();

                    var filtroE = from v in Sistema.vagas// Filtra os veículos na lista Sistema.vagas onde o proprietário corresponde a entrada do usuário.
                                  where proprietario.ToUpper() == v.Proprietario.ToUpper()
                                  select v;

                    Console.WriteLine();
                    // Verifica se o filtro obteve algum resultado, se sim retorna os veiculos filtra, se não retorna uma mensagem de que nenhum dado foi encontrado.
                    Console.WriteLine(filtroE.Count() > 0 ? $"Segue o(s) veículo(s) do proprietário {proprietario}:" : "Não encontramos nenhum dado de referente ao filtro solicitado.");
                    Console.WriteLine();

                    int e = 1;
                    foreach (var veiculo in filtroE)
                    {
                        Console.WriteLine("==================================================");
                        Console.WriteLine($"{e}° veículo encontrado:");
                        Console.WriteLine($"Placa: {veiculo.Placa}");
                        Console.WriteLine($"Modelo: {veiculo.Modelo}");
                        Console.WriteLine($"Marca: {veiculo.Marca}");
                        Console.WriteLine($"Proprietário: {veiculo.Proprietario}");
                        Console.WriteLine($"Cor: {veiculo.Cor}");
                        Console.WriteLine($"Prioritária: {(veiculo.Preferencial ? "Sim" : "Não")}");

                        e++;
                    }
                    break;

                case "F":
                    string preferencia;

                    //Solicita ao usuario uma entrada 'n' ou 's', caso contrario a entrada será solicitada até está correta
                    do
                    {
                        Console.WriteLine("Digite 's' se deseja filtrar vagas preferenciais ou 'n' se deseja filtrar vagas não preferencias: ");
                        preferencia = Console.ReadLine();

                    } while (preferencia != "s" && preferencia != "n");

                    bool preferenciaBooleana = false;

                    //Atribui a variavel preferenciaBoolena true ou false de acordo com a entrada do usuario
                    if (preferencia == "s")
                    {
                        preferenciaBooleana = true;
                    }
                    else if (preferencia == "n")
                    {
                        preferenciaBooleana = false;
                    }

                    var filtroF = from v in Sistema.vagas// Filtra os veículos na lista Sistema.vagas onde a propriedade Preferencial é verdadeira ou falsa, com base na escolha do usuário.
                                  where v.Preferencial == preferenciaBooleana
                                  select v;

                    Console.WriteLine();
                    // Verifica se o filtro obteve algum resultado, se sim retorna os veiculos filtra, se não retorna uma mensagem de que nenhum dado foi encontrado.
                    Console.WriteLine(filtroF.Count() > 0 ? $"Segue o(s) veículo(s) que estão em vagas prioritárias:" : "Não encontramos nem um dado de referente ao filtro solicitado.");
                    Console.WriteLine();

                    int f = 1;
                    foreach (var veiculo in filtroF)
                    {
                        Console.WriteLine("============================================");
                        Console.WriteLine($"{f}° veículo encontrado:");
                        Console.WriteLine($"Placa: {veiculo.Placa}");
                        Console.WriteLine($"Modelo: {veiculo.Modelo}");
                        Console.WriteLine($"Marca: {veiculo.Marca}");
                        Console.WriteLine($"Proprietário: {veiculo.Proprietario}");
                        Console.WriteLine($"Cor: {veiculo.Cor}");
                        Console.WriteLine($"Prioritária: {(veiculo.Preferencial ? "Sim" : "Não")}");

                        f++;
                    }
                    break;
            }

            Console.ReadKey();
        }

        public static void EditaVeículo(Veiculo editarVeiculo, Estacionamento Sistema) // Edita informações do veículo
        {
            bool achou = false;

            Estacionamento x = new Estacionamento();
            Console.WriteLine();
            Console.Write("Para editar as informações do seu veículo, por favor, informe a placa: "); // Busca pela placa (chave primária)
            editarVeiculo.Placa = Console.ReadLine();

            foreach (var v in Sistema.vagas)
            {

                Console.Clear();

                if (v.Placa == editarVeiculo.Placa) // Verifica se a placa existe na lista de veículos
                {

                    // Se a placa existir, solicita as novas informações

                    Console.WriteLine();
                    Console.WriteLine("Placa encontrada! Por favor, insira as novas informações do veículo:\n");

                    Console.Write("Modelo: ");
                    v.Modelo = Console.ReadLine();
                    Console.Write("Marca: ");
                    v.Marca = Console.ReadLine();
                    Console.Write("Proprietário: ");
                    v.Proprietario = Console.ReadLine();
                    Console.Write("Cor: ");
                    v.Cor = Console.ReadLine();

                    bool eraPreferencial = v.Preferencial;

                    Console.Write("É preferencial? (s/n): ");
                    string pref = Console.ReadLine().ToLower();
                    v.Preferencial = (pref == "s");

                    if (eraPreferencial != v.Preferencial)
                    {
                        if (v.Preferencial)
                        {
                            Sistema.VagasLivresPreferencias++;
                            Sistema.VagasLivres--;
                        }
                        else
                        {
                            Sistema.VagasLivresPreferencias--;
                            Sistema.VagasLivres++;
                        }
                    }


                    // Salva as alterações no XML e Atualiza o veículo na lista

                    Sistema.SalvarXML();
                    achou = true;
                    Console.WriteLine("\nSeu veículo foi editado com sucesso!");

                    Console.ReadKey();

                }
            }
            if (!achou) // Se somente não encontrar
            {
                Console.WriteLine("");
                Console.WriteLine("Placa não encontrada! Por favor , insira uma placa válida.");
                Console.ReadKey();
            }

        }


    }
}