// GraphAlgorithmsLab.Core/Algorithms/DijkstraAlgorithm.cs
using System.Collections.Generic;
using System.Linq;
using GraphAlgorithmsLab.Core.Entities;

namespace GraphAlgorithmsLab.Core.Algorithms
{
    public class DijkstraAlgorithm
    {
        private readonly Graph _graph;

        public DijkstraAlgorithm(Graph graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Выполняет алгоритм Дейкстры для поиска кратчайших путей от стартовой вершины.
        /// Данные о расстояниях и предшественниках сохраняются непосредственно в объектах Vertex.
        /// </summary>
        /// <param name="startVertex">Стартовая вершина.</param>
        public void Run(Vertex startVertex)
        {
            if (startVertex == null || !_graph.Vertices.Contains(startVertex))
            {
                // Можно выбросить исключение или просто ничего не делать
                System.Diagnostics.Debug.WriteLine("Dijkstra Error: Start vertex is null or not in graph.");
                return;
            }

            // 1. Инициализация:
            //    ResetAllVerticesAlgorithmData() должен быть вызван вовне перед запуском,
            //    чтобы Distance были PositiveInfinity, а Predecessor - null.
            //    Здесь мы устанавливаем расстояние до стартовой вершины = 0.
            startVertex.Distance = 0;

            // Приоритетная очередь для выбора вершины с наименьшим расстоянием.
            // Будем использовать простой List и сортировать его, или искать минимум.
            // Для большей эффективности в реальных задачах используют MinHeap.
            // Для учебных целей List + Linq.Min() подойдет.
            var priorityQueue = new List<Vertex>();
            priorityQueue.AddRange(_graph.Vertices); // Добавляем все вершины

            // Множество посещенных вершин (хотя у нас есть Vertex.Visited, но для явности)
            // var settledNodes = new HashSet<Vertex>(); // Если бы не было Vertex.Visited

            while (priorityQueue.Any())
            {
                // 2. Выбрать вершину 'u' из priorityQueue с наименьшим значением distance[u],
                //    которая еще не была посещена (Vertex.Visited == false).
                Vertex u = null;
                double minDistance = double.PositiveInfinity;

                foreach (var vertexInQueue in priorityQueue)
                {
                    if (!vertexInQueue.Visited && vertexInQueue.Distance < minDistance)
                    {
                        minDistance = vertexInQueue.Distance;
                        u = vertexInQueue;
                    }
                }

                if (u == null) // Все оставшиеся вершины недостижимы или уже посещены
                {
                    break;
                }

                u.Visited = true;
                priorityQueue.Remove(u); // "Извлекаем" из очереди (удаляем из списка кандидатов)

                // 3. Для каждого соседа 'v' вершины 'u':
                foreach (var (neighborV, edge) in _graph.GetNeighbors(u))
                {
                    if (neighborV.Visited) // Пропускаем уже обработанных соседей
                    {
                        continue;
                    }

                    // Учитываем вес ребра. Алгоритм Дейкстры не работает с отрицательными весами.
                    // Если есть ребра с отрицательным весом, нужно использовать Беллмана-Форда.
                    // Мы предполагаем, что веса неотрицательные.
                    if (edge.Weight < 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Warning: Dijkstra's algorithm encountered a negative weight edge ({u.Label} -> {neighborV.Label}, Weight: {edge.Weight}). Results may be incorrect.");
                        // Для строгости можно здесь прервать выполнение или выдать ошибку.
                    }

                    double newDist = u.Distance + edge.Weight;

                    // Релаксация ребра: если найден более короткий путь к 'v' через 'u'
                    if (newDist < neighborV.Distance)
                    {
                        neighborV.Distance = newDist;
                        neighborV.Predecessor = u;
                        // Если бы мы использовали настоящий MinHeap, здесь была бы операция DecreaseKey.
                        // С нашим List<Vertex> изменения отразятся при следующем поиске минимума.
                    }
                }
            }
        }

        /// <summary>
        /// Восстанавливает кратчайший путь от стартовой вершины (с которой запускался Run) до целевой.
        /// Должен вызываться ПОСЛЕ выполнения метода Run().
        /// </summary>
        /// <param name="targetVertex">Целевая вершина.</param>
        /// <returns>Список ребер, составляющих кратчайший путь, или пустой список, если пути нет.</returns>
        public List<Edge> GetPath(Vertex targetVertex)
        {
            var path = new List<Edge>();
            if (targetVertex == null || targetVertex.Distance == double.PositiveInfinity)
            {
                return path; // Пути нет или цель недостижима
            }

            Vertex current = targetVertex;
            while (current.Predecessor != null)
            {
                Vertex prev = current.Predecessor;
                // Найти ребро, соединяющее prev и current
                // Это может быть как prev -> current, так и current -> prev (если граф неориентированный и ребро одно)
                Edge edgeOnPath = _graph.Edges.FirstOrDefault(e =>
                    (e.From.Equals(prev) && e.To.Equals(current)) ||
                    (!e.IsDirected && e.From.Equals(current) && e.To.Equals(prev))
                    );

                if (edgeOnPath != null)
                {
                    path.Add(edgeOnPath);
                }
                else
                {
                    // Этого не должно произойти, если Predecessor установлен корректно
                    System.Diagnostics.Debug.WriteLine($"Error reconstructing path: Edge not found between {prev.Label} and {current.Label}");
                    break;
                }
                current = prev;
            }
            path.Reverse(); // Путь восстанавливается от цели к началу, поэтому его нужно развернуть
            return path;
        }
    }
}