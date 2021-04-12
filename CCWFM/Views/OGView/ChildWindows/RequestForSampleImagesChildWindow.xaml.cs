using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using CCWFM.Helpers.Utilities;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class RequestForSampleImagesChildWindow
    {
        private readonly RequestforSampleImageViewModel _viewModel;

        public RequestForSampleImagesChildWindow(TblRequestForSampleViewModel requestForSample)
        {
            InitializeComponent();

            _viewModel = new RequestforSampleImageViewModel(requestForSample);
            DataContext = _viewModel;
        }

        private void btnAddGalaryImage_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Image Files (*.png, *.jpg, *.jpeg)|*.png;*.jpg;*.jpeg",
                Multiselect = true
            };
            if (dlg.ShowDialog().GetValueOrDefault(false))
            {
                foreach (var image in dlg.Files)
                {
                    WriteableBitmap wb;
                    using (var stream = image.OpenRead())
                    {
                        wb = ImageHelper.GetImageSource(stream, 100, 100);
                    }

                    byte[] buffer;
                    using (var source = JpgEncoder.Encode(wb, 50))
                    {
                        var bufferSize = Convert.ToInt32(source.Length);
                        buffer = new byte[bufferSize];
                        source.Read(buffer, 0, bufferSize);
                        source.Close();
                    }

                    _viewModel.MainRowList.Add(new TblStyleImageViewModel
                    {
                        ImagePathThumbnail = buffer,
                        ImagePerRow = image,
                        TblRequestForSample = _viewModel.RequestForSample. Iserial,
                    });
                }
            }
        }

        private void btnSaveGalary_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveImages();
        }

        private void BtnDownloadButton_MouseEnter(object sender, MouseEventArgs e)
        {
            var btn = sender as Button;
            Storyboard.SetTarget(Storyboard1, btn);
            Storyboard1.Begin();
        }

        private void BtnDownloadButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Storyboard1.Stop();
        }

        private void BtnDownloadButton_Click(object sender, RoutedEventArgs e)
        {
            var wClient = new WebClient();
            var uri = Application.Current.Host.Source.AbsoluteUri;
            var index = uri.IndexOf("/ClientBin");
            uri = uri.Substring(0, index);
            uri = uri + "/" + _viewModel.SelectedMainRow.ImagePath;
            wClient.OpenReadCompleted += wClient_OpenReadCompleted;
            wClient.DownloadProgressChanged += wClient_DownloadProgressChanged;

            wClient.OpenReadAsync(new Uri(uri), UriKind.Relative);
        }

        private void wClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // gets the downloaded percentage of the async operation
            MyProgressBar.Value = e.ProgressPercentage;
        }

        private void wClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                var streams = e.Result;
                var ms = new MemoryStream();
                streams.CopyTo(ms);
                var bytes = ms.ToArray();
                var saveDialog = new SaveFileDialog { Filter = "Image Files (*.png, *.jpg, *.jpeg)|*.png;*.jpg;*.jpeg" };
                
                var dialogResult = saveDialog.ShowDialog();

                if (dialogResult != true) return;

                using (var stream = saveDialog.OpenFile())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                }
            }
        }

        private void ChkDefault_OnChecked(object sender, RoutedEventArgs e)
        {
            var chk = sender as CheckBox;
            _viewModel.SelectedMainRow = chk.DataContext as TblStyleImageViewModel;
        }

        private Popup _popup = null;

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            
            var grid = sender as Border;
           // _viewModel.SelectedMainRow = grid.DataContext as TblStyleImageViewModel;
            var currentMousePosition = e.GetPosition(grid);
            ShowPopup(currentMousePosition);
        }

        private void ShowPopup(Point currentMousePosition)
        {
            if (_popup != null)
            {
                _popup.IsOpen = false;
                _popup = null;
            }
            _popup = CreateContextMenu(currentMousePosition);
            _popup.IsOpen = true;
        }

        private void HidePopup()
        {
            _popup.IsOpen = false;
        }

        private Popup CreateContextMenu(Point currentMousePosition)
        {
            var popup = new Popup();
            var popupGrid = new Grid();
            var popupCanvas = new Canvas();

            popup.Child = popupGrid;
            popupCanvas.MouseLeftButtonDown += (sender, e) => HidePopup();
            popupCanvas.MouseRightButtonDown += (sender, e) => { e.Handled = true; HidePopup(); };
            popupCanvas.Background = new SolidColorBrush(Colors.Transparent);
            popupGrid.Children.Add(popupCanvas);
            popupGrid.Children.Add(CreateContextMenuItems(currentMousePosition));

            popupGrid.Width = ActualWidth;
            popupGrid.Height = ActualHeight;
            popupCanvas.Width = popupGrid.Width;
            popupCanvas.Height = popupGrid.Height;

            return popup;
        }

        private FrameworkElement CreateContextMenuItems(Point currentMousePosition)
        {
            
            var img = new BitmapImage {UriSource = new Uri("/CCWFM;component/Images/DownloadButton.png",UriKind.Relative)};

            var lstContextMenu = new ListBox();
            var downloadImage = new Image {Stretch = Stretch.Fill, Source = img};
            var btnDownloadButton = new Button();
            btnDownloadButton.MouseEnter += BtnDownloadButton_MouseEnter;
            btnDownloadButton.MouseLeave += BtnDownloadButton_MouseLeave;
            btnDownloadButton.Click += BtnDownloadButton_Click;
            btnDownloadButton.Content = downloadImage;
            lstContextMenu.Items.Add(btnDownloadButton);

            var rootGrid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(currentMousePosition.X, currentMousePosition.Y, 0, 0)
            };
            rootGrid.Children.Add(lstContextMenu);

            return rootGrid;
        }

        protected void txb_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            HidePopup();
        }
    }
}