using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CCWFM.ViewModel.OGViewModels.ControlsOverride
{
    public class ChildWindowsOverride : ChildWindow
    {
        public ChildWindowsOverride()
        {
            UpdateSize(null, EventArgs.Empty);

            Application.Current.Host.Content.Resized += UpdateSize;
        }

        protected override void OnClosed(EventArgs e)
        {
            Application.Current.Host.Content.Resized -= UpdateSize;
        }

        private void UpdateSize(object sender, EventArgs e)
        {
            Width = Application.Current.Host.Content.ActualWidth;
            Height = Application.Current.Host.Content.ActualHeight;
            UpdateLayout();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                DialogResult = false;
              //  Close();
            }
            base.OnKeyDown(e);
        }
    }
}