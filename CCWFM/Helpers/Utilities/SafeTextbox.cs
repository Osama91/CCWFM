using System.Windows;
using System.Windows.Controls;

namespace CCWFM.Helpers.Utilities
{
    public class SafeTextbox : TextBox
    {
        public SafeTextbox()
        {
            LostFocus += SafeTextbox_LostFocus;
            TextChanged += SafeTextbox_TextChanged;
        }

        private void SafeTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = Text.Replace(",", "");
            Text = Text.Replace("'", "");
        }

        private void SafeTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            Text = Text.Replace(",", "");
            Text = Text.Replace("'", "");
        }

    }
}