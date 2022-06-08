using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace association_bubble_ver1.Models
{
    class Bubble : ContentPresenter
    {
        DispatcherTimer timer;
        TimeSpan timeSpan = new TimeSpan(10000);
        public Thumb thumb = new Thumb();
        double EllipseWidth = 0;
        double EllipseHeight = 0;
        double radius;
        public double HorizontalSpeed = 0.0;
        public double VerticalSpeed = 0.0;
        double initialHorizontalPositin = 10;
        double initialVerticalPosition = 10;
        public Bubble(string input)
        {
            double FontSizeValue = 20.0;
            //Label
            FrameworkElementFactory label = new FrameworkElementFactory(typeof(Label));
            label.SetValue(Label.FontSizeProperty, Convert.ToDouble(FontSizeValue));
            label.SetValue(Label.ContentProperty, input);
            Label Dummy = new Label() { FontSize = FontSizeValue, Content = input };
            Dummy.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            Dummy.Arrange(new Rect(this.DesiredSize));
            label.SetValue(Label.WidthProperty, Convert.ToDouble(Dummy.ActualWidth));
            label.SetValue(Label.HeightProperty, Convert.ToDouble(Dummy.ActualHeight));
            label.SetValue(Label.BackgroundProperty, new SolidColorBrush(Colors.Transparent));
            label.SetValue(Label.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
            label.SetValue(Label.VerticalContentAlignmentProperty, VerticalAlignment.Center);
            //Ellipse
            radius = Math.Max(Convert.ToDouble(Dummy.ActualWidth), Convert.ToDouble(Dummy.ActualHeight));
            FrameworkElementFactory ellipse = new FrameworkElementFactory(typeof(Ellipse));
            ellipse.SetValue(Ellipse.WidthProperty, Convert.ToDouble(radius));
            ellipse.SetValue(Ellipse.HeightProperty, Convert.ToDouble(radius));
            ellipse.SetValue(Ellipse.FillProperty, new SolidColorBrush(Colors.Aquamarine));
            ellipse.SetValue(Ellipse.StrokeProperty, new SolidColorBrush(Colors.Black));
            ellipse.SetValue(Ellipse.StrokeThicknessProperty, Convert.ToDouble(2));
            ellipse.SetValue(Ellipse.OpacityProperty, Convert.ToDouble(0.5));
            //Grid
            FrameworkElementFactory grid = new FrameworkElementFactory(typeof(Grid));
            grid.SetValue(Grid.WidthProperty, Convert.ToDouble(radius));
            grid.SetValue(Grid.HeightProperty, Convert.ToDouble(radius));
            grid.SetValue(Grid.BackgroundProperty, new SolidColorBrush(Colors.Transparent));
            grid.AppendChild(ellipse);
            grid.AppendChild(label);
            //
            ControlTemplate template = new ControlTemplate(typeof(Thumb));
            template.VisualTree = grid;
            thumb.Template = template;
            this.SetValue(ContentProperty, thumb);
            //
            HorizontalSpeed = 1;
            VerticalSpeed = 1;
            EllipseWidth = radius;
            EllipseHeight = radius;
            //
            this.AddHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(OnLeftDown));
            this.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnDragDelta));
            this.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(OnLeftUp));
            //
            this.SetValue(Canvas.TopProperty, initialHorizontalPositin);
            this.SetValue(Canvas.LeftProperty, initialVerticalPosition);
            UpDatePosition();
            //
            timer = new DispatcherTimer() { Interval = timeSpan };
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void UpDatePosition()
        {
            Canvas canvas = (Canvas)Parent;
            if (canvas == null) return;
            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;
            double Threshold = 1.1;
            double Dumping = 1.0;
            double[] Velocity = Repulsion();
            HorizontalSpeed *= Dumping; VerticalSpeed *= Dumping;
            HorizontalSpeed += Velocity[0]; VerticalSpeed += Velocity[1];
            if ((X <= 0) | (X >= width - EllipseWidth)) HorizontalSpeed *= (-1);
            if ((Y <= 0) | (Y >= height - EllipseHeight)) VerticalSpeed *= (-1);
            if (Math.Abs(HorizontalSpeed) > Threshold) HorizontalSpeed *= 0.9;
            if (Math.Abs(VerticalSpeed) > Threshold) VerticalSpeed *= 0.9;
            if (X <= 0) { X *= (-1); }
            if (X >= width - EllipseWidth) { X -= X - (width - EllipseWidth); }
            if (Y <= 0) { Y *= (-1); }
            if (Y >= height - EllipseHeight) { Y -= Y - (height - EllipseHeight); }
            Canvas.SetTop(this, Canvas.GetTop(this) + VerticalSpeed);
            Canvas.SetLeft(this, Canvas.GetLeft(this) + HorizontalSpeed);
        }
        double[] Repulsion()
        {
            Canvas canvas = (Canvas)Parent;
            double[] Velocity = new double[] { 0, 0 };
            if (canvas == null) return Velocity;
            if (canvas.Children.Count < 1) return Velocity;
            double HorizontalEffect = 0;
            double VerticalEffect = 0;
            double Weight = 0.02;
            double WallEffect = 2;
            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;
            if ((X > 0) & (width - EllipseWidth > X))
            {
                HorizontalEffect = (1.0 / Math.Pow(X, 2) - 1.0 / Math.Pow(width - EllipseWidth - X, 2)) * WallEffect;
            }
            if ((Y > 0) & (height - EllipseHeight > Y))
            {
                VerticalEffect = (1.0 / Math.Pow(Y, 2) - 1.0 / Math.Pow(height - EllipseHeight - Y, 2)) * WallEffect;
            }
            foreach (Bubble ui in canvas.Children)
            {
                if ((this != ui) & (ui is Bubble))
                {
                    double HorizontalDistance = ((double)Canvas.GetLeft(ui) + ui.EllipseWidth * 0.5) - (X + EllipseWidth * 0.5);
                    double VerticalDistance = ((double)Canvas.GetTop(ui) + ui.EllipseHeight * 0.5) - (Y + EllipseHeight * 0.5);
                    double RadialDistance = Math.Sqrt(Math.Pow(HorizontalDistance, 2.0) + Math.Pow(VerticalDistance, 2.0)) - (this.radius + ui.radius);
                    if (RadialDistance <= 1) RadialDistance = 1.0;
                    HorizontalEffect -= HorizontalDistance / (RadialDistance + this.radius + ui.radius) / Math.Pow(RadialDistance, 2) * Weight;
                    VerticalEffect -= VerticalDistance / (RadialDistance + this.radius + ui.radius) / Math.Pow(RadialDistance, 2) * Weight;
                }
            }
            if (!Double.IsNaN(HorizontalEffect)) Velocity[0] = HorizontalEffect;
            if (!Double.IsNaN(VerticalEffect)) Velocity[1] = VerticalEffect;
            return Velocity;
        }
        void timer_Tick(object? sender, EventArgs e)
        {
            UpDatePosition();
        }
        private void OnLeftDown(object sender, DragStartedEventArgs e)
        {
            timer.Tick -= timer_Tick;
        }
        private void OnLeftUp(object sender, DragCompletedEventArgs e)
        {
            timer.Tick += timer_Tick;
        }
        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var bubble = sender as Bubble;
            if (bubble == null) return;
            Canvas.SetLeft(this, Canvas.GetLeft(this) + e.HorizontalChange);
            Canvas.SetTop(this, Canvas.GetTop(this) + e.VerticalChange);
        }
        double X
        {
            get { return (double)Canvas.GetLeft(this); }
            set { Canvas.SetLeft(this, value); }
        }
        double Y
        {
            get { return (double)Canvas.GetTop(this); }
            set { Canvas.SetTop(this, value); }
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpDatePosition();
        }
    }
}
