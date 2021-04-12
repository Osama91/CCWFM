using System;
using System.Windows;
using CCWFM.ViewModel.Gl;
using System.Windows.Controls;

namespace CCWFM.Views.Gl.ChildWindow
{
    public partial class GeneratePeroidLinesChildWindow
    {        
        private readonly PeriodGlViewModel _viewModelGl;     
        public GeneratePeroidLinesChildWindow(PeriodGlViewModel viewModel)
        {
            InitializeComponent();
            _viewModelGl = viewModel;
            DataContext = viewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (FromDatePicker.SelectedDate != null && ToDatePicker.SelectedDate != null)
            {
               
                    _viewModelGl.GeneratePeriodLines((DateTime) FromDatePicker.SelectedDate,
                        (DateTime) ToDatePicker.SelectedDate);
              
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }       
    }
}