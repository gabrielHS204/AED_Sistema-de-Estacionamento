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
        public string Placa { get; set; } // Gerada aleatoriamente com o Guid
        public string Modelo { get; set; }
        public string Marca { get; set; }
        public string Proprietario { get; set; } // Corrigido nome da variável
        public string Cor { get; set; }
    }

    public class Vaga
    {
        public string Placa; // Informação do veículo estacionado nessa vaga

        public int Andar;
        public int Fila;  // Endereço da vaga dentro do estacionamento

        public bool Livre; // Marcar se a vaga está ocupada ou não

        public DateTime HoraDeEntrada; // Corrigido tipo e nome
    }

    public class Estacionamento
    {
        public Vaga[][] Matriz; // Matriz do tipo Vaga

        public string Endereco; // Exemplo: “Monte Carmo”
        public string Nome; // Nome do estacionamento

        public int VagasLivres; // Sempre alterado conforme ocupação

        public double PrecoFixo;
    }

    // Classe para gerar comprovante de entrada/saída
    public class Comprovante
    {
        public string Placa;
        public int Andar;
        public int Fila;

        public string HoraDeEntrada;
        public string HoraDeSaida;

        public double ValorPago;
    }

    public class InclirNaVaga
    {

    }


    class program
    {

        public static void Main(string[] args)
        {
            int op;
            do
            {
                Console.Clear();

                Console.WriteLine("========== MENU DO SISTEMA ==========");
                Console.WriteLine("1 - Inserir veículo");
                Console.WriteLine("2 - Mostrar veículos");
                Console.WriteLine("3 - Remover veículo");
                Console.WriteLine("4 - Pesquisar veículo");
                Console.WriteLine("5 - Alterar veículo");
                Console.WriteLine("6 - Filtrar veículos");
                Console.WriteLine("7 - Exportar para XML");
                Console.WriteLine("8 - Sair");
                Console.WriteLine("=====================================");
                Console.Write("Escolha uma opção: ");
                op = int.Parse(Console.ReadLine());
            } while (op != 8);
            
            System.Console.WriteLine("OIIE");
            System.Console.WriteLine();
            Console.ReadKey();

        }
    }
}
