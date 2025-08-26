using System.Collections.Generic;

namespace JogoDePoker
{
    public class Jogador
    {
        public string Nome { get; }
        public List<Carta> Mao { get; } = new();

        public int Fichas { get; set; } = 100;
        public int ApostaAtual { get; set; }

        public Jogador(string nome)
        {
            Nome = nome;
        }

        public void FazerAposta(int quantia)
        {
            Fichas -= quantia;
            ApostaAtual += quantia;
        }

        public void LimparMao()
        {
            Mao.Clear();
            ApostaAtual = 0;
        }
    }
}

