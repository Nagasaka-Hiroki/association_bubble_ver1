using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using association_bubble_ver1.Models;
using System.Windows.Controls.Primitives;

namespace association_bubble_ver1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        Queue<Bubble> queue = new Queue<Bubble>();
        private void Button_Dequeue(object sender, RoutedEventArgs e)
        {
            if (queue.Count <= 0) return;
            Bubble bubble = queue.Dequeue();
            canvas.Children.Remove(bubble);
        }
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Return) & (textbox.Text != ""))
            {
                Bubble bubble = new Bubble(textbox.Text);
                queue.Enqueue(bubble);
                canvas.Children.Add(bubble);
                textbox.Text = "";
            }
        }
    }
}
