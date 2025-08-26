using System;
using System.Collections.Generic;
using System.Linq;

namespace JogoDePoker
{
    public class Baralho
    {
        private readonly List<Carta> _cartas = new();
        private readonly Random _aleatorio = new();

        public Baralho()
        {
            foreach (Naipe naipe in Enum.GetValues(typeof(Naipe)))
            {
                foreach (ValorCarta valor in Enum.GetValues(typeof(ValorCarta)))
                {
                    _cartas.Add(new Carta(naipe, valor));
                }
            }
        }

        public void Embaralhar()
        {
            for (int i = _cartas.Count - 1; i > 0; i--)
            {
                int j = _aleatorio.Next(i + 1);
                var cartaTemp = _cartas[i];
                _cartas[i] = _cartas[j];
                _cartas[j] = cartaTemp;
            }
        }

        public Carta Distribuir()
        {
            var carta = _cartas.First();
            _cartas.RemoveAt(0);
            return carta;
        }
    }
}

