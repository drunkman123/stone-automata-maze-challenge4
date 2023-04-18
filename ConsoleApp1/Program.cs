using System.Diagnostics;
using System.Globalization;
using System.Text;


var watch = new Stopwatch();
watch.Start();

#region Global Variables
(int, int) Goal = (0, 0);
(int, int) Start = (0, 0);
bool DeadEnd = false;
bool Found = false;
int Foundid = 0;
int loopCounter = 0;
int newIndex = 0;
//Stack<PositionStep> stack = new Stack<PositionStep>(52000);

#endregion

#region Setup Data Set
byte[][] baseMatrice;
byte[][] extractMatrice = new byte[10][];
(int,int) longestCell = (0,0);

ReadFileToMatrix();
ReadAndInsertExtract();
//printMatrix2(extractMatrice, "extraída");

for (int i = 2301; i < 2311; i++)
{
    for (int j = 2301; j < 2311; j++)
    {
        //Console.Write(baseMatrice[i][j].ToString() + " ");

        baseMatrice[i][j] = extractMatrice[i - 2301][j-2301];

    }
    Console.WriteLine("\r");
}
int diagonal = (int)(Math.Sqrt(Math.Pow(Goal.Item1 + 1, 2) + Math.Pow(Goal.Item2 + 1, 2)) * 2);


int lines;
int columns;
int lengthTotal = lines * columns;
PositionStep?[] positionSteps = new PositionStep?[lengthTotal + 100];
List<int> toBeAddedIndex = new List<int>(lengthTotal + 100);
List<PositionStep> listRodada = new List<PositionStep>(lengthTotal / 4);
//stack.Push(new PositionStep(Start.Item1, Start.Item2, '\0', null));
listRodada.Add(new PositionStep(Start.Item1, Start.Item2, '\0', null, 1));
byte[][] tempMatrice = new byte[baseMatrice.Length][];
//Console.WriteLine("\n");

//for (int i = 2301; i < 2311; i++)
//{
//    for (int j = 2301; j < 2311; j++)
//    {
//        //baseMatrice[i][j] = extractMatrice[i - 2301][j - 2301];
//        Console.Write(baseMatrice[i][j].ToString() + " ");

//    }
//    Console.WriteLine("\r");
//}
for (int i = 0; i < baseMatrice.Length; i++)
{
    tempMatrice[i] = new byte[columns + 2];
}
baseMatrice[Start.Item1][Start.Item2] = 0;
baseMatrice[Goal.Item1][Goal.Item2] = 0;
#endregion
//printMatrix(baseMatrice, "1");
while (!Found && !DeadEnd)
{
    //PathTeste(); //descomente o método embaixo e escolha o tipo de input
    //start and goal set to 0 to not mess with the stepexecute
    StepExecute();
    //CheckChanges();

    baseMatrice[Start.Item1][Start.Item2] = 0;
    baseMatrice[Goal.Item1][Goal.Item2] = 0;
    //printMatrix(baseMatrice, "1");
    TryPath();
    //Console.WriteLine(loopCounter);
}
if (DeadEnd)
{
    Console.WriteLine("No Paths Available!");
}
else
{
    Console.WriteLine($"O número da célula mais longe foi {longestCell.Item1 - 1},{longestCell.Item2 - 1}");
    GetPathNew();
}
watch.Stop();
TimeSpan ts = watch.Elapsed;
Console.WriteLine($"\r\nExecution Time: {ts.TotalMinutes} minutes and {ts.TotalMilliseconds} ms");


//Methods

static void Swap(ref byte[][] m, ref byte[][] mr)
{
    var tmp = m;
    m = mr;
    mr = tmp;
}
void GetPathNew()
{
    Console.WriteLine("Steps:");

    Stack<char> sb = new Stack<char>();
    //while (stack.Count > 0)
    //{
    //PositionStep step = listRodada.Last();
    //PositionStep? currentStep = step;
    while (listRodada[0]?.Parent != null)
    {
        sb.Push(listRodada[0].Step);
        listRodada[0] = listRodada[0].Parent;
    }
    //break;
    //}

    var EndRoute = sb.ToArray();
    //Array.Reverse(EndRoute);
    Console.WriteLine(string.Join(' ', EndRoute));
    Console.WriteLine($"{loopCounter} Steps to found a solution\r\n");

}
void StepExecute()
{

    Parallel.For(1, (int)lines + 1, i =>
    {
        for (int j = 1; j <= columns; j++)
        {
            byte countGreen = (byte)(baseMatrice[i - 1][j - 1] + baseMatrice[i - 1][j] + baseMatrice[i - 1][j + 1] + baseMatrice[i][j - 1] + baseMatrice[i][j + 1] + baseMatrice[i + 1][j - 1] + baseMatrice[i + 1][j] + baseMatrice[i + 1][j + 1]);
            if (baseMatrice[i][j] == 0)
            {
                if (countGreen > 1 && countGreen < 5) tempMatrice[i][j] = 1;
                else { tempMatrice[i][j] = 0; }
            }
            else
            {
                if (countGreen <= 3 || countGreen >= 6) tempMatrice[i][j] = 0;
                else { tempMatrice[i][j] = 1; }
            }
        }
    });
    Swap(ref baseMatrice, ref tempMatrice);
}


/*void StepExecute()
{
    //tempMatrice = baseMatrice;
    Parallel.For(1, (int)lines + 1, i =>
    {
        for (int j = 1; j <= columns; j++)
        {
            if (baseMatrice[i][j] == 0) continue;
            Interlocked.Increment(ref tempMatrice[i - 1][j - 1]);
            Interlocked.Increment(ref tempMatrice[i - 1][j]);
            Interlocked.Increment(ref tempMatrice[i - 1][j + 1]);
            Interlocked.Increment(ref tempMatrice[i][j - 1]);
            Interlocked.Increment(ref tempMatrice[i][j + 1]);
            Interlocked.Increment(ref tempMatrice[i + 1][j - 1]);
            Interlocked.Increment(ref tempMatrice[i + 1][j]);
            Interlocked.Increment(ref tempMatrice[i + 1][j + 1]);
        }
    });
}
void CheckChanges()
{
    for (int i = 1; i <= lines; i++)
    {
        for (int j = 1; j <= columns; j++)
        {
            if (baseMatrice[i][j] == 0 && (tempMatrice[i][j] > 1 && tempMatrice[i][j] < 5))
            {
                baseMatrice[i][j]++;
                continue;
            }
            if (baseMatrice[i][j] == 1 && (tempMatrice[i][j] <= 3 || tempMatrice[i][j] >= 6)) 
                baseMatrice[i][j]--;
        }
    }
    for (int i = 0; i < tempMatrice.Length; i++)
    {
        Array.Clear(tempMatrice[i], 0, tempMatrice[i].Length);
    }
}*/
void TryPath()
{

    //while (stack.Count > 0)
    //{
    //    PositionStep loopStack = stack.Pop();
    //    if (baseMatrice[loopStack.item1][loopStack.item2 - 1] == 0) listRodada.Add(new PositionStep(loopStack.item1, loopStack.item2 - 1, 'L', loopStack));

    //    if (baseMatrice[loopStack.item1 - 1][loopStack.item2] == 0) listRodada.Add(new PositionStep(loopStack.item1 - 1, loopStack.item2, 'U', loopStack));

    //    if (baseMatrice[loopStack.item1][loopStack.item2 + 1] == 0) listRodada.Add(new PositionStep(loopStack.item1, loopStack.item2 + 1, 'R', loopStack));

    //    if (baseMatrice[loopStack.item1 + 1][loopStack.item2] == 0) listRodada.Add(new PositionStep(loopStack.item1+1, loopStack.item2, 'D', loopStack));
    //}
    //foreach (var i in listRodada)
    //{
    //    if (i.item1 == Goal.Item1)
    //    {
    //        if (i.item2 == Goal.Item2)
    //        {
    //            stack.Push(i);
    //            Found = true;
    //            break;
    //        }
    //    }
    //    stack.Push(i);
    //};

    ////DeadEnd = listRodada.Count == 0;
    //listRodada.Clear();
    //loopCounter++;
    if(listRodada.Count == 0)
    {
        Found = true;
        return;
    }
    foreach (var i in listRodada)
    {
        if (i.item1 > longestCell.Item1)
        {
            if (i.item2 > longestCell.Item2)
            {
                longestCell.Item1 = i.item1;
                longestCell.Item2 = i.item2;
                //Console.WriteLine(loopCounter);
            }
        }
        if (i.item1 == 1 && i.item2 == 1 && !Found)
        {
            checkRight(i);
            checkDown(i);
            continue;
        }
        if (i.item1 == 1 && i.item2 != 1 && i.item2 != columns && !Found)
        {
            checkRight(i);
            checkDown(i);
            checkLeft(i);
            continue;
        }
        if (i.item1 == 1 && i.item2 == columns && !Found)
        {
            checkLeft(i);
            checkDown(i);
            continue;
        }
        if (i.item1 != 1 && i.item1 != lines && i.item2 == 1 && !Found)
        {
            checkRight(i);
            checkDown(i);
            checkUp(i);
            continue;
        }
        if (i.item1 != 1 && i.item1 != lines && i.item2 != 1 && i.item2 != columns && !Found)
        {
            checkRight(i);
            checkDown(i);
            checkUp(i);
            checkLeft(i);
            continue;
        }
        if (i.item1 != 1 && i.item1 != lines && i.item2 == columns && !Found)
        {
            checkUp(i);
            checkLeft(i);
            checkLastDown(i);
            continue;
        }
        if (i.item1 == lines && i.item2 == 1 && !Found)
        {
            checkUp(i);
            checkRight(i);
            continue;
        }
        if (i.item1 == lines && i.item2 != 1 && i.item2 != columns && !Found)
        {
            checkLeft(i);
            checkUp(i);
            checkLastRight(i);
            continue;
        }

        if (Found)
        {
            break;
        }

    }
    listRodada.Clear();
    //toBeAddedIndex = toBeAddedIndex.Distinct().ToList();

    foreach (var index in toBeAddedIndex)
    {
        var element = positionSteps[index];
        if (element != null)
        {
            listRodada.Add(element);
            positionSteps[index] = null;
        }
    }
    if (listRodada.Count > diagonal)
    {
        var itens = listRodada.OrderByDescending(c => c.item1 + c.item2).Skip(diagonal).ToArray();
        foreach (var item in itens)
            listRodada.Remove(item);
    }
    toBeAddedIndex.Clear();
    //DeadEnd = listRodada.Count == 0;
    loopCounter++;
}



void checkUp(PositionStep i)
{
    if (baseMatrice[i.item1 - 1][i.item2] == 0)
    {
        newIndex = (i.item1 - 1) * lines + i.item2 - lines;
        positionSteps[newIndex] = new PositionStep(i.item1 - 1, i.item2, 'U', i, newIndex);
        toBeAddedIndex.Add(newIndex);
    }

}
void checkRight(PositionStep i)
{
    if (baseMatrice[i.item1][i.item2 + 1] == 0)
    {
        newIndex = i.item1 * lines + i.item2 + 1 - lines;
        positionSteps[newIndex] = new PositionStep(i.item1, i.item2 + 1, 'R', i, newIndex);
        toBeAddedIndex.Add(newIndex);

    }

}
void checkLastRight(PositionStep i)
{
    if (baseMatrice[i.item1][i.item2 + 1] == 0)
    {
        if ((i.item1, i.item2 + 1) == Goal)
        {
            newIndex = i.item1 * lines + i.item2 + 1 - lines;
            positionSteps[newIndex] = new PositionStep(i.item1, i.item2 + 1, 'R', i, newIndex);
            toBeAddedIndex.Clear();
            toBeAddedIndex.Add(newIndex);
            Foundid = newIndex;
            Found = true;
        }
        else
        {
            newIndex = i.item1 * lines + i.item2 + 1 - lines;
            positionSteps[newIndex] = new PositionStep(i.item1, i.item2 + 1, 'R', i, newIndex);
            toBeAddedIndex.Add(newIndex);
        }
    }

}
void checkDown(PositionStep i)
{
    if (baseMatrice[i.item1 + 1][i.item2] == 0)
    {
        newIndex = (i.item1 + 1) * lines + i.item2 - lines;
        positionSteps[newIndex] = new PositionStep(i.item1 + 1, i.item2, 'D', i, newIndex);
        toBeAddedIndex.Add(newIndex);

    }
}
void checkLastDown(PositionStep i)
{
    if (baseMatrice[i.item1 + 1][i.item2] == 0)
    {
        if ((i.item1 + 1, i.item2) == Goal)
        {
            newIndex = (i.item1 + 1) * lines + i.item2 - lines;
            positionSteps[newIndex] = new PositionStep(i.item1 + 1, i.item2, 'D', i, newIndex);
            toBeAddedIndex.Clear();
            toBeAddedIndex.Add(newIndex);
            Foundid = newIndex;
            Found = true;
        }
        else
        {
            newIndex = (i.item1 + 1) * lines + i.item2 - lines;
            positionSteps[newIndex] = new PositionStep(i.item1 + 1, i.item2, 'D', i, newIndex);
            toBeAddedIndex.Add(newIndex);
        }
    }
}
void checkLeft(PositionStep i)
{
    if (baseMatrice[i.item1][i.item2 - 1] == 0)
    {
        newIndex = i.item1 * lines + i.item2 - 1 - lines;
        positionSteps[newIndex] = new PositionStep(i.item1, i.item2 - 1, 'L', i, newIndex);
        toBeAddedIndex.Add(newIndex);
    }


}
void printMatrix(byte[][] matrix, string name)
{
    Console.Write(name + "\n");

    for (int i = 0; i < lines + 2; i++)
    {
        for (int j = 0; j < columns + 2; j++)
        {

            Console.Write(matrix[i][j].ToString() + " ");
        }

        Console.Write("\n");

    }
    Console.Write("\n");

}
void printMatrix2(byte[][] matrix, string name)
{
    Console.Write(name + "\n");

    for (int i = 0; i < 10; i++)
    {
        for (int j = 0; j < 10; j++)
        {

            Console.Write(matrix[i][j].ToString() + " ");
        }

        Console.Write("\n");

    }
    Console.Write("\n");

}
/*void AddRoute(PositionStep parent, (int, int) lastPosition, char route)
{
    var newPosition = lastPosition;
    switch (route)
    {
        case 'D':
            newPosition.Item1 += 1;
            break;
        case 'R':
            newPosition.Item2 += 1;
            break;
        case 'L':
            newPosition.Item2 -= 1;
            break;
        case 'U':
            newPosition.Item1 -= 1;
            break;
    }
    listRodada.Add(new PositionStep { End = newPosition, Start = lastPosition, Step = route, Parent = parent });

    //countDeadEnd++;
    //Found = newPosition == Goal;
    //if (newPosition == Goal)
    //{
    //    Found = true;
    //    PositionStep lastElement = listRodada.Last();
    //    listRodada.Clear();
    //    stack.Clear();
    //    stack.Push(lastElement);
    //}
}*/
void ReadFileToMatrix()
{
    //using (FileStream fileStream = new FileStream("C:\\Users\\felip\\OneDrive\\Área de Trabalho\\input.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    //using (FileStream fileStream = new FileStream("C:\\Users\\41140878859\\Desktop\\projetos_git\\stone-automata-maze-challenge\\input.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    using (FileStream fileStream = new FileStream("E:\\Git pessoal\\stone-automata-maze-challenge4\\input4.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    {
        // Construct the input string
        StringBuilder inputBuilder = new StringBuilder();
        byte[] buffer = new byte[1024];
        int bytesRead;
        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            string chunk = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            inputBuilder.Append(chunk);
        }
        string input = inputBuilder.ToString();

        // Get matrix dimensions from input contents
        string[] FileLines = input.Trim().Split('\n');
        columns = FileLines[0].Split(' ').Length;
        lines = FileLines.Length;

        // Create new jagged array with border
        baseMatrice = new byte[lines + 2][];
        for (int i = 0; i < baseMatrice.Length; i++)
        {
            baseMatrice[i] = new byte[columns + 2];
        }

        // Read and parse lines
        for (int row = 1; row <= lines; row++)
        {
            string[] values = FileLines[row - 1].Trim().Split(' ');
            for (int col = 1; col <= columns; col++)
            {
                if (byte.TryParse(values[col - 1], NumberStyles.None, CultureInfo.InvariantCulture, out byte value))
                {
                    baseMatrice[row][col] = value;
                    if (value == 3) Start = (row, col);
                    if (value == 4) Goal = (row, col);
                }
            }
        }
        //for (int j = 0; j <= columns + 1; j++)
        //{
        //    baseMatrice[0][j] = 1; // top border
        //    baseMatrice[lines + 1][j] = 1; // bottom border
        //}
        //for (int i = 1; i <= lines + 1; i++)
        //{
        //    baseMatrice[i][0] = 1; // left border
        //    baseMatrice[i][columns + 1] = 1; // right border
        //}
    }
}
void ReadAndInsertExtract()
{
    //using (FileStream fileStream = new FileStream("C:\\Users\\felip\\OneDrive\\Área de Trabalho\\input.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    //using (FileStream fileStream = new FileStream("C:\\Users\\41140878859\\Desktop\\projetos_git\\stone-automata-maze-challenge\\input.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    using (FileStream fileStream = new FileStream("E:\\Git pessoal\\stone-automata-maze-challenge4\\extracted_input.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    {
        // Construct the input string
        StringBuilder inputBuilder = new StringBuilder();
        byte[] buffer = new byte[1024];
        int bytesRead;
        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            string chunk = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            inputBuilder.Append(chunk);
        }
        string input = inputBuilder.ToString();

        // Get matrix dimensions from input contents
        string[] FileLines = input.Trim().Split('\n');
        var columnsExtract = FileLines[0].Split(' ').Length;
        var linesExtract = FileLines.Length;

        // Create new jagged array with border
        
        for (int i = 0; i < extractMatrice.Length; i++)
        {
            extractMatrice[i] = new byte[columnsExtract];
        }

        // Read and parse lines
        for (int row = 0; row < linesExtract; row++)
        {
            string[] values = FileLines[row].Trim().Split(' ');
            for (int col = 0; col < columnsExtract; col++)
            {
                if (byte.TryParse(values[col], NumberStyles.None, CultureInfo.InvariantCulture, out byte value))
                {
                    extractMatrice[row][col] = value;
                }
            }
        }
        //for (int j = 0; j <= columns + 1; j++)
        //{
        //    baseMatrice[0][j] = 1; // top border
        //    baseMatrice[lines + 1][j] = 1; // bottom border
        //}
        //for (int i = 1; i <= lines + 1; i++)
        //{
        //    baseMatrice[i][0] = 1; // left border
        //    baseMatrice[i][columns + 1] = 1; // right border
        //}
    }
}

/*void PathTeste()
{
    PathTest teste= PathTest.Default;
    int i = 1, j = 1;
    (int, int) posicao = (i, j);
    int countt = 0;
    foreach (var rota in teste.PathChallenge1)
    {
        StepExecute();
        for (int u = 0; u < baseMatrice.Length; u++)
        {
            tempMatrice[u] = new byte[columns + 2];
        }
        baseMatrice[Start.Item1][Start.Item2] = 0;
        baseMatrice[Goal.Item1][Goal.Item2] = 0;
        //printMatrix(baseMatrice, "Matriz para andar");

        if (rota == "R")
        {
            j++;
            posicao = (i, j);
            if (baseMatrice[i][j] == 0)
            {
                Console.WriteLine("Passo ok");
            }
            else
            {
                Console.WriteLine("passo proibido");
                i = 99;
                break;

            }
        }
        else if(rota == "L")
        {
            j--;
            posicao = (i, j);
            if (baseMatrice[i][j] == 0)
            {
                Console.WriteLine("Passo ok");
            }
            else
            {
                Console.WriteLine("passo proibido");
                i = 99;
                break;

            }

        }
        else if(rota == "U")
        {
            i--;
            posicao = (i, j);
            if (baseMatrice[i][j] == 0)
            {
                Console.WriteLine("Passo ok");
            }
            else
            {
                Console.WriteLine("passo proibido");
                i = 99;
                break;
            }
        }
        else if(rota == "D")
        {
            i++;
            posicao = (i, j);
            if (baseMatrice[i][j] == 0)
            {
                Console.WriteLine("Passo ok");
            }
            else
            {
                Console.WriteLine("passo proibido");
                i = 99;
                break;
            }
        }
        countt++;
        baseMatrice[posicao.Item1][posicao.Item2] = 0;
        //printMatrix(baseMatrice, "matrix movimentada");
        baseMatrice[posicao.Item1][posicao.Item2] = 0;

    }
    if (i == 99)
    {
        Console.WriteLine("bateu xabu");
    }
}*/

//public class TupleEqualityComparer : IEqualityComparer<PositionStep>
//{
//    public bool Equals(PositionStep x, PositionStep y)
//    {
//        return x.id.Equals(y.id);
//    }

//    public int GetHashCode(PositionStep obj)
//    {
//        return obj.id.GetHashCode();
//    }
//}
public class PositionStep
{
    //public (int,int) end { get; set; }
    public readonly int item1;
    public readonly int item2;
    public readonly char Step;
    public readonly int id;
    public PositionStep? Parent { get; set; }

    public PositionStep(int Item1, int Item2, char step, PositionStep? parent, int Id)
    {
        item1 = Item1;
        item2 = Item2;
        Step = step;
        Parent = parent;
        id = Id;
    }

    //public PositionStep((int,int) End, char step, PositionStep? parent)
    //{
    //    end = End;
    //    Step = step;
    //    Parent = parent;
    //}




    //public bool Equals(PositionStep? other)
    //{
    //    if (other == null)
    //        return false;

    //    return this.end == other.end;
    //}
}