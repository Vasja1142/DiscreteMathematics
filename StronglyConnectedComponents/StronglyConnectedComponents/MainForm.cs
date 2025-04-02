using System.Text;

namespace StronglyConnectedComponents
{
    public partial class MainForm : Form
    {
        public static CheckBox[,] adjacencyCheckBoxes;
        private const int MatrixSize = 10;
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

        public static bool[,] GetAdjacencyMatrix()
        {
            bool[,] matrix = new bool[MatrixSize, MatrixSize];

            for (int i = 0; i < MatrixSize; i++)
            {
                for (int j = 0; j < MatrixSize; j++)
                {
                    // Проверяем, что чекбокс для этой ячейки был найден и инициализирован
                    if (adjacencyCheckBoxes[i, j] != null)
                    {
                        // Записываем 1, если отмечен, иначе 0
                        matrix[i, j] = adjacencyCheckBoxes[i, j].Checked ? true : false;
                    }
                    else
                    {
                        // Если чекбокс не был найден, ставим 0 (или выбрасываем ошибку, если это критично)
                        matrix[i, j] = false;
                        // Можно добавить: throw new InvalidOperationException($"CheckBox for matrix[{i},{j}] was not initialized.");
                    }
                }
            }
            return matrix;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Graph mainGraph = new Graph(MatrixSize);
            mainGraph.AddGraphWithForm();

            textBox1.Text = mainGraph.SccFinder();
            

        }

    }
}
