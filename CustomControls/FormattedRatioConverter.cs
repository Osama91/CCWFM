using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace CustomControls
{
    public class FormattedRatioConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var currentElement = value as TextBlock;
            if (currentElement != null)
            {
                var label = TreeHelper.FindAncestor<PieChartLabel>(currentElement);
                if (label != null)
                {
                    return label.FormattedRatio;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}