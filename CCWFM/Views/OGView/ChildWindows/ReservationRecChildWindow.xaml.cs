using System.Windows;
using System.Windows.Input;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class ReservationRecChildWindow
    {
        public ReservationRecChildWindow(ReservationViewModel reservationViewModel)
        {
            InitializeComponent();
            DataContext = reservationViewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ChildWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    DialogResult = false;
                    break;
            }
        }
    }
}