// GraphAlgorithmsLab.WinFormsApp/MainForm.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GraphAlgorithmsLab.Core.Entities;
using GraphAlgorithmsLab.WinFormsApp.Rendering;
using GraphAlgorithmsLab.Core.Algorithms; // Для TopologicalSortAlgorithm и других

namespace GraphAlgorithmsLab.WinFormsApp
{
    public partial class MainForm : Form
    {
        private Graph _graph;
        private GraphRenderer _graphRenderer;

        private enum InteractionMode
        {
            Select,
            AddVertex,
            AddDirectedEdge,
            AddUndirectedEdge,
            SelectDijkstraStart,
            SelectDijkstraEnd
        }
        private InteractionMode _currentMode = InteractionMode.Select;

        private Vertex _selectedVertex1 = null;
        private Vertex _selectedVertex2 = null; // Пока не используется активно, но задел на будущее

        // Для хранения оригинальных позиций вершин перед их изменением алгоритмами (например, ЯПФ)
        private Dictionary<int, PointF> _originalVertexPositions = new Dictionary<int, PointF>();
        private bool _isLayoutAlteredByAlgorithm = false;


        public MainForm()
        {
            InitializeComponent();
            InitializeGraphComponents();
            SetupEventHandlers();
            UpdateStatusLabel("Приложение готово. Выберите режим работы.");
            // Изначально кнопка удаления неактивна
            if (Controls.Find("btnDeleteSelected", true).FirstOrDefault() is Button btnDel)
            {
                btnDel.Enabled = false;
            }
        }

        private void InitializeGraphComponents()
        {
            _graph = new Graph();
            // panelGraph - это Panel на форме, убедись, что он существует
            _graphRenderer = new GraphRenderer(_graph, panelGraph);
            // Цвет фона panelGraph устанавливается в дизайнере
        }

        private void SetupEventHandlers()
        {
            panelGraph.Paint += PanelGraph_Paint;
            panelGraph.MouseClick += PanelGraph_MouseClick;
            panelGraph.MouseMove += PanelGraph_MouseMove;

            rbModeSelect.CheckedChanged += ModeRadioButton_CheckedChanged;
            rbModeAddVertex.CheckedChanged += ModeRadioButton_CheckedChanged;
            rbModeAddDirectedEdge.CheckedChanged += ModeRadioButton_CheckedChanged;
            rbModeAddUndirectedEdge.CheckedChanged += ModeRadioButton_CheckedChanged;

            btnClearGraph.Click += BtnClearGraph_Click;
            btnResetHighlights.Click += BtnResetHighlights_Click; // Кнопка для сброса выделения и расположения

            // Кнопка удаления (убедись, что она называется btnDeleteSelected в дизайнере)
            if (Controls.Find("btnDeleteSelected", true).FirstOrDefault() is Button btnDel)
            {
                btnDel.Click += BtnDeleteSelected_Click;
            }


            btnTopologicalSort.Click += BtnTopologicalSort_Click;
            btnDijkstra.Click += BtnDijkstra_Click;
            btnKruskal.Click += BtnKruskal_Click;
        }

        #region Event Handlers for UI Controls

        private void ModeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!(sender is RadioButton rb) || !rb.Checked) return;

            if (rb == rbModeAddVertex) _currentMode = InteractionMode.AddVertex;
            else if (rb == rbModeAddDirectedEdge) _currentMode = InteractionMode.AddDirectedEdge;
            else if (rb == rbModeAddUndirectedEdge) _currentMode = InteractionMode.AddUndirectedEdge;
            else if (rb == rbModeSelect) _currentMode = InteractionMode.Select;

            _selectedVertex1 = null;
            _selectedVertex2 = null;
            ResetVertexVisualState(); // Сбросить подсветку выбранных вершин

            // Активация/деактивация кнопки удаления
            if (Controls.Find("btnDeleteSelected", true).FirstOrDefault() is Button btnDel)
            {
                btnDel.Enabled = (_currentMode == InteractionMode.Select && _selectedVertex1 != null);
            }

            panelGraph.Invalidate();
            UpdateStatusLabel();
        }

        private void BtnClearGraph_Click(object sender, EventArgs e)
        {
            _graph.Clear();
            _selectedVertex1 = null;
            _selectedVertex2 = null;
            _originalVertexPositions.Clear();
            _isLayoutAlteredByAlgorithm = false;
            _graphRenderer.ResetHighlights();

            if (Controls.Find("btnDeleteSelected", true).FirstOrDefault() is Button btnDel)
            {
                btnDel.Enabled = false;
            }

            panelGraph.Invalidate();
            UpdateStatusLabel("Граф очищен.");
        }

        private void BtnResetHighlights_Click(object sender, EventArgs e)
        {
            _selectedVertex1 = null;
            _selectedVertex2 = null;

            // Восстанавливаем оригинальные позиции, если они были изменены
            if (_isLayoutAlteredByAlgorithm)
            {
                foreach (var vertex in _graph.Vertices)
                {
                    if (_originalVertexPositions.TryGetValue(vertex.Id, out PointF originalPos))
                    {
                        vertex.Position = originalPos;
                    }
                }
                _isLayoutAlteredByAlgorithm = false; // Считаем, что расположение восстановлено
                _originalVertexPositions.Clear(); // Очищаем сохраненные, т.к. они применены
            }

            _graphRenderer.ResetHighlights(); // Сбрасывает цвета вершин и подсветку ребер
            panelGraph.Invalidate();
            UpdateStatusLabel("Выделение и расположение сброшены к исходным.");
        }

        private void BtnDeleteSelected_Click(object sender, EventArgs e)
        {
            if (_currentMode != InteractionMode.Select)
            {
                UpdateStatusLabel("Для удаления элемента перейдите в режим 'Выбор/Действие' и выберите вершину.");
                return;
            }

            if (_selectedVertex1 != null)
            {
                Vertex vertexToDelete = _selectedVertex1;
                string vertexLabel = vertexToDelete.Label;

                // Удаляем из сохраненных оригинальных позиций, если есть
                _originalVertexPositions.Remove(vertexToDelete.Id);

                if (_graph.RemoveVertex(vertexToDelete))
                {
                    _selectedVertex1 = null;
                    panelGraph.Invalidate();
                    UpdateStatusLabel($"Вершина {vertexLabel} и связанные с ней ребра удалены.");
                }
                else
                {
                    UpdateStatusLabel($"Не удалось удалить вершину {vertexLabel}.");
                }
                // Обновляем состояние кнопки удаления
                if (Controls.Find("btnDeleteSelected", true).FirstOrDefault() is Button btnDel)
                {
                    btnDel.Enabled = false;
                }
            }
            else
            {
                UpdateStatusLabel("Никакая вершина не выбрана для удаления.");
            }
        }

        #endregion

        #region Panel Drawing and Interaction

        private void PanelGraph_Paint(object sender, PaintEventArgs e)
        {
            _graphRenderer.Draw(e.Graphics);
        }

        private void PanelGraph_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            PointF clickLocation = e.Location;
            Vertex clickedVertex = _graphRenderer.GetVertexAt(clickLocation);

            // Если расположение было изменено алгоритмом, любой клик для добавления/выбора
            // должен сначала предложить сбросить расположение или работать с текущим.
            // Для простоты, пока не будем это усложнять. Пользователь может нажать "Сбросить выделение".

            switch (_currentMode)
            {
                case InteractionMode.AddVertex:
                    HandleAddVertexMode(clickLocation, clickedVertex);
                    break;
                case InteractionMode.AddDirectedEdge:
                case InteractionMode.AddUndirectedEdge:
                    HandleAddEdgeMode(clickedVertex);
                    break;
                case InteractionMode.Select:
                    HandleSelectMode(clickedVertex);
                    break;
                case InteractionMode.SelectDijkstraStart:
                    HandleSelectDijkstraStart(clickedVertex);
                    break;
            }
            panelGraph.Invalidate();
        }

        private void PanelGraph_MouseMove(object sender, MouseEventArgs e)
        {
            // this.Text = $"X: {e.X}, Y: {e.Y}"; 
        }

        private void HandleAddVertexMode(PointF clickLocation, Vertex clickedOnExistingVertex)
        {
            if (clickedOnExistingVertex == null)
            {
                var newVertex = _graph.AddVertex(clickLocation);
                // Если добавляем вершину после изменения расположения ЯПФ, ее позиция будет по клику.
                // Старые вершины останутся на ЯПФ-позициях. Это может быть ожидаемо или нет.
                // Можно добавить ее оригинальную позицию в _originalVertexPositions, если _isLayoutAlteredByAlgorithm == true
                // Но это усложнит логику восстановления. Проще всего - сбросить расположение перед редактированием.
                UpdateStatusLabel($"Вершина {newVertex.Label} добавлена.");
            }
            else
            {
                UpdateStatusLabel("Нельзя добавить вершину поверх существующей.");
            }
        }

        private void HandleAddEdgeMode(Vertex clickedVertex)
        {
            if (clickedVertex == null)
            {
                _selectedVertex1 = null;
                ResetVertexVisualState();
                UpdateStatusLabel("Выберите первую вершину для ребра.");
                return;
            }

            if (_selectedVertex1 == null)
            {
                _selectedVertex1 = clickedVertex;
                _selectedVertex1.DisplayColor = Color.Orange;
                UpdateStatusLabel($"Выбрана первая вершина ({_selectedVertex1.Label}). Выберите вторую.");
            }
            else
            {
                if (_selectedVertex1.Equals(clickedVertex))
                {
                    UpdateStatusLabel("Петли не поддерживаются этим интерфейсом.");
                    return;
                }

                double weight = (double)nudEdgeWeight.Value;
                bool preferDirected = (_currentMode == InteractionMode.AddDirectedEdge);

                bool success = _graph.AddOrUpdateSmartEdge(_selectedVertex1, clickedVertex, weight, preferDirected, out string message);

                UpdateStatusLabel(message);

                _selectedVertex1.DisplayColor = Color.SkyBlue;
                _selectedVertex1 = null;
            }
        }

       

        private void HandleSelectDijkstraStart(Vertex clickedVertex)
        {
            if (clickedVertex != null)
            {
                ResetVertexVisualState();
                _selectedVertex1 = clickedVertex;
                _selectedVertex1.DisplayColor = Color.Gold;
                UpdateStatusLabel($"Стартовая вершина для Дейкстры: {_selectedVertex1.Label}. Нажмите кнопку 'Дейкстра'.");
            }
            else
            {
                UpdateStatusLabel("Кликните на вершину, чтобы выбрать стартовую для Дейкстры.");
            }
        }

        private void ResetVertexVisualState(Vertex exceptThisOne = null)
        {
            foreach (var v in _graph.Vertices)
            {
                if (exceptThisOne != null && v.Equals(exceptThisOne)) continue;
                v.DisplayColor = Color.SkyBlue;
            }
        }

        #endregion

        #region Algorithm Button Handlers

        private void BtnTopologicalSort_Click(object sender, EventArgs e)
        {
            if (!_graph.Vertices.Any())
            {
                UpdateStatusLabel("Граф пуст. Добавьте вершины и ребра.");
                return;
            }

            _graph.ResetAllVerticesAlgorithmData();

            if (!_isLayoutAlteredByAlgorithm)
            {
                _originalVertexPositions.Clear();
                foreach (var vertex in _graph.Vertices)
                {
                    _originalVertexPositions[vertex.Id] = vertex.Position;
                }
            }

            foreach (var v_reset_color in _graph.Vertices) { v_reset_color.DisplayColor = Color.SkyBlue; }
            foreach (var e_reset_color in _graph.Edges) { e_reset_color.IsHighlighted = false; }

            _graph.RecalculateInDegrees();

            TopologicalSortAlgorithm sorter = new TopologicalSortAlgorithm(_graph);
            var (isAcyclic, tiers) = sorter.Sort();

            if (isAcyclic)
            {
                if (tiers.Any())
                {
                    _isLayoutAlteredByAlgorithm = true;

                    float startY = 50;
                    float yStep = 90;
                    float panelWidth = panelGraph.ClientSize.Width > 0 ? panelGraph.ClientSize.Width : 600;

                    // Коэффициент сжатия для шага по X. 1.0f - без сжатия. Меньше -> ближе.
                    float xCompressionFactor = 0.8f; // НАСТРАИВАЕМЫЙ ПАРАМЕТР
                                                     // Минимально допустимое расстояние между центрами соседних вершин
                    float minimumGuaranteedSpacing = Vertex.Radius * 2.0f + 10.0f; // Диаметр + 10px зазора

                    // Набор предопределенных смещений по X (для "разбиения" вертикальных линий между ярусами)
                    float[] xOffsets = { 0, 25.0f, -25.0f, 12.0f, -12.0f }; // Уменьшил амплитуду для более плотного размещения

                    for (int i = 0; i < tiers.Count; i++)
                    {
                        var currentTier = tiers[i];
                        if (!currentTier.Any()) continue;

                        float currentY = startY + i * yStep;
                        float actualXStep;
                        float currentStartX;

                        if (currentTier.Count == 1)
                        {
                            currentStartX = panelWidth / 2;
                            actualXStep = 0;
                        }
                        else
                        {
                            // Доступная ширина для размещения центров крайних вершин
                            float availableWidthForLayout = panelWidth - 2 * (Vertex.Radius + 5);
                            if (availableWidthForLayout < 0) availableWidthForLayout = 0; // Панель слишком узкая

                            float idealFullWidthStep = (currentTier.Count > 1) ? availableWidthForLayout / (currentTier.Count - 1) : 0;

                            float compressedStep = idealFullWidthStep * xCompressionFactor;
                            actualXStep = Math.Max(compressedStep, minimumGuaranteedSpacing);

                            float currentTierWidth = (currentTier.Count - 1) * actualXStep;
                            currentStartX = (panelWidth - currentTierWidth) / 2;

                            if (currentStartX < Vertex.Radius + 5)
                            {
                                currentStartX = Vertex.Radius + 5;
                            }
                        }

                        for (int j = 0; j < currentTier.Count; j++)
                        {
                            Vertex vertex = currentTier[j];
                            float currentX;

                            if (currentTier.Count == 1)
                            {
                                currentX = currentStartX;
                            }
                            else
                            {
                                currentX = currentStartX + j * actualXStep;
                            }

                            int offsetIndex = vertex.Id % xOffsets.Length;
                            float deterministicShiftX = xOffsets[offsetIndex];
                            currentX += deterministicShiftX;

                            float minDrawableX = Vertex.Radius + 5;
                            float maxDrawableX = panelWidth - Vertex.Radius - 5;
                            // Обеспечиваем, чтобы после всех смещений вершина не вылезла за края
                            if (currentX < minDrawableX) currentX = minDrawableX;
                            if (currentX > maxDrawableX) currentX = maxDrawableX;

                            vertex.Position = new PointF(currentX, currentY);
                        }
                    }

                    Color[] tierColors = { Color.LightGreen, Color.LightYellow, Color.LightCoral, Color.Aqua, Color.LightPink, Color.Orange };
                    int tierIndex = 0;
                    string resultText = "Топологическая сортировка (ЯПФ):\n";
                    foreach (var tier in tiers)
                    {
                        resultText += $"Ярус {tierIndex + 1}: ";
                        foreach (var vertex in tier)
                        {
                            if (vertex != null)
                            {
                                vertex.DisplayColor = tierColors[tierIndex % tierColors.Length];
                                resultText += vertex.Label + " ";
                            }
                        }
                        resultText += "\n";
                        tierIndex++;
                    }
                    UpdateStatusLabel(resultText.Trim());
                }
                else
                {
                    UpdateStatusLabel("Топологическая сортировка успешна, но результат (ярусы) пуст.");
                    _isLayoutAlteredByAlgorithm = false;
                }
            }
            else
            {
                UpdateStatusLabel("Граф содержит цикл! Топологическая сортировка невозможна.");
                if (_originalVertexPositions.Any())
                {
                    foreach (var vertex in _graph.Vertices)
                    {
                        if (_originalVertexPositions.TryGetValue(vertex.Id, out PointF originalPos))
                        {
                            vertex.Position = originalPos;
                        }
                    }
                }
                _isLayoutAlteredByAlgorithm = false;
            }

            panelGraph.Invalidate();
        }

        private void BtnDijkstra_Click(object sender, EventArgs e)
        {
            // 1. Сброс состояния перед новым запуском или выбором вершины
            _graph.ResetAllVerticesAlgorithmData(); // Сбрасывает Distance, Predecessor, Visited
            _graphRenderer.ResetHighlights();      // Сбрасывает цвета вершин и подсветку ребер
            RestoreOriginalPositionsIfNeeded();    // Восстановить позиции, если они были изменены ЯПФ

            // 2. Логика выбора стартовой вершины
            if (_currentMode != InteractionMode.SelectDijkstraStart && _selectedVertex1 == null)
            {
                // Если мы еще не в режиме выбора старта И стартовая вершина не выбрана ранее,
                // переключаемся в режим выбора стартовой вершины.
                _currentMode = InteractionMode.SelectDijkstraStart;
                ResetVertexVisualState(); // Сбросить предыдущие выделения
                UpdateStatusLabel("Режим: Выберите СТАРТОВУЮ вершину для алгоритма Дейкстры.");
                panelGraph.Invalidate();
                return; // Ждем клика пользователя для выбора стартовой вершины
            }

            if (_selectedVertex1 == null) // Если стартовая вершина так и не была выбрана
            {
                UpdateStatusLabel("Стартовая вершина для Дейкстры не выбрана! Кликните на вершину.");
                _currentMode = InteractionMode.SelectDijkstraStart; // Остаемся в режиме выбора
                return;
            }

            // 3. Стартовая вершина выбрана (_selectedVertex1) - запускаем алгоритм
            Vertex dijkstraStartVertex = _selectedVertex1;
            UpdateStatusLabel($"Запуск алгоритма Дейкстры от вершины: {dijkstraStartVertex.Label}...");
            panelGraph.Invalidate(); // Обновить, если были изменения в статусе/цветах

            DijkstraAlgorithm dijkstra = new DijkstraAlgorithm(_graph);
            dijkstra.Run(dijkstraStartVertex);

            UpdateStatusLabel($"Алгоритм Дейкстры выполнен от {dijkstraStartVertex.Label}. " +
                              "Кликните на другие вершины в режиме 'Выбор', чтобы увидеть путь и расстояние.");

            // 4. Визуализация и отображение результатов
            // Результаты (Distance и Predecessor) теперь хранятся в самих вершинах.
            // Мы можем подсветить пути при выборе целевой вершины или просто отображать информацию.
            // Для примера, подсветим стартовую вершину особым цветом.
            dijkstraStartVertex.DisplayColor = Color.Gold; // Цвет стартовой вершины

            // Сбрасываем _selectedVertex1, чтобы следующий клик в режиме Select показывал путь
            // _selectedVertex1 = null; // Или оставляем, чтобы сразу видеть ее данные

            // Переключаемся в режим "Выбор", чтобы пользователь мог кликать на вершины и видеть пути
            rbModeSelect.Checked = true; // Это вызовет ModeRadioButton_CheckedChanged, который сменит _currentMode
                                         // и вызовет UpdateStatusLabel для режима Select.

            // В HandleSelectMode мы уже выводим Distance и Predecessor.
            // Теперь нужно добавить подсветку пути к выбранной вершине.
            panelGraph.Invalidate();
        }

        // Модифицируем HandleSelectMode для отображения пути Дейкстры
        private void HandleSelectMode(Vertex clickedVertex)
        {
            // Сначала сбрасываем любую предыдущую подсветку пути и вершин (кроме стартовой Дейкстры, если она есть)
            _graphRenderer.ResetHighlights(); // Сбросит IsHighlighted у ребер и цвета вершин на стандартные

            // Если есть стартовая вершина Дейкстры, восстановим ее цвет
            Vertex dijkstraRunStartNode = _graph.Vertices.FirstOrDefault(v => v.Distance == 0 && v.Predecessor == null);
            if (dijkstraRunStartNode != null && dijkstraRunStartNode.Visited) // Проверяем, что Дейкстра был запущен от нее
            {
                dijkstraRunStartNode.DisplayColor = Color.Gold;
            }


            if (Controls.Find("btnDeleteSelected", true).FirstOrDefault() is Button btnDel)
            {
                btnDel.Enabled = false;
            }

            if (clickedVertex != null)
            {
                _selectedVertex1 = clickedVertex; // Запоминаем текущую выбранную вершину
                _selectedVertex1.DisplayColor = Color.LightGreen; // Подсветить выбранную (целевую)

                string statusMessage = $"Выбрана вершина: {clickedVertex.Label}. ";
                if (clickedVertex.Distance == double.PositiveInfinity)
                {
                    statusMessage += "Недостижима от стартовой вершины.";
                }
                else
                {
                    statusMessage += $"Расстояние: {clickedVertex.Distance}. ";
                    if (clickedVertex.Predecessor != null)
                    {
                        statusMessage += $"Предшественник: {clickedVertex.Predecessor.Label}.";
                    }
                    else if (clickedVertex.Distance == 0)
                    { // Это стартовая вершина
                        statusMessage += "Это стартовая вершина.";
                    }
                    else
                    {
                        statusMessage += "Предшественник не определен (возможно, ошибка или стартовая не была достижима).";
                    }

                    // Восстанавливаем и подсвечиваем путь
                    if (dijkstraRunStartNode != null && clickedVertex.Distance > 0) // Не для самой стартовой
                    {
                        DijkstraAlgorithm pathFinder = new DijkstraAlgorithm(_graph); // Можно создать один раз и переиспользовать
                        List<Edge> pathToClicked = pathFinder.GetPath(clickedVertex);
                        if (pathToClicked.Any())
                        {
                            statusMessage += " Путь: ";
                            foreach (var edgeInPath in pathToClicked)
                            {
                                edgeInPath.IsHighlighted = true;
                                // Добавляем метки вершин в сообщение о пути
                                // (нужно определить, как лучше отобразить путь текстом)
                            }
                        }
                    }
                }
                UpdateStatusLabel(statusMessage);

                if (Controls.Find("btnDeleteSelected", true).FirstOrDefault() is Button btnDelExisting)
                {
                    btnDelExisting.Enabled = true;
                }
            }
            else // Кликнули мимо вершин
            {
                _selectedVertex1 = null;
                UpdateStatusLabel("Выберите действие или элемент.");
            }
            panelGraph.Invalidate(); // Перерисовать для отображения подсветки
        }


        private void BtnKruskal_Click(object sender, EventArgs e)
        {
            if (!_graph.Vertices.Any())
            {
                UpdateStatusLabel("Граф пуст для алгоритма Краскала.");
                return;
            }
            if (_graph.Vertices.Count < 2)
            {
                UpdateStatusLabel("Для алгоритма Краскала нужно как минимум 2 вершины.");
                return;
            }

            // 1. Сброс состояния
            _graph.ResetAllVerticesAlgorithmData(); // Сбросит ComponentId (используемый DSU внутри себя через Id)
            _graphRenderer.ResetHighlights();      // Сбросить цвета вершин и подсветку ребер
            RestoreOriginalPositionsIfNeeded();    // Восстановить позиции, если они были изменены ЯПФ

            UpdateStatusLabel("Запуск алгоритма Краскала для поиска минимального остова...");
            panelGraph.Invalidate(); // Обновить, если были изменения

            KruskalAlgorithm kruskal = new KruskalAlgorithm(_graph);
            var (mstEdges, totalWeight) = kruskal.FindMinimumSpanningTree();

            if (mstEdges.Any())
            {
                // Сначала сбрасываем подсветку всех ребер, если она была от других операций
                foreach (var edge in _graph.Edges)
                {
                    edge.IsHighlighted = false;
                }
                // Подсвечиваем ребра, вошедшие в MST
                foreach (var edgeInMst in mstEdges)
                {
                    edgeInMst.IsHighlighted = true;
                }

                string message = $"Алгоритм Краскала: Минимальное остовное дерево найдено.\n" +
                                 $"Количество ребер в остове: {mstEdges.Count}.\n" +
                                 $"Общий вес остова: {totalWeight}.";

                if (_graph.Vertices.Count > 0 && mstEdges.Count < _graph.Vertices.Count - 1)
                {
                    message += "\nВнимание: Граф может быть несвязным, построен остовный лес.";
                }
                UpdateStatusLabel(message);
            }
            else
            {
                if (_graph.Vertices.Count > 1) // Если есть вершины, но остов не найден
                    UpdateStatusLabel("Алгоритм Краскала: Не удалось построить остовное дерево (возможно, граф несвязный или нет ребер).");
                else // Если всего 1 вершина или 0
                    UpdateStatusLabel("Алгоритм Краскала: Остовное дерево не применимо к графу с менее чем 2 вершинами.");
            }

            panelGraph.Invalidate(); // Перерисовать граф с подсвеченным MST
        }

        #endregion

        private void RestoreOriginalPositionsIfNeeded()
        {
            if (_isLayoutAlteredByAlgorithm && _originalVertexPositions.Any())
            {
                foreach (var vertex in _graph.Vertices)
                {
                    if (_originalVertexPositions.TryGetValue(vertex.Id, out PointF originalPos))
                    {
                        vertex.Position = originalPos;
                    }
                }
                // _originalVertexPositions.Clear(); // Не очищаем, чтобы можно было несколько раз сбрасывать
                _isLayoutAlteredByAlgorithm = false;
                UpdateStatusLabel("Расположение вершин восстановлено к исходному.");
            }
        }

        private void UpdateStatusLabel(string message = null)
        {
            // Убедимся, что txtMessages существует (если он добавляется динамически, хотя обычно через дизайнер)
            var txtMessagesControl = Controls.Find("txtMessages", true).FirstOrDefault() as TextBox;
            if (txtMessagesControl == null) return;


            if (message != null)
            {
                txtMessagesControl.AppendText(message + Environment.NewLine);
            }
            else
            {
                string modeStatus = "";
                switch (_currentMode)
                {
                    case InteractionMode.AddVertex:
                        modeStatus = "Режим: Добавление вершин. Кликните на свободное место.";
                        break;
                    case InteractionMode.AddDirectedEdge:
                        modeStatus = _selectedVertex1 == null ? "Режим: Добавление ор. ребра. Выберите начальную вершину." : $"Выберите конечную вершину для ребра от {_selectedVertex1.Label}.";
                        break;
                    case InteractionMode.AddUndirectedEdge:
                        modeStatus = _selectedVertex1 == null ? "Режим: Добавление неор. ребра. Выберите первую вершину." : $"Выберите вторую вершину для ребра с {_selectedVertex1.Label}.";
                        break;
                    case InteractionMode.SelectDijkstraStart:
                        modeStatus = "Режим: Выберите стартовую вершину для алгоритма Дейкстры.";
                        break;
                    case InteractionMode.Select:
                    default:
                        modeStatus = "Режим: Выбор/Действие. Выберите элемент или запустите алгоритм.";
                        break;
                }
                txtMessagesControl.AppendText(modeStatus + Environment.NewLine);
            }
            txtMessagesControl.ScrollToCaret();
        }
    }
}