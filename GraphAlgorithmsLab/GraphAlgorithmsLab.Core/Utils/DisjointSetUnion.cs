// GraphAlgorithmsLab.Core/Utils/DisjointSetUnion.cs
using System.Collections.Generic;
using System.Linq;
using GraphAlgorithmsLab.Core.Entities; // Для Vertex

namespace GraphAlgorithmsLab.Core.Utils
{
    public class DisjointSetUnion
    {
        private Dictionary<int, int> parent; // Словарь для хранения родителя каждого элемента (Id вершины -> Id родителя)
        private Dictionary<int, int> rank;   // Словарь для хранения ранга (глубины) дерева для оптимизации объединения

        public DisjointSetUnion(IEnumerable<Vertex> vertices)
        {
            parent = new Dictionary<int, int>();
            rank = new Dictionary<int, int>();

            foreach (var vertex in vertices)
            {
                MakeSet(vertex.Id);
            }
        }

        // Создает новое множество, содержащее только один элемент x
        private void MakeSet(int x)
        {
            parent[x] = x; // Каждый элемент изначально является своим собственным родителем
            rank[x] = 0;   // Начальный ранг 0
        }

        // Находит представителя (корень) множества, которому принадлежит элемент x
        // Использует эвристику сжатия пути для оптимизации
        public int Find(int x)
        {
            if (!parent.ContainsKey(x))
            {
                // Этого не должно происходить, если DSU инициализирован всеми вершинами графа
                MakeSet(x); // На всякий случай, если вершина была добавлена после инициализации DSU
                            // (хотя для Краскала DSU обычно инициализируется один раз)
            }

            if (parent[x] == x) // Если x - корень своего дерева
            {
                return x;
            }
            // Рекурсивно находим корень и переподвешиваем x напрямую к корню (сжатие пути)
            parent[x] = Find(parent[x]);
            return parent[x];
        }

        // Объединяет два множества, содержащие элементы x и y
        // Использует эвристику объединения по рангу для оптимизации
        public bool Union(int x, int y)
        {
            int rootX = Find(x);
            int rootY = Find(y);

            if (rootX != rootY) // Если x и y уже не в одном множестве
            {
                // Объединяем дерево с меньшим рангом с деревом с большим рангом
                if (rank[rootX] < rank[rootY])
                {
                    parent[rootX] = rootY;
                }
                else if (rank[rootX] > rank[rootY])
                {
                    parent[rootY] = rootX;
                }
                else // Ранги равны, выбираем любой корень и увеличиваем его ранг
                {
                    parent[rootY] = rootX;
                    rank[rootX]++;
                }
                return true; // Объединение произошло
            }
            return false; // Элементы уже в одном множестве
        }
    }
}