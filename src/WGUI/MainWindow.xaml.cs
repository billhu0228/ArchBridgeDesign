using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Model;

namespace WGUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private struct Point3DPlus
        {
            public Point3DPlus(Point3D point, Color color, double thickness)
            {
                this.point = point;
                this.color = color;
                this.thickness = thickness;
            }

            public Point3D point;
            public Color color;
            public double thickness;
        }

        private Stopwatch stopwatch = Stopwatch.StartNew();

        private string filePath;

        private BackgroundWorker bw;

        private ConcurrentQueue<List<Point3DPlus>> pointsList = new ConcurrentQueue<List<Point3DPlus>>();

        private ArchAxis theArchAxis;

        public MainWindow()
        {
            InitializeComponent();
            plot.CameraChanged += Plot_CameraChanged;

            List<Location> persons = new List<Location>();
            Location person1 = new Location() { Name = "基本设置", };
            Location child1 = new Location() { Name = "拱轴", };
            person1.Children.Add(child1);
            Location child2 = new Location() { Name = "拱系", };
            person1.Children.Add(child2);
            person1.Children.Add(new Location() { Name = "拱脚标高", });

            Location person2 = new Location() { Name = "材料与截面", };
            Location person3 = new Location() { Name = "拱上建筑", };

            persons.Add(person1);
            persons.Add(person2);
            persons.Add(person3);
            person1.IsExpanded = true;
            person1.IsSelected = true;
            tree.ItemsSource = persons;
            //OpenFile();

            bw = new BackgroundWorker();

            theArchAxis = new ArchAxis(518 / 4.5, 2.1, 518);

        }

        //private void btnSelectNext_Click(object sender, RoutedEventArgs e)
        //{
        //    if (tree.SelectedItem != null)
        //    {
        //        var list = (tree.ItemsSource as List<Location>);
        //        int curIndex = list.IndexOf(tree.SelectedItem as Location);
        //        if (curIndex >= 0)
        //            curIndex++;
        //        if (curIndex >= list.Count)
        //            curIndex = 0;
        //        if (curIndex >= 0)
        //            list[curIndex].IsSelected = true;
        //    }
        //}
        //private void btnToggleExpansion_Click(object sender, RoutedEventArgs e)
        //{
        //    if (tree.SelectedItem != null)
        //        (tree.SelectedItem as Location).IsExpanded = !(tree.SelectedItem as Location).IsExpanded;
        //}

        private void Plot_CameraChanged(object sender, RoutedEventArgs e)
        {
            CameraPosBlk.Text = ToStringPretty(plot.Camera.Position);
            CameraLookAtBlk.Text = "\t" + ToStringPretty(plot.Camera.LookDirection);
        }


        private string ToStringPretty(Point3D pt)
        {
            return string.Format("({0:F1},{1:F1},{2:F1})", pt.X, pt.Y, pt.Z);
        }
        private string ToStringPretty(Vector3D pt)
        {
            return string.Format("({0:F1},{1:F1},{2:F1})", pt.X, pt.Y, pt.Z);
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            var openDialog = new System.Windows.Forms.OpenFileDialog();
            openDialog.Multiselect = false;
            openDialog.CheckPathExists = true;

            if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = openDialog.FileName;

                if (!File.Exists(filePath))
                {
                    System.Windows.MessageBox.Show("File is NOT exists!", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            else
            {
                Environment.Exit(-1);
            }
            bw.DoWork += GatherData;
            bw.RunWorkerAsync();
        }

        private void GatherData(object sender, DoWorkEventArgs e)
        {
            var readDataStream = new System.IO.StreamReader(filePath, Encoding.UTF8);

            while (true)
            {
                Thread.Sleep(100);  // can use to change the poltting rate

                //Generate a test trace: an upward spiral with square corners

                #region old test code

                //double t = stopwatch.Elapsed.TotalSeconds * 0.25;
                //double sint = Math.Sin(t);
                //double cost = Math.Cos(t);
                //double x, y, z = t * 0.5;
                //Color color;

                //if (sint > 0.0 && cost > 0.0)
                //{
                //    if (sint > cost)
                //    {
                //        x = 100.0;
                //        y = 70.71 * cost + 50.0;
                //    }
                //    else
                //    {
                //        x = 70.71 * sint + 50.0;
                //        y = 100.0;
                //    }
                //    color = Colors.Red;
                //}
                //else if (sint < 0.0 && cost < 0.0)
                //{
                //    if (sint < cost)
                //    {
                //        x = 0.0;
                //        y = 70.71 * cost + 50.0;
                //    }
                //    else
                //    {
                //        x = 70.71 * sint + 50.0;
                //        y = 0.0;
                //    }
                //    color = Colors.Red;
                //}
                //else
                //{
                //    x = 50.0 * sint + 50.0;
                //    y = 50.0 * cost + 50.0;
                //    color = Colors.Blue;
                //}

                #endregion old test code

                List<Point3DPlus> points = new List<Point3DPlus>();

                Color color = Colors.Black;
                double x = 0, y = 0, z = 0;

                const int addPointSum = 50; //can use to change the poltting rate

                for (int i = 0; i < addPointSum; i++)
                {
                    if (readDataStream.EndOfStream)
                    {
                        break;
                    }

                    var readDataLine = readDataStream.ReadLine();

                    var readDataNumbers = readDataLine.Split(' ');

                    try
                    {
                        x = Convert.ToDouble(readDataNumbers[0]);
                        y = Convert.ToDouble(readDataNumbers[1]);

                        if (readDataNumbers.Length >= 3)
                        {
                            z = Convert.ToDouble(readDataNumbers[2]);
                        }
                        else
                        {
                            z = 0;
                        }
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show("Data format is error. The format is 'x y z'.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        Environment.Exit(-1);
                    }

                    var point = new Point3DPlus(new Point3D(x, y, z), color, 1.5);

                    points.Add(point);
                }

                bool invoke = false;
                if (points.Count > 0)
                {
                    pointsList.Enqueue(points);
                    invoke = true;
                }

                if (invoke)
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)PlotData);
                }
            }
        }

        private void PlotData()
        {
            if (pointsList.Count > 0)
            {
                bool resultPop = pointsList.TryDequeue(out var points);
                if (resultPop)
                {
                    var pointsArray = points.ToArray();

                    foreach (Point3DPlus point in pointsArray)
                        plot.AddPoint(point.point, point.color, point.thickness);
                }
            }
        }

        private void btnZoom_Click(object sender, RoutedEventArgs e)
        {
            plot.ZoomExtents(500);  // zoom to extents
                                    //plot.ResetCamera();  // orient and zoom
        }

        private void btnLeave_Click(object sender, RoutedEventArgs e)
        {
            plot.Camera.Position -= (10 * plot.Camera.LookDirection);
        }

        private void tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MessageBox.Show(((Location)e.NewValue).Name);

        }

        private void Test_Button_Click(object sender, RoutedEventArgs e)
        {
            plot.CreateAxis(theArchAxis);
        }

        private void tree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var tv = (TreeView)sender;
            Location item = (Location)tv.SelectedValue;


            if (item.Name == "拱轴")
            {
                InputWindow ipt = new InputWindow();
                ipt.Show();
            }
            else if (item.Children.Count == 0)
            {
                MessageBox.Show(item.Name);
            }

            ;//MessageBox.Show(((Location)e.NewValue).Name);
        }
    }
}
