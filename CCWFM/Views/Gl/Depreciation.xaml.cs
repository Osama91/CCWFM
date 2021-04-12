using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel.Gl;
using CCWFM.Views.Gl.ChildWindow;
using Os.Controls.DataGrid.Events;

namespace CCWFM.Views.Gl
{
    public partial class Depreciation
    {
        private readonly DepreciationViewModel _viewModel;

        public Depreciation()
        {
            InitializeComponent();

            _viewModel = new DepreciationViewModel();
            DataContext = _viewModel;
            MyPopup.DataContext = _viewModel;
            _viewModel.PremCompleted += (s, sv) =>
            {
                BtnApprove.Visibility = _viewModel.CustomePermissions.SingleOrDefault(x => x.Ename == "Approval") != null ? Visibility.Visible : Visibility.Collapsed;
            };
            SwitchFormMode(FormMode.Add);
        }

        public FormMode FormMode { get; set; }

        private void ResetMode()
        {
            FormMode = FormMode.Standby;
            SwitchFormMode(FormMode);
        }

        private void ClearScreen()
        {
            _viewModel.SelectedMainRow = null;
            _viewModel.AddNewMainRow(false);
            BtnAddNewCard.IsChecked = false;
        }

        private void SwitchFormMode(FormMode formMode)
        {
            switch (formMode)
            {
                case FormMode.Add:
                    ClearScreen();
                    BtnSave.Visibility = Visibility.Visible;
                    BtnShowSearch.Visibility = Visibility.Collapsed;
                    BtnCancel.Visibility = Visibility.Visible;
                    BtnCancel.IsEnabled = true;
                    break;

                case FormMode.Standby:
                    BtnAddNewCard.IsEnabled = true;
                    BtnAddNewCard.Visibility = Visibility.Visible;
                    BtnSearch.IsEnabled = true;
                    BtnSave.Visibility = Visibility.Collapsed;
                    BtnShowSearch.Visibility = Visibility.Visible;
                    BtnShowSearch.IsEnabled = true;
                    ClearScreen();
                    break;

                case FormMode.Search:
                    BtnAddNewCard.IsEnabled = false;
                    BtnAddNewCard.Visibility = Visibility.Collapsed;
                    BtnSave.Visibility = Visibility.Visible;
                    BtnShowSearch.IsEnabled = false;
                    break;

                case FormMode.Update:
                    BtnSave.Visibility = Visibility.Visible;
                    break;

                case FormMode.Read:
                    BtnSearch.IsEnabled = false;
                    BtnSave.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void btnAddNewCard_Checked(object sender, RoutedEventArgs e)
        {
            FormMode = FormMode.Add;
            SwitchFormMode(FormMode);
        }

        private void btnShowSearch_Checked(object sender, RoutedEventArgs e)
        {
            BtnCancel.Visibility = Visibility.Visible;
            BtnCancel.IsEnabled = true;
            FormMode = FormMode.Search;
            SwitchFormMode(FormMode);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.MainRowList.Clear();
            _viewModel.GetMaindata();
            var childWindowSeach = new DepreciationChildWindow(_viewModel);
            childWindowSeach.Show();
            FormMode = FormMode.Search;
            SwitchFormMode(FormMode);
        }

        private void btnDeleteCard_Click(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.SelectedMainRow.Approved)
            {
                var r =
                    MessageBox
                        .Show(
                            "You are about to delete The Transaction permanently!!\nPlease note that this action cannot be undone!"
                            , "Delete", MessageBoxButton.OKCancel);
                if (r == MessageBoxResult.OK)
                {
                    _viewModel.DeleteMainRow();
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow(false);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ResetMode();
            BtnCancel.IsEnabled = false;
            BtnCancel.Visibility = Visibility.Collapsed;
            BtnDeleteCard.Visibility = Visibility.Collapsed;
            BtnDeleteCard.IsEnabled = false;
            BtnShowSearch.IsChecked = false;
        }

        private void btnPrintPreview_Click(object sender, RoutedEventArgs e)
        {
            //var para = new ObservableCollection<string> { _viewModel.SelectedMainRow.Iserial.ToString(CultureInfo.InvariantCulture) };

            //var reportViewmodel = new GenericReportViewModel();
            //reportViewmodel.GenerateReport(_setting.Report, para);
        }

        private void BtnAddDetail_Click(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;

            var gt = b.TransformToVisual(Application.Current.RootVisual);
            var p = gt.Transform(new Point(0, b.ActualHeight));
            MyPopup.HorizontalOffset = p.X;
            MyPopup.VerticalOffset = p.Y + 60;
            MyPopup.IsOpen = true;
            _viewModel.GetLedgerDetail();
        }

        private void BtnApprove_OnClick(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow(true);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var row = DetailGrid.SelectedItem as TblLedgerMainDetailViewModel;

            _viewModel.SelectedMainRow.TblLedgerMainDetail = row.Iserial;
            _viewModel.SelectedMainRow.BookValue = (double?) row.CrAmount;
            if (row.CrAmount == 0 || row.CrAmount==null)
            {
                _viewModel.SelectedMainRow.BookValue = (double?) row.DrAmount;
            }
           
            MyPopup.IsOpen = false;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = false;
        }

        private void DetailGrid_OnOnFilter(object sender, FilterEvent e)
        {
            _viewModel.TblLedgerDetailsList.Clear();
            string filter;
            Dictionary<string, object> valueObjecttemp;
            GeneralFilter.GeneralFilterMethod(out filter, out valueObjecttemp, e);
            _viewModel.DetailFilter = filter;
            _viewModel.DetailValuesObjects = valueObjecttemp;
            _viewModel.GetLedgerDetail();
        }
    }
}