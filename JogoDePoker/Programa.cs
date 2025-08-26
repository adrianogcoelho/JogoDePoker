using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JogoDePoker
{
    internal class Programa
    {
        static Random aleatorio = new();

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var jogador = new Jogador("Você");
            var computador = new Jogador("Computador");
            var comunitarias = new List<Carta>();

            while (true)
            {
                var baralho = new Baralho();
                baralho.Embaralhar();
                jogador.LimparMao();
                computador.LimparMao();
                comunitarias.Clear();

                int apostaAtual = 1;
                int pote = 0;
                jogador.FazerAposta(apostaAtual);
                computador.FazerAposta(apostaAtual);
                pote = jogador.ApostaAtual + computador.ApostaAtual;

                // Distribui as cartas iniciais
                jogador.Mao.Add(baralho.Distribuir());
                computador.Mao.Add(baralho.Distribuir());
                jogador.Mao.Add(baralho.Distribuir());
                computador.Mao.Add(baralho.Distribuir());

                Console.WriteLine("=== Nova Mão ===");
                Console.WriteLine($"Suas cartas: {jogador.Mao[0]}, {jogador.Mao[1]}");
                Console.WriteLine($"Fichas - Você: {jogador.Fichas}, Computador: {computador.Fichas}");
                Console.WriteLine($"Pote: {pote}\n");

                if (!RodadaApostas(jogador, computador, ref apostaAtual, ref pote))
                {
                    if (!PerguntarJogarNovamente(jogador, computador))
                        break;
                    else
                        continue;
                }

                // Flop
                Console.WriteLine("Aperte Enter para o flop...");
                Console.ReadLine();
                comunitarias.Add(baralho.Distribuir());
                comunitarias.Add(baralho.Distribuir());
                comunitarias.Add(baralho.Distribuir());
                MostrarComunitarias(jogador, computador, comunitarias);
                if (!RodadaApostas(jogador, computador, ref apostaAtual, ref pote))
                {
                    if (!PerguntarJogarNovamente(jogador, computador))
                        break;
                    else
                        continue;
                }

                // Turn
                Console.WriteLine("Aperte Enter para o turn...");
                Console.ReadLine();
                comunitarias.Add(baralho.Distribuir());
                MostrarComunitarias(jogador, computador, comunitarias);
                if (!RodadaApostas(jogador, computador, ref apostaAtual, ref pote))
                {
                    if (!PerguntarJogarNovamente(jogador, computador))
                        break;
                    else
                        continue;
                }

                // River
                Console.WriteLine("Aperte Enter para o river...");
                Console.ReadLine();
                comunitarias.Add(baralho.Distribuir());
                MostrarComunitarias(jogador, computador, comunitarias);
                if (!RodadaApostas(jogador, computador, ref apostaAtual, ref pote))
                {
                    if (!PerguntarJogarNovamente(jogador, computador))
                        break;
                    else
                        continue;
                }

                var valorJogador = AvaliadorDeMao.Avaliar(jogador.Mao.Concat(comunitarias));
                var valorComputador = AvaliadorDeMao.Avaliar(computador.Mao.Concat(comunitarias));

                Console.WriteLine($"Cartas do computador: {computador.Mao[0]}, {computador.Mao[1]}");
                Console.WriteLine($"Sua melhor mão: {valorJogador}");
                Console.WriteLine($"Melhor mão do computador: {valorComputador}");

                var comparacao = valorJogador.CompareTo(valorComputador);
                if (comparacao > 0)
                {
                    Console.WriteLine("Você venceu!");
                    jogador.Fichas += pote;
                }
                else if (comparacao < 0)
                {
                    Console.WriteLine("Computador venceu!");
                    computador.Fichas += pote;
                }
                else
                {
                    Console.WriteLine("Empate!");
                    jogador.Fichas += pote / 2;
                    computador.Fichas += pote - pote / 2;
                }

                if (!PerguntarJogarNovamente(jogador, computador))
                    break;
            }
        }

        static bool RodadaApostas(Jogador jogador, Jogador computador, ref int apostaAtual, ref int pote)
        {
            while (true)
            {
                Console.WriteLine($"Aposta atual: {apostaAtual}. Pote: {pote}");

                string? entrada;
                while (true)
                {
                    Console.WriteLine("Você quer (d)obrar, (c)obrir ou (f)old?");
                    entrada = Console.ReadLine()?.Trim().ToLower();
                    if (entrada == "d" || entrada == "c" || entrada == "f")
                        break;
                    Console.WriteLine("Opção inválida. Digite 'd', 'c' ou 'f'.");
                }

                if (entrada == "f")
                {
                    computador.Fichas += pote;
                    Console.WriteLine("Você desistiu. Computador vence o pote.");
                    Console.WriteLine($"Suas cartas: {jogador.Mao[0]}, {jogador.Mao[1]}");
                    Console.WriteLine($"Cartas do computador: {computador.Mao[0]}, {computador.Mao[1]}");
                    return false;
                }

                if (entrada == "d")
                {
                    int novaAposta = Math.Min(apostaAtual * 2, jogador.ApostaAtual + jogador.Fichas);
                    int diferenca = novaAposta - jogador.ApostaAtual;
                    if (diferenca == 0)
                    {
                        Console.WriteLine("Você não tem fichas para dobrar.");
                        entrada = "c";
                    }
                    else
                    {
                        jogador.FazerAposta(diferenca);
                        pote += diferenca;
                        apostaAtual = novaAposta;
                        Console.WriteLine($"Você dobrou para {apostaAtual}");

                        int acao = aleatorio.Next(3); // 0 dobra, 1 desiste, 2 cobre
                        if (acao == 1)
                        {
                            Console.WriteLine("Computador desiste. Você vence o pote.");
                            Console.WriteLine($"Suas cartas: {jogador.Mao[0]}, {jogador.Mao[1]}");
                            Console.WriteLine($"Cartas do computador: {computador.Mao[0]}, {computador.Mao[1]}");
                            jogador.Fichas += pote;
                            return false;
                        }
                        if (acao == 0)
                        {
                            novaAposta = Math.Min(apostaAtual * 2, computador.ApostaAtual + computador.Fichas);
                            diferenca = novaAposta - computador.ApostaAtual;
                            if (diferenca == 0)
                            {
                                diferenca = apostaAtual - computador.ApostaAtual;
                                if (diferenca > computador.Fichas)
                                {
                                    diferenca = computador.Fichas;
                                    apostaAtual = computador.ApostaAtual + diferenca;
                                }
                                computador.FazerAposta(diferenca);
                                pote += diferenca;
                                Console.WriteLine("Computador cobre.");
                                return true;
                            }
                            computador.FazerAposta(diferenca);
                            pote += diferenca;
                            apostaAtual = novaAposta;
                            Console.WriteLine($"Computador dobra para {apostaAtual}");
                            continue;
                        }
                        else
                        {
                            diferenca = apostaAtual - computador.ApostaAtual;
                            if (diferenca > computador.Fichas)
                            {
                                diferenca = computador.Fichas;
                                apostaAtual = computador.ApostaAtual + diferenca;
                            }
                            computador.FazerAposta(diferenca);
                            pote += diferenca;
                            Console.WriteLine("Computador cobre.");
                            return true;
                        }
                    }
                }

                if (entrada == "c")
                {
                    int diferenca = apostaAtual - jogador.ApostaAtual;
                    if (diferenca > jogador.Fichas)
                    {
                        diferenca = jogador.Fichas;
                        apostaAtual = jogador.ApostaAtual + diferenca;
                    }
                    jogador.FazerAposta(diferenca);
                    pote += diferenca;

                    int acao = aleatorio.Next(2); // 0 dobra, 1 cobre
                    if (acao == 0)
                    {
                        int novaAposta = Math.Min(apostaAtual * 2, computador.ApostaAtual + computador.Fichas);
                        diferenca = novaAposta - computador.ApostaAtual;
                        if (diferenca == 0)
                        {
                            Console.WriteLine("Computador tenta dobrar mas não tem fichas.");
                            diferenca = apostaAtual - computador.ApostaAtual;
                            if (diferenca > computador.Fichas)
                            {
                                diferenca = computador.Fichas;
                                apostaAtual = computador.ApostaAtual + diferenca;
                            }
                            computador.FazerAposta(diferenca);
                            pote += diferenca;
                            Console.WriteLine("Computador cobre.");
                            return true;
                        }
                        computador.FazerAposta(diferenca);
                        pote += diferenca;
                        apostaAtual = novaAposta;
                        Console.WriteLine($"Computador dobra para {apostaAtual}");
                        continue;
                    }
                    else
                    {
                        diferenca = apostaAtual - computador.ApostaAtual;
                        if (diferenca > computador.Fichas)
                        {
                            diferenca = computador.Fichas;
                            apostaAtual = computador.ApostaAtual + diferenca;
                        }
                        computador.FazerAposta(diferenca);
                        pote += diferenca;
                        Console.WriteLine("Computador cobre.");
                        return true;
                    }
                }
            }
        }

        static bool PerguntarJogarNovamente(Jogador jogador, Jogador computador)
        {
            Console.WriteLine($"Fichas - Você: {jogador.Fichas}, Computador: {computador.Fichas}");
            if (jogador.Fichas <= 0 || computador.Fichas <= 0) 
            {
                Console.WriteLine("Acabou!");
                Console.ReadKey();
                return false;
            }
            string? entrada;
            do
            {
                Console.WriteLine("Jogar novamente? (s/n)");
                entrada = Console.ReadLine()?.Trim().ToLower();
                if (entrada != "s" && entrada != "n")
                    Console.WriteLine("Opção inválida. Digite 's' para sim ou 'n' para não.");
            } while (entrada != "s" && entrada != "n");
            Console.WriteLine();
            return entrada == "s";
        }

        static void MostrarComunitarias(Jogador jogador, Jogador computador, List<Carta> comunitarias)
        {
            Console.WriteLine($"Suas cartas: {jogador.Mao[0]}, {jogador.Mao[1]}");
            Console.WriteLine("Cartas comunitárias: " + string.Join(", ", comunitarias));
        }
    }
}
