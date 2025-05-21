using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphAlgorithmsLab.Core.Entities
{
    public class Edge : IComparable<Edge>
    {
        public Vertex From { get; }
        public Vertex To { get; }
        public double Weight { get; set; }
        public bool IsDirected { get; }
        public Color DisplayColor { get; set; } = Color.Black;
        public bool IsHighlighted { get; set; } = false;

        public Edge(Vertex from, Vertex to, double weight = 1.0, bool isDirected = true)
        {
            From = from;
            To = to;
            Weight = weight;
            IsDirected = isDirected;
        }

        // Для сортировки ребер по весу (для алгоритма Краскала)
        public int CompareTo(Edge other)
        {
            if (other == null) return 1;
            return Weight.CompareTo(other.Weight);
        }

        // Вспомогательный метод для получения "другой" вершины, если ребро неориентированное
        public Vertex GetOtherVertex(Vertex current)
        {
            if (IsDirected)
            {
                // Для ориентированного ребра этот метод не так осмыслен,
                // но если вызвали, и current == From, вернем To
                return current.Equals(From) ? To : null; // или выбросить исключение
            }

            if (current.Equals(From)) return To;
            if (current.Equals(To)) return From;
            return null; // Если current не является одной из вершин этого ребра
        }
    }
}
