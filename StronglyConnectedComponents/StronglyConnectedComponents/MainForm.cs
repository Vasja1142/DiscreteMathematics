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
            for (int n = 1; n <= MatrixSize * MatrixSize; n++) // ���� �� 1 �� 100
            {
                // ��������� ��� ��������: "checkBox1", "checkBox2", ..., "checkBox100"
                string checkBoxName = $"checkBox{n}";

                // ������� ������� �� ����� �� ����� (� �� ��������� �����������)
                Control[] foundControls = this.Controls.Find(checkBoxName, true);

                if (foundControls.Length > 0 && foundControls[0] is CheckBox cb)
                {
                    // --- ���������� ������� i � j �� ������ n ---
                    // ������ � "��������" ������� (� 0): k = n - 1
                    int k = n - 1;
                    // ������: i = k / 10 (������������� �������)
                    int i = k / MatrixSize;
                    // �������: j = k % 10 (������� �� �������)
                    int j = k % MatrixSize;
                    // --- ����� ����������� �������� ---

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
                    // ���������, ��� ������� ��� ���� ������ ��� ������ � ���������������
                    if (adjacencyCheckBoxes[i, j] != null)
                    {
                        // ���������� 1, ���� �������, ����� 0
                        matrix[i, j] = adjacencyCheckBoxes[i, j].Checked ? true : false;
                    }
                    else
                    {
                        // ���� ������� �� ��� ������, ������ 0 (��� ����������� ������, ���� ��� ��������)
                        matrix[i, j] = false;
                        // ����� ��������: throw new InvalidOperationException($"CheckBox for matrix[{i},{j}] was not initialized.");
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
