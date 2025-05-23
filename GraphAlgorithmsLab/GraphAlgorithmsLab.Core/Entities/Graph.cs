using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphAlgorithmsLab.Core.Entities
{
    public class Graph
    {
        public List<Vertex> Vertices { get; } = new List<Vertex>();
        public List<Edge> Edges { get; } = new List<Edge>();

        // Список смежности: Vertex.Id -> List of incident Edges
        public Dictionary<int, List<Edge>> AdjacencyList { get; } = new Dictionary<int, List<Edge>>();

        public Vertex AddVertex(PointF position, string label = null)
        {
            var vertex = new Vertex(position, label);
            if (!Vertices.Contains(vertex)) // Проверка на дубликат по ID (хотя ID уникальны по определению)
            {
                Vertices.Add(vertex);
                AdjacencyList[vertex.Id] = new List<Edge>();
            }
            return Vertices.First(v => v.Id == vertex.Id); // Возвращаем добавленную или существующую вершину
        }

        // Вспомогательный метод для поиска существующего ребра между двумя вершинами
        public Edge FindEdge(Vertex from, Vertex to)
        {
            // Ищем прямое ориентированное ребро
            var directedEdge = Edges.FirstOrDefault(e => e.From.Equals(from) && e.To.Equals(to) && e.IsDirected);
            if (directedEdge != null) return directedEdge;

            // Ищем обратное ориентированное ребро
            var reverseDirectedEdge = Edges.FirstOrDefault(e => e.From.Equals(to) && e.To.Equals(from) && e.IsDirected);
            if (reverseDirectedEdge != null) return reverseDirectedEdge;

            // Ищем неориентированное ребро между этими вершинами
            var undirectedEdge = Edges.FirstOrDefault(e => !e.IsDirected &&
                                                         ((e.From.Equals(from) && e.To.Equals(to)) ||
                                                          (e.From.Equals(to) && e.To.Equals(from))));
            return undirectedEdge;
        }

        public void AddDirectedEdge(Vertex from, Vertex to, double weight = 1.0)
        {
            if (from == null || to == null || !Vertices.Contains(from) || !Vertices.Contains(to) || from.Equals(to))
            {
                System.Diagnostics.Debug.WriteLine("Error: Invalid vertices for directed edge.");
                return;
            }

            // Проверим, нет ли уже неориентированного ребра или обратного ориентированного
            // Если есть неориентированное, то добавление ориентированного может быть избыточным или конфликтным
            // Если есть обратное, то это как раз наш случай "наложения"
            var existingUndirected = Edges.FirstOrDefault(e => !e.IsDirected &&
                                               ((e.From.Equals(from) && e.To.Equals(to)) || (e.From.Equals(to) && e.To.Equals(from))));
            if (existingUndirected != null)
            {
                System.Diagnostics.Debug.WriteLine($"Info: Undirected edge already exists between {from.Label} and {to.Label}. Directed edge not added.");
                // Можно здесь обновить вес существующего неориентированного, если логика это предполагает
                // existingUndirected.Weight = weight; // Например
                return;
            }

            // Проверяем, нет ли уже такого же ориентированного ребра
            if (Edges.Any(e => e.IsDirected && e.From.Equals(from) && e.To.Equals(to)))
            {
                System.Diagnostics.Debug.WriteLine($"Info: Directed edge from {from.Label} to {to.Label} already exists.");
                return; // Уже есть такое ребро, не добавляем дубликат
            }

            var edge = new Edge(from, to, weight, true);
            Edges.Add(edge);
            AdjacencyList[from.Id].Add(edge);
            // RecalculateInDegrees() будет вызван перед ЯПФ
        }

        public void AddUndirectedEdge(Vertex from, Vertex to, double weight = 1.0)
        {
            if (from == null || to == null || !Vertices.Contains(from) || !Vertices.Contains(to) || from.Equals(to))
            {
                System.Diagnostics.Debug.WriteLine("Error: Invalid vertices for undirected edge.");
                return;
            }

            // Проверяем, нет ли уже какого-либо ребра между этими вершинами
            if (Edges.Any(e => ((e.From.Equals(from) && e.To.Equals(to)) || (e.From.Equals(to) && e.To.Equals(from)))))
            {
                System.Diagnostics.Debug.WriteLine($"Info: An edge (directed or undirected) already exists between {from.Label} and {to.Label}. New undirected edge not added.");
                // Можно здесь решить: если есть ориентированные, удалить их и добавить это неориентированное.
                // Или просто не добавлять, если хоть какое-то ребро уже есть.
                // Пока что просто не добавляем, если что-то уже есть.
                return;
            }

            var edge = new Edge(from, to, weight, false);
            Edges.Add(edge);
            AdjacencyList[from.Id].Add(edge);
            AdjacencyList[to.Id].Add(edge);
        }

        // Новый метод для "умного" добавления или обновления ребра
        // Возвращает true, если ребро было добавлено/обновлено, false если нет (например, петля)
        // string& message - для вывода информации в UI
        public bool AddOrUpdateSmartEdge(Vertex from, Vertex to, double weight, bool preferDirected, out string message)
        {
            message = "";
            if (from == null || to == null || !Vertices.Contains(from) || !Vertices.Contains(to))
            {
                message = "Ошибка: одна из вершин не существует.";
                return false;
            }
            if (from.Equals(to))
            {
                message = "Петли не поддерживаются этим интерфейсом.";
                return false;
            }

            // Ищем существующие ребра между этими двумя вершинами
            var edgeFromTo = Edges.FirstOrDefault(e => e.IsDirected && e.From.Equals(from) && e.To.Equals(to));
            var edgeToFrom = Edges.FirstOrDefault(e => e.IsDirected && e.From.Equals(to) && e.To.Equals(from));
            var undirectedEdge = Edges.FirstOrDefault(e => !e.IsDirected &&
                                                     ((e.From.Equals(from) && e.To.Equals(to)) ||
                                                      (e.From.Equals(to) && e.To.Equals(from))));

            if (preferDirected) // Пользователь хочет добавить ОРИЕНТИРОВАННОЕ ребро from -> to
            {
                if (edgeFromTo != null) // Такое же ориентированное ребро уже есть
                {
                    edgeFromTo.Weight = weight; // Просто обновляем вес
                    message = $"Вес существующего ориентированного ребра от {from.Label} к {to.Label} обновлен на {weight}.";
                    return true;
                }
                if (undirectedEdge != null) // Уже есть неориентированное ребро
                {
                    message = $"Между {from.Label} и {to.Label} уже существует неориентированное ребро. Удалите его, чтобы добавить ориентированное.";
                    return false; // Не добавляем ориентированное поверх неориентированного
                }
                if (edgeToFrom != null) // Есть обратное ориентированное ребро (to -> from)
                {
                    // Вот наш случай! Превращаем в неориентированное
                    Edges.Remove(edgeToFrom); // Удаляем старое обратное
                                              // Удаляем из списков смежности
                    if (AdjacencyList.ContainsKey(to.Id)) AdjacencyList[to.Id].Remove(edgeToFrom);

                    // Добавляем новое неориентированное
                    // Вопрос: какой вес использовать? Вес нового? Средний?
                    // Пока используем вес нового (предполагаемого) ребра from -> to
                    var newUndirected = new Edge(from, to, weight, false);
                    Edges.Add(newUndirected);
                    AdjacencyList[from.Id].Add(newUndirected);
                    AdjacencyList[to.Id].Add(newUndirected);
                    message = $"Создано неориентированное ребро между {from.Label} и {to.Label} (вес {weight}) вместо двух встречных ориентированных.";
                    return true;
                }
                // Если ничего из вышеперечисленного, просто добавляем новое ориентированное ребро
                var newDirected = new Edge(from, to, weight, true);
                Edges.Add(newDirected);
                AdjacencyList[from.Id].Add(newDirected);
                message = $"Добавлено ориентированное ребро от {from.Label} к {to.Label} (вес {weight}).";
                return true;
            }
            else // Пользователь хочет добавить НЕОРИЕНТИРОВАННОЕ ребро
            {
                if (undirectedEdge != null) // Неориентированное уже есть
                {
                    undirectedEdge.Weight = weight; // Обновляем вес
                    message = $"Вес существующего неориентированного ребра между {from.Label} и {to.Label} обновлен на {weight}.";
                    return true;
                }
                // Если есть какие-то ориентированные ребра, удаляем их
                bool removedAny = false;
                if (edgeFromTo != null)
                {
                    Edges.Remove(edgeFromTo);
                    if (AdjacencyList.ContainsKey(from.Id)) AdjacencyList[from.Id].Remove(edgeFromTo);
                    removedAny = true;
                }
                if (edgeToFrom != null)
                {
                    Edges.Remove(edgeToFrom);
                    if (AdjacencyList.ContainsKey(to.Id)) AdjacencyList[to.Id].Remove(edgeToFrom);
                    removedAny = true;
                }

                var newUndirected = new Edge(from, to, weight, false);
                Edges.Add(newUndirected);
                AdjacencyList[from.Id].Add(newUndirected);
                AdjacencyList[to.Id].Add(newUndirected);
                if (removedAny)
                    message = $"Ориентированные ребра между {from.Label} и {to.Label} заменены на неориентированное (вес {weight}).";
                else
                    message = $"Добавлено неориентированное ребро между {from.Label} и {to.Label} (вес {weight}).";
                return true;
            }
        }

        public IEnumerable<(Vertex neighbor, Edge edge)> GetNeighbors(Vertex vertex)
        {
            if (vertex == null || !AdjacencyList.ContainsKey(vertex.Id))
            {
                yield break; // Возвращаем пустое перечисление
            }

            foreach (var edge in AdjacencyList[vertex.Id])
            {
                if (edge.IsDirected)
                {
                    // Для ориентированного ребра, оно должно исходить из текущей вершины
                    if (edge.From.Equals(vertex))
                    {
                        yield return (edge.To, edge);
                    }
                }
                else // Неориентированное ребро
                {
                    // Если ребро (A,B) неориентированное, и мы в A, сосед B. Если мы в B, сосед A.
                    if (edge.From.Equals(vertex))
                    {
                        yield return (edge.To, edge);
                    }
                    else if (edge.To.Equals(vertex)) // Это ребро инцидентно vertex, но From != vertex
                    {
                        yield return (edge.From, edge);
                    }
                }
            }
        }

        public bool RemoveVertex(Vertex vertexToRemove)
        {
            if (vertexToRemove == null || !Vertices.Contains(vertexToRemove))
            {
                return false; // Вершины нет в графе
            }

            // 1. Удалить все ребра, связанные с этой вершиной
            //    Создаем копию списка Edges для итерации, так как будем изменять оригинальный список
            var edgesToRemove = Edges.Where(e => e.From.Equals(vertexToRemove) || e.To.Equals(vertexToRemove)).ToList();
            foreach (var edge in edgesToRemove)
            {
                Edges.Remove(edge);
                // Также нужно удалить это ребро из списков смежности других вершин, если оно там есть
                if (AdjacencyList.ContainsKey(edge.From.Id))
                {
                    AdjacencyList[edge.From.Id].Remove(edge);
                }
                if (AdjacencyList.ContainsKey(edge.To.Id))
                {
                    AdjacencyList[edge.To.Id].Remove(edge);
                }
            }

            // 2. Удалить вершину из списка вершин
            Vertices.Remove(vertexToRemove);

            // 3. Удалить вершину из списка смежности
            if (AdjacencyList.ContainsKey(vertexToRemove.Id))
            {
                AdjacencyList.Remove(vertexToRemove.Id);
            }

            // 4. (Опционально, но хорошо для консистентности) Пересчитать InDegree, если это важно для текущего состояния
            // RecalculateInDegrees(); // Может быть вызвано отдельно, если нужно

            return true;
        }

        public void Clear()
        {
            Vertices.Clear();
            Edges.Clear();
            AdjacencyList.Clear();
            Vertex.ResetIdCounter(); // Сбрасываем статический счетчик ID вершин
        }

        public void ResetAllVerticesAlgorithmData()
        {
            foreach (var vertex in Vertices)
            {
                vertex.ResetAlgorithmData();
            }
        }

        public void RecalculateInDegrees()
        {
            // Сначала обнуляем InDegree для всех вершин
            foreach (var vertex in Vertices)
            {
                vertex.InDegree = 0;
            }

            // Затем проходим по всем ребрам и считаем InDegree
            foreach (var edge in Edges)
            {
                if (edge.IsDirected)
                {
                    // Убедимся, что To вершина все еще существует (на всякий случай)
                    if (Vertices.Contains(edge.To))
                    {
                        edge.To.InDegree++;
                    }
                }
                // Для неориентированных ребер InDegree в контексте ЯПФ не используется.
            }
        }
    }
}
