# Documentação do código
# Integrantes do Grupo:

Arthur Souza Carvalho,
Anthony Augusto Neindite Nunes da Silva,
Gabriel Henrique da Silva Santos,
Gardenia Iris Anselmo Rodrigues da Fonseca,
Geovanna Quezia Oliveira de Cassia,
Sara de Almeida Pasolini

# Sistema de Gerenciamento de Estacionamento

Este é um programa desenvolvido em C# puro para gerenciamento de entrada e saída de veículos em um estacionamento com controle de vagas preferenciais (ou não) e persistência de dados através de arquivos XML. Todo o trabalho foi desenvolvido de acordo com os conhecimentos de estrtura de dados adquiridos ao longo desse período, além de outros conhecimentos prévios. 

# Objetivo

Aplicar nossos conhecimentos adquiridos na disciplina de AED, desenvolvendo um trabalho prático para gerenciamento de carros em um estacionamento com operações de cadastro, consulta, remoção, edição e emissão de comprovantes XML.

# Funcionamento do Sistema

# Classes

- A Classe Veiculo: armazena as informações de placa, modelo, marca, proprietário, cor, se é vaga preferencial ou não e hora de entrada (foi usado o DateTime para registrar a hora).
- A Classe Estacionamento: controla a lista de veículos, gerencia vagas disponíveis e arquivos de dados (veiculos.xml).
- A Classe Comprovante: gera recibos com a hora de entrada, saída e valor pago, tudo isso fica salvos em comprovantes.xml.

 # Listas 

List <Veiculo> vagas  
  - Lista principal que armazena os veículos que estiverem estacionados. 
  - Adicionar e remover veículos dinamicamente;
  - Ordenar por atributos (placa);
  - Editar dados com base na chave placa.

# LINQ

  - Utilizamos from, in, where e select para fazer consultas personalizadas na lista vagas:
  - Busca por placa, marca, modelo, cor, proprietário;
  - Filtragem de veículos preferenciais.

# Funcionalidades

- Inserção de veículo: cadastro completo com validação de placa e tipo de vaga (usuário deve inserir um tamanho adequado para a placa).
- Listagem: exibe todos os veículos, ordenados pela placa.
- Remoção: busca o veículo através da placa, (a placa foi escolhida como chave por ser um identificador único), remove o veículo, atualiza a quantidade de vagas e gera o comprovante de saída.
- Filtros: busca por placa.
- Edição: atualiza dados de um veículo existente com base na placa (a placa não pode ser editada pois é o idenficdor único para cada carro e sua edição não faria sentido).
- Persistência: todos os dados são salvos e lidos nos arquivos XML.



