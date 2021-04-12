using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.OGViewModels;
using System.ComponentModel;
using System.Diagnostics;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using CCWFM.CRUDManagerService;
using System.ComponentModel.DataAnnotations;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class StyleTNAStatus
    {
        private readonly StyleTNAStatusViewModel ViewModel;

        public StyleTNAStatus(StyleHeaderViewModel styleViewModel)
        {
            InitializeComponent();
            ViewModel = new StyleTNAStatusViewModel(styleViewModel);
            DataContext = ViewModel;

            ViewModel.ProductionClient.UpdateOrInsertTblStyleTNAStatusDetailCompleted += (s, x) =>
            {
                var savedRow = (TblStyleTNAStatusDetailModel)ViewModel.MainRowList.GetItemAt(x.outindex);
                if (savedRow != null) savedRow.InjectFrom(x.Result);

                ViewModel.TempStyleViewModel.Loading = false;
                ViewModel.TempStyleViewModel.SelectedTnaRow.TblStyleTNAStatus = x.Result.TblStyleTnaStatus;
                ViewModel.Loading = false;
                DialogResult = true;
            };

        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveMainRow();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public void SaveFromStyleHeader()
        {
            ViewModel.AddNewMainRow();
            ProductionService.ProductionServiceClient _client = new ProductionService.ProductionServiceClient();
            foreach (var row in ViewModel.MainRowList.Where(x => x.Iserial == 0))
            {
                var isvalid = Validator.TryValidateObject(row,
                    new ValidationContext(row, null, null), null, true);

                if (isvalid)
                {
                    var saveRow = new ProductionService.TblStyleTNAStatusDetail();

                    saveRow.InjectFrom(row);
                    int x = ViewModel.MainRowList.IndexOf(row); //Retail Approved
                    saveRow.TblStyleTnaStatus = 1;
                    _client.UpdateOrInsertTblStyleTNAStatusDetailAsync(saveRow, ViewModel.MainRowList.IndexOf(row));

                }


                //_client.GetTblStyleTNAStatusDetailAsync(ViewModel.TempStyleViewModel.SelectedTnaRow.Iserial);
                //_client.GetTblStyleTNAStatusDetailCompleted += (s, sv) =>
                //{
                //    //if (sv.Error != null)
                //    //{
                //    //    MessageBox.Show(sv.Error.Message);
                //    //}

                //    //foreach (var row in sv.Result)
                //    //{
                //    //    var newrow = new TblStyleTNAStatusDetailModel();
                //    //    newrow.InjectFrom(row);
                //    //    newrow.UserPerRow = row.TblAuthUser1;
                //    //    newrow.StyleTNAStatusPerRow = new GenericTable();
                //    //    if (row.TblStyleTNAStatu != null)
                //    //    {
                //    //        GenericTable newTempRow = new GenericTable().InjectFrom(row.TblStyleTNAStatu) as GenericTable;
                //    //        newrow.StyleTNAStatusPerRow = newTempRow;
                //    //    }
                //    //    ViewModel.MainRowList.Add(newrow);
                //    //}

                //    }
                //   // ViewModel.SaveMainRow();
                //};

            }
        }
    }
}