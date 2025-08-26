using System;
using System.Collections.Generic;

namespace JogoDePoker
{
    public enum Naipe
    {
        Copas,
        Ouros,
        Paus,
        Espadas
    }

    public enum ValorCarta
    {
        Dois = 2,
        Tres,
        Quatro,
        Cinco,
        Seis,
        Sete,
        Oito,
        Nove,
        Dez,
        Valete,
        Dama,
        Rei,
        As
    }

    public class Carta
    {
        public Naipe Naipe { get; }
        public ValorCarta Valor { get; }

        private static readonly Dictionary<Naipe, string> SimbolosNaipe = new()
        {
            { Naipe.Copas, "\u2665" },
            { Naipe.Ouros, "\u2666" },
            { Naipe.Paus, "\u2663" },
            { Naipe.Espadas, "\u2660" }
        };

        private static readonly Dictionary<ValorCarta, string> SimbolosValor = new()
        {
            { ValorCarta.Dois, "2" },
            { ValorCarta.Tres, "3" },
            { ValorCarta.Quatro, "4" },
            { ValorCarta.Cinco, "5" },
            { ValorCarta.Seis, "6" },
            { ValorCarta.Sete, "7" },
            { ValorCarta.Oito, "8" },
            { ValorCarta.Nove, "9" },
            { ValorCarta.Dez, "10" },
            { ValorCarta.Valete, "J" },
            { ValorCarta.Dama, "Q" },
            { ValorCarta.Rei, "K" },
            { ValorCarta.As, "A" }
        };

        public Carta(Naipe naipe, ValorCarta valor)
        {
            Naipe = naipe;
            Valor = valor;
        }

        public override string ToString()
        {
            return SimbolosValor[Valor] + SimbolosNaipe[Naipe];
        }
    }
}

