using System;

namespace Estacionamento
{
    public class Veiculo
    {
        public string Placa; // Gerada aleatoriamente com o Guid
        public string Modelo;
        public string Marca;
        public string Proprietario; // Corrigido nome da variável
        public string Cor;
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
}
teste