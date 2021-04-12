using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.OGViewModels;
using System.Collections.Generic;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class SalesOrderColorCancelRequest
    {
        public StyleHeaderViewModel ViewModel;

        public SalesOrderColorCancelRequest(StyleHeaderViewModel styleViewModel)
        {
            InitializeComponent();
            DataContext = styleViewModel;
            ViewModel = styleViewModel;
            GetSalesOrderColorsBySalesOrderType();
        }
  
        private void GetSalesOrderColorsBySalesOrderType()
        {
            LkpData.LkpDataClient _client = new LkpData.LkpDataClient();
            _client.GetTblSalesOrderColorAsync( ViewModel.SelectedMainRow.Iserial, 
                                                (int) ViewModel.SalesOrderType);

            _client.GetTblSalesOrderColorCompleted += (s, sv) => 
            {
                List<CancelColorRequest> SalesOrderColors = new List<CancelColorRequest>();

                foreach (var item in sv.Result)
                {
                    CancelColorRequest color = new CancelColorRequest();
                    color.Iserial = item.Iserial;
                    color.Checked =  (item.RequestForCancel == 1  || item.RequestForCancel == 2) ? true : false;
                    color.Ename = item.TblColor1.Ename;
                    color.IsEnabled = item.RequestForCancel == 2 ? false : true;
                    color.StyleCode = ViewModel.SelectedMainRow.StyleCode;
                    SalesOrderColors.Add(color);
                }
                grdColorCancelRequest.ItemsSource = null;
                grdColorCancelRequest.ItemsSource = SalesOrderColors;
                
            };
        }

        private void CancelColorCheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ColorCancelRequestItem = grdColorCancelRequest.SelectedItem as CancelColorRequest;
                if (ColorCancelRequestItem.IsEnabled == true)
                {
                    LkpData.LkpDataClient _client = new LkpData.LkpDataClient();
                    _client.UpdateSalesOrderColorCancelRequestAsync(ColorCancelRequestItem.Iserial, ColorCancelRequestItem.Checked);
                    _client.UpdateSalesOrderColorCancelRequestCompleted += (s, sv) => { };
                }
            }
            catch { }
        }


    }

    public class CancelColorRequest
    {
        public int Iserial { get; set; }

        public string StyleCode { get; set; }

        public string Ename { get; set; }

        public bool Checked { get; set; }

        public bool IsEnabled { get; set; }
    }
}