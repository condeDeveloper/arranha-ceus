using System.Numerics;

namespace Conde.ArranhaCeus;

/// <summary>Uma linha da tabela de unicidade.</summary>
public sealed record LinhaUnicas(int N, long Grades, long ConjuntosDePistas, long Unicas, int MaiorRepeticao);

/// <summary>As tabelas do README, cada uma calculada uma vez e guardada.</summary>
public static class Medidas
{
    private static readonly Dictionary<int, List<int[]>> CacheLatinos = new();
    private static readonly Dictionary<int, Dictionary<string, int>> CacheAssinaturas = new();

    public static List<int[]> Latinos(int n)
    {
        lock (CacheLatinos)
        {
            if (!CacheLatinos.TryGetValue(n, out var l))
                CacheLatinos[n] = l = Grade.Latinos(n);
            return l;
        }
    }

    public static string Chave(int[] pistas) => string.Concat(pistas.Select(p => (char)('0' + p)));

    /// <summary>Quantas grades de ordem n tem cada conjunto completo de pistas.</summary>
    public static Dictionary<string, int> Assinaturas(int n)
    {
        lock (CacheAssinaturas)
        {
            if (CacheAssinaturas.TryGetValue(n, out var d)) return d;
            d = new Dictionary<string, int>();
            foreach (var g in Latinos(n))
            {
                string k = Chave(Grade.Pistas(g, n));
                d[k] = d.GetValueOrDefault(k) + 1;
            }
            CacheAssinaturas[n] = d;
            return d;
        }
    }

    // ---- Stirling ----

    /// <summary>
    /// Quantas das n! fileiras deixam ver exatamente k predios da ponta,
    /// contando fileira por fileira.
    /// </summary>
    public static long[] VisiveisContados(int n)
    {
        var h = new long[n + 1];
        var p = Enumerable.Range(1, n).ToArray();
        do h[Grade.Visiveis(p)]++;
        while (Proxima(p));
        return h;
    }

    /// <summary>Os numeros de Stirling sem sinal de primeira especie, pela recorrencia.</summary>
    public static long[] Stirling(int n)
    {
        var c = new long[n + 1, n + 1];
        c[0, 0] = 1;
        for (int i = 1; i <= n; i++)
            for (int k = 1; k <= i; k++)
                c[i, k] = c[i - 1, k - 1] + (i - 1) * c[i - 1, k];
        return Enumerable.Range(0, n + 1).Select(k => c[n, k]).ToArray();
    }

    /// <summary>
    /// Para cada par (pista da esquerda, pista da direita) da mesma fileira,
    /// quantas fileiras de tamanho n o produzem.
    /// </summary>
    public static long[,] Pares(int n)
    {
        var m = new long[n + 1, n + 1];
        var p = Enumerable.Range(1, n).ToArray();
        do m[Grade.Visiveis(p), Grade.Visiveis(p.Reverse())]++;
        while (Proxima(p));
        return m;
    }

    public static bool Proxima(int[] p)
    {
        int i = p.Length - 2;
        while (i >= 0 && p[i] >= p[i + 1]) i--;
        if (i < 0) return false;
        int j = p.Length - 1;
        while (p[j] <= p[i]) j--;
        (p[i], p[j]) = (p[j], p[i]);
        Array.Reverse(p, i + 1, p.Length - i - 1);
        return true;
    }

    // ---- unicidade ----

    public static LinhaUnicas Unicas(int n)
    {
        var d = Assinaturas(n);
        return new LinhaUnicas(n, Latinos(n).Count, d.Count, d.Values.Count(v => v == 1), d.Values.Max());
    }

    // ---- o menor numero de pistas ----

    private static int[]? _minimo4;

    /// <summary>
    /// Para cada grade 4 por 4 que as 16 pistas determinam, o menor numero de
    /// pistas (escolhidas entre as 16 dela) que ainda deixa so ela. -1 para as
    /// que nem as 16 determinam.
    /// </summary>
    public static int[] Minimo4()
    {
        if (_minimo4 != null) return _minimo4;
        const int n = 4, np = 16;
        var gs = Latinos(n);
        var ps = gs.Select(g => Grade.Pistas(g, n)).ToArray();
        var porTamanho = Enumerable.Range(0, 1 << np).OrderBy(s => BitOperations.PopCount((uint)s)).ToArray();
        var r = new int[gs.Count];
        for (int x = 0; x < gs.Count; x++)
        {
            // D: as posicoes em que cada outra grade DIFERE de x; S precisa tocar todas
            var difere = new HashSet<int>();
            bool determinada = true;
            for (int y = 0; y < gs.Count && determinada; y++)
            {
                if (y == x) continue;
                int m = 0;
                for (int i = 0; i < np; i++)
                    if (ps[x][i] != ps[y][i]) m |= 1 << i;
                if (m == 0) determinada = false;
                difere.Add(m);
            }
            if (!determinada) { r[x] = -1; continue; }
            // so os minimos por inclusao importam
            var ds = difere.Where(a => !difere.Any(b => b != a && (b & a) == b)).ToArray();
            r[x] = -1;
            foreach (int s in porTamanho)
            {
                bool toca = true;
                foreach (int dm in ds)
                    if ((dm & s) == 0) { toca = false; break; }
                if (toca) { r[x] = BitOperations.PopCount((uint)s); break; }
            }
        }
        return _minimo4 = r;
    }

    // ---- o solucionador contra a forca bruta ----

    /// <summary>
    /// Para cada conjunto completo de pistas de ordem n: o solucionador conta
    /// as grades sem listar nada, e a forca bruta ja sabe quantas sao. Devolve
    /// quantos conjuntos foram conferidos e quantos divergiram.
    /// </summary>
    public static (int Conferidos, int Divergencias) Juiz(int n)
    {
        int conf = 0, div = 0;
        foreach (var (k, v) in Assinaturas(n))
        {
            var pistas = k.Select(c => c - '0').ToArray();
            if (Solucionador.Contar(n, pistas) != v) div++;
            conf++;
        }
        return (conf, div);
    }
}
