using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView
{
    public partial class VariableTermManualFactory
    {
        private readonly VariableTermManualFactoryViewModel _viewModel;

        public VariableTermManualFactory()
        {
            InitializeComponent();
            _viewModel = (VariableTermManualFactoryViewModel)LayoutRoot.DataContext;
            DataContext = _viewModel;
        }

        private void BtnAddNewMainRow_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNewMainRow(MainGrid.SelectedIndex != -1);
        }      

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {           
             if (e.Key == Key.Delete)
            {
                _viewModel.SelectedMainRows.Clear();
                foreach (var row in MainGrid.SelectedItems)
                {
                    _viewModel.SelectedMainRows.Add(row as TblVariableTermManualFactoryViewModel);
                }
                _viewModel.DeleteMainRow();
            }
        }      

        private void BtnSave_Onclick(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }

        private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
        {
            _viewModel.GetEmpByStore();
        }
    }
}