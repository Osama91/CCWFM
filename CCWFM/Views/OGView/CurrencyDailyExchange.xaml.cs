using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView
{
    public partial class CurrencyDailyExchange
    {
        public CurrencyDailyExchangeViewModel ViewModel;

        public CurrencyDailyExchange()
        {
            InitializeComponent();

            ViewModel = LayoutRoot.DataContext as CurrencyDailyExchangeViewModel;
            DataContext = ViewModel;
        }

        private void FrameworkElement_OnBindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                (e.OriginalSource as Control).Background = new SolidColorBrush(Colors.Yellow);
                ToolTipService.SetToolTip((e.OriginalSource as TextBox), e.Error.Exception.Message);
            }
            else if (e.Action == ValidationErrorEventAction.Removed)
            {
                (e.OriginalSource as Control).Background = new SolidColorBrush(Colors.White);
                ToolTipService.SetToolTip((e.OriginalSource as TextBox), null);
            }
        }

        private void DetailGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
            }
            else if (e.Key == Key.Delete)
            {
                ViewModel.SelectedDetailRows.Clear();
                foreach (var row in DetailGrid.SelectedItems)
                {
                    ViewModel.SelectedDetailRows.Add(row as TblCurrencyDailyExchangeViewModel);
                }

                ViewModel.DeleteMainRow();
            }
        }
        
        private void DetailGrid_OnRowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            ViewModel.SaveMainRow();
        }

        private Popup _popup = null;

        private void ShowPopup()
        {
            if (_popup != null)
            {
                _popup.IsOpen = false;
            }

            _popup.IsOpen = true;
        }

        private void HidePopup()
        {
            _popup.IsOpen = false;
        }

        private Popup CreateContextMenu(Point currentMousePosition)
        {
            var popup = new Popup { Width = 350 };
            var popupGrid = new Grid();
            var popupCanvas = new Canvas();

            popup.Child = popupGrid;
            popupCanvas.MouseLeftButtonDown += (sender, e) => HidePopup();
            popupCanvas.MouseRightButtonDown += (sender, e) => { e.Handled = true; HidePopup(); };
            popupCanvas.Background = new SolidColorBrush(Colors.Transparent);
            popupGrid.Children.Add(popupCanvas);
            popupGrid.Children.Add(CreateContextMenuItems(currentMousePosition));

            popupGrid.Width = 350;
            popupGrid.Height = ActualHeight;
            popupCanvas.Width = popupGrid.Width;
            popupCanvas.Height = popupGrid.Height;

            return popup;
        }

        private DatePicker fromdatePicker = new DatePicker { Width = 200 };
        private DatePicker todatePicker = new DatePicker { Width = 200 };
        private TextBox txtExch = new TextBox { Width = 200 };

        private FrameworkElement CreateContextMenuItems(Point currentMousePosition)
        {
            fromdatePicker = new DatePicker { Width = 200 };
            todatePicker = new DatePicker { Width = 200 };
            txtExch = new TextBox { Width = 200 };
            var lstContextMenu = new ListBox();
            var btnCloseButton = new Button();
            btnCloseButton.Click += btnCloseButton_Click;
            btnCloseButton.Content = "Close";

            var btnGenerateButton = new Button();
            btnGenerateButton.Click += btnGenerateButton_Click;
            btnGenerateButton.Content = "Generate";

            var stackDate = new StackPanel { Orientation = Orientation.Vertical };
            var stackFromDate = new StackPanel { Orientation = Orientation.Horizontal };
            var stackToDate = new StackPanel { Orientation = Orientation.Horizontal };
            var stackExch = new StackPanel { Orientation = Orientation.Horizontal };

            var fromdateText = new TextBlock { Text = "From Date", Width = 70 };
            var todateText = new TextBlock { Text = "To Date", Width = 70 };

            var exchText = new TextBlock { Text = "Rate", Width = 70 };

            var fromDateBinding = new Binding("FromDate") { Source = ViewModel.FromDate, Mode = BindingMode.TwoWay };
            var toDateBinding = new Binding("ToDate") { Source = ViewModel.ToDate, Mode = BindingMode.TwoWay };
            
            fromdatePicker.SetBinding(DatePicker.SelectedDateFormatProperty, fromDateBinding);
            todatePicker.SetBinding(DatePicker.SelectedDateFormatProperty, toDateBinding);

            stackFromDate.Children.Add(fromdateText);
            stackFromDate.Children.Add(fromdatePicker);
            stackToDate.Children.Add(todateText);
            stackToDate.Children.Add(todatePicker);
            stackExch.Children.Add(exchText);
            stackExch.Children.Add(txtExch);
            stackDate.Children.Add(stackFromDate);
            stackDate.Children.Add(stackToDate);
            stackDate.Children.Add(stackExch);
            lstContextMenu.Items.Add(btnCloseButton);
            lstContextMenu.Items.Add(btnGenerateButton);
            lstContextMenu.Items.Add(stackDate);

            var rootGrid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(currentMousePosition.X, currentMousePosition.Y, 0, 0)
            };
            rootGrid.Children.Add(lstContextMenu);

            return rootGrid;
        }

        private void btnCloseButton_Click(object sender, RoutedEventArgs e)
        {
            _popup.IsOpen = false;
        }

        private void btnGenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (fromdatePicker.SelectedDate != null)
                if (todatePicker.SelectedDate != null)
                    if (txtExch.Text != null)
                        ViewModel.GenerateCurrencyDailyExchange((DateTime)fromdatePicker.SelectedDate, (DateTime)todatePicker.SelectedDate, Convert.ToSingle(txtExch.Text));
            HidePopup();
        }

        private void BtnGenerate_Checked(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            var gt = b.TransformToVisual(Application.Current.RootVisual);
            var p = gt.Transform(new Point(0, b.ActualHeight));
            _popup = CreateContextMenu(p);
            _popup.HorizontalOffset = p.X;
            _popup.VerticalOffset = p.Y;
            ShowPopup();
        }
    }
}