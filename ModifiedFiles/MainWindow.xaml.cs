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
using System.ComponentModel;
using System.IO;

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
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            //执行任务代码
            bgWorker.DoWork += DoWork_Handler;
            //执行过程触发
            bgWorker.ProgressChanged += ProgressChanged_Handler;
            //执行结束，或有异常结束触发
            bgWorker.RunWorkerCompleted += RunWorkerCompleted_Handler;
        }

        private bool mouseDown;
        private Point mouseXY;
        private Core CalculateCore;
        private int calmode = -1;
        System.Collections.ObjectModel.ObservableCollection<Member> memberData;
        BackgroundWorker bgWorker = new BackgroundWorker();
        string[] pathresult;
        int speed = 2000;


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
            try
            {
                memberData.Clear();
            }
            catch (NullReferenceException e1)
            {
                System.Console.WriteLine(e1.Message);
                //TODO
                MessageBox.Show(e1.Message);
                return;
            }
            dataGrid.DataContext = memberData;
            Tuple<string, int> result;
            if (calmode == -1)
                await this.ShowMessageAsync("出错啦!", "请选择路径计算方式");
            else if (calmode == 0)
            {
                try
                {
                    result = CalculateCore.DijkstraPath(from, to);
                }
                catch (InputStationException e1)
                {
                    //TODO
                    System.Console.WriteLine(e1.Message);
                    MessageBox.Show(e1.Message);
                    return;
                }
                handleResult(result);
                tabControl.SelectedIndex = 1;
            }
            else if (calmode == 1)
            {
                try
                {
                    result = CalculateCore.BFSPath(from, to);
                }
                catch (InputStationException e1)
                {
                    //TODO
                    System.Console.WriteLine(e1.Message);
                    MessageBox.Show(e1.Message);
                    return;
                }

                handleResult(result);
                tabControl.SelectedIndex = 1;
            }
            else if (calmode == 2)
            {
                try
                {
                    result = CalculateCore.BFSPath(from, to);
                }
                catch (InputStationException e1)
                {
                    //TODO
                    System.Console.WriteLine(e1.Message);
                    MessageBox.Show(e1.Message);
                    return;
                }

                handleResult(result);
                tabControl.SelectedIndex = 1;
            }

        }

        private async void ProgressChanged_Handler(object sender, ProgressChangedEventArgs args)
        {
            //在过程改变事件中可以修改UI内容
            string line = pathresult[args.ProgressPercentage];
            string[] linesplit = line.Split(' ');
            if (linesplit.Length == 1)
            {
                string xaml1 = System.Windows.Markup.XamlWriter.Save(sta);
                Rectangle station1 = System.Windows.Markup.XamlReader.Parse(xaml1) as Rectangle;
                station1.SetValue(Canvas.LeftProperty, CalculateCore.StaCollection[linesplit[0]].position.Item1 - 5.00);
                station1.SetValue(Canvas.TopProperty, CalculateCore.StaCollection[linesplit[0]].position.Item2 - 24.00);
                IMG1.Children.Add(station1);
            }
            else if (linesplit.Length == 2)
            {
                string xaml2 = System.Windows.Markup.XamlWriter.Save(transsta);
                Rectangle station2 = System.Windows.Markup.XamlReader.Parse(xaml2) as Rectangle;
                station2.SetValue(Canvas.LeftProperty, CalculateCore.StaCollection[linesplit[0]].position.Item1 - 9.00);
                station2.SetValue(Canvas.TopProperty, CalculateCore.StaCollection[linesplit[0]].position.Item2 - 9.00);
                IMG1.Children.Add(station2);
            }
            else
            {
                await this.ShowMessageAsync("出错啦!", "呀计算模块好像出了问题");
            }
        }
        private void DoWork_Handler(object sender, DoWorkEventArgs args)
        {
            //在DoWork中修改UI同样会抛出异常
            //label.Content = "DoWork方法执行完成";
            BackgroundWorker worker = sender as BackgroundWorker;
            string[] s = args.Argument.ToString().Split('\n');
            for(int i=1;i<s.Length;i++)
            {
                string line = s[i];
                if (worker.CancellationPending)
                {
                    args.Cancel = true;
                    break;
                }
                else
                {
                    string[] linesplit = line.Split(' ');
                    if (line.Equals(""))
                        continue;
                    worker.ReportProgress(i);
                    int n = int.Parse(s[0]);
                    Thread.Sleep(n);
                }
            }
        }
        private async void RunWorkerCompleted_Handler(object sender, RunWorkerCompletedEventArgs args)
        {

            if (args.Cancelled)
            {
                await this.ShowMessageAsync("新消息!", "动画展示被用户取消");
            }
            else
            {
                await this.ShowMessageAsync("新消息!", "动画结束");
            }
        }

    
        private async void handleResult(Tuple<string, int> result)
        {
            pathresult = result.Item1.Split('\n');
            string[] s = result.Item1.Split('\n');
            if (result.Item2 == 0)
            {
                await this.ShowMessageAsync("出错啦!", result.Item1);//TODO???
                return;
            }
            //删除之前的控件
            IEnumerable<Rectangle> query1 = IMG1.Children.OfType<Rectangle>();
            for(int i=0;i<query1.Count();i++)
            {
                
            }
            IMG1.Children.Clear();
            IMG1.Children.Add(IMG2);

            if (!bgWorker.IsBusy&&visual.IsChecked.Value)
            {
                bgWorker.RunWorkerAsync(speed.ToString()+result.Item1);
            }

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
                    await this.ShowMessageAsync("出错啦!", "呀计算模块好像出了问题");//TODO???
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
            try
            {
                CalculateCore.ReadData();
            }
            catch (FileNotFoundException e1)
            {
                //TODO:这里要输出一个错误信息
                System.Console.WriteLine(e1.Message);
                MessageBox.Show(e1.Message);
                return;
            }
            catch (IOException e2)
            {
                //TODO:同样要输出错误信息
                System.Console.WriteLine(e2.Message);
                MessageBox.Show(e2.Message);
                return;
            }
            catch (MapErrorException e3)
            {
                //TODO:
                System.Console.WriteLine(e3.Message);
                MessageBox.Show(e3.Message);
                return;
            }

            CalculateCore.BuildGragph("", "", "");
            memberData = new System.Collections.ObjectModel.ObservableCollection<Member>();
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                memberData.Clear();
            }
            catch (NullReferenceException e1)
            {
                System.Console.WriteLine(e1.Message);
                //TODO
                MessageBox.Show(e1.Message);
                return;
            }
            dataGrid.DataContext = memberData;
            labelLen.Content = "0";
            bgWorker.CancelAsync();
            IMG1.Children.Clear();
            IMG1.Children.Add(IMG2);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            speed = (101 - (int)e.NewValue) * 20;
        }
    }
    public class Member
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public bool isTrans { get; set; }
    }
}
