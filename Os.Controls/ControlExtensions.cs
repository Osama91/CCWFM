using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Os.Controls
{
    public static class ControlExtensions
    {
        public static bool HasFocus(this Control aControl, bool aCheckChildren)
        {
            var oFocused = FocusManager.GetFocusedElement() as DependencyObject;
            if (!aCheckChildren)
                return oFocused == aControl;
            while (oFocused != null)
            {
                if (oFocused == aControl)
                    return true;
                oFocused = VisualTreeHelper.GetParent(oFocused);
            }
            return false;
        }
    }
}