using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace RoboticArmUI
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort;
        VirtualJoystick joystick;

        bool up;
        bool forward;
        bool left;

        double targetUp;
        double targetForward;
        double targetLeft;

        double currentUp;
        double currentForward;
        double currentLeft;
        double currentUpU;
        double currentForwardU;
        double currentLeftU;

        bool autoStabilize;

        int error = 30;
        double forwardStrength = .1;
        double upStrength = .25;
        double leftStrength = .1;

        private int timeElapsed = 0;




        private void InitializeVirtualJoystick()
        {
            joystick = new VirtualJoystick();
            joystick.Location = new Point(400, 200);
            joystick.Size = new Size(400, 400);
            joystick.JoystickMoved += Joystick_JoystickMoved;
            Controls.Add(joystick);
        }

        private void Joystick_JoystickMoved(object sender, Point position)
        {
            // Normalize joystick position to range from -1 to 1
            float normalizedX = (position.X - joystick.Width / 2) / (float)(joystick.Width / 2);
            float normalizedY = (position.Y - joystick.Height / 2) / (float)(joystick.Height / 2);

            // Calculate speed components
            float speedX = normalizedX * (float)numericUpDown1.Value;
            float speedY = -normalizedY * (float)numericUpDown1.Value; // Invert Y axis for typical joystick orientation

            // Send speed values to motors
            SendMotorSpeed(speedX, speedY);
        }

        private void SendMotorSpeed(float speedX, float speedY)
        {
            if (serialPort == null || !serialPort.IsOpen)
            {
                return;
            }

            // Calculate magnitude of the vector
            float magnitude = (float)Math.Sqrt(speedX * speedX + speedY * speedY);

            // Normalize speed values based on magnitude

            speedX /= magnitude;
            speedY /= magnitude;

            //if(joystick.Height)

            // Determine the direction for each motor
            string motor1Direction = (speedY >= 0) ? "forward" : "backward";
            string motor2Direction = (speedX >= 0) ? "left" : "right";

            // Send motor speed messages over serial port

            serialPort.WriteLine($"{motor1Direction} {Math.Abs(speedY)}");
            serialPort.WriteLine($"{motor2Direction} {Math.Abs(speedX)}");
        }
        private Dictionary<Keys, bool> keyStates = new Dictionary<Keys, bool>();

        public Form1()
        {
            InitializeComponent();
            InitializeVirtualJoystick();

            // Initialize key states
            keyStates[Keys.W] = false;
            keyStates[Keys.S] = false;
            keyStates[Keys.A] = false;
            keyStates[Keys.D] = false;
            keyStates[Keys.Q] = false;
            keyStates[Keys.E] = false;
            keyStates[Keys.Space] = false;

            // Subscribe to the KeyDown and KeyUp events
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
            InitializeChart();

            // Start a timer to update the chart
            timer1.Tick += Timer1_Tick;
            timer1.Start();
        }

        private void InitializeChart()
        {
            // Set chart area
            chart1.ChartAreas.Add(new ChartArea("MainArea"));

            // Set X-axis
            chart1.ChartAreas["MainArea"].AxisX.Title = "Time (s)";

            // Set Y-axis
            chart1.ChartAreas["MainArea"].AxisY.Title = "Up Current";

            // Add series for upCurrent
            chart1.Series.Add("UpCurrent");
            chart1.Series["UpCurrent"].ChartType = SeriesChartType.Line;
            chart1.Series["UpCurrent"].XValueType = ChartValueType.Double;
            chart1.Series["UpCurrent"].BorderWidth = 2; // Example border width

            // Add series for targetUp
            chart1.Series.Add("TargetUp");
            chart1.Series["TargetUp"].ChartType = SeriesChartType.Line;
            chart1.Series["TargetUp"].XValueType = ChartValueType.Double;
            chart1.Series["TargetUp"].BorderWidth = 2; // Example border width
            chart1.Series["TargetUp"].Color = System.Drawing.Color.Red; // Example color for the line
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            // Update the upCurrent and targetUp variables (replace with your actual data)
            //upCurrent = Math.Sin(timeElapsed / 1000.0);
            //targetUp = 0.5 * Math.Sin(timeElapsed / 1000.0); // Example target data

            // Add the new data points to the chart
            chart1.Series["UpCurrent"].Points.AddXY(timeElapsed / 1000.0, currentUp);
            chart1.Series["TargetUp"].Points.AddXY(timeElapsed / 1000.0, targetUp);

            // Increment timeElapsed
            timeElapsed += timer1.Interval; // Use timer1.Interval to advance time correctly

            // Remove old data points to prevent the chart from growing indefinitely
            while (chart1.Series["UpCurrent"].Points.Count > 0 && (timeElapsed - chart1.Series["UpCurrent"].Points[0].XValue * 1000) > 10000)
            {
                chart1.Series["UpCurrent"].Points.RemoveAt(0);
                chart1.Series["TargetUp"].Points.RemoveAt(0);
            }

            // Update the x-axis range
            double maxXValue = timeElapsed / 1000.0;
            double minXValue = maxXValue - 10; // Set the x-axis range to a 10-second window
            chart1.ChartAreas[0].AxisX.Minimum = minXValue;
            chart1.ChartAreas[0].AxisX.Maximum = maxXValue;
        }





        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            if (!keyStates[e.KeyCode])
            {
                keyStates[e.KeyCode] = true;

                switch (e.KeyCode)
                {
                    case Keys.W:
                        Button_Click(buttonUp, null);
                        break;
                    case Keys.S:
                        Button_Click(buttonDown, null);
                        break;
                    case Keys.A:
                        Button_Click(buttonLeft, null);
                        break;
                    case Keys.D:
                        Button_Click(buttonRight, null);
                        break;
                    case Keys.Q:
                        Button_Click(buttonForward, null);
                        break;
                    case Keys.E:
                        Button_Click(buttonBackward, null);
                        break;
                    case Keys.Space:
                        button1_MouseDown(null, null);
                        break;
                }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            keyStates[e.KeyCode] = false;

            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.S:
                case Keys.A:
                case Keys.D:
                case Keys.Q:
                case Keys.E:
                    Button_Release(null, null);
                    break;
            }
        }


        private void PopulateComPorts()
        {
            string[] ports = SerialPort.GetPortNames();
            comPortComboBox.Items.Clear();
            comPortComboBox.Items.AddRange(ports);
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            PopulateComPorts();
        }

        private void OpenPort(string portName)
        {
            if (serialPort != null && serialPort.IsOpen)
                serialPort.Close();

            serialPort = new SerialPort(portName);
            serialPort.BaudRate = 115200; // Set your baud rate
            serialPort.DataReceived += SerialPort_DataReceived; // Subscribe to the DataReceived event
            try
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening port: " + ex.Message);
            }
            serialPort.RtsEnable = true;
            serialPort.DtrEnable = true;
        }

        private int rollingWindowSize = 5; // Adjust this window size as needed
        private Queue<double> rollingWindowLeft = new Queue<double>();
        private Queue<double> rollingWindowUp = new Queue<double>();
        private Queue<double> rollingWindowForward = new Queue<double>();

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Read the data from the serial port and display it in the TextBox
            string dataS = serialPort.ReadExisting();
            string[] dataArr = dataS.Split("\r\n");
            foreach (string data in dataArr)
            {
                if (data.StartsWith("E"))
                {
                    if (data.Substring(2).Contains("E"))
                        continue;
                    int encoder = int.Parse(data.Substring(2, 1));
                    double newValue = double.Parse((data.Substring(4)));

                    switch (encoder)
                    {
                        case 1:
                            currentLeftU = newValue;
                            currentLeft = RollingAverage(currentLeftU, rollingWindowLeft);
                            break;
                        case 2:
                            currentUpU = newValue;
                            currentUp = RollingAverage(currentUpU, rollingWindowUp);
                            break;
                        case 3:
                            currentForwardU = newValue;
                            currentForward = RollingAverage(currentForwardU, rollingWindowForward);
                            break;
                    }

                    if (autoStabilize)
                        UpdateMotorAuto();
                    return;
                }
                AppendTextBox(data + Environment.NewLine);
            }
        }

        private double RollingAverage(double newValue, Queue<double> rollingWindow)
        {
            rollingWindow.Enqueue(newValue);
            if (rollingWindow.Count > rollingWindowSize)
                rollingWindow.Dequeue();

            double sum = 0.0;
            foreach (double value in rollingWindow)
            {
                sum += value;
            }

            return sum / rollingWindow.Count;
        }


        public void UpdateMotorAuto()
        {
            /*if (!forward)
            {
                if (currentForward > targetForward - error && currentForward < targetForward - error)
                    SendCommand("forward 0");
                else if (currentForward > targetForward)
                    SendCommand($"forward {forwardStrength}");
                else if (currentForward < targetForward)
                    SendCommand($"backward {forwardStrength}");
            }
            if (!left)
            {
                if (currentLeft > targetLeft - error && currentLeft < targetLeft - error)
                    SendCommand("left 0");
                else if (currentLeft > targetLeft)
                    SendCommand($"left {leftStrength}");
                else if (currentLeft < targetLeft)
                    SendCommand($"right {leftStrength}");
            }*/
            if (!up)
            {
                if (currentUp > targetUp - error && currentUp < targetUp + error)
                {
                    SendCommand("up 0");
                }
                else if (currentUp < targetUp)
                    SendCommand($"down {upStrength}");
                else if (currentUp > targetUp)
                    SendCommand($"up {upStrength}");
                up = false;

            }
        }

        private void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }
            textBoxArduino.AppendText(value); // Append the received data to the TextBox
        }

        private void SendCommand(string command)
        {
            if (serialPort == null || !serialPort.IsOpen)
            {
                MessageBox.Show("No open port selected.");
                return;
            }
            if (command.StartsWith("forward") || command.StartsWith("backward"))
                forward = true;
            if (command.StartsWith("left") || command.StartsWith("right"))
                left = true;
            if (command.StartsWith("up") || command.StartsWith("down"))
                up = true;

            if (command.StartsWith("stop"))
            {
                if (command.Contains("forward") || command.Contains("back"))
                {
                    forward = false;
                    targetForward = currentForward;
                }
                else if (command.Contains("up") || command.Contains("down"))
                {
                    up = false;
                    
                    targetUp = currentUp;
                }
                else if (command.Contains("left") || command.Contains("right"))
                {
                    left = false;
                    targetLeft = currentLeft;
                }
            }
            serialPort.WriteLine(command);
            
        }


        private async void Button_Click(object sender, MouseEventArgs e)
        {
            if (sender is Button button)
            {
                string command = button.Tag.ToString() + " " + numericUpDown1.Value + "\n";
                SendCommand(command);
                Debug.WriteLine(command);
            }
        }

        private async void Button_Release(object sender, MouseEventArgs e)
        {

            if (sender is Button)
                SendCommand("stop " + (sender as Button).Tag);
            else
                SendCommand("stop");
            Debug.WriteLine("stop");

            // Resend commands for keys that are still pressed
            foreach (KeyValuePair<Keys, bool> kvp in keyStates)
            {
                if (kvp.Value)
                {
                    switch (kvp.Key)
                    {
                        case Keys.W:
                            Button_Click(buttonUp, null);
                            up = false;
                            break;
                        case Keys.S:
                            Button_Click(buttonDown, null);
                            up = false;
                            break;
                        case Keys.A:
                            Button_Click(buttonLeft, null);
                            left = false;
                            break;
                        case Keys.D:
                            Button_Click(buttonRight, null);
                            left = false;
                            break;
                        case Keys.Q:
                            Button_Click(buttonForward, null);
                            forward = false;
                            break;
                        case Keys.E:
                            Button_Click(buttonBackward, null);
                            forward = false;
                            break;
                        case Keys.Space:
                            button1_MouseDown(null, null);
                            break;
                    }
                }
            }
        }


        private void ComPortComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedPort = comPortComboBox.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedPort))
            {
                OpenPort(selectedPort);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
                serialPort.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Subscribe to the MouseWheel event of the form
            this.MouseWheel += Form1_MouseWheel;
            timer1.Start();
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            // Check if the scroll wheel moved up
            if (e.Delta > 0)
            {
                // Increase the value of the NumericUpDown control by one
                if (numericUpDown1.Value <= (decimal).99)
                    numericUpDown1.Value += (decimal).01;
            }
            // Check if the scroll wheel moved down
            else if (e.Delta < 0)
            {
                // Decrease the value of the NumericUpDown control by one
                if (numericUpDown1.Value >= (decimal).01)
                    numericUpDown1.Value -= (decimal).01;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;

        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            SendCommand("relay\n");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            autoStabilize = checkBox1.Checked;
            if (!autoStabilize)
                SendCommand("stop");
        }



    }
}
