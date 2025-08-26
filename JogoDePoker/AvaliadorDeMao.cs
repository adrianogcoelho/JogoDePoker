using System;
using System.Collections.Generic;
using System.Linq;

namespace JogoDePoker
{
    public enum ClassificacaoMao
    {
        CartaAlta = 1,
        Par,
        DoisPares,
        Trinca,
        Sequencia,
        Flush,
        FullHouse,
        Quadra,
        StraightFlush,
        RoyalFlush
    }

    public class ValorMao : IComparable<ValorMao>
    {
        public ClassificacaoMao Classificacao { get; }
        public List<int> CartasAltas { get; }

        public ValorMao(ClassificacaoMao classificacao, IEnumerable<int> cartasAltas)
        {
            Classificacao = classificacao;
            CartasAltas = cartasAltas.ToList();
        }

        public int CompareTo(ValorMao? outro)
        {
            if (outro == null) return 1;
            if (Classificacao != outro.Classificacao)
                return Classificacao.CompareTo(outro.Classificacao);
            for (int i = 0; i < CartasAltas.Count && i < outro.CartasAltas.Count; i++)
            {
                if (CartasAltas[i] != outro.CartasAltas[i])
                    return CartasAltas[i].CompareTo(outro.CartasAltas[i]);
            }
            return 0;
        }

        public override string ToString()
        {
            return Classificacao switch
            {
                ClassificacaoMao.CartaAlta => "Carta Alta",
                ClassificacaoMao.Par => "Par",
                ClassificacaoMao.DoisPares => "Dois Pares",
                ClassificacaoMao.Trinca => "Trinca",
                ClassificacaoMao.Sequencia => "Sequência",
                ClassificacaoMao.Flush => "Flush",
                ClassificacaoMao.FullHouse => "Full House",
                ClassificacaoMao.Quadra => "Quadra",
                ClassificacaoMao.StraightFlush => "Straight Flush",
                ClassificacaoMao.RoyalFlush => "Royal Flush",
                _ => Classificacao.ToString()
            };
        }
    }

    public static class AvaliadorDeMao
    {
        public static ValorMao Avaliar(IEnumerable<Carta> cartas)
        {
            var listaCartas = cartas.ToList();
            var valores = listaCartas.Select(c => (int)c.Valor).ToList();
            var gruposValor = valores.GroupBy(r => r)
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Key)
                .ToList();

            var grupoNaipe = listaCartas.GroupBy(c => c.Naipe)
                .FirstOrDefault(g => g.Count() >= 5);
            bool ehFlush = grupoNaipe != null;
            List<int>? valoresNaipe = ehFlush ? grupoNaipe!.Select(c => (int)c.Valor).OrderByDescending(r => r).ToList() : null;

            int sequenciaAlta = ObterSequencia(valores.Distinct().ToList());
            int sequenciaFlushAlta = -1;
            if (ehFlush)
            {
                sequenciaFlushAlta = ObterSequencia(valoresNaipe!.Distinct().ToList());
            }

            if (sequenciaFlushAlta == 14)
                return new ValorMao(ClassificacaoMao.RoyalFlush, new List<int> { 14 });
            if (sequenciaFlushAlta > 0)
                return new ValorMao(ClassificacaoMao.StraightFlush, new List<int> { sequenciaFlushAlta });

            var quadra = gruposValor.FirstOrDefault(g => g.Count() == 4);
            if (quadra != null)
            {
                var desempate = gruposValor.Where(g => g.Count() != 4).Select(g => g.Key).Max();
                return new ValorMao(ClassificacaoMao.Quadra, new List<int> { quadra.Key, desempate });
            }

            var trinca = gruposValor.FirstOrDefault(g => g.Count() == 3);
            var par = gruposValor.FirstOrDefault(g => g.Count() == 2);
            if (trinca != null && par != null)
            {
                return new ValorMao(ClassificacaoMao.FullHouse, new List<int> { trinca.Key, par.Key });
            }

            if (ehFlush)
            {
                return new ValorMao(ClassificacaoMao.Flush, valoresNaipe!.Take(5));
            }

            if (sequenciaAlta > 0)
            {
                return new ValorMao(ClassificacaoMao.Sequencia, new List<int> { sequenciaAlta });
            }

            if (trinca != null)
            {
                var desempates = gruposValor.Where(g => g.Count() == 1).Select(g => g.Key).OrderByDescending(r => r).Take(2);
                return new ValorMao(ClassificacaoMao.Trinca, new List<int> { trinca.Key }.Concat(desempates));
            }

            var pares = gruposValor.Where(g => g.Count() == 2).ToList();
            if (pares.Count >= 2)
            {
                var paresAltos = pares.Take(2).Select(g => g.Key).OrderByDescending(r => r).ToList();
                var desempate = gruposValor.Where(g => g.Count() == 1).Select(g => g.Key).Max();
                return new ValorMao(ClassificacaoMao.DoisPares, new List<int> { paresAltos[0], paresAltos[1], desempate });
            }

            if (par != null)
            {
                var desempates = gruposValor.Where(g => g.Count() == 1).Select(g => g.Key).OrderByDescending(r => r).Take(3);
                return new ValorMao(ClassificacaoMao.Par, new List<int> { par.Key }.Concat(desempates));
            }

            var cartasAltas = valores.OrderByDescending(r => r).Take(5);
            return new ValorMao(ClassificacaoMao.CartaAlta, cartasAltas);
        }

        private static int ObterSequencia(List<int> valores)
        {
            var ordenados = valores.OrderBy(r => r).ToList();
            if (ordenados.Contains(14))
                ordenados.Insert(0, 1); // Ás baixo
            int contagem = 1;
            int melhorAlta = 0;
            for (int i = 1; i < ordenados.Count; i++)
            {
                if (ordenados[i] == ordenados[i - 1] + 1)
                {
                    contagem++;
                    if (contagem >= 5)
                        melhorAlta = ordenados[i];
                }
                else if (ordenados[i] != ordenados[i - 1])
                {
                    contagem = 1;
                }
            }
            return melhorAlta;
        }
    }
}
