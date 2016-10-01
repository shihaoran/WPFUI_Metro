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
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading;

namespace WpfMetro
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool mouseDown;
        private Point mouseXY;
        private Core CalculateCore;
        private int calmode = -1;
        System.Collections.ObjectModel.ObservableCollection<Member> memberData;


        private void IMG1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as ContentControl;
            if (img == null)
            {
                return;
            }
            img.CaptureMouse();
            mouseDown = true;
            mouseXY = e.GetPosition(img);
            Button a = new Button();
            a.Width = 50;
            a.Height = 50;
            a.Margin = new Thickness(10, 10, 0, 0);
            a.Visibility = Visibility.Visible;
        }



        private void IMG1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var img = sender as ContentControl;
            if (img == null)
            {
                return;
            }
            img.ReleaseMouseCapture();
            mouseDown = false;
        }



        private void IMG1_MouseMove(object sender, MouseEventArgs e)
        {
            var img = sender as ContentControl;
            if (img == null)
            {
                return;
            }
            if (mouseDown)
            {
                Domousemove(img, e);
            }
        }



        private void Domousemove(ContentControl img, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            var group = IMG.FindResource("Imageview") as TransformGroup;
            var transform = group.Children[1] as TranslateTransform;
            var position = e.GetPosition(img);
            transform.X -= mouseXY.X - position.X;
            transform.Y -= mouseXY.Y - position.Y;
            mouseXY = position;
        }


        private void IMG1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var img = sender as ContentControl;
            if (img == null)
            {
                return;
            }
            var point = e.GetPosition(img);
            var group = IMG.FindResource("Imageview") as TransformGroup;
            var delta = e.Delta * 0.001;
            DowheelZoom(group, point, delta);
        }



        private void DowheelZoom(TransformGroup group, Point point, double delta)
        {
            var pointToContent = group.Inverse.Transform(point);
            var transform = group.Children[0] as ScaleTransform;
            if (transform.ScaleX + delta < 0.1) return;
            transform.ScaleX += delta;
            transform.ScaleY += delta;
            var transform1 = group.Children[1] as TranslateTransform;
            transform1.X = -1 * ((pointToContent.X * transform.ScaleX) - point.X);
            transform1.Y = -1 * ((pointToContent.Y * transform.ScaleY) - point.Y);
        }

        private void button1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string s = textBoxFrom.Text;
            textBoxFrom.Text = textBoxTo.Text;
            textBoxTo.Text = s;
        }

        private async void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            string from = textBoxFrom.Text;
            string to = textBoxTo.Text;
            memberData.Clear();
            dataGrid.DataContext = memberData;
            Tuple<string, int> result;
            if (calmode == -1)
                await this.ShowMessageAsync("出错啦!", "请选择路径计算方式");
            else if (calmode == 0)
            {
                result = CalculateCore.DjistraPath(from, to);
                handleResult(result);
                tabControl.SelectedIndex = 1;
            }
            else if (calmode == 1)
            {
                result = CalculateCore.BFSPath(from, to);
                handleResult(result);
                tabControl.SelectedIndex = 1;
            }
            else if (calmode == 2)
            {
                result = CalculateCore.BFSPath(from, to);
                handleResult(result);
                tabControl.SelectedIndex = 1;
            }




        }
        private void Paint(object o)
        {
            string temp=(string)o;
            string[] s = temp.Split('\n');
            foreach (string line in s)
            {
                string[] linesplit = line.Split(' ');
                if (line.Equals(""))
                    continue;
                if (linesplit.Length == 1)
                {
                    Rectangle r = new Rectangle();
                    r.OpacityMask.
                    r.SetValue(Canvas.LeftProperty, 1.0);
                    r.SetValue(Canvas.TopProperty, 1.0);

                }
                else if (linesplit.Length == 2)
                {

                }
                else
                {
                    //await this.ShowMessageAsync("出错啦!", "呀计算模块好像出了问题");
                }
                dataGrid.DataContext = memberData;
            }

        }
        private async void handleResult(Tuple<string, int> result)
        {
            string[] s = result.Item1.Split('\n');
            if (result.Item2 == 0)
            {
                await this.ShowMessageAsync("出错啦!", result.Item1);
                return;
            }
            //删除之前的控件
            IEnumerable<Rectangle> query1 = IMG1.Children.OfType<Rectangle>();
            foreach (Rectangle r in query1)
            {
                IMG1.Children.Remove(r);
            }
            Thread paint = new Thread(Paint);
            paint.Start(result.Item1);


            Button b1 = new Button();
            b1.SetValue(Canvas.LeftProperty, 1.0);
            b1.SetValue(Canvas.TopProperty, 1.0);
            //b1.RenderTransform =IMG.FindResource("Imageview");
            Button b2 = new Button();
            b1.SetValue(Canvas.LeftProperty, 400.0);
            b1.SetValue(Canvas.TopProperty, 400.0);
            List<Button> list = new List<Button>();
            list.Add(b1);
            list.Add(b2);

            IMG1.Children.Add(b1);
            IMG1.Children.Add(b2);

            labelLen.Content = Convert.ToString(result.Item2);
            foreach (string line in s)
            {
                string[] linesplit = line.Split(' ');
                if (line.Equals(""))
                    continue;
                if (linesplit.Length == 1)
                {
                    memberData.Add(new Member()
                    {
                        Name = linesplit[0],
                        ID = CalculateCore.StaCollection[linesplit[0]].ID,
                        isTrans = false
                    });
                }
                else if (linesplit.Length == 2)
                {
                    memberData.Add(new Member()
                    {
                        Name = linesplit[0],
                        ID = CalculateCore.StaCollection[linesplit[0]].ID,
                        isTrans = true
                    });
                }
                else
                {
                    await this.ShowMessageAsync("出错啦!", "呀计算模块好像出了问题");
                }
                dataGrid.DataContext = memberData;
            }

        }

        private void radioButtonD_Checked(object sender, RoutedEventArgs e)
        {
            calmode = 0;
        }

        private void radioButtonB_Checked(object sender, RoutedEventArgs e)
        {
            calmode = 1;
        }

        private void radioButtonC_Checked(object sender, RoutedEventArgs e)
        {
            calmode = 2;
        }

        private void BackFrame_Loaded(object sender, RoutedEventArgs e)
        {
            CalculateCore = new Core();
            CalculateCore.ReadData();
            CalculateCore.BuildGragph("", "", "");
            memberData = new System.Collections.ObjectModel.ObservableCollection<Member>();
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            memberData.Clear();
            dataGrid.DataContext = memberData;
            labelLen.Content = "0";
        }
    }
    public class Member
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public bool isTrans { get; set; }
    }
}
