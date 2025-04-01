using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SetCalculator;
namespace SetCalculator
{
    internal class Create
    {
        internal static Set SetConsole(char nameArr)
        {
            int count = App.ConsoleTryParse($"Введите длину множества целочисленных значений {nameArr}: ");
            while (count < 0)
            {
                Console.WriteLine("Введите положительное число!");
                count = App.ConsoleTryParse($"Введите длину множества целочисленных значений {nameArr}: ");
            }
            Set U = new Set(count, 'U');

            int val;
            bool flag;
            for (int i = 0; i < count; i++)
            {
                val = App.ConsoleTryParse($"Введите {i + 1}-е значение множества {nameArr}");
                flag = U.pushBack(val);
                while (!flag)
                {
                    Console.WriteLine("Данное значение уже присутствует в множестве");
                    val = App.ConsoleTryParse($"Введите {i + 1}-е значение множества {nameArr}");
                    flag = U.pushBack(val);
                }
            }

            return U;
        }
        internal static Set SetConsole(char nameArr, Set mainSet)
        {
            int count = App.ConsoleTryParse($"Введите длину подмножества {nameArr} множества целочисленных значений U (не больше {mainSet.Length()}): ");
            while (count > mainSet.Length() || count < 0)
            {
                Console.WriteLine("Длина подмножества не может быть больше длины множества и меньше 0.");
                count = App.ConsoleTryParse($"Введите длину подмножества {nameArr} множества целочисленных значений U (не больше {mainSet.Length()}): ");
            }
            Set A = new Set(count, nameArr);

            int val;
            bool flag;
            for (int i = 0; i < count; i++)
            {
                flag = false;
                while (!flag)
                {

                    val = App.ConsoleTryParse($"Введите {i + 1}-е значение множества {nameArr}");
                    flag = A.pushBack(val, mainSet.Get());
                }
            }

            return A;
        }

        internal static Set SetRandom(char nameArr)
        {
            int count = App.ConsoleTryParse($"Введите длину множества целочисленных значений {nameArr}: ");
            while (count < 0)
            {
                Console.WriteLine("Введите положительное число!");
                count = App.ConsoleTryParse($"Введите длину множества целочисленных значений {nameArr}: ");
            }
            Set U = new Set(count, 'U');

            int min = App.ConsoleTryParse($"Введите минимальное значение {nameArr}: ");
            int max = App.ConsoleTryParse($"Введите максимальное значение {nameArr}: ");

            while (min > max-count)
            {
                Console.WriteLine("Слишком маленькое значение!");
                max = App.ConsoleTryParse($"Введите максимальное значение {nameArr}: ");
            }

            
            for (int i = 0; i < count; i++)
            {
                bool flag = false;
                while (!flag)
                {
                    flag = U.pushBack(min, max);
                }
                
            }

            return U;
        }
        internal static Set SetRandom(char nameArr, Set mainSet)
        {
            int count = App.ConsoleTryParse($"Введите длину множества целочисленных значений {nameArr}: ");
            while (count < 0 || count > mainSet.Length())
            {
                Console.WriteLine($"Введите положительное число до {mainSet.Length()}!");
                count = App.ConsoleTryParse($"Введите длину множества целочисленных значений {nameArr}: ");
            }
            Set A = new Set(count, nameArr);

            Random random = new Random();
            int val;
            bool flag;
            for (int i = 0; i < count; i++)
            {
                val = mainSet.Get(random.Next(0, mainSet.Length()));
                flag = A.pushBack(val);
                while (!flag)
                {
                    val = mainSet.Get(random.Next(0, mainSet.Length()));
                    flag = A.pushBack(val);
                }
            }

            return A;
        }

    }
}
