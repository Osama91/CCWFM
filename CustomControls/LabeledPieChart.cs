using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;

namespace CustomControls
{
    [TemplatePart(Name = "LabelArea_PART", Type = typeof(PieChartLabelArea))]
    public class LabeledPieChart : Chart
    {
        public LabeledPieChart()
        {
            this.DefaultStyleKey = typeof(LabeledPieChart);
        }
    }
}