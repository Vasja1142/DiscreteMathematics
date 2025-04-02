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

        public Graph(int size) {
            graph = new bool[size,size];
            this.size = size;
        }

        public Graph(bool[,] graph)
        {
            this.graph = graph;
            size = graph.GetLength(0);
        }

        public List<List<char>> SccFinder()
        {
            List<List<char>> resVerticeOfGraph = new List<List<char>>();
            Graph resGraph = new Graph(graph);
            Graph tempGraph = new Graph(size);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                }
            }
           

            return resVerticeOfGraph;
        }

        public void AddGraphWithForm()
        {
            graph = MainForm.GetAdjacencyMatrix();
        }


    }
}
