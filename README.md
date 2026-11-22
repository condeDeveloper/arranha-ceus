# Arranha-ceus

O quebra-cabeca: uma grade n por n de predios com alturas de 1 a n, cada
altura uma vez por linha e uma vez por coluna. Em volta da grade, as pistas:
quantos predios se veem olhando daquela ponta, sabendo que um predio alto
esconde os mais baixos que estao atras dele.

```
       5 3 3 2 1
   5   1 2 3 4 5   1
   3   2 1 4 5 3   2
   3   3 4 5 1 2   2
   2   4 5 2 3 1   3
   1   5 3 1 2 4   2
       1 2 3 3 2
```

Este repositorio varre todas as grades ate 5 por 5 (161.280) e mede quanto as
pistas decidem: com todas elas, e com o menor numero possivel.

## Como rodar

```bash
dotnet test testes/ArranhaCeus.Testes/ArranhaCeus.Testes.csproj -c Release
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- stirling
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- unicas
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- minimo
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- juiz
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- desenho
```

## Os dois lados e os juizes

- **A forca bruta** gera todos os quadrados latinos de ordem n e calcula as
  4n pistas de cada um. Agrupando pelas pistas, ela sabe exatamente quantas
  grades cada conjunto de pistas admite.
- **O solucionador** recebe as pistas e conta as grades casa por casa, sem
  listar quadrado nenhum. Ele acompanha o predio mais alto e quantos ja se
  veem, na linha e na coluna, e corta o ramo quando ja se ve demais, quando o
  predio de altura n ja apareceu e a conta nao fechou, ou quando as casas que
  faltam nao bastam.
- **Stirling** julga a fileira sozinha: quantas das n! fileiras deixam ver k
  predios e o numero de Stirling sem sinal de primeira especie `c(n, k)`,
  pela recorrencia `c(n, k) = c(n-1, k-1) + (n-1) c(n-1, k)`.

```
o solucionador contra a forca bruta, em todo conjunto completo de pistas
n = 4: 438 conjuntos conferidos, 0 divergencias
n = 5: 102398 conjuntos conferidos, 0 divergencias
```

Com pistas parciais, 300 conjuntos sorteados de ordem 4 (metade das pistas
apagada em cada um) tambem batem com a forca bruta.

## O que isto mediu

**As 16 pistas completas decidem so 64,6% das grades 4 por 4, e as 20 pistas
decidem so 42,4% das 5 por 5.** No 4 por 4 sao 372 de 576; no 5 por 5,
68.368 de 161.280. O conjunto de pistas mais ambiguo do 5 por 5 serve para
20 grades diferentes.

```
n   grades     conjuntos de pistas   determinadas   maior repeticao
3   12         12                    12             1
4   576        438                   372            6
5   161280     102398                68368          20
```

**Tres pistas bastam para 208 grades 4 por 4, e nenhuma precisa de mais de
cinco.** Entre as 372 que as pistas completas decidem, o menor numero de
pistas que ainda deixa so aquela grade e 3 para 208, 4 para 142 e 5 para 22.
Nenhuma se decide com duas. As outras 204 nao se decidem nem com as 16.

**Esquerda mais direita nunca passa de n + 1, e as fileiras que batem no teto
sao 2^(n-1).** O predio mais alto se ve das duas pontas, entao a soma tem no
maximo uma unidade a mais que n. As que chegam la sobem ate o n e descem
depois; de 2 a 8 sao exatamente `2^(n-1)`. No 5 elas se repartem em 1, 4, 6, 4
e 1 pelo lugar do predio mais alto, e nenhuma fileira mostra so um predio das
duas pontas.

```
n = 5: fileiras por (pista da esquerda, pista da direita)
esq\dir    1     2     3     4     5
   1       0     6    11     6     1
   2       6    22    18     4     0
   3      11    18     6     0     0
   4       6     4     0     0     0
   5       1     0     0     0     0
```

**A distribuicao de predios visiveis e Stirling, fileira por fileira, ate 8
predios.** As 40.320 fileiras de 8 se repartem em 5.040, 13.068, 13.132, 6.769,
1.960, 322, 28 e 1 predios visiveis, a linha `c(8, k)` da recorrencia.

## O que as medidas me corrigiram

**A primeira versao do solucionador levava 137 segundos para conferir as
pistas de ordem 5.** Ele conferia as fileiras fechadas com consultas montadas
a cada casa e so olhava a pista da esquerda durante a linha. Com lacos diretos
caiu para 73 segundos; acompanhando tambem a coluna de cima e cortando quando
a pista ficou inalcancavel, para uns 20. As 102.398 contagens nao mudaram.

## Os arquivos

```
src/ArranhaCeus/Grade.cs          visibilidade, pistas, definicao e os latinos
src/ArranhaCeus/Solucionador.cs   conta as grades de umas pistas, casa por casa
src/ArranhaCeus/Medidas.cs        as tabelas deste README, guardadas
ferramentas/Medidor/              o executavel que imprime as tabelas e desenha
testes/                           36 testes; o pesado roda num job proprio
```

36 testes, 1 deles pesado.
