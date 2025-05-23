// GraphAlgorithmsLab.WinFormsApp/Rendering/GraphRenderer.cs
using System.Drawing;
using System.Drawing.Drawing2D; // Для SmoothingMode и стрелок
using System.Windows.Forms;     // Для Control (Panel/PictureBox)
using GraphAlgorithmsLab.Core.Entities; // Ссылка на наши основные классы
using System.Linq;              // Для удобства работы с коллекциями

namespace GraphAlgorithmsLab.WinFormsApp.Rendering
{
    public class GraphRenderer
    {
        private readonly Graph _graph;
        private readonly Control _panel; // Панель, на которой рисуем

        // Настройки отрисовки (можно вынести в отдельный класс конфигурации позже)
        public Font VertexFont { get; set; } = new Font("Arial", 8);
        public Font EdgeWeightFont { get; set; } = new Font("Arial", 10);
        public Color VertexTextColor { get; set; } = Color.Black;
        public Color EdgeWeightColor { get; set; } = Color.DarkRed;
        public Color HighlightColor { get; set; } = Color.Red;
        public float ArrowSize { get; set; } = 3.0f; // Размер стрелки для ориентированных ребер
        public int EdgePenWidth { get; set; } = 1;
        public int HighlightedEdgePenWidth { get; set; } = 3;


        public GraphRenderer(Graph graph, Control drawingPanel)
        {
            _graph = graph;
            _panel = drawingPanel;
        }

        public void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias; // Для красивого сглаживания

            // Очищаем предыдущий кадр (если панель не делает это автоматически)
            // g.Clear(_panel.BackColor); // Обычно панель сама себя очищает

            DrawEdges(g);
            DrawVertices(g);
        }

        private void DrawVertices(Graphics g)
        {
            foreach (var vertex in _graph.Vertices)
            {
                // Заливка вершины
                using (var brush = new SolidBrush(vertex.DisplayColor))
                {
                    float x = vertex.Position.X - Vertex.Radius;
                    float y = vertex.Position.Y - Vertex.Radius;
                    g.FillEllipse(brush, x, y, Vertex.Radius * 2, Vertex.Radius * 2);
                }

                // Обводка вершины
                using (var pen = new Pen(Color.Black)) // Или можно сделать настраиваемым
                {
                    float x = vertex.Position.X - Vertex.Radius;
                    float y = vertex.Position.Y - Vertex.Radius;
                    g.DrawEllipse(pen, x, y, Vertex.Radius * 2, Vertex.Radius * 2);
                }

                // Текст (метка) вершины
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (var brush = new SolidBrush(VertexTextColor))
                {
                    g.DrawString(vertex.Label, VertexFont, brush, vertex.Position, sf);
                }
            }
        }

        private void DrawEdges(Graphics g)
        {
            foreach (var edge in _graph.Edges)
            {
                Pen pen;
                if (edge.IsHighlighted)
                {
                    pen = new Pen(HighlightColor, HighlightedEdgePenWidth);
                }
                else
                {
                    pen = new Pen(edge.DisplayColor, EdgePenWidth);
                }

                if (edge.IsDirected)
                {
                    pen.CustomEndCap = new AdjustableArrowCap(ArrowSize / (pen.Width * 0.5f), ArrowSize / (pen.Width * 0.5f), true);
                }

                PointF startPoint = edge.From.Position;
                PointF endPoint = edge.To.Position;

                // Рассчитываем "видимую" длину ребра после корректировки на радиусы вершин
                PointF adjustedStart = startPoint;
                PointF adjustedEnd = endPoint;
                AdjustLinePointsForVertexRadius(ref adjustedStart, ref adjustedEnd, Vertex.Radius);

                // Рисуем само ребро (уже скорректированными точками)
                g.DrawLine(pen, adjustedStart, adjustedEnd);

                // --- Улучшенное размещение текста веса ---
                float dx = endPoint.X - startPoint.X; // Используем оригинальные точки для расчета направления
                float dy = endPoint.Y - startPoint.Y;
                float length = (float)Math.Sqrt(dx * dx + dy * dy);

                // Минимальная длина ребра для отображения веса (чтобы не было каши на коротких ребрах)
                const float minLengthForWeightDisplay = 40.0f; // Подбери экспериментально

                if (length > minLengthForWeightDisplay)
                {
                    PointF midPoint = new PointF(startPoint.X + dx * 0.5f, startPoint.Y + dy * 0.5f);

                    // Нормаль к ребру (не нормированная по длине)
                    float nx = -dy;
                    float ny = dx;

                    // Нормализуем нормаль и задаем величину смещения
                    float normalLength = (float)Math.Sqrt(nx * nx + ny * ny);
                    float offsetAmount = 10.0f; // Величина смещения от линии, подбери экспериментально

                    if (normalLength > 0) // Избегаем деления на ноль, если startPoint == endPoint (хотя length уже проверили)
                    {
                        nx = (nx / normalLength) * offsetAmount;
                        ny = (ny / normalLength) * offsetAmount;
                    }
                    else // Если вдруг normalLength == 0 (хотя не должно быть при length > minLength)
                    {
                        nx = offsetAmount; // Смещаем просто вправо
                        ny = 0;
                    }

                    // Если ребро идет "в основном вниз" (dy > 0), смещаем текст "вверх" относительно направления ребра.
                    // Если ребро идет "в основном вверх" (dy < 0), смещаем текст "вниз".
                    // Это помогает тексту не переворачиваться, если линия почти вертикальная.
                    // Однако, простое смещение по нормали уже должно дать хороший результат.
                    // Для большей предсказуемости, можно всегда смещать в одну "сторону" нормали,
                    // например, ту, что дает положительный ny после нормализации (если dx != 0).
                    // Или, проще: если dy (разница по Y) положительна (линия идет вниз), смещаем "вверх" по нормали (уменьшаем Y смещения, если ny положительный).
                    // Если dy отрицательна (линия идет вверх), смещаем "вниз" по нормали.

                    // Простой вариант: всегда смещать в одну сторону нормали.
                    // Например, чтобы текст был "над" линией, если она идет слева направо.
                    // Если dx < 0 (линия идет справа налево), инвертируем нормаль, чтобы текст был с той же "визуальной" стороны.
                    if (dx < 0)
                    {
                        nx = -nx;
                        ny = -ny;
                    }
                    // Если линия почти вертикальная (dx очень мал), то предыдущее условие может не дать нужного эффекта.
                    // В этом случае, если dy < 0 (линия идет снизу вверх), лучше сместить текст вправо (nx > 0).
                    // Если dy > 0 (линия идет сверху вниз), лучше сместить текст влево (nx < 0).
                    if (Math.Abs(dx) < 0.1 * Math.Abs(dy)) // Если линия почти вертикальная
                    {
                        if (dy < 0)
                        { // Идет снизу вверх
                            nx = offsetAmount; ny = 0; // Смещаем вправо
                        }
                        else
                        { // Идет сверху вниз
                            nx = -offsetAmount; ny = 0; // Смещаем влево
                        }
                    }


                    midPoint.X += nx;
                    midPoint.Y += ny;

                    using (var brush = new SolidBrush(EdgeWeightColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        g.DrawString(edge.Weight.ToString(), EdgeWeightFont, brush, midPoint, sf);
                    }
                }
                // --- Конец улучшенного размещения ---

                pen.Dispose();
            }
        }

        // Вспомогательный метод для корректировки точек линии, чтобы она выходила из края круга вершины
        private void AdjustLinePointsForVertexRadius(ref PointF start, ref PointF end, float radius)
        {
            float dx = end.X - start.X;
            float dy = end.Y - start.Y;
            float length = (float)Math.Sqrt(dx * dx + dy * dy);

            if (length == 0) return; // Вершины в одной точке

            float ratioStart = radius / length;
            float ratioEnd = radius / length;

            start.X += dx * ratioStart;
            start.Y += dy * ratioStart;

            end.X -= dx * ratioEnd;
            end.Y -= dy * ratioEnd;
        }


        public Vertex GetVertexAt(PointF location)
        {
            // Ищем вершину, в которую попал клик мыши
            // Проверяем расстояние от клика до центра каждой вершины
            foreach (var vertex in _graph.Vertices.AsEnumerable().Reverse()) // Reverse, чтобы верхние вершины проверялись первыми, если они перекрываются
            {
                float dx = location.X - vertex.Position.X;
                float dy = location.Y - vertex.Position.Y;
                if (Math.Sqrt(dx * dx + dy * dy) <= Vertex.Radius)
                {
                    return vertex;
                }
            }
            return null; // Вершина не найдена
        }

        // Метод для подсветки ярусов (пример, как можно расширить для визуализации ЯПФ)
        public void HighlightTiers(List<List<Vertex>> tiers)
        {
            // Сначала сбросить все цвета
            foreach (var vertex in _graph.Vertices)
            {
                vertex.DisplayColor = Color.SkyBlue; // Стандартный цвет
            }

            Color[] tierColors = { Color.LightGreen, Color.LightYellow, Color.LightCoral, Color.LightSalmon, Color.LightSeaGreen, Color.LightPink };
            int colorIndex = 0;

            foreach (var tier in tiers)
            {
                foreach (var vertex in tier)
                {
                    vertex.DisplayColor = tierColors[colorIndex % tierColors.Length];
                }
                colorIndex++;
            }
            _panel.Invalidate(); // Запросить перерисовку
        }

        // Метод для сброса всей подсветки (ребер и вершин)
        public void ResetHighlights()
        {
            foreach (var vertex in _graph.Vertices)
            {
                vertex.DisplayColor = Color.SkyBlue; // Стандартный цвет
            }
            foreach (var edge in _graph.Edges)
            {
                edge.IsHighlighted = false;
            }
            _panel.Invalidate();
        }
    }
}