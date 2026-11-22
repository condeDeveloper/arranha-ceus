namespace Conde.ArranhaCeus;

/// <summary>
/// O solucionador: recebe as pistas (0 = sem pista) e conta as grades que as
/// satisfazem, casa por casa, sem nunca listar os quadrados latinos.
///
/// Ele acompanha, para a linha corrente olhando da esquerda e para cada
/// coluna olhando de cima, o predio mais alto ate ali e quantos ja se veem, e
/// corta o ramo em tres casos: ja se ve mais do que a pista; o predio de
/// altura n ja apareceu (nada mais vai aparecer) e a conta nao fechou; ou as
/// casas que faltam nao bastam para chegar na pista. A direita e a de baixo
/// sao conferidas quando a linha ou a coluna fecha.
/// </summary>
public static class Solucionador
{
    public static long Contar(int n, int[] pistas, int limite = int.MaxValue)
    {
        if (pistas.Length != 4 * n) throw new ArgumentException("sao 4n pistas", nameof(pistas));
        var g = new int[n * n];
        var usadoLinha = new bool[n, n + 1];
        var usadoColuna = new bool[n, n + 1];
        var maiorCol = new int[n];
        var vistosCol = new int[n];
        long total = 0;
        Preenche(0, 0, 0);
        return total;

        // maior e vistos: o predio mais alto da linha ate aqui e quantos se veem da esquerda
        void Preenche(int k, int maior, int vistos)
        {
            if (total >= limite) return;
            if (k == n * n) { total++; return; }
            int lin = k / n, col = k % n;
            if (col == 0) { maior = 0; vistos = 0; }
            int pEsq = pistas[3 * n + lin], pCima = pistas[col];
            int mc = maiorCol[col], vc = vistosCol[col];
            for (int h = 1; h <= n; h++)
            {
                if (usadoLinha[lin, h] || usadoColuna[col, h]) continue;
                int m2 = Math.Max(maior, h), v2 = vistos + (h > maior ? 1 : 0);
                if (!Possivel(pEsq, m2, v2, n - 1 - col)) continue;
                int mc2 = Math.Max(mc, h), vc2 = vc + (h > mc ? 1 : 0);
                if (!Possivel(pCima, mc2, vc2, n - 1 - lin)) continue;
                g[k] = h;
                usadoLinha[lin, h] = usadoColuna[col, h] = true;
                maiorCol[col] = mc2; vistosCol[col] = vc2;
                if (Fecha(lin, col))
                    Preenche(k + 1, m2, v2);
                maiorCol[col] = mc; vistosCol[col] = vc;
                usadoLinha[lin, h] = usadoColuna[col, h] = false;
            }
            g[k] = 0;
        }

        // ainda da para a fileira terminar mostrando exatamente 'pista' predios?
        bool Possivel(int pista, int maior, int vistos, int faltam)
        {
            if (pista == 0) return true;
            if (vistos > pista) return false;
            if (maior == n) return vistos == pista;
            return vistos + faltam >= pista;
        }

        bool Fecha(int lin, int col)
        {
            if (col == n - 1)
            {
                if (!Bate(pistas[3 * n + lin], lin * n, 1)) return false;
                if (!Bate(pistas[n + lin], lin * n + n - 1, -1)) return false;
            }
            if (lin == n - 1)
            {
                if (!Bate(pistas[col], col, n)) return false;
                if (!Bate(pistas[2 * n + col], (n - 1) * n + col, -n)) return false;
            }
            return true;
        }

        // n casas a partir de 'inicio', andando 'passo' de cada vez
        bool Bate(int pista, int inicio, int passo)
        {
            if (pista == 0) return true;
            int maior = 0, vistos = 0;
            for (int i = 0, k = inicio; i < n; i++, k += passo)
                if (g[k] > maior) { maior = g[k]; vistos++; }
            return vistos == pista;
        }
    }
}
