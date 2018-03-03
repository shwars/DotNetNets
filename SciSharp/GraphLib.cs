using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SciSharp
{
    public class GraphLib
    {
        public static GraphLib Create()
        {
            GraphLib G;
            G = new GraphLib();

            var T = new Thread(() =>
            {
                G.MyThread = Thread.CurrentThread;
                G.RunningInBackground = true;
                G.MainForm.ShowDialog();
            });
            T.Start();
            return G;
        }

        public Thread MyThread { get; set; }
        public Form MainForm { get; set; }
        public ChartArea Area { get; set; }
        public Chart Chart { get; set; }
        
        public bool RunningInBackground { get; private set; }

        public GraphLib()
        {
            MainForm = new Form()
            {
                // Visible = true,
                TopMost = true,
                Width = 700,
                Height = 500
            };

            RunningInBackground = false;

            Chart = new Chart() { Dock = DockStyle.Fill };
            Area = new ChartArea("Main");
            Chart.ChartAreas.Add(Area);
            MainForm.Controls.Add(Chart);
        }

        public void Plot(IEnumerable<float> x, SeriesChartType type = SeriesChartType.Bar)
        {
            var seriesColumns = new Series("Data");
            seriesColumns.ChartType = type;
            Chart.Series.Add(seriesColumns);
            foreach (var z in x) seriesColumns.Points.Add(z);
            if (!RunningInBackground) MainForm.ShowDialog();
        }

        public void Plot(IEnumerable<float> x, IEnumerable<float> y, SeriesChartType type = SeriesChartType.Point)
        {
            var seriesColumns = new Series("Data");
            seriesColumns.ChartType = type;
            Chart.Series.Add(seriesColumns);
            var xa = x.ToArray(); var ya = y.ToArray();
            for (int i = 0; i < xa.Length; i++)
                seriesColumns.Points.Add(new DataPoint(xa[i], ya[i]));
            if (!RunningInBackground) MainForm.ShowDialog();
        }
    }
}
