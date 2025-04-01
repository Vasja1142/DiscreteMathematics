using System.Reflection;

internal class Program
{
    public static List<List<int>> binTable;
    public static List<List<int>> resultTable;
    public static char[] nameValues = { 'a', 'b', 'c', 'd', 'e' };
    private static void Main(string[] args)
    {
        int n = ImputPositiveNumber("Введите количество аргументов (до 5): ");
        while (n > 5) n = ImputPositiveNumber("Введите количество аргументов (до 5): ");
        Console.WriteLine("Вводите значения коньюнкций 1 или 0. Любые положительные числа больше 1 воспринимаются программой за 1.");
        binTable = new List<List<int>>((int)Math.Pow(2, n));
        resultTable = new List<List<int>>();
        List<int> boolNums = new List<int>(n);
        RecFunc(boolNums, n);


        PrintSKNF(n);
        PrintSDNF(n);
        PrintMDNF(n);

    }




    public static void PrintSDNF(int len)
    {
        Console.Write("СДНФ: ");
        bool flag = false;
        foreach (List<int> list in binTable)
        {

            if (list[list.Count - 1] == 1)
            {
                if (flag) Console.Write(" v ");
                else flag = true;
                for (int i = 0; i < len; i++)
                {

                    if (list[i] == 0) Console.Write('!');
                    Console.Write(nameValues[i]);
                }

            }
        }
        Console.WriteLine();
    }


    public static List<List<int>> MDNFFunction(List<List<int>> ternMDNF, int count)
    {
        if (count == 0 || ternMDNF.Count == 0)
        {
            for (int i = 0; i < ternMDNF.Count; i++)
            {
                resultTable.Add(ternMDNF[i]);
            }
            return ternMDNF;
        }
        else
        {
            List<List<int>> resTernMDNF = new List<List<int>>();
            bool isEmpty;
            for (int k = 0; k < ternMDNF[0].Count - 1; k++)
            {
                for (int i = 0; i < ternMDNF.Count - 1; i++)
                {
                    for (int j = i + 1; j < ternMDNF.Count; j++)
                    {
                        isEmpty = true;
                        for (int l = 0; l < ternMDNF[0].Count - 1; l++)
                        {
                            if (k == l) continue;
                            if (ternMDNF[i][l] != ternMDNF[j][l])
                            {
                                isEmpty = false;
                                break;
                            }
                        }
                        if (isEmpty)
                        {
                            resTernMDNF.Add(ternMDNF[i].ToList());
                            resTernMDNF[resTernMDNF.Count - 1][k] = -1;
                            resTernMDNF[resTernMDNF.Count - 1][resTernMDNF[0].Count - 1] = 0;
                            ternMDNF[i][ternMDNF[i].Count - 1]++;
                            ternMDNF[j][ternMDNF[j].Count - 1]++;
                        }
                    }
                }
            }
            for (int i = 0; i < ternMDNF.Count; i++)
            {
                if (ternMDNF[i][ternMDNF[i].Count - 1] == 0)
                {
                    resultTable.Add(ternMDNF[i].ToList());
                }
            }
            resTernMDNF = DeletintIdenticalLines(resTernMDNF);
            return MDNFFunction(resTernMDNF, count - 1);
        }
    }

    public static void PrintMDNF(int len)
    {
        Console.Write("MДНФ: ");
        List<List<int>> disTable = new List<List<int>>();
        for (int i = 0; i < binTable.Count; i++)
        {
            List<int> list = new List<int>(binTable.Count - 1);
            if (binTable[i][binTable[i].Count - 1] == 1)
            {
                for (int j = 0; j < binTable[i].Count - 1; j++)
                {
                    list.Add(binTable[i][j]);
                }
                list.Add(0);
                disTable.Add(list);
            }
        }
        MDNFFunction(disTable, len - 1);


        List<List<int>> MDNF = Absorbing(resultTable);

        bool flag = false;
        foreach (List<int> list in MDNF)
        {


            if (flag) Console.Write(" v ");
            else flag = true;
            for (int i = 0; i < len; i++)
            {
                if (list[i] == -1) continue;

                if (list[i] == 0) Console.Write('!');
                Console.Write(nameValues[i]);
            }


        }
        Console.WriteLine();
    }



    public static List<List<int>> DeletintIdenticalLines(List<List<int>> list)
    {
        List<List<int>> resLists = new List<List<int>>();

        if (list.Count > 0)
        {
            bool isEmpty;

            for (int i = 0; i < list.Count - 1; i++)
            {

                isEmpty = true;
                for (int j = i + 1; j < list.Count; j++)
                {
                    isEmpty = true;
                    for (int k = 0; k < list[0].Count; k++)
                    {
                        if (list[i][k] != list[j][k])
                        {
                            isEmpty = false;
                        }
                    }
                    if (isEmpty) break;
                }
                if (!isEmpty)
                {
                    resLists.Add(list[i]);
                    continue;
                }

            }
            resLists.Add(list[list.Count - 1]);
        }

        return resLists;
    }

    public static List<List<int>> Absorbing(List<List<int>> mdnf)
    {
        List<List<int>> resultLists = DeletintIdenticalLines(mdnf);

        if (mdnf.Count != 0) resultLists.Add(mdnf[mdnf.Count - 1]);
        int counter;
        foreach (var ints in resultLists)
        {
            counter = -1;
            foreach (int i in ints)
            {
                if (i != -1) counter++;
            }
            ints[ints.Count - 1] = counter;
        }
        List<List<int>> sortedList = new List<List<int>>(resultLists.Count);
        for (int i = 5; i > 0; i--)
        {
            for (int j = 0; j < resultLists.Count; j++)
            {
                if (resultLists[j][resultLists[0].Count - 1] == i) sortedList.Add(resultLists[j]);
            }
        }


        resultLists = sortedList;
        bool isEmpty;
        for (int i = 0; i < resultLists.Count; i++)
        {
            isEmpty = false;
            for (int k = 0; k < resultLists[0].Count - 1; k++)
            {
                isEmpty = false;
                for (int j = 0; j < resultLists.Count; j++)
                {
                    if (resultLists[i][k] == resultLists[j][k] && j != i)
                    {
                        isEmpty = true;
                        break;
                    }
                }
                if (!isEmpty) break;
            }
            if (isEmpty)
            {
                resultLists.RemoveAt(i--);
            }
        }
        return resultLists;

    }
    public static void PrintSKNF(int len)
    {
        Console.Write("СКНФ: ");
        foreach (List<int> list in binTable)
        {
            if (list[list.Count - 1] == 0)
            {
                Console.Write('(');
                for (int i = 0; i < len; i++)
                {
                    if (list[i] == 1) Console.Write('!');
                    Console.Write(nameValues[i]);
                    if (i < len - 1) Console.Write("v");
                }
                Console.Write(")");
            }
        }
        Console.WriteLine();
    }

    public static void RecFunc(List<int> boolNums, int len)
    {
        if (len == 0)
        {
            for (int i = 0; i < boolNums.Count; i++)
            {
                Console.Write(boolNums[i]);
            }
            int res = ImputPositiveNumber(" ");

            List<int> listBool = new List<int>(boolNums);
            if (res == 0) listBool.Add(0);
            else listBool.Add(1);
            binTable.Add(listBool);
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                boolNums.Add(i);
                RecFunc(boolNums, len - 1);
                boolNums.RemoveAt(boolNums.Count - 1);
            }
        }
    }

    public static int ImputPositiveNumber(string message, string errorMessage = "Введите положительное число: ")
    {
        Console.Write(message);
        int num = ImputInt();
        while (num < 0)
        {
            Console.WriteLine(errorMessage);
            num = ImputInt();
        }
        return num;
    }
    public static int ImputInt()
    {
        int time;
        bool isIntTime = int.TryParse(Console.ReadLine(), out time);
        while (!isIntTime)
        {
            Console.WriteLine("Введено не целочисленное значение!");
            isIntTime = int.TryParse(Console.ReadLine(), out time);
        }
        return time;
    }


}

 