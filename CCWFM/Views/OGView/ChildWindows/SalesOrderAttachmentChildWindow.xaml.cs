using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class SalesOrderAttachmentChildWindow
    {
        private readonly SalesOrderAttachmentViewModel _viewModel;

        public SalesOrderAttachmentChildWindow(TblSalesOrderViewModel SalesOrder)
        {
            InitializeComponent();

            _viewModel = new SalesOrderAttachmentViewModel(SalesOrder);
            DataContext = _viewModel;
        }

        private void btnAddGalaryAttachment_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                // Filter = "Attachment Files (*.png, *.jpg, *.jpeg)|*.png;*.jpg;*.jpeg",
                Multiselect = true
            };
            if (dlg.ShowDialog().GetValueOrDefault(false))
            {
                foreach (var attachment in dlg.Files)
                {
                    //WriteableBitmap wb;
                    //using (var stream = Attachment.OpenRead())
                    //{
                    //    wb = AttachmentHelper.GetAttachmentSource(stream, 100, 100);
                    //}

                    //byte[] buffer;
                    //using (var source = JpgEncoder.Encode(wb, 50))
                    //{
                    //    var bufferSize = Convert.ToInt32(source.Length);
                    //    buffer = new byte[bufferSize];
                    //    source.Read(buffer, 0, bufferSize);
                    //    source.Close();
                    //}

                    _viewModel.MainRowList.Add(new TblSalesOrderAttachmentViewModel
                    {
                        //AttachmentPathThumbnail = buffer,
                        AttachmentPerRow = attachment,
                        TblSalesOrder = _viewModel.SalesOrder.Iserial,
                        OrginalFileName = attachment.Name,
                    });
                }
            }
        }

        private void btnSaveGalary_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveAttachments();
        }

        private void BtnDownloadButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var btn = sender as Button;
            Storyboard.SetTarget(Storyboard1, btn);
            Storyboard1.Begin();
        }

        private void BtnDownloadButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Storyboard1.Stop();
        }

        private SaveFileDialog _dialog;
        private bool? _dialogResult;

        private void BtnDownloadButton_Click(object sender, RoutedEventArgs e)
        {
            _dialog = new SaveFileDialog { Filter = "All Files|*.*" };
            _dialog.DefaultFileName = _viewModel.SelectedMainRow.OrginalFileName;
            _dialogResult = _dialog.ShowDialog();
            var btn = sender as Button;
            var SalesOrderAttachment = btn.DataContext as TblSalesOrderAttachmentViewModel;

            var wClient = new WebClient();
            var uri = Application.Current.Host.Source.AbsoluteUri;
            var index = uri.IndexOf("/ClientBin");
            uri = uri.Substring(0, index);
            uri = uri + "/" + SalesOrderAttachment.AttachmentPath;
            wClient.OpenReadCompleted += wClient_OpenReadCompleted;
            wClient.DownloadProgressChanged += wClient_DownloadProgressChanged;

            wClient.OpenReadAsync(new Uri(uri), UriKind.Relative);
        }

        private void wClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _viewModel.SelectedMainRow.Percentage = e.ProgressPercentage;

            _viewModel.SelectedMainRow.FileMessage = e.ProgressPercentage + "% done " + e.BytesReceived + " bytes received and " + e.TotalBytesToReceive + " bytes still needed.";
        }

        private void wClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                var streams = e.Result;
                var ms = new MemoryStream();
                streams.CopyTo(ms);
                var bytes = ms.ToArray();

                if (_dialogResult != true) return;

                using (var stream = _dialog.OpenFile())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                }

                //var streamZip = e.Result;
                //var sri = new StreamResourceInfo(streamZip, null);
                //// Assuming abc.jpg is an Attachment inside the Attachment.zip
                //var AttachmentSourceInfo =
                //    Application.GetResourceStream(sri, new Uri(_viewModel.SelectedMainRow.AttachmentPath, UriKind.Relative));

                // Converting the stream to Attachment
                //var bi = new BitmapAttachment();
                //var img = new Attachment();
                //img.Source = bi;
                // Do something with this Attachment
            }
        }
    }
}