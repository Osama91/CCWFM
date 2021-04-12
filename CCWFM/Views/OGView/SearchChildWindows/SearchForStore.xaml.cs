using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.ViewModel.OGViewModels;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class SearchForStore
    {
        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            var handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        public class TblStoreViewModel : TblStore
        {
            private bool _saved;

            public bool Saved
            {
                get { return _saved; }
                set { _saved = value; RaisePropertyChanged("Saved"); }
            }
        }

        private readonly DepositViewModel _viewModel;

        public SearchForStore(DepositViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
            _viewModel.StoreList.Clear();
        }

        private readonly CRUD_ManagerServiceClient _client = new CRUD_ManagerServiceClient();

        public SearchForStore(UserJobsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            _client.SeasonForStoreByUserCompleted += (s, sv) =>
            {
                viewModel.StoreList.Clear();
                var iserials = new List<int>();
                if (viewModel.SelectedMainRow.AllowedStores != null)
                {
                    iserials = viewModel.SelectedMainRow.AllowedStores.Split('|').Select(int.Parse).ToList();
                }
                foreach (var tblStore in sv.Result)
                    {
                        var newrow = new TblStoreViewModel();
                        newrow.InjectFrom(tblStore);
                        //newrow.Saved = iserials.Contains(tblStore.iserial);
                        viewModel.StoreList.Add(newrow);
                    }
                    
                
            };
            string code = viewModel.SelectedMainRow.CompanyPerRow.Code;

            if (viewModel.SelectedMainRow.TblCompanySecondary!=null)
            {
                code =
                    viewModel.CompanyList.FirstOrDefault(x => x.Iserial == viewModel.SelectedMainRow.TblCompanySecondary)
                        .Code;

            }
               
               if (code=="HQ")
            {
                code = "Ccnew";
            }

            _client.SeasonForStoreByUserAsync(null, null, code);
        }

   
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.SelectedItem != null)
            {
                if (_viewModel != null)
                {
                    _viewModel.TransactionHeader.StorePerRow = (TblStore)MainGrid.SelectedItem;
                }
                else
                {
                    var viewModel = DataContext as UserJobsViewModel;
                    if (viewModel != null)
                    {
                        viewModel.SelectedMainRow.AllowedStores = null;
                        foreach (TblStoreViewModel row in viewModel.StoreList.Where(x => x.Saved))
                        {
                            viewModel.SelectedMainRow.AllowedStores = viewModel.SelectedMainRow.AllowedStores + row.iserial + "|";
                        }
                        if (viewModel.SelectedMainRow.AllowedStores != null)
                            viewModel.SelectedMainRow.AllowedStores = viewModel.SelectedMainRow.AllowedStores.Remove(viewModel.SelectedMainRow.AllowedStores.Length - 1, 1);
                    }
                }
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}