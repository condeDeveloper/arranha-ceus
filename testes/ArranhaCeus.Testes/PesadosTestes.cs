using Conde.ArranhaCeus;
using Xunit;

namespace Conde.ArranhaCeus.Testes;

/// <summary>
/// O solucionador contra a forca bruta nos 102.398 conjuntos completos de
/// pistas de ordem 5: uns 20 segundos. Roda uma vez so, no job proprio do CI.
/// </summary>
[Trait("Category", "Pesado")]
public class PesadosTestes
{
    [Fact]
    public void Todo_conjunto_completo_de_pistas_de_ordem_5()
    {
        var (c, d) = Medidas.Juiz(5);
        Assert.Equal(102398, c);
        Assert.Equal(0, d);
    }
}
