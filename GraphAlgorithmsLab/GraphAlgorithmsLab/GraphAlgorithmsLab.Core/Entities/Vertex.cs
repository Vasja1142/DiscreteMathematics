using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphAlgorithmsLab.Core.Entities
{
    public class Vertex
    {
        private static int _nextId = 0;

        public int Id { get; }
        public string Label { get; set; }
        public PointF Position { get; set; }
        public Color DisplayColor { get; set; } = Color.SkyBlue;
        public static int Radius { get; } = 15; // Уменьшим немного для более компактного вида

        // Данные для алгоритмов
        public bool Visited { get; set; }
        public double Distance { get; set; }
        public Vertex Predecessor { get; set; }
        public int InDegree { get; set; }       // Для топологической сортировки (алгоритм Кана)
        public int ComponentId { get; set; }    // Для системы непересекающихся множеств (алгоритм Краскала)

        public Vertex(PointF position, string label = null)
        {
            Id = _nextId++;
            Position = position;
            Label = label ?? Id.ToString();
            ResetAlgorithmData();
        }

        public void ResetAlgorithmData()
        {
            Visited = false;
            Distance = double.PositiveInfinity;
            Predecessor = null;
            InDegree = 0;
            ComponentId = Id; // Изначально каждая вершина в своей компоненте (для DSU)
        }

        public override int GetHashCode() => Id;

        public override bool Equals(object obj) => obj is Vertex other && other.Id == Id;

        // Статический метод для сброса счетчика ID, если потребуется (например, при полной очистке графа)
        public static void ResetIdCounter()
        {
            _nextId = 0;
        }
    }
}
