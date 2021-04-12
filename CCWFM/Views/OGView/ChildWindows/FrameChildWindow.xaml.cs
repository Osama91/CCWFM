using System;
using System.Windows;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class FrameChildWindow
    {
        public EventHandler ClosedHandler;
        public FrameChildWindow()
        {
            InitializeComponent();
            Application.Current.RootVisual.SetValue(IsEnabledProperty, true);

        }

        protected virtual void OnItemPopulatingCompleted()
        {
            var handler = ClosedHandler;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void ImgClose_OnMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DialogResult = false;
        }
    }
}