using Conde.ArranhaCeus;
using Xunit;

namespace Conde.ArranhaCeus.Testes;

/// <summary>O solucionador contra a forca bruta sobre os quadrados latinos.</summary>
public class SolucionadorTestes
{
    private static long ForcaBruta(int n, int[] pistas) =>
        Medidas.Latinos(n).Count(g => Grade.Satisfaz(g, n, pistas));

    [Fact]
    public void Sem_pista_nenhuma_conta_os_latinos()
    {
        Assert.Equal(576, Solucionador.Contar(4, new int[16]));
        Assert.Equal(12, Solucionador.Contar(3, new int[12]));
    }

    [Fact]
    public void Todo_conjunto_completo_de_pistas_de_ordem_4()
    {
        var (c, d) = Medidas.Juiz(4);
        Assert.Equal(438, c);
        Assert.Equal(0, d);
    }

    [Fact]
    public void Pistas_parciais_sorteadas_de_ordem_4()
    {
        // metade das pistas de cada grade, escolhida por um sorteio de semente fixa
        var rng = new Random(2211);
        var gs = Medidas.Latinos(4);
        for (int t = 0; t < 300; t++)
        {
            var g = gs[rng.Next(gs.Count)];
            var p = Grade.Pistas(g, 4);
            for (int i = 0; i < p.Length; i++)
                if (rng.Next(2) == 0) p[i] = 0;
            Assert.Equal(ForcaBruta(4, p), Solucionador.Contar(4, p));
        }
    }

    [Fact]
    public void Pistas_impossiveis_dao_zero()
    {
        var p = new int[16];
        p[3 * 4 + 0] = 1; // da esquerda so se ve 1: o 4 esta na ponta
        p[4 + 0] = 1;     // da direita tambem: o 4 estaria nas duas pontas
        Assert.Equal(0, Solucionador.Contar(4, p));
        Assert.Equal(0, ForcaBruta(4, p));
    }

    [Fact]
    public void O_limite_para_a_contagem()
    {
        Assert.Equal(5, Solucionador.Contar(4, new int[16], limite: 5));
    }
}
