using Conde.ArranhaCeus;
using Xunit;

namespace Conde.ArranhaCeus.Testes;

/// <summary>Todo numero do README esta preso aqui.</summary>
public class MedidasTestes
{
    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    public void Os_visiveis_sao_os_numeros_de_Stirling(int n)
    {
        Assert.Equal(Medidas.Stirling(n), Medidas.VisiveisContados(n));
    }

    [Fact]
    public void A_linha_do_cinco()
    {
        Assert.Equal(new long[] { 0, 24, 50, 35, 10, 1 }, Medidas.VisiveisContados(5));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    public void Esquerda_mais_direita_nunca_passa_de_n_mais_1(int n)
    {
        var m = Medidas.Pares(n);
        long noTeto = 0;
        for (int e = 1; e <= n; e++)
            for (int d = 1; d <= n; d++)
            {
                if (e + d > n + 1) Assert.Equal(0, m[e, d]);
                if (e + d == n + 1) noTeto += m[e, d];
            }
        Assert.Equal(1L << (n - 1), noTeto);
    }

    [Fact]
    public void No_teto_do_cinco_os_binomiais()
    {
        var m = Medidas.Pares(5);
        Assert.Equal(new long[] { 1, 4, 6, 4, 1 }, Enumerable.Range(1, 5).Select(e => m[e, 6 - e]).ToArray());
        Assert.Equal(0, m[1, 1]);
        Assert.Equal(22, m[2, 2]);
    }

    // n, grades, conjuntos de pistas, determinadas, maior repeticao
    [Theory]
    [InlineData(2, 2, 2, 2, 1)]
    [InlineData(3, 12, 12, 12, 1)]
    [InlineData(4, 576, 438, 372, 6)]
    [InlineData(5, 161280, 102398, 68368, 20)]
    public void Quantas_grades_as_pistas_completas_determinam(int n, long grades, long conjuntos, long unicas, int maior)
    {
        var l = Medidas.Unicas(n);
        Assert.Equal(grades, l.Grades);
        Assert.Equal(conjuntos, l.ConjuntosDePistas);
        Assert.Equal(unicas, l.Unicas);
        Assert.Equal(maior, l.MaiorRepeticao);
    }

    [Fact]
    public void O_menor_numero_de_pistas_no_4x4()
    {
        var r = Medidas.Minimo4();
        Assert.Equal(204, r.Count(x => x == -1));
        Assert.Equal(208, r.Count(x => x == 3));
        Assert.Equal(142, r.Count(x => x == 4));
        Assert.Equal(22, r.Count(x => x == 5));
        Assert.Equal(576, r.Length);
        Assert.DoesNotContain(r, x => x is 0 or 1 or 2 or > 5);
    }

    [Fact]
    public void O_minimo_vale_de_verdade_com_o_solucionador()
    {
        // para cada grade que 3 pistas decidem, achar UM trio que decide e conferir com o solucionador
        var gs = Medidas.Latinos(4);
        var r = Medidas.Minimo4();
        int conferidas = 0;
        for (int x = 0; x < gs.Count && conferidas < 20; x++)
        {
            if (r[x] != 3) continue;
            var p = Grade.Pistas(gs[x], 4);
            bool achou = false;
            for (int a = 0; a < 16 && !achou; a++)
                for (int b = a + 1; b < 16 && !achou; b++)
                    for (int c = b + 1; c < 16 && !achou; c++)
                    {
                        var q = new int[16];
                        q[a] = p[a]; q[b] = p[b]; q[c] = p[c];
                        if (Solucionador.Contar(4, q, limite: 2) == 1) achou = true;
                    }
            Assert.True(achou);
            conferidas++;
        }
        Assert.Equal(20, conferidas);
    }
}
