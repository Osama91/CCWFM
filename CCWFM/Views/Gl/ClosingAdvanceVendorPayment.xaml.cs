using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.GlService;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.Helpers.Enums;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel;
using CCWFM.ViewModel.Gl;
using CCWFM.Views.Gl.ChildWindow;
using CCWFM.Views.PrintPreviews;
using Omu.ValueInjecter.Silverlight;

namespace CCWFM.Views.Gl
{
    public partial class ClosingAdvanceVendorPayment
    {
        private readonly ClosingAdvanceVendorPaymentViewModel _viewModel;
    

        public ClosingAdvanceVendorPayment()
        {
            InitializeComponent();
            _viewModel = new ClosingAdvanceVendorPaymentViewModel();
            DataContext = _viewModel;
    
       
        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.MainRowList.Clear();
            _viewModel.GetMaindata();
       
        }

     
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveMainRow();
        }
    }
}