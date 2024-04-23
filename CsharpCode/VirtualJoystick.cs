using System;
using System.Drawing;
using System.Windows.Forms;

namespace RoboticArmUI
{
    public class VirtualJoystick : Control
    {
        private Point center;
        private Point thumbPosition;
        private bool thumbDragging;

        public event EventHandler<Point> JoystickMoved;

        public VirtualJoystick()
        {
            center = new Point(Width / 2, Height / 2);
            thumbPosition = center;
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Calculate the smaller radius for the circle
            int smallerRadius = Math.Min(Width, Height) / 4;

            // Draw the smaller red background circle
            int diameter = smallerRadius * 2;
            g.FillEllipse(Brushes.Red, center.X - smallerRadius, center.Y - smallerRadius, diameter, diameter);

            // Draw the thumb circle (20% of the size of the background circle)
            int thumbRadius = smallerRadius / 4;
            g.FillEllipse(Brushes.Gray, thumbPosition.X - thumbRadius, thumbPosition.Y - thumbRadius, thumbRadius * 2, thumbRadius * 2);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                if (Math.Sqrt(Math.Pow(e.X - center.X, 2) + Math.Pow(e.Y - center.Y, 2)) <= (Width / 4))
                {
                    thumbPosition = e.Location;
                    thumbDragging = true;
                    Invalidate();
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (thumbDragging)
            {
                // Calculate the vector from the center to the mouse position
                int deltaX = e.X - center.X;
                int deltaY = e.Y - center.Y;

                // Calculate the distance from the center to the mouse position
                double distanceToCenter = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                // If the distance is greater than the radius, limit it to the radius
                if (distanceToCenter > Width / 4)
                {
                    double scaleFactor = (Width / 4) / distanceToCenter;
                    deltaX = (int)(deltaX * scaleFactor);
                    deltaY = (int)(deltaY * scaleFactor);
                }

                // Set the thumb position to the closest point on the circle to the mouse cursor
                thumbPosition = new Point(center.X + deltaX, center.Y + deltaY);

                Invalidate();
                OnJoystickMoved(thumbPosition);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (thumbDragging && e.Button == MouseButtons.Left)
            {
                thumbDragging = false;
                thumbPosition = center;
                Invalidate();
                OnJoystickMoved(thumbPosition);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            center = new Point(Width / 2, Height / 2);
            thumbPosition = center;
            Invalidate();
        }

        protected virtual void OnJoystickMoved(Point position)
        {
            JoystickMoved?.Invoke(this, position);
        }
    }
}
