namespace System.Windows.Controls
{
    public class CustomeToolTip : ToolTip
    {
        public static DependencyProperty TooltipProperty;

        static CustomeToolTip()
        {
            TooltipProperty = DependencyProperty.RegisterAttached
                ("Tooltip", typeof(object),
                typeof(CustomeToolTip),
                new PropertyMetadata(TooltipChanged));
        }

        public static object GetToolTip(DependencyObject d)
        {
            return d.GetValue(TooltipProperty);
        }

        public static void SetTooltip(DependencyObject d, object value)
        {
            d.SetValue(TooltipProperty, value);
        }

        private static void owner_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement)
            {
                var owner = sender as FrameworkElement;
                // remove the event handler owner.Loaded -= new RoutedEventHandler(owner_Loaded);

                var tooltip =
                    owner.GetValue(TooltipProperty) as DependencyObject;
                if (tooltip != null)
                {
                    // assign the data context of the current owner control to the tooltip's datacontext tooltip.SetValue(FrameworkElement.DataContextProperty,
                    owner.GetValue(DataContextProperty);
                }
                ToolTipService.SetToolTip(owner, tooltip);
            }
        }

        private static void TooltipChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            if (sender is FrameworkElement)
            {
                var owner = sender as FrameworkElement;
                // wait for it to be in the visual tree so that // context can be established owner.Loaded += new RoutedEventHandler(owner_Loaded);
            }
        }
    }
}