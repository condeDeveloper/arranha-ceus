using System.Diagnostics;
using Conde.ArranhaCeus;

if (args.Length == 0)
{
    Console.WriteLine("medidor <stirling|unicas|minimo|juiz|desenho|tudo>");
    return 1;
}

switch (args[0])
{
    case "stirling": Stirling(); break;
    case "unicas": Unicas(); break;
    case "minimo": Minimo(); break;
    case "juiz": JuizCmd(); break;
    case "desenho": Desenho(); break;
    case "tudo": Stirling(); Unicas(); Minimo(); JuizCmd(); Desenho(); break;
    default:
        Console.WriteLine($"nao conheco '{args[0]}'");
        return 1;
}
return 0;

void Stirling()
{
    Console.WriteLine("fileiras de n predios por quantos se veem da ponta: contadas e pela recorrencia de Stirling");
    for (int n = 1; n <= 8; n++)
    {
        var contados = Medidas.VisiveisContados(n);
        var formula = Medidas.Stirling(n);
        Console.WriteLine($"n = {n}: {string.Join(" ", contados.Skip(1))}   {(contados.SequenceEqual(formula) ? "confere" : "NAO CONFERE")}");
    }
    Console.WriteLine();
    var m = Medidas.Pares(5);
    Console.WriteLine("n = 5: fileiras por (pista da esquerda, pista da direita)");
    Console.WriteLine("esq\\dir    1     2     3     4     5");
    for (int e = 1; e <= 5; e++)
        Console.WriteLine($"   {e}   " + string.Join(" ", Enumerable.Range(1, 5).Select(d => $"{m[e, d],5}")));
    Console.WriteLine();
}

void Unicas()
{
    Console.WriteLine("quantas grades as 4n pistas completas determinam sozinhas");
    Console.WriteLine("n   grades     conjuntos de pistas   determinadas   maior repeticao");
    for (int n = 2; n <= 5; n++)
    {
        var l = Medidas.Unicas(n);
        Console.WriteLine($"{n}   {l.Grades,-10} {l.ConjuntosDePistas,-21} {l.Unicas,-14} {l.MaiorRepeticao}");
    }
    Console.WriteLine();
}

void Minimo()
{
    var r = Medidas.Minimo4();
    Console.WriteLine("4 por 4: o menor numero de pistas que deixa so aquela grade");
    Console.WriteLine("pistas   grades");
    foreach (var g in r.GroupBy(x => x).OrderBy(g => g.Key))
        Console.WriteLine($"{(g.Key < 0 ? "nem 16" : g.Key.ToString()),-8} {g.Count()}");
    Console.WriteLine();
}

void JuizCmd()
{
    Console.WriteLine("o solucionador contra a forca bruta, em todo conjunto completo de pistas");
    for (int n = 2; n <= 5; n++)
    {
        var sw = Stopwatch.StartNew();
        var (c, d) = Medidas.Juiz(n);
        Console.WriteLine($"n = {n}: {c} conjuntos conferidos, {d} divergencias ({sw.ElapsedMilliseconds} ms)");
    }
    Console.WriteLine();
}

void Desenho()
{
    const int n = 5;
    var d = Medidas.Assinaturas(n);
    var g = Medidas.Latinos(n).First(x => d[Medidas.Chave(Grade.Pistas(x, n))] == 1);
    Console.WriteLine("a primeira grade 5 por 5 (em ordem) que as 20 pistas determinam:");
    foreach (var linha in Grade.Desenho(g, n, Grade.Pistas(g, n)))
        Console.WriteLine("   " + linha);
    Console.WriteLine();
}
