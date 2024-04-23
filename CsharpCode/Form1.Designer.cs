namespace RoboticArmUI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            comPortComboBox = new ComboBox();
            buttonRefresh = new Button();
            buttonLeft = new Button();
            buttonRight = new Button();
            buttonDown = new Button();
            buttonUp = new Button();
            buttonBackward = new Button();
            buttonForward = new Button();
            textBoxArduino = new TextBox();
            numericUpDown1 = new NumericUpDown();
            button1 = new Button();
            button2 = new Button();
            checkBox1 = new CheckBox();
            timer1 = new System.Windows.Forms.Timer(components);
            chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            pictureBoxVideo = new PictureBox();
            pictureBoxContour = new PictureBox();
            checkBox2 = new CheckBox();
            label1 = new Label();
            button3 = new Button();
            label2 = new Label();
            checkBox3 = new CheckBox();
            button4 = new Button();
            checkBox4 = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chart1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxVideo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxContour).BeginInit();
            SuspendLayout();
            // 
            // comPortComboBox
            // 
            comPortComboBox.FormattingEnabled = true;
            comPortComboBox.Location = new Point(29, 38);
            comPortComboBox.Name = "comPortComboBox";
            comPortComboBox.Size = new Size(151, 28);
            comPortComboBox.TabIndex = 0;
            comPortComboBox.SelectedIndexChanged += ComPortComboBox_SelectedIndexChanged;
            // 
            // buttonRefresh
            // 
            buttonRefresh.Location = new Point(205, 38);
            buttonRefresh.Name = "buttonRefresh";
            buttonRefresh.Size = new Size(94, 29);
            buttonRefresh.TabIndex = 1;
            buttonRefresh.Text = "Refresh";
            buttonRefresh.UseVisualStyleBackColor = true;
            buttonRefresh.Click += RefreshButton_Click;
            // 
            // buttonLeft
            // 
            buttonLeft.Location = new Point(509, 38);
            buttonLeft.Name = "buttonLeft";
            buttonLeft.Size = new Size(94, 29);
            buttonLeft.TabIndex = 2;
            buttonLeft.Tag = "left";
            buttonLeft.Text = "Left";
            buttonLeft.UseVisualStyleBackColor = true;
            buttonLeft.MouseDown += Button_Click;
            buttonLeft.MouseUp += Button_Release;
            // 
            // buttonRight
            // 
            buttonRight.Location = new Point(638, 37);
            buttonRight.Name = "buttonRight";
            buttonRight.Size = new Size(94, 29);
            buttonRight.TabIndex = 3;
            buttonRight.Tag = "right";
            buttonRight.Text = "Right";
            buttonRight.UseVisualStyleBackColor = true;
            buttonRight.MouseDown += Button_Click;
            buttonRight.MouseUp += Button_Release;
            // 
            // buttonDown
            // 
            buttonDown.Location = new Point(638, 100);
            buttonDown.Name = "buttonDown";
            buttonDown.Size = new Size(94, 29);
            buttonDown.TabIndex = 5;
            buttonDown.Tag = "down";
            buttonDown.Text = "Down";
            buttonDown.UseVisualStyleBackColor = true;
            buttonDown.MouseDown += Button_Click;
            buttonDown.MouseUp += Button_Release;
            // 
            // buttonUp
            // 
            buttonUp.Location = new Point(509, 101);
            buttonUp.Name = "buttonUp";
            buttonUp.Size = new Size(94, 29);
            buttonUp.TabIndex = 4;
            buttonUp.Tag = "up";
            buttonUp.Text = "Up";
            buttonUp.UseVisualStyleBackColor = true;
            buttonUp.MouseDown += Button_Click;
            buttonUp.MouseUp += Button_Release;
            // 
            // buttonBackward
            // 
            buttonBackward.Location = new Point(638, 176);
            buttonBackward.Name = "buttonBackward";
            buttonBackward.Size = new Size(94, 29);
            buttonBackward.TabIndex = 7;
            buttonBackward.Tag = "backward";
            buttonBackward.Text = "Backward";
            buttonBackward.UseVisualStyleBackColor = true;
            buttonBackward.MouseDown += Button_Click;
            buttonBackward.MouseUp += Button_Release;
            // 
            // buttonForward
            // 
            buttonForward.Location = new Point(509, 177);
            buttonForward.Name = "buttonForward";
            buttonForward.Size = new Size(94, 29);
            buttonForward.TabIndex = 6;
            buttonForward.Tag = "forward";
            buttonForward.Text = "Forward";
            buttonForward.UseVisualStyleBackColor = true;
            buttonForward.MouseDown += Button_Click;
            buttonForward.MouseUp += Button_Release;
            // 
            // textBoxArduino
            // 
            textBoxArduino.AcceptsReturn = true;
            textBoxArduino.AcceptsTab = true;
            textBoxArduino.AllowDrop = true;
            textBoxArduino.BackColor = Color.LightGray;
            textBoxArduino.Location = new Point(29, 121);
            textBoxArduino.MinimumSize = new Size(200, 100);
            textBoxArduino.Multiline = true;
            textBoxArduino.Name = "textBoxArduino";
            textBoxArduino.ReadOnly = true;
            textBoxArduino.ScrollBars = ScrollBars.Vertical;
            textBoxArduino.Size = new Size(368, 100);
            textBoxArduino.TabIndex = 8;
            // 
            // numericUpDown1
            // 
            numericUpDown1.DecimalPlaces = 2;
            numericUpDown1.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            numericUpDown1.Location = new Point(509, 233);
            numericUpDown1.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(150, 27);
            numericUpDown1.TabIndex = 9;
            numericUpDown1.Value = new decimal(new int[] { 5, 0, 0, 65536 });
            // 
            // button1
            // 
            button1.Location = new Point(752, 101);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 10;
            button1.Tag = "relay";
            button1.Text = "Suction";
            button1.UseVisualStyleBackColor = true;
            button1.MouseDown += button1_MouseDown;
            button1.MouseUp += button1_MouseDown;
            // 
            // button2
            // 
            button2.Location = new Point(752, 233);
            button2.Name = "button2";
            button2.Size = new Size(94, 29);
            button2.TabIndex = 11;
            button2.Text = "Focus";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            button2.KeyDown += Form1_KeyDown;
            button2.KeyUp += Form1_KeyUp;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(303, 41);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(88, 24);
            checkBox1.TabIndex = 12;
            checkBox1.Text = "Stabilize";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // chart1
            // 
            chartArea2.Name = "ChartArea1";
            chart1.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            chart1.Legends.Add(legend2);
            chart1.Location = new Point(12, 233);
            chart1.Name = "chart1";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            chart1.Series.Add(series2);
            chart1.Size = new Size(464, 328);
            chart1.TabIndex = 13;
            chart1.Text = "chart1";
            // 
            // pictureBoxVideo
            // 
            pictureBoxVideo.Location = new Point(852, 61);
            pictureBoxVideo.Name = "pictureBoxVideo";
            pictureBoxVideo.Size = new Size(800, 400);
            pictureBoxVideo.TabIndex = 14;
            pictureBoxVideo.TabStop = false;
            // 
            // pictureBoxContour
            // 
            pictureBoxContour.Location = new Point(852, 467);
            pictureBoxContour.Name = "pictureBoxContour";
            pictureBoxContour.Size = new Size(800, 400);
            pictureBoxContour.TabIndex = 15;
            pictureBoxContour.TabStop = false;
            pictureBoxContour.Click += pictureBoxContour_Click;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(303, 71);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(71, 24);
            checkBox2.TabIndex = 16;
            checkBox2.Text = "Vision";
            checkBox2.UseVisualStyleBackColor = true;
            checkBox2.CheckedChanged += checkBox2_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(500, 441);
            label1.Name = "label1";
            label1.Size = new Size(50, 20);
            label1.TabIndex = 17;
            label1.Text = "label1";
            // 
            // button3
            // 
            button3.Location = new Point(665, 233);
            button3.Name = "button3";
            button3.Size = new Size(94, 29);
            button3.TabIndex = 18;
            button3.Text = "Home";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(500, 284);
            label2.Name = "label2";
            label2.Size = new Size(50, 20);
            label2.TabIndex = 19;
            label2.Text = "label2";
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Checked = true;
            checkBox3.CheckState = CheckState.Checked;
            checkBox3.Location = new Point(303, 100);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(101, 24);
            checkBox3.TabIndex = 20;
            checkBox3.Text = "LimitBreak";
            checkBox3.UseVisualStyleBackColor = true;
            checkBox3.CheckedChanged += checkBox3_CheckedChanged;
            // 
            // button4
            // 
            button4.Location = new Point(752, 176);
            button4.Name = "button4";
            button4.Size = new Size(94, 29);
            button4.TabIndex = 21;
            button4.Tag = "relay";
            button4.Text = "Sequence";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // checkBox4
            // 
            checkBox4.AutoSize = true;
            checkBox4.Checked = true;
            checkBox4.CheckState = CheckState.Checked;
            checkBox4.Location = new Point(406, 38);
            checkBox4.Name = "checkBox4";
            checkBox4.Size = new Size(63, 24);
            checkBox4.TabIndex = 22;
            checkBox4.Text = "Auto";
            checkBox4.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1582, 753);
            Controls.Add(checkBox4);
            Controls.Add(button4);
            Controls.Add(checkBox3);
            Controls.Add(label2);
            Controls.Add(button3);
            Controls.Add(label1);
            Controls.Add(checkBox2);
            Controls.Add(pictureBoxContour);
            Controls.Add(pictureBoxVideo);
            Controls.Add(chart1);
            Controls.Add(checkBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(numericUpDown1);
            Controls.Add(textBoxArduino);
            Controls.Add(buttonBackward);
            Controls.Add(buttonForward);
            Controls.Add(buttonDown);
            Controls.Add(buttonUp);
            Controls.Add(buttonRight);
            Controls.Add(buttonLeft);
            Controls.Add(buttonRefresh);
            Controls.Add(comPortComboBox);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)chart1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxVideo).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxContour).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox comPortComboBox;
        private Button buttonRefresh;
        private Button buttonLeft;
        private Button buttonRight;
        private Button buttonDown;
        private Button buttonUp;
        private Button buttonBackward;
        private Button buttonForward;
        private TextBox textBoxArduino;
        private NumericUpDown numericUpDown1;
        private Button button1;
        private Button button2;
        private CheckBox checkBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private PictureBox pictureBoxVideo;
        private PictureBox pictureBoxContour;
        private CheckBox checkBox2;
        private Label label1;
        private Button button3;
        private Label label2;
        private CheckBox checkBox3;
        private Button button4;
        private CheckBox checkBox4;
    }
}