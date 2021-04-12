using System.Windows;
using System.Windows.Controls;

namespace CCWFM.Helpers
{
    public class DataGridExtenders
    {
        public static void SetIsReadOnly(DependencyObject obj, bool isReadOnly)
        {
            obj.SetValue(IsReadOnlyProperty, isReadOnly);
        }

        public static bool GetIsReadOnly(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsReadOnlyProperty);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.RegisterAttached("IsReadOnly", typeof(bool), typeof(DataGridExtenders), new PropertyMetadata(false, Callback));

        private static void Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataGridTextColumn)d).IsReadOnly = (bool)e.NewValue;
        }


    }
}