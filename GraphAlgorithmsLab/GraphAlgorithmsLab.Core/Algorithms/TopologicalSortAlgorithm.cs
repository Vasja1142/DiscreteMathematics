// GraphAlgorithmsLab.Core/Algorithms/TopologicalSortAlgorithm.cs
using System.Collections.Generic;
using System.Linq;
using GraphAlgorithmsLab.Core.Entities;

namespace GraphAlgorithmsLab.Core.Algorithms
{
    public class TopologicalSortAlgorithm
    {
        private readonly Graph _graph;

        public TopologicalSortAlgorithm(Graph graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Выполняет топологическую сортировку графа с использованием алгоритма Кана.
        /// </summary>
        /// <returns>
        /// Кортеж:
        /// - bool IsAcyclic: true, если граф ациклический и сортировка успешна, иначе false.
        /// - List<List<Vertex>> Tiers: Список ярусов, где каждый ярус - это список вершин.
        ///   Если граф циклический, Tiers будет null или пустым.
        /// </returns>
        // GraphAlgorithmsLab.Core/Algorithms/TopologicalSortAlgorithm.cs
        // GraphAlgorithmsLab.Core/Algorithms/TopologicalSortAlgorithm.cs
        public (bool IsAcyclic, List<List<Vertex>> Tiers) Sort()
        {
            System.Diagnostics.Debug.WriteLine("--- Topological Sort Started ---");
            var localInDegrees = _graph.Vertices.ToDictionary(v => v, v => v.InDegree);

            System.Diagnostics.Debug.WriteLine("Initial InDegrees:");
            foreach (var kvp in localInDegrees)
            {
                System.Diagnostics.Debug.WriteLine($"Vertex {kvp.Key.Label}: {kvp.Value}");
            }

            var tiers = new List<List<Vertex>>();
            int processedVerticesCount = 0;

            var currentProcessingTier = new List<Vertex>();
            foreach (var vertex in _graph.Vertices)
            {
                if (localInDegrees[vertex] == 0)
                {
                    currentProcessingTier.Add(vertex);
                }
            }
            System.Diagnostics.Debug.WriteLine($"First tier candidates: {string.Join(", ", currentProcessingTier.Select(v => v.Label))}");


            if (!currentProcessingTier.Any() && _graph.Vertices.Any())
            {
                System.Diagnostics.Debug.WriteLine("Error: No vertices with InDegree 0 found, but graph is not empty.");
                return (false, new List<List<Vertex>>());
            }

            int tierNum = 0;
            while (currentProcessingTier.Any())
            {
                tierNum++;
                currentProcessingTier.Sort((v1, v2) => v1.Id.CompareTo(v2.Id));
                tiers.Add(new List<Vertex>(currentProcessingTier));
                System.Diagnostics.Debug.WriteLine($"Added Tier {tierNum}: {string.Join(", ", currentProcessingTier.Select(v => v.Label))}");

                var nextTierCandidates = new List<Vertex>();
                System.Diagnostics.Debug.WriteLine($"Processing Tier {tierNum} vertices:");
                foreach (var u in currentProcessingTier)
                {
                    System.Diagnostics.Debug.WriteLine($"  Processing vertex U: {u.Label}");
                    processedVerticesCount++;
                    foreach (var (neighborV, edge) in _graph.GetNeighbors(u))
                    {
                        if (edge.IsDirected && edge.From.Equals(u))
                        {
                            System.Diagnostics.Debug.WriteLine($"    Neighbor V: {neighborV.Label} (Edge {u.Label}->{neighborV.Label})");
                            System.Diagnostics.Debug.WriteLine($"    Old InDegree for {neighborV.Label}: {localInDegrees[neighborV]}");
                            localInDegrees[neighborV]--;
                            System.Diagnostics.Debug.WriteLine($"    New InDegree for {neighborV.Label}: {localInDegrees[neighborV]}");
                            if (localInDegrees[neighborV] == 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"    !!! Adding {neighborV.Label} to nextTierCandidates");
                                nextTierCandidates.Add(neighborV);
                            }
                        }
                    }
                }
                currentProcessingTier = nextTierCandidates;
                System.Diagnostics.Debug.WriteLine($"Next tier candidates for Tier {tierNum + 1}: {string.Join(", ", currentProcessingTier.Select(v => v.Label))}");
            }

            System.Diagnostics.Debug.WriteLine($"--- Topological Sort Finished. Processed: {processedVerticesCount}, Total: {_graph.Vertices.Count} ---");
            if (processedVerticesCount == _graph.Vertices.Count)
            {
                return (true, tiers);
            }
            else
            {
                return (false, new List<List<Vertex>>());
            }
        }
    }
}