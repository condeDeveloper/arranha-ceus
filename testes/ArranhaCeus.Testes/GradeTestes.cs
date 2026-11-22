using Conde.ArranhaCeus;
using Xunit;

namespace Conde.ArranhaCeus.Testes;

/// <summary>A visibilidade, as pistas e a definicao, com casos escritos a mao.</summary>
public class GradeTestes
{
    [Theory]
    [InlineData(new[] { 1, 2, 3, 4 }, 4)]
    [InlineData(new[] { 4, 3, 2, 1 }, 1)]
    [InlineData(new[] { 2, 1, 4, 3 }, 2)]
    [InlineData(new[] { 3, 1, 2, 5, 4 }, 2)]
    public void Visiveis(int[] fileira, int vistos)
    {
        Assert.Equal(vistos, Grade.Visiveis(fileira));
    }

    [Fact]
    public void As_pistas_de_uma_grade_3x3()
    {
        // 1 2 3 / 2 3 1 / 3 1 2
        var g = new[] { 1, 2, 3, 2, 3, 1, 3, 1, 2 };
        // de cima: colunas 1 2 3, 2 3 1, 3 1 2 -> 3, 2, 1
        // da direita: linhas ao contrario 3 2 1, 1 3 2, 2 1 3 -> 1, 2, 2
        // de baixo: colunas ao contrario 3 2 1, 1 3 2, 2 1 3 -> 1, 2, 2
        // da esquerda: 1 2 3, 2 3 1, 3 1 2 -> 3, 2, 1
        Assert.Equal(new[] { 3, 2, 1, 1, 2, 2, 1, 2, 2, 3, 2, 1 }, Grade.Pistas(g, 3));
    }

    [Fact]
    public void Latino_e_o_que_nao_e()
    {
        Assert.True(Grade.Latino(new[] { 1, 2, 2, 1 }, 2));
        Assert.False(Grade.Latino(new[] { 1, 2, 1, 2 }, 2));
        Assert.False(Grade.Latino(new[] { 1, 2, 3 }, 2));
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 12)]
    [InlineData(4, 576)]
    public void A_geracao_dos_latinos(int n, int quantos)
    {
        var l = Medidas.Latinos(n);
        Assert.Equal(quantos, l.Count);
        Assert.All(l, g => Assert.True(Grade.Latino(g, n)));
        Assert.Equal(quantos, l.Select(g => string.Join(",", g)).Distinct().Count());
    }

    [Fact]
    public void Pista_zero_e_sem_pista()
    {
        var g = new[] { 1, 2, 2, 1 };
        Assert.True(Grade.Satisfaz(g, 2, new int[8]));
        var p = Grade.Pistas(g, 2);
        Assert.True(Grade.Satisfaz(g, 2, p));
        p[0] = p[0] == 1 ? 2 : 1;
        Assert.False(Grade.Satisfaz(g, 2, p));
    }
}
