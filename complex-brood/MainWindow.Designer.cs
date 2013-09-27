namespace complex_brood
{
    partial class MainWindow
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
            if(disposing && (components != null))
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
            System.Windows.Forms.Label labelCenterX;
            System.Windows.Forms.Label labelCenterY;
            System.Windows.Forms.Label labelScale;
            System.Windows.Forms.Label labelMaxIterations;
            System.Windows.Forms.Label labelViewWidth;
            this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.spinnerScale = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.spinnerCenterX = new System.Windows.Forms.NumericUpDown();
            this.spinnerCenterY = new System.Windows.Forms.NumericUpDown();
            this.btnGo = new System.Windows.Forms.Button();
            this.mandelDisplay = new complex_brood.MandelDisplay();
            labelCenterX = new System.Windows.Forms.Label();
            labelCenterY = new System.Windows.Forms.Label();
            labelScale = new System.Windows.Forms.Label();
            labelMaxIterations = new System.Windows.Forms.Label();
            labelViewWidth = new System.Windows.Forms.Label();
            this.tableLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerCenterX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerCenterY)).BeginInit();
            this.SuspendLayout();
            // 
            // labelCenterX
            // 
            labelCenterX.AutoSize = true;
            labelCenterX.Dock = System.Windows.Forms.DockStyle.Fill;
            labelCenterX.Location = new System.Drawing.Point(3, 0);
            labelCenterX.Name = "labelCenterX";
            labelCenterX.Size = new System.Drawing.Size(69, 32);
            labelCenterX.TabIndex = 1;
            labelCenterX.Text = "Midden X:";
            labelCenterX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelCenterY
            // 
            labelCenterY.AutoSize = true;
            labelCenterY.Dock = System.Windows.Forms.DockStyle.Fill;
            labelCenterY.Location = new System.Drawing.Point(3, 32);
            labelCenterY.Name = "labelCenterY";
            labelCenterY.Size = new System.Drawing.Size(69, 32);
            labelCenterY.TabIndex = 2;
            labelCenterY.Text = "Midden Y:";
            labelCenterY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelScale
            // 
            labelScale.AutoSize = true;
            labelScale.Dock = System.Windows.Forms.DockStyle.Fill;
            labelScale.Location = new System.Drawing.Point(186, 0);
            labelScale.Name = "labelScale";
            labelScale.Size = new System.Drawing.Size(139, 32);
            labelScale.TabIndex = 3;
            labelScale.Text = "Pixelbreedte:";
            labelScale.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelMaxIterations
            // 
            labelMaxIterations.AutoSize = true;
            labelMaxIterations.Dock = System.Windows.Forms.DockStyle.Fill;
            labelMaxIterations.Location = new System.Drawing.Point(186, 32);
            labelMaxIterations.Name = "labelMaxIterations";
            labelMaxIterations.Size = new System.Drawing.Size(139, 32);
            labelMaxIterations.TabIndex = 4;
            labelMaxIterations.Text = "Maximaal aantal iteraties:";
            labelMaxIterations.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelViewWidth
            // 
            labelViewWidth.AutoSize = true;
            labelViewWidth.Dock = System.Windows.Forms.DockStyle.Fill;
            labelViewWidth.Location = new System.Drawing.Point(439, 0);
            labelViewWidth.Name = "labelViewWidth";
            labelViewWidth.Size = new System.Drawing.Size(114, 32);
            labelViewWidth.TabIndex = 5;
            labelViewWidth.Text = "Weergave breedte:";
            labelViewWidth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayout
            // 
            this.tableLayout.ColumnCount = 6;
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 145F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tableLayout.Controls.Add(labelCenterY, 0, 1);
            this.tableLayout.Controls.Add(this.mandelDisplay, 0, 2);
            this.tableLayout.Controls.Add(labelCenterX, 0, 0);
            this.tableLayout.Controls.Add(labelScale, 2, 0);
            this.tableLayout.Controls.Add(labelMaxIterations, 2, 1);
            this.tableLayout.Controls.Add(labelViewWidth, 4, 0);
            this.tableLayout.Controls.Add(this.spinnerScale, 3, 0);
            this.tableLayout.Controls.Add(this.numericUpDown2, 3, 1);
            this.tableLayout.Controls.Add(this.numericUpDown3, 5, 0);
            this.tableLayout.Controls.Add(this.spinnerCenterX, 1, 0);
            this.tableLayout.Controls.Add(this.spinnerCenterY, 1, 1);
            this.tableLayout.Controls.Add(this.btnGo, 5, 1);
            this.tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout.Location = new System.Drawing.Point(0, 0);
            this.tableLayout.Name = "tableLayout";
            this.tableLayout.RowCount = 3;
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayout.Size = new System.Drawing.Size(670, 400);
            this.tableLayout.TabIndex = 0;
            // 
            // spinnerScale
            // 
            this.spinnerScale.DecimalPlaces = 10;
            this.spinnerScale.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spinnerScale.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.spinnerScale.Location = new System.Drawing.Point(331, 6);
            this.spinnerScale.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.spinnerScale.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spinnerScale.Name = "spinnerScale";
            this.spinnerScale.Size = new System.Drawing.Size(102, 20);
            this.spinnerScale.TabIndex = 6;
            this.spinnerScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericUpDown2.Location = new System.Drawing.Point(331, 38);
            this.numericUpDown2.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDown2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(102, 20);
            this.numericUpDown2.TabIndex = 7;
            this.numericUpDown2.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.DecimalPlaces = 10;
            this.numericUpDown3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericUpDown3.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDown3.Location = new System.Drawing.Point(559, 6);
            this.numericUpDown3.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(108, 20);
            this.numericUpDown3.TabIndex = 8;
            // 
            // spinnerCenterX
            // 
            this.spinnerCenterX.DecimalPlaces = 10;
            this.spinnerCenterX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spinnerCenterX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.spinnerCenterX.Location = new System.Drawing.Point(78, 6);
            this.spinnerCenterX.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.spinnerCenterX.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.spinnerCenterX.Name = "spinnerCenterX";
            this.spinnerCenterX.Size = new System.Drawing.Size(102, 20);
            this.spinnerCenterX.TabIndex = 9;
            // 
            // spinnerCenterY
            // 
            this.spinnerCenterY.DecimalPlaces = 10;
            this.spinnerCenterY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spinnerCenterY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.spinnerCenterY.Location = new System.Drawing.Point(78, 38);
            this.spinnerCenterY.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.spinnerCenterY.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.spinnerCenterY.Name = "spinnerCenterY";
            this.spinnerCenterY.Size = new System.Drawing.Size(102, 20);
            this.spinnerCenterY.TabIndex = 10;
            // 
            // btnGo
            // 
            this.btnGo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnGo.Location = new System.Drawing.Point(559, 35);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(108, 26);
            this.btnGo.TabIndex = 11;
            this.btnGo.Text = "Geef weer";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // mandelDisplay
            // 
            this.tableLayout.SetColumnSpan(this.mandelDisplay, 6);
            this.mandelDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mandelDisplay.Location = new System.Drawing.Point(3, 67);
            this.mandelDisplay.Name = "mandelDisplay";
            this.mandelDisplay.Size = new System.Drawing.Size(664, 330);
            this.mandelDisplay.TabIndex = 0;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 400);
            this.Controls.Add(this.tableLayout);
            this.MinimumSize = new System.Drawing.Size(686, 300);
            this.Name = "MainWindow";
            this.Text = "Complex brood";
            this.tableLayout.ResumeLayout(false);
            this.tableLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerCenterX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerCenterY)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayout;
        private MandelDisplay mandelDisplay;
        private System.Windows.Forms.NumericUpDown spinnerScale;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.NumericUpDown spinnerCenterX;
        private System.Windows.Forms.NumericUpDown spinnerCenterY;
        private System.Windows.Forms.Button btnGo;

    }
}