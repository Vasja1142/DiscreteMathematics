using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace SetCalculator
{
    internal class Set
    {
        internal int[] arr;
        internal int counter;
        internal char nameArr;
        public Set(int len, char nameArr)
        {
            arr = new int[len];
            this.nameArr = nameArr;
            counter = 0;
        }
        public Set(int[] array, char nameArr)
        {
            List<int> list = new List<int>();
            foreach (int x in array)
            {
                if (!list.Contains(x)) list.Add(x);
            }
            this.nameArr = nameArr;
            counter = list.Count;
            arr  = list.ToArray();
        }

        public Set(List<int> list, char nameArr)
        {
            counter = list.Count;
            arr = list.ToArray();
            this.nameArr = nameArr;
        }

        public int[] Get() { 
            return arr;
        }

        public int Get(int index)
        {
            return arr[index];
        }

        internal int Length()
        {
            return arr.Length;
        }

        internal bool pushBack(int val) {
            bool flag = true;
            foreach (int x in arr)
            {
                if (x == val)
                {
                    flag = false;
                    
                }
            }
            if (flag)
            {
                arr[counter++] = val;
            }
            return flag;
        }

        internal bool isThereNum(int val)
        {
            bool flag = false;
            foreach (int x in arr)
            {
                if (x == val)
                {
                    flag = true;

                }
            }
            return flag;
        }
        internal bool isThereSet(Set set)
        {
            bool flag = true;
            bool flag2 = true;
            foreach (int x in arr)
            {
                flag = false;
                foreach (int y in set.arr)
                {
                   
                    if (x == y)
                    {
                        flag = true;
                    }
                }
                if (!flag) flag2 = false;
            }
            return flag2;
        }

        public bool pushBack(int val, int[] array)
        {
            bool flag2 = true;
            foreach (int x in arr)
            {
                if (x == val)
                {
                    flag2 = false;
                    Console.WriteLine("Данное значение уже присутствует в множестве");
                }
            }
            if (flag2)
            {
                bool flag = false;
                foreach (int x in array)
                {
                    if (x == val)
                    {
                        flag = true;
                    }
                }
                
                if (flag)
                {
                    arr[counter++] = val;
                }
                else Console.WriteLine("Такого значения нет в множестве U!");
                return flag;
            }
            else return false;
        }
        public bool pushBack(int min, int max)
        {
            Random random = new Random();
            int val = random.Next(min, max+1);

            bool flag = true;
            foreach (int x in arr)
            {
                if (x == val)
                {
                    flag = false;
                }
            }
            if (flag)
            {
                arr[counter++] = val;
            }
            return flag;
        }

        public void print()
        {
            Console.Write($"Множество {nameArr}: {{");
            for (int i = 0; i < arr.Length; i++)
            {
                Console.Write(arr[i]);
                if (i + 1 != arr.Length) Console.Write(", ");
            }
            Console.WriteLine("}");
        }

        public Set union(Set set)
        {
            int len = arr.Length + set.Length();
            int[] nums = new int[len];
            int counter = 0;
            foreach (int x in set.Get()) {
                nums[counter++] = x;
            }
            foreach (int item in arr)
            {
                nums[counter++] = item;
            }
            Set resSet = new Set(nums, 'R');
            return resSet;
        }
        public Set intersection(Set set)
        {
            List<int> list = new List<int>();
            foreach (int x in set.Get())
            {
                foreach (int item in arr)
                {
                    if (item == x)
                    {
                        list.Add(x);
                    }
                }
            }
            Set resSet = new Set(list, 'R');
            return resSet;
        }
        public Set difference(Set set)
        {
            List<int> list = new List<int>();
            bool flag;
            foreach (int x in arr)
            {
                flag = true;
                foreach (int item in set.Get())
                {
                    if (item == x)
                    {
                        flag = false;
                    }
                    
                }
                if (flag)
                {
                    list.Add(x);
                }
            }
            Set resSet = new Set(list, 'R');
            return resSet;
        }
        public Set symmetricalDiff(Set set)
        {
            List<int> list = new List<int>();
            List<int> resList = new List<int>();

            foreach (int x in set.Get())
            {
                foreach (int item in arr)
                {
                    if (item == x)
                    {
                        list.Add(x);
                    }
                }
            }

            foreach (int x in set.Get())
            {
                if (!list.Contains(x))
                {
                    resList.Add(x);
                }
            }

            foreach (int x in arr)
            {
                if (!list.Contains(x))
                {
                    resList.Add(x);
                }
            }

            Set resSet = new Set(resList, 'R');
            return resSet;
        }
        public Set compliment(Set set)
        {
            return difference(set);
        }
    }
}
