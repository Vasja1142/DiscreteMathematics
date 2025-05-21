// GraphAlgorithmsLab.Core/Algorithms/KruskalAlgorithm.cs
using System.Collections.Generic;
using System.Linq;
using GraphAlgorithmsLab.Core.Entities;
using GraphAlgorithmsLab.Core.Utils; // Для DisjointSetUnion

namespace GraphAlgorithmsLab.Core.Algorithms
{
    public class KruskalAlgorithm
    {
        private readonly Graph _graph;

        public KruskalAlgorithm(Graph graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Находит минимальное остовное дерево (MST) с использованием алгоритма Краскала.
        /// </summary>
        /// <returns>
        /// Кортеж:
        /// - List<Edge> MstEdges: Список ребер, входящих в MST.
        /// - double TotalWeight: Общий вес MST.
        /// </returns>
        public (List<Edge> MstEdges, double TotalWeight) FindMinimumSpanningTree()
        {
            var mstEdges = new List<Edge>();
            double totalWeight = 0;

            // 1. Создать копию списка всех ребер графа.
            //    Алгоритм Краскала работает с неориентированными ребрами.
            //    Если в графе есть ориентированные, их можно либо игнорировать,
            //    либо рассматривать как неориентированные (если это имеет смысл для задачи).
            //    Для данной реализации мы будем брать все ребра из _graph.Edges
            //    и полагаться на то, что их веса корректны.
            //    Класс Edge имеет IComparable для сортировки по весу.
            var sortedEdges = new List<Edge>(_graph.Edges);

            // 2. Отсортировать все ребра по возрастанию их веса.
            sortedEdges.Sort(); // Использует Edge.CompareTo(Edge other)

            // 3. Инициализировать систему непересекающихся множеств (DSU).
            //    Каждая вершина изначально находится в своем собственном множестве.
            var dsu = new DisjointSetUnion(_graph.Vertices);

            int edgesInMstCount = 0;
            int vertexCount = _graph.Vertices.Count;

            // 4. Пройтись по отсортированным ребрам.
            foreach (var edge in sortedEdges)
            {
                // Для Краскала ребро всегда рассматривается как связь между двумя компонентами,
                // поэтому его направленность (IsDirected) здесь не так важна, как то, что оно соединяет From и To.
                Vertex u = edge.From;
                Vertex v = edge.To;

                // Если вершины u и v находятся в разных множествах (компонентах)
                if (dsu.Find(u.Id) != dsu.Find(v.Id))
                {
                    // Добавить ребро в MST
                    mstEdges.Add(edge);
                    totalWeight += edge.Weight;
                    edgesInMstCount++;

                    // Объединить множества u и v
                    dsu.Union(u.Id, v.Id);

                    // 5. Если в MST уже V-1 ребер, можно остановиться.
                    if (edgesInMstCount == vertexCount - 1)
                    {
                        break;
                    }
                }
            }

            // Проверка, удалось ли построить остов (для связного графа должно быть V-1 ребер)
            // Если граф был несвязным, MST будет построено для одной из компонент,
            // или edgesInMstCount будет меньше V-1.
            if (vertexCount > 0 && edgesInMstCount < vertexCount - 1)
            {
                System.Diagnostics.Debug.WriteLine($"Kruskal Warning: Graph might be disconnected. MST found with {edgesInMstCount} edges, expected {vertexCount - 1}.");
            }


            return (mstEdges, totalWeight);
        }
    }
}