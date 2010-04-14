using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfChart.TimeSeriesDataLib;
using System.Windows.Threading;
using System.Threading;

namespace WpfChart
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mChart.SetTitle("Li Gao's Stock Chart Simulation");
            mChart.SetStaticYLabel("Million of Dollars");

            FillSampleData();

        }

        private void FillSampleData()
        {
            mChart.ClearAllSeries();

            TimeSeriesData data1 = new TimeSeriesData();
            data1.Name = "LG001";
            data1.StrokeColor = Colors.DarkBlue;
            data1.IsAreaMode = false;
            data1.Clear();

            TimeSeriesData data2 = new TimeSeriesData();
            data2.Name = "LG002";
            data2.StrokeColor = Colors.Red;
            data2.IsAreaMode = false;
            data2.Clear();

            TimeSeriesData data3 = new TimeSeriesData();
            data3.Name = "LG003";
            data3.StrokeColor = Colors.Green;
            data3.IsAreaMode = false;
            data3.Clear();

            RandomDataSeries ranData1 = new RandomDataSeries();
            Thread.Sleep(137);
            RandomDataSeries ranData2 = new RandomDataSeries();
            Thread.Sleep(153);
            RandomDataSeries ranData3 = new RandomDataSeries();
            ranData1.ValueScale = 300;
            ranData2.ValueScale = 400;
            ranData2.SampleFrequencyPerMin = 0.8;
            ranData3.ValueScale = 200;
            ranData3.SampleFrequencyPerMin = 0.5;
                    
            List<TimeSeriesDataPoint> points1 = new List<TimeSeriesDataPoint>();
            ranData1.ReSeed();
            for (int i = 0; i <= 65; i++)
            {
                points1.Add(ranData1.NextDataPoint);
            }

            List<TimeSeriesDataPoint> points2 = new List<TimeSeriesDataPoint>();
            ranData2.ReSeed();
            for (int i = 0; i <= 13; i++)
            {
                points2.Add(ranData2.NextDataPoint);
            }

            List<TimeSeriesDataPoint> points3 = new List<TimeSeriesDataPoint>();
            ranData3.ReSeed();
            for (int i = 0; i <= 8; i++)
            {
                points3.Add(ranData3.NextDataPoint);
            }
            
            mChart.AddSeries(data1);
            mChart.AddSeries(data2);           
            mChart.AddSeries(data3);
          
            data1.AddPointsRange(points1.ToArray());
            data2.AddPointsRange(points2.ToArray());            
            data3.AddPointsRange(points3.ToArray());
        }
    }
}
