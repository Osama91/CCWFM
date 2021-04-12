
using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;


namespace Os.Controls.DataGrid
{

    public class WatermarkTextBox : TextBox
    {
        private bool _displayWatermark = true;
        private bool _hasFocus;

        public WatermarkTextBox()
        {
            GotFocus += WatermarkTextBox_GotFocus;
            LostFocus += WatermarkTextBox_LostFocus;
            TextChanged += WatermarkTextBox_TextChanged;
            Unloaded += WatermarkTextBox_Unloaded;
            Loaded += WatermarkTextBox_Loaded;
        }

        void WatermarkTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            SetMode(true);
            _displayWatermark = true;
            if (Watermark != null) Text = Watermark;
        }

        private void WatermarkTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_hasFocus && Text == "")
            {
                SetMode(true);
                _displayWatermark = true;
                if (Watermark != null) Text = Watermark;
            }
        }

        private void WatermarkTextBox_Unloaded(object sender, RoutedEventArgs e)
        {
            GotFocus -= WatermarkTextBox_GotFocus;
            LostFocus -= WatermarkTextBox_LostFocus;
            Unloaded -= WatermarkTextBox_Unloaded;
            TextChanged -= WatermarkTextBox_TextChanged;
        }

        private void WatermarkTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _hasFocus = true;
            if (_displayWatermark)
            {
                SetMode(false);
                Text = "";
            }
        }

        private void WatermarkTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _hasFocus = false;
            if (Text == "")
            {
                _displayWatermark = true;
                SetMode(true);
                Text = Watermark;
            }
            else
            {
                _displayWatermark = false;
            }
        }

        private void SetMode(bool watermarkStyle)
        {
            if (watermarkStyle)
            {
                FontStyle = FontStyles.Italic;
                Foreground = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                FontStyle = FontStyles.Normal;
                Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        public new string Watermark
        {
            get { return GetValue(WatermarkProperty) as string; }
            set { SetValue(WatermarkProperty, value); }
        }

        public new static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof (string), typeof (WatermarkTextBox),
                new PropertyMetadata(WatermarkPropertyChanged));

        private static void WatermarkPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var textBox = obj as WatermarkTextBox;
            if (textBox._displayWatermark)
            {
                textBox.Text = e.NewValue.ToString();
                textBox.SetMode(true);
            }
        }

    }
}
