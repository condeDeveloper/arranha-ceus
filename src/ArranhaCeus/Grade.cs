namespace Conde.ArranhaCeus;

/// <summary>
/// O quebra-cabeca dos arranha-ceus: uma grade n por n com predios de altura
/// 1 a n, cada altura uma vez por linha e uma vez por coluna (um quadrado
/// latino). Em volta, as pistas: quantos predios se veem olhando daquela
/// ponta, sabendo que o mais alto esconde os mais baixos que estao atras.
///
/// A grade e guardada linha por linha num vetor de n*n alturas.
/// As 4n pistas seguem a ordem: de cima (coluna 0 a n-1), da direita (linha 0
/// a n-1), de baixo (coluna 0 a n-1) e da esquerda (linha 0 a n-1).
/// </summary>
public static class Grade
{
    /// <summary>Quantos predios se veem do comeco da sequencia.</summary>
    public static int Visiveis(IEnumerable<int> alturas)
    {
        int maior = 0, vistos = 0;
        foreach (int h in alturas)
            if (h > maior) { maior = h; vistos++; }
        return vistos;
    }

    public static IEnumerable<int> Linha(int[] g, int n, int r) => Enumerable.Range(0, n).Select(c => g[r * n + c]);
    public static IEnumerable<int> Coluna(int[] g, int n, int c) => Enumerable.Range(0, n).Select(r => g[r * n + c]);

    /// <summary>As 4n pistas completas da grade.</summary>
    public static int[] Pistas(int[] g, int n)
    {
        var p = new int[4 * n];
        for (int i = 0; i < n; i++)
        {
            p[i] = Visiveis(Coluna(g, n, i));                   // de cima
            p[n + i] = Visiveis(Linha(g, n, i).Reverse());      // da direita
            p[2 * n + i] = Visiveis(Coluna(g, n, i).Reverse()); // de baixo
            p[3 * n + i] = Visiveis(Linha(g, n, i));            // da esquerda
        }
        return p;
    }

    /// <summary>A definicao de quadrado latino, linha por linha e coluna por coluna.</summary>
    public static bool Latino(int[] g, int n)
    {
        if (g.Length != n * n) return false;
        for (int i = 0; i < n; i++)
        {
            if (!Linha(g, n, i).OrderBy(x => x).SequenceEqual(Enumerable.Range(1, n))) return false;
            if (!Coluna(g, n, i).OrderBy(x => x).SequenceEqual(Enumerable.Range(1, n))) return false;
        }
        return true;
    }

    /// <summary>A grade satisfaz as pistas dadas? Pista 0 quer dizer "sem pista".</summary>
    public static bool Satisfaz(int[] g, int n, int[] pistas)
    {
        var p = Pistas(g, n);
        for (int i = 0; i < p.Length; i++)
            if (pistas[i] != 0 && pistas[i] != p[i])
                return false;
        return true;
    }

    /// <summary>Todos os quadrados latinos de ordem n (n ate 5), por backtracking simples.</summary>
    public static List<int[]> Latinos(int n)
    {
        if (n < 1 || n > 5) throw new ArgumentOutOfRangeException(nameof(n), "ate a ordem 5");
        var r = new List<int[]>();
        var g = new int[n * n];
        Preenche(0);
        return r;

        void Preenche(int k)
        {
            if (k == n * n) { r.Add((int[])g.Clone()); return; }
            int lin = k / n, col = k % n;
            for (int h = 1; h <= n; h++)
            {
                bool ok = true;
                for (int c = 0; c < col && ok; c++) if (g[lin * n + c] == h) ok = false;
                for (int l = 0; l < lin && ok; l++) if (g[l * n + col] == h) ok = false;
                if (!ok) continue;
                g[k] = h;
                Preenche(k + 1);
            }
            g[k] = 0;
        }
    }

    /// <summary>A grade com as pistas em volta, para o README.</summary>
    public static List<string> Desenho(int[] g, int n, int[] p)
    {
        string Dig(int x) => x == 0 ? " " : x.ToString();
        var r = new List<string> { "    " + string.Join(" ", Enumerable.Range(0, n).Select(c => Dig(p[c]))) };
        for (int i = 0; i < n; i++)
            r.Add($"{Dig(p[3 * n + i])}   " + string.Join(" ", Linha(g, n, i)) + $"   {Dig(p[n + i])}");
        r.Add("    " + string.Join(" ", Enumerable.Range(0, n).Select(c => Dig(p[2 * n + c]))));
        return r;
    }
}
