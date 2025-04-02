using System.Text;

namespace StronglyConnectedComponents
{
    public partial class MainForm : Form
    {
        private CheckBox[,] adjacencyCheckBoxes;
        private const int MatrixSize = 10;
        Random randomGenerator = new Random();
        public MainForm()
        {
            InitializeComponent();
            InitializeCheckBoxArray();
        }


        private void InitializeCheckBoxArray()
        {
            adjacencyCheckBoxes = new CheckBox[MatrixSize, MatrixSize];
            for (int n = 1; n <= MatrixSize * MatrixSize; n++) // Идем от 1 до 100
            {
                // Формируем имя чекбокса: "checkBox1", "checkBox2", ..., "checkBox100"
                string checkBoxName = $"checkBox{n}";

                // Находим контрол по имени на форме (и во вложенных контейнерах)
                Control[] foundControls = this.Controls.Find(checkBoxName, true);

                if (foundControls.Length > 0 && foundControls[0] is CheckBox cb)
                {
                    // --- Определяем индексы i и j по номеру n ---
                    // Индекс в "линейном" массиве (с 0): k = n - 1
                    int k = n - 1;
                    // Строка: i = k / 10 (целочисленное деление)
                    int i = k / MatrixSize;
                    // Столбец: j = k % 10 (остаток от деления)
                    int j = k % MatrixSize;
                    // --- Конец определения индексов ---

                    adjacencyCheckBoxes[i, j] = cb;

                }

            }
        }


        public bool[,] GetAdjacencyMatrix()
        {
            bool[,] matrix = new bool[MatrixSize, MatrixSize];

            for (int i = 0; i < MatrixSize; i++)
            {
                for (int j = 0; j < MatrixSize; j++)
                {
                    matrix[i, j] = adjacencyCheckBoxes[i, j] != null ? adjacencyCheckBoxes[i, j].Checked : false;
                }
            }

            return matrix;
        }


        private void CalculateSccButton_Click(object sender, EventArgs e)
        {
            Graph mainGraph = new Graph(GetAdjacencyMatrix());
            textBox1.Text = mainGraph.SccFinder();

        }

        private void GenerateGraphButton_Click(object sender, EventArgs e)
        {
            int randomArcs = randomGenerator.Next(5, 25);
            int randomVertex;
            for (int i = 0; i < randomArcs; i++)
            {
                randomVertex = randomGenerator.Next(1, MatrixSize * MatrixSize + 1);

                string checkBoxName = $"checkBox{randomVertex}";

                // Находим контрол по имени на форме (и во вложенных контейнерах)
                Control[] foundControls = this.Controls.Find(checkBoxName, true);

                if (foundControls.Length > 0 && foundControls[0] is CheckBox cb)
                {
                    cb.Checked = true;
                }

            }
        }

        private void ClearGraphButton_Click(object sender, EventArgs e)
        {
            for (int n = 1; n <= MatrixSize * MatrixSize; n++) // Идем от 1 до 100
            {
                string checkBoxName = $"checkBox{n}";

                Control[] foundControls = this.Controls.Find(checkBoxName, true);

                if (foundControls.Length > 0 && foundControls[0] is CheckBox cb)
                {
                    cb.Checked = false;
                }

            }
        }
    }
}
