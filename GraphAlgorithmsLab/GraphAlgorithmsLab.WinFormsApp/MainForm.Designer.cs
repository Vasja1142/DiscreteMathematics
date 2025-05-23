namespace GraphAlgorithmsLab.WinFormsApp
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panelGraph = new Panel();
            panelControls = new Panel();
            txtMessages = new TextBox();
            groupBox4 = new GroupBox();
            btnKruskal = new Button();
            btnDijkstra = new Button();
            btnTopologicalSort = new Button();
            groupBox3 = new GroupBox();
            btnDeleteSelected = new Button();
            btnResetHighlights = new Button();
            btnClearGraph = new Button();
            groupBox2 = new GroupBox();
            nudEdgeWeight = new NumericUpDown();
            label1 = new Label();
            groupBox1 = new GroupBox();
            rbModeSelect = new RadioButton();
            rbModeAddUndirectedEdge = new RadioButton();
            rbModeAddDirectedEdge = new RadioButton();
            rbModeAddVertex = new RadioButton();
            panelControls.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudEdgeWeight).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // panelGraph
            // 
            panelGraph.BackColor = Color.FromArgb(255, 224, 192);
            panelGraph.Location = new Point(221, 0);
            panelGraph.Name = "panelGraph";
            panelGraph.Size = new Size(759, 558);
            panelGraph.TabIndex = 1;
            // 
            // panelControls
            // 
            panelControls.AutoScroll = true;
            panelControls.AutoSize = true;
            panelControls.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelControls.BackColor = Color.LightGray;
            panelControls.Controls.Add(txtMessages);
            panelControls.Controls.Add(groupBox4);
            panelControls.Controls.Add(groupBox3);
            panelControls.Controls.Add(groupBox2);
            panelControls.Controls.Add(groupBox1);
            panelControls.Dock = DockStyle.Left;
            panelControls.Location = new Point(0, 0);
            panelControls.Name = "panelControls";
            panelControls.Size = new Size(218, 558);
            panelControls.TabIndex = 0;
            // 
            // txtMessages
            // 
            txtMessages.Location = new Point(3, 391);
            txtMessages.Multiline = true;
            txtMessages.Name = "txtMessages";
            txtMessages.ReadOnly = true;
            txtMessages.ScrollBars = ScrollBars.Vertical;
            txtMessages.Size = new Size(212, 167);
            txtMessages.TabIndex = 4;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(btnKruskal);
            groupBox4.Controls.Add(btnDijkstra);
            groupBox4.Controls.Add(btnTopologicalSort);
            groupBox4.Location = new Point(12, 298);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(183, 87);
            groupBox4.TabIndex = 3;
            groupBox4.TabStop = false;
            groupBox4.Text = "Алгоритмы";
            // 
            // btnKruskal
            // 
            btnKruskal.Location = new Point(95, 51);
            btnKruskal.Name = "btnKruskal";
            btnKruskal.Size = new Size(75, 23);
            btnKruskal.TabIndex = 2;
            btnKruskal.Text = "Краскал";
            btnKruskal.UseVisualStyleBackColor = true;
            // 
            // btnDijkstra
            // 
            btnDijkstra.Location = new Point(6, 51);
            btnDijkstra.Name = "btnDijkstra";
            btnDijkstra.Size = new Size(75, 23);
            btnDijkstra.TabIndex = 1;
            btnDijkstra.Text = "Дейкстра";
            btnDijkstra.UseVisualStyleBackColor = true;
            // 
            // btnTopologicalSort
            // 
            btnTopologicalSort.Location = new Point(6, 22);
            btnTopologicalSort.Name = "btnTopologicalSort";
            btnTopologicalSort.Size = new Size(164, 23);
            btnTopologicalSort.TabIndex = 0;
            btnTopologicalSort.Text = "ЯПФ (Топологическая)";
            btnTopologicalSort.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btnDeleteSelected);
            groupBox3.Controls.Add(btnResetHighlights);
            groupBox3.Controls.Add(btnClearGraph);
            groupBox3.Location = new Point(12, 191);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(183, 101);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "Управление";
            // 
            // btnDeleteSelected
            // 
            btnDeleteSelected.Location = new Point(6, 69);
            btnDeleteSelected.Name = "btnDeleteSelected";
            btnDeleteSelected.Size = new Size(164, 23);
            btnDeleteSelected.TabIndex = 2;
            btnDeleteSelected.Text = "Удалить выделенное";
            btnDeleteSelected.UseVisualStyleBackColor = true;
            btnDeleteSelected.Click += BtnDeleteSelected_Click;
            // 
            // btnResetHighlights
            // 
            btnResetHighlights.Location = new Point(95, 22);
            btnResetHighlights.Name = "btnResetHighlights";
            btnResetHighlights.Size = new Size(75, 41);
            btnResetHighlights.TabIndex = 1;
            btnResetHighlights.Text = "Сбросить выделение";
            btnResetHighlights.UseCompatibleTextRendering = true;
            btnResetHighlights.UseVisualStyleBackColor = true;
            // 
            // btnClearGraph
            // 
            btnClearGraph.Location = new Point(6, 22);
            btnClearGraph.Name = "btnClearGraph";
            btnClearGraph.Size = new Size(75, 41);
            btnClearGraph.TabIndex = 0;
            btnClearGraph.Text = "Очистить граф";
            btnClearGraph.UseCompatibleTextRendering = true;
            btnClearGraph.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(nudEdgeWeight);
            groupBox2.Controls.Add(label1);
            groupBox2.Location = new Point(12, 129);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(183, 56);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Параметры ребра";
            // 
            // nudEdgeWeight
            // 
            nudEdgeWeight.DecimalPlaces = 1;
            nudEdgeWeight.Location = new Point(82, 22);
            nudEdgeWeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudEdgeWeight.Name = "nudEdgeWeight";
            nudEdgeWeight.Size = new Size(88, 23);
            nudEdgeWeight.TabIndex = 1;
            nudEdgeWeight.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(11, 24);
            label1.Name = "label1";
            label1.Size = new Size(65, 15);
            label1.TabIndex = 0;
            label1.Text = "Вес ребра:";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rbModeSelect);
            groupBox1.Controls.Add(rbModeAddUndirectedEdge);
            groupBox1.Controls.Add(rbModeAddDirectedEdge);
            groupBox1.Controls.Add(rbModeAddVertex);
            groupBox1.Location = new Point(12, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(183, 120);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Режим работы";
            // 
            // rbModeSelect
            // 
            rbModeSelect.AutoSize = true;
            rbModeSelect.Checked = true;
            rbModeSelect.Location = new Point(9, 95);
            rbModeSelect.Name = "rbModeSelect";
            rbModeSelect.Size = new Size(118, 19);
            rbModeSelect.TabIndex = 3;
            rbModeSelect.TabStop = true;
            rbModeSelect.Text = "Выбор/Действие";
            rbModeSelect.UseVisualStyleBackColor = true;
            // 
            // rbModeAddUndirectedEdge
            // 
            rbModeAddUndirectedEdge.AutoSize = true;
            rbModeAddUndirectedEdge.Location = new Point(9, 70);
            rbModeAddUndirectedEdge.Name = "rbModeAddUndirectedEdge";
            rbModeAddUndirectedEdge.Size = new Size(147, 19);
            rbModeAddUndirectedEdge.TabIndex = 2;
            rbModeAddUndirectedEdge.TabStop = true;
            rbModeAddUndirectedEdge.Text = "Добавить неор. ребро";
            rbModeAddUndirectedEdge.UseVisualStyleBackColor = true;
            // 
            // rbModeAddDirectedEdge
            // 
            rbModeAddDirectedEdge.AutoSize = true;
            rbModeAddDirectedEdge.Location = new Point(9, 45);
            rbModeAddDirectedEdge.Name = "rbModeAddDirectedEdge";
            rbModeAddDirectedEdge.Size = new Size(134, 19);
            rbModeAddDirectedEdge.TabIndex = 1;
            rbModeAddDirectedEdge.TabStop = true;
            rbModeAddDirectedEdge.Text = "Добавить ор. ребро";
            rbModeAddDirectedEdge.UseVisualStyleBackColor = true;
            // 
            // rbModeAddVertex
            // 
            rbModeAddVertex.AutoSize = true;
            rbModeAddVertex.Location = new Point(9, 20);
            rbModeAddVertex.Name = "rbModeAddVertex";
            rbModeAddVertex.Size = new Size(130, 19);
            rbModeAddVertex.TabIndex = 0;
            rbModeAddVertex.TabStop = true;
            rbModeAddVertex.Text = "Добавить вершину";
            rbModeAddVertex.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(980, 558);
            Controls.Add(panelControls);
            Controls.Add(panelGraph);
            Name = "MainForm";
            Text = "Основные алгоритмы на графах";
            panelControls.ResumeLayout(false);
            panelControls.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudEdgeWeight).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Panel panelGraph;
        private Panel panelControls;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private NumericUpDown nudEdgeWeight;
        private Label label1;
        private RadioButton rbModeSelect;
        private RadioButton rbModeAddUndirectedEdge;
        private RadioButton rbModeAddDirectedEdge;
        private RadioButton rbModeAddVertex;
        private GroupBox groupBox4;
        private GroupBox groupBox3;
        private Button btnResetHighlights;
        private Button btnClearGraph;
        private Button btnKruskal;
        private Button btnDijkstra;
        private Button btnTopologicalSort;
        private TextBox txtMessages;
        private Button btnDeleteSelected;
    }
}