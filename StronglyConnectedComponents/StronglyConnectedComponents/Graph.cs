using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StronglyConnectedComponents
{
    internal class Graph
    {
        private bool[,] graph;
        private int size;

        // Создает квадратную матрицу 
        public Graph(int size) {

            graph = new bool[size,size];
            this.size = size;
        }



        public string SccFinder()
        {

            HashSet<string>  SCCs = new HashSet<string>();
            bool[,] resGraph = new bool[size,size];
            bool[,] tempGraph = new bool[size, size];
            
            //Клонирование графа в resgraph и сложение с единичной матрицей E^0 + E^1
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i == j)
                        resGraph[i, j] = true;
                    else
                        resGraph[i, j] = graph[i, j];
                }
            }

            //resGraph = E^0 + E^1 + E^2 + ... + E^(size-1) 
            for (int degree = 2; degree <= (size-1); degree++) 
            {
                

                //Увеличение степени графа и запись его в tempGraph
                bool temp;
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        temp = false;
                        for (int k = 0; k < size; k++)
                        {
                            temp = temp || (resGraph[i, k] && graph[k, j]);
                        }
                        tempGraph[i, j] = temp;
                    }
                }

                //Сложение resGraph со следующей его степенью tempGraph
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        resGraph[i, j] = resGraph[i, j] || tempGraph[i, j];
                    }
                }
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    tempGraph[i,j] = resGraph[i, j] && resGraph[j, i];
                }
            }
            resGraph = tempGraph;

            //Запись компонентов сильной связности в HashSet
            for (int i = 0; i < size; i++)
            {
                StringBuilder SCC = new StringBuilder();
                SCC.Append('{');
                for (int j = 0; j < size; j++)
                {
                    if (resGraph[i, j]) SCC.Append((char)(j + 65) + ", ");
                }
                SCC.Remove(SCC.Length - 2, 2);
                SCC.Append("} ");
                SCCs.Add(SCC.ToString());
            }

            //Генерация текста для вывода
            StringBuilder SCCsText = new StringBuilder();
            foreach(string SCC in SCCs)
            {
                SCCsText.Append(SCC);
            }

            return SCCsText.ToString();
        }

        public void AddGraphWithForm()
        {
            graph = MainForm.GetAdjacencyMatrix();

            // Сложение с единичной матрицей 
            for (int i = 0; i < size; i++)
                graph[i, i] = true;
        }


    }
}
