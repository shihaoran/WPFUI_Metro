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
        Task<Tuple<string, int>> t1;
        


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
                //空引用异常 程序直接结束
                await this.ShowMessageAsync("出错啦!", e1.Message);
                Application.Current.Shutdown();
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
                    //站点输入异常
                    await this.ShowMessageAsync("出错啦!", e1.Message);
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
                    //站点输入异常
                    await this.ShowMessageAsync("出错啦!", e1.Message);
                    return;
                }

                handleResult(result);
                tabControl.SelectedIndex = 1;
            }
            else if (calmode == 2)
            {
                if(t1!=null)
                {
                    if (!t1.IsCompleted)
                    {
                        await this.ShowMessageAsync("Running!", "当前的后台任务还没有执行完毕，请您耐心等待，您现在可以使用遍历全线以外的功能查询其他路线");
                        return;
                    }
                }
                t1 = new Task<Tuple<string, int>>
                    (CalChinPath, from, TaskCreationOptions.PreferFairness | TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
                t1.Start();
                await this.ShowMessageAsync("Running!", "您启动了遍历全线功能，程序正在后台为您计算，大概需要1分钟左右，在此期间您可以继续查询其他路线，当计算完毕时，结果将为您展示");
                result = await t1; 
                if (result == null)
                    return;
                await this.ShowMessageAsync("算完啦!", "您之前查询的由\"" + from + "\"站开始的遍历结果已经算完啦，将为您呈现动画");
                handleResult(result);
                tabControl.SelectedIndex = 1;
            }
        }
        public Tuple<string,int> CalChinPath(object o)
        {
            string from = (string)o;
            Tuple<string,int> result;
            try
            {
                result = CalculateCore.ChinPostPath(from);
            }
            catch (InputStationException e)
            {
                MessageBox.Show(e.Message + " 本次计算无效。");
                return null;
            }
            return result;
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
                await this.ShowMessageAsync("出错啦!", result.Item1);
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
                        isTrans = false,
                        TransLine=""
                    });
                }
                else if (linesplit.Length == 2)
                {
                    memberData.Add(new Member()
                    {
                        Name = linesplit[0],
                        ID = CalculateCore.StaCollection[linesplit[0]].ID,
                        isTrans = true,
                        TransLine= linesplit[1]
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

        private async void BackFrame_Loaded(object sender, RoutedEventArgs e)
        {
            memberData = new System.Collections.ObjectModel.ObservableCollection<Member>();

            CalculateCore = new Core();
            try
            {
                CalculateCore.ReadData();
            }
            catch (FileNotFoundException e1)
            {
                //这里要输出一个错误信息  程序退出 以下三个try catch
                await this.ShowMessageAsync("地图文件错误！", "找不到地图文件，程序即将关闭，请稍后重新启动程序。错误信息:" + e1.Message);
                Application.Current.Shutdown();
                return;
            }
            catch (OutOfMemoryException e1)
            {
                await this.ShowMessageAsync("发生错误！", "程序即将关闭，请稍后重新启动程序。错误信息:" + e1.Message);
                Application.Current.Shutdown();
                return;
            }
            catch (IOException e2)
            {
                //同样要输出错误信息
                await this.ShowMessageAsync("发生错误！", "读入文件时发生IO错误，程序即将关闭，请稍后重新启动程序。错误信息:" + e2.Message);
                Application.Current.Shutdown();
                return;
            }
            catch (MapErrorException e3)
            {
                //
                await this.ShowMessageAsync("地图文件错误！", e3.Message + "程序即将关闭，请稍后重新启动程序。");
                Application.Current.Shutdown();
                return;
            }
            CalculateCore.BuildGragph("", "", "");
        }

        private async void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                memberData.Clear();
            }
            catch (NullReferenceException e1)
            {
                //空引用异常 程序直接结束
                await this.ShowMessageAsync("出错啦!", e1.Message);
                Application.Current.Shutdown();
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
        public string TransLine { get; set; }
    }
}
