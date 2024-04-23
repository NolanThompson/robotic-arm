using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows.Forms.DataVisualization.Charting;

namespace RoboticArmUI
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort;
        VirtualJoystick joystick;

        static VideoCapture capture;
        Task video;

        bool up;
        bool forward;
        bool left;

        bool autoVision = false;

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

        int error = 5;
        int errorL = 12;
        double forwardProp = 0.003;
        double upProp = 0.004;
        //double leftProp = 0.004;
        double forwardStrength = .0006;
        double upStrength = .0007;
        double leftStrength = .045;

        private int timeElapsed = 0;

        PointF box;
        PointF arm;
        PointF armB;
        Point size;

        double leftLimit = 5;
        double rightLimit = 364;
        double upLimit = 100;
        double downLimit = 200;
        double forwardLimit = 345;
        double backwardLimit = 190;

        bool limitBreak = true;


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
            // Initialize VideoCapture
            capture = new VideoCapture(1);
            size = new Point(capture.Width, capture.Height);

            // Start processing in a separate task
            video = Task.Run(() => ProcessFrames(100, 200, 10, 30, 70, 90)); // Example values for the two integers

        }
        async Task ProcessFrames(int hueMin, int hueMax, int yhueMin, int yhueMax, int ghueMin, int ghueMax)
        {
            while (true)
            {
                // Read frame from webcam
                Mat frame = capture.QueryFrame();

                // If the frame is null, break out of the loop
                if (frame == null)
                    break;

                // Process frame
                await ProcessFrame(frame, hueMin, hueMax, yhueMin, yhueMax, ghueMin, ghueMax);
            }
        }

        async Task ProcessFrame(Mat frame, int hueMin, int hueMax, int hueMinY, int hueMaxY, int hueMinG, int hueMaxG)
        {
            // Convert frame to HSL color space
            Mat hslFrame = new Mat();
            CvInvoke.CvtColor(frame, hslFrame, ColorConversion.Bgr2Hls);

            // Define tighter range of blue color in HSL
            int saturationMin = 100; // Adjust saturation and lightness ranges as needed
            int saturationMax = 255;
            int lightnessMin = 50;
            int lightnessMax = 255;

            // Create mask for blue color
            Mat maskB = new Mat();
            Mat maskY = new Mat();
            Mat maskG = new Mat();
            CvInvoke.InRange(hslFrame, new ScalarArray(new MCvScalar(hueMin, saturationMin, lightnessMin)),
                new ScalarArray(new MCvScalar(hueMax, saturationMax, lightnessMax)), maskB);
            CvInvoke.InRange(hslFrame, new ScalarArray(new MCvScalar(hueMinY, saturationMin, lightnessMin)),
                new ScalarArray(new MCvScalar(hueMaxY, saturationMax, lightnessMax)), maskY);
            CvInvoke.InRange(hslFrame, new ScalarArray(new MCvScalar(hueMinG, saturationMin, lightnessMin)),
                new ScalarArray(new MCvScalar(hueMaxG, saturationMax, lightnessMax)), maskG);

            // Find contours in the mask
            VectorOfVectorOfPoint contoursB = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(maskB, contoursB, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            VectorOfVectorOfPoint contoursY = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(maskY, contoursY, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            VectorOfVectorOfPoint contoursG = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(maskG, contoursG, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            // Find the largest contour
            double maxAreaB = 0;
            int maxContourIndexB = -1;
            for (int i = 0; i < contoursB.Size; i++)
            {
                double area = CvInvoke.ContourArea(contoursB[i]);
                if (area > maxAreaB)
                {
                    maxAreaB = area;
                    maxContourIndexB = i;
                }
            }

            double maxAreaY = 0;
            int maxContourIndexY = -1;
            for (int i = 0; i < contoursY.Size; i++)
            {
                double area = CvInvoke.ContourArea(contoursY[i]);
                if (area > maxAreaY)
                {
                    maxAreaY = area;
                    maxContourIndexY = i;
                }
            }

            double maxAreaG = 0;
            int maxContourIndexG = -1;
            for (int i = 0; i < contoursG.Size; i++)
            {
                double area = CvInvoke.ContourArea(contoursG[i]);
                if (area > maxAreaG)
                {
                    maxAreaG = area;
                    maxContourIndexG = i;
                }
            }

            // Draw minimum enclosing rectangle and display coordinates of the largest contour
            if (maxContourIndexB != -1)
            {
                // Get minimum enclosing rectangle of the largest contour
                RotatedRect minRect = CvInvoke.MinAreaRect(contoursB[maxContourIndexB]);

                // Draw minimum enclosing rectangle
                PointF[] vertices = minRect.GetVertices();
                for (int i = 0; i < vertices.Length; i++)
                {
                    CvInvoke.Line(frame, Point.Round(vertices[i]), Point.Round(vertices[(i + 1) % vertices.Length]), new MCvScalar(255, 0, 0), 2);
                }

                box = minRect.Center;
                // Display coordinates of the minimum enclosing rectangle
                string text = $"({minRect.Center.X}, {minRect.Center.Y})";
                CvInvoke.PutText(frame, text, new Point((int)minRect.Center.X, (int)minRect.Center.Y - 10),
                    FontFace.HersheySimplex, 0.5, new MCvScalar(0, 255, 0), 2);
            }

            // Draw minimum enclosing rectangle and display coordinates of the largest contour
            if (maxContourIndexY != -1)
            {
                // Get minimum enclosing rectangle of the largest contour
                RotatedRect minRect = CvInvoke.MinAreaRect(contoursY[maxContourIndexY]);

                // Draw minimum enclosing rectangle
                PointF[] vertices = minRect.GetVertices();
                for (int i = 0; i < vertices.Length; i++)
                {
                    CvInvoke.Line(frame, Point.Round(vertices[i]), Point.Round(vertices[(i + 1) % vertices.Length]), new MCvScalar(0, 255, 255), 2);
                }

                arm = minRect.Center;
                // Display coordinates of the minimum enclosing rectangle
                string text = $"({minRect.Center.X}, {minRect.Center.Y})";
                CvInvoke.PutText(frame, text, new Point((int)minRect.Center.X, (int)minRect.Center.Y - 10),
                    FontFace.HersheySimplex, 0.5, new MCvScalar(0, 255, 0), 2);
            }

            if (maxContourIndexG != -1)
            {
                // Get minimum enclosing rectangle of the largest contour
                RotatedRect minRect = CvInvoke.MinAreaRect(contoursG[maxContourIndexG]);

                // Draw minimum enclosing rectangle
                PointF[] vertices = minRect.GetVertices();
                for (int i = 0; i < vertices.Length; i++)
                {
                    CvInvoke.Line(frame, Point.Round(vertices[i]), Point.Round(vertices[(i + 1) % vertices.Length]), new MCvScalar(0, 255, 0), 2);
                }

                armB = minRect.Center;
                // Display coordinates of the minimum enclosing rectangle
                string text = $"({minRect.Center.X}, {minRect.Center.Y})";
                CvInvoke.PutText(frame, text, new Point((int)minRect.Center.X, (int)minRect.Center.Y - 10),
                    FontFace.HersheySimplex, 0.5, new MCvScalar(0, 255, 0), 2);
            }

            // Update PictureBox controls
            UpdatePictureBox(frame, maskG);

            await Task.Delay(30); // Adjust delay as needed
        }

        public void UpdatePictureBox(Mat frame, Mat mask)
        {
            // Convert Mat to Bitmap
            Bitmap bitmapFrame = frame.ToBitmap();
            Bitmap bitmapMask = mask.ToBitmap();

            // Scale bitmaps to fit the size of PictureBox controls
            Bitmap scaledFrame = new Bitmap(pictureBoxVideo.Width, pictureBoxVideo.Height);
            Bitmap scaledMask = new Bitmap(pictureBoxContour.Width, pictureBoxContour.Height);
            using (Graphics gFrame = Graphics.FromImage(scaledFrame))
            using (Graphics gMask = Graphics.FromImage(scaledMask))
            {
                gFrame.DrawImage(bitmapFrame, 0, 0, pictureBoxVideo.Width, pictureBoxVideo.Height);
                gMask.DrawImage(bitmapMask, 0, 0, pictureBoxContour.Width, pictureBoxContour.Height);
            }

            // Update PictureBox controls with the new images
            if (pictureBoxVideo.Image != null)
                pictureBoxVideo.Image.Dispose(); // Dispose previous image to prevent memory leak
            if (pictureBoxContour.Image != null)
                pictureBoxContour.Image.Dispose(); // Dispose previous image to prevent memory leak
            pictureBoxVideo.Image = scaledFrame;
            pictureBoxContour.Image = scaledMask;
            this.Invoke(new Action(() => label1.Text = $"arm = ({arm.X}, {arm.Y}) \narmB = ({armB.X}, {armB.Y}) \nBox = ({box.X}, {box.Y})"));
        }

        private void InitializeChart()
        {
            // Set chart area
            chart1.ChartAreas.Add(new ChartArea("MainArea"));

            // Set X-axis
            chart1.ChartAreas["MainArea"].AxisX.Title = "Time (s)";

            // Set Y-axis
            chart1.ChartAreas["MainArea"].AxisY.Title = "Up Current";
            chart1.ChartAreas["MainArea"].AxisY.Maximum = 370;


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
            chart1.Series["UpCurrent"].Points.AddXY(timeElapsed / 1000.0, currentLeft);
            chart1.Series["TargetUp"].Points.AddXY(timeElapsed / 1000.0, targetLeft);

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
                        case 0:
                            currentLeftU = newValue;
                            currentLeft = RollingAverage(currentLeftU, rollingWindowLeft);
                            break;
                        case 2:
                            currentUpU = newValue;
                            currentUp = RollingAverage(currentUpU, rollingWindowUp);
                            break;
                        case 1:
                            currentForwardU = newValue;
                            currentForward = RollingAverage(currentForwardU, rollingWindowForward);
                            break;
                    }
                    if (autoVision)
                    {
                        AutoVision();
                    }
                    if (autoStabilize)
                        UpdateMotorAuto();
                    
                    return;
                }
                AppendTextBox(data + Environment.NewLine);

                this.Invoke(new Action(() => label2.Text = $"Target Left = {targetLeft}\nActual Left = {currentLeft}\nTarget Up = {targetUp}\nCurrent Up = {currentUp}\nTarget Forward = {targetForward}\nActual Forward = {currentForward}"));
            }
        }

        private void AutoVision()
        {
            //double m1 = (arm.Y - armB.Y)/(arm.X-armB.X);
            double m2 = (box.Y - armB.Y) / (box.X - armB.X);

            //double angle = Math.Atan((m2 - m1) / (1 + m1 * m2))*2;


            // Output the result
            targetLeft = Math.Atan(m2) * 2 * (180 / Math.PI);
            if (targetLeft < 0)
                targetLeft += 360;

            double d = Math.Sqrt(Math.Pow(box.X - armB.X, 2) + Math.Pow(box.Y - armB.Y, 2));
            double theta1 = Math.Asin(d / 27);
            targetUp = 200-(theta1 / 360 * 100);
            double theta2 = theta1 - Math.Asin(18.5 / 27);
            targetForward = theta2 / 360 * 200;
            Invoke(new Action(() => checkBox2.CheckState = CheckState.Unchecked));
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
            if (!forward)
            {
                if (Math.Abs(currentForward - targetForward) < error)
                    SendCommand("forward 0");
                else if (Math.Min((currentForward - targetForward + 369) % 369, (targetForward - currentForward + 369) % 369) <= error)
                    SendCommand("forward 0");
                else if (currentForward < targetForward)
                    SendCommand($"backward {forwardStrength * forwardProp * Math.Abs(targetForward - currentForward)}");
                else
                    SendCommand($"forward {forwardStrength * forwardProp * Math.Abs(targetForward - currentForward)}");


                forward = false;

            }
            if (!left)
            {

                if (Math.Abs(currentLeft - targetLeft) < errorL)
                    SendCommand("left 0");
                else if (Math.Min((currentLeft - targetLeft + 369) % 369, (targetLeft - currentLeft + 369) % 369) <= errorL)
                    SendCommand("left 0");
                else if (currentLeft < targetLeft)
                    SendCommand($"right {leftStrength}");
                else
                    SendCommand($"left {leftStrength}");

                left = false;

            }
            if (!up)
            {
                if (Math.Abs(currentUp - targetUp) < error)
                    SendCommand("up 0");
                else if (Math.Min((currentUp - targetUp + 369) % 369, (targetUp - currentUp + 369) % 369) <= error)
                    SendCommand("up 0");
                else if (currentUp < targetUp)
                    SendCommand($"down {upStrength * upProp * Math.Abs(targetUp - currentUp)}");
                else
                    SendCommand($"up {upStrength * upProp * Math.Abs(targetUp - currentUp)}");

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

        private async void SendCommand(string command)
        {
            if (serialPort == null || !serialPort.IsOpen)
            {
                MessageBox.Show("No open port selected.");
                return;
            }
            if (command.StartsWith("forward"))
            {
                //if (currentForward > forwardLimit)
                //return;
                forward = true;
            }
            if (command.StartsWith("backward"))
            {
                //if (currentForward < backwardLimit)
                //return;
                forward = true;
            }
            if (command.StartsWith("left"))
            {
                //if (currentLeft < leftLimit && currentLeft > rightLimit)
                //return;
                left = true;
            }
            if (command.StartsWith("right"))
            {
                //if (currentLeft < leftLimit && currentLeft > rightLimit)
                //return;
                left = true;
            }
            if (command.StartsWith("up"))
            {
                //if (currentUp < upLimit)
                //return;
                up = true;
            }
            if (command.StartsWith("down"))
            {
                //if (currentUp > downLimit)
                //return;
                up = true;
            }
            if (command.StartsWith("stop"))
            {
                if (command.Contains("forward") || command.Contains("back"))
                {
                    forward = false;
                    SerialPort_DataReceived(null, null);
                    targetForward = currentForward;
                }
                else if (command.Contains("up") || command.Contains("down"))
                {
                    up = false;
                    SerialPort_DataReceived(null, null);
                    targetUp = currentUp;
                }
                else if (command.Contains("left") || command.Contains("right"))
                {
                    left = false;
                    SerialPort_DataReceived(null, null);
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
            SendCommand("stop");
            autoStabilize = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            autoVision = checkBox2.Checked;
        }

        private void pictureBoxContour_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            targetForward = 165;
            targetUp = 130;
            targetLeft = 175;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            limitBreak = checkBox3.Checked;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Task.Run(() => Sequence());
        }

        private async Task Sequence()
        {
            while (checkBox4.Checked)
            {
                targetLeft = 270;
                await Task.Delay(500);
                targetUp = 195;
                targetForward = 63;

                await Task.Delay(3000);
                SendCommand("relay");
                await Task.Delay(1000);
                targetUp = 130;
                await Task.Delay(3000);

                targetLeft = 120;
                await Task.Delay(1000);
                targetUp = 195;
                await Task.Delay(3000);
                SendCommand("relay");
                await Task.Delay(1500);

                targetUp = 130;
                await Task.Delay(1000);
                targetLeft = 30;

                await Task.Delay(5000);
                targetUp = 195;
                targetForward = 63;
                await Task.Delay(1000);
                SendCommand("relay");
                await Task.Delay(2000);
                targetUp = 130;
                await Task.Delay(3000);

                targetLeft = 185;
                await Task.Delay(1000);
                targetUp = 195;
                await Task.Delay(1000);
                SendCommand("relay");
                await Task.Delay(1500);
                targetUp = 130;
                await Task.Delay(1000);
                targetLeft = 120;
await Task.Delay(1000);
                targetUp = 195;
                await Task.Delay(1000);
                SendCommand("relay");
                await Task.Delay(1500);
                targetUp = 120;
                await Task.Delay(2000);
                targetLeft = 275;
                await Task.Delay(900);
                targetUp = 195;
                await Task.Delay(500);
                SendCommand("relay");
                await Task.Delay(1000);
                targetUp = 130;
                await Task.Delay(1000);
                targetLeft = 185;
                await Task.Delay(1000);
                targetUp = 195;
                await Task.Delay(1000);
                SendCommand("relay");
                await Task.Delay(1500);
                targetUp = 130;
                await Task.Delay(1000);
                targetLeft = 340;
                await Task.Delay(1000);
                targetUp = 195;
                await Task.Delay(1000);
                targetUp = 130;
                await Task.Delay(1000);
                targetLeft = 35;
                await Task.Delay(2000);
                targetUp = 195;
                await Task.Delay(1000);
                SendCommand("relay");
                await Task.Delay(1500);
                targetUp = 130;
                await Task.Delay(500);
                targetForward = 165;
                targetUp = 130;
                targetLeft = 175;
                await Task.Delay(1500);


            }
            targetForward = 165;
            targetUp = 130;
            targetLeft = 175;
        }
    }
}
